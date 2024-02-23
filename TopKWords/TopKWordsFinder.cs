using CircuitBreaker;
using EssaysProvider.EssaysList;
using EssaysProvider.SingleEssay;
using Logger;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using TopKWords.Contracts;
using TopKWordsConfigProvider;
using WordValidation;

namespace TopKWords
{
    internal class TopKWordsFinder
    {
        private ILogger _logger;
        private ITopKWordsConfigProvider _configProvider;
        private IEssaysListProvider _essaysProvider;
        private ISingleEssayProvider _singleEssayProvider;
        private IWordsValidator _wordsValidator;
        private ICircuitBreaker _circuitBreaker;

        private ConcurrentDictionary<string, int> _wordsCount;

        private Random _random;

        public TopKWordsFinder(ILogger logger,
            ITopKWordsConfigProvider configProvider,
            IEssaysListProvider essaysProvider,
            ISingleEssayProvider singleEssayProvider,
            IWordsValidator wordsValidator,
            ICircuitBreaker circuitBreaker)
        {
            _logger = logger;
            _random = new();

            //TODO: consider keeping the config obj and not the providers
            _configProvider = configProvider;
            _essaysProvider = essaysProvider;
            _singleEssayProvider = singleEssayProvider;
            _circuitBreaker = circuitBreaker;
            _wordsValidator = wordsValidator;

            _wordsCount = new();
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInfo(nameof(ExecuteAsync), "Start");

            try
            {
                await _wordsValidator.Init();
                List<Uri> essaysList = await _essaysProvider.GetEssaysListAsync();
                ConcurrentQueue<CountEssayWordsJob> jobsQueue = CreateJobsQueue(essaysList);
                List<Task> workers = Enumerable.Range(0, _configProvider.GetMaxRequestsPerMinute()).Select(_ => Task.Run(() => PerformWordsCountJobs(jobsQueue))).ToList();

                await Task.WhenAll(workers);

                //TODO: consider heap
                List<WordCount> topK = _wordsCount
                    .OrderByDescending(pair => pair.Value)
                    .Take(_configProvider.GetTopKWordsToFind())
                    .Select(w => new WordCount { Word = w.Key, Count = w.Value })
                    .ToList();

                await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(topK, Formatting.Indented));
            }
            catch (Exception ex)
            {
                _logger.LogFatalError(nameof(ExecuteAsync), $"{ex.GetType()}, {ex.Message}, {ex.StackTrace}");
            }
            finally
            {
                _logger.LogInfo(nameof(ExecuteAsync), "End");
                CleanUp();
            }
        }

        private async Task PerformWordsCountJobs(ConcurrentQueue<CountEssayWordsJob> jobsQueue)
        {
            while (jobsQueue.Count > 0)
            {
                while (_circuitBreaker.IsOpen())
                {
                    _logger.LogInfo(nameof(PerformWordsCountJobs), "Circuit breaker is open.");
                    await Task.Delay(1000);
                }

                if (jobsQueue.TryDequeue(out CountEssayWordsJob job))
                {
                    try
                    {
                        int initialWait = _random.Next(0, 59);
                        await Task.Delay(TimeSpan.FromSeconds(initialWait));

                        string essayContent = await _singleEssayProvider.GetEssayContentAsync(job.EssayUri);
                        string[] tokens = essayContent.Split(" ");

                        foreach (string token in tokens)
                        {
                            if (await _wordsValidator.IsValid(token))
                            {
                                _wordsCount.AddOrUpdate(token, 1, (existingKey, existingValue) => existingValue + 1);
                            }
                        }

                        string log = $"Successfully counted word from essay {job.EssayUri}";
                        _logger.LogInfo(nameof(PerformWordsCountJobs), log);
                        await Console.Out.WriteLineAsync(log);

                        await Task.Delay((60 - initialWait) * 1000);

                    }
                    catch (HttpRequestException ex)
                    {
                        if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            _logger.LogError(nameof(PerformWordsCountJobs), $"Failed fetching essay content, not retrying, {job.EssayUri}, Exception: {ex.GetType()}, {ex.Message}");
                        }
                        else if (ex.Message.Contains("999"))
                        {
                            jobsQueue.Enqueue(job);
                            _logger.LogWarning(nameof(PerformWordsCountJobs), $"Rate limited, waiting");
                            await _circuitBreaker.OpenForIntervalAsync();
                        }
                        else if (job.Retry < _configProvider.GetMaxRetriesForFetchingEssayContent())
                        {
                            _logger.LogError(nameof(PerformWordsCountJobs),
                                $"Failed fetching essay content, retryable exception, retry number: {job.Retry}, " +
                                $"retries left: {_configProvider.GetMaxRetriesForFetchingEssayContent() - job.Retry}, {job.EssayUri}, Exception: {ex.GetType()}, Code: {ex.StatusCode}, {ex.Message}");

                            job.Retry++;
                            jobsQueue.Enqueue(job);
                        }
                        else
                        {
                            _logger.LogError(nameof(PerformWordsCountJobs), $"Failed on last retry of fetching essay content, {job.EssayUri}, Exception: {ex.GetType()}, {ex.Message}");
                        }
                    }
                    catch (TaskCanceledException ex)
                    {
                        //TODO: consider limiting retries even on TaskCancelled exception
                        _logger.LogError(nameof(PerformWordsCountJobs), $"Task cancelled fetching essay content, retrying, {job.EssayUri}, Exception: {ex.GetType()}, {ex.Message}");
                        jobsQueue.Enqueue(job);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(nameof(PerformWordsCountJobs), $"Failed fetching essay content, not retrying, {job.EssayUri}, Exception: {ex.GetType()}, {ex.Message}");
                    }
                }
            }
        }

        private ConcurrentQueue<CountEssayWordsJob> CreateJobsQueue(List<Uri> essaysList)
        {
            ConcurrentQueue<CountEssayWordsJob> jobs = new();

            foreach (Uri uri in essaysList)
            {
                jobs.Enqueue(new CountEssayWordsJob() { EssayUri = uri, Retry = 0 });
            }

            return jobs;
        }

        private void CleanUp()
        {
            _logger.LogInfo(nameof(CleanUp), $"Start");

            //add cleanup

            _logger.LogInfo(nameof(CleanUp), $"End");
            _logger.Cleanup();
        }
    }
}
