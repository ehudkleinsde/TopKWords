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
    public class TopKWordsFinder
    {
        private ILogger _logger;
        private ITopKWordsConfigProvider _configProvider;
        private IEssaysListProvider _essaysProvider;
        private ISingleEssayProvider _singleEssayProvider;
        private IWordsValidator _wordsValidator;
        private ICircuitBreaker _circuitBreaker;

        private ConcurrentDictionary<string, int> _wordsCount;

        private Random _random;
        private volatile int _jobsCount;

        private object _locker = new();

        public TopKWordsFinder(ILogger logger,
            ITopKWordsConfigProvider configProvider,
            IEssaysListProvider essaysProvider,
            ISingleEssayProvider singleEssayProvider,
            IWordsValidator wordsValidator,
            ICircuitBreaker circuitBreaker)
        {
            _logger = logger;
            _random = new();

            _jobsCount = 0;

            //TODO: consider keeping the config obj and not the providers
            _configProvider = configProvider;
            _essaysProvider = essaysProvider;
            _singleEssayProvider = singleEssayProvider;
            _circuitBreaker = circuitBreaker;
            _wordsValidator = wordsValidator;

            _wordsCount = new();
        }

        public async Task<List<WordCount>> ExecuteAsync()
        {
            _logger.LogInfo(nameof(ExecuteAsync), "Start");
            List<WordCount> result = null;

            try
            {
                await _wordsValidator.InitAsync();
                List<Uri> essaysList = await _essaysProvider.GetEssaysListAsync();
                ConcurrentQueue<CountEssayWordsJob> jobsQueue = CreateJobsQueue(essaysList);
                List<Task> workers = Enumerable.Range(0, _configProvider.GetMaxRequestsPerMinute()).Select(_ => Task.Run(() => PerformWordsCountJobs(jobsQueue))).ToList();

                await Task.WhenAll(workers);

                //TODO: consider heap
                result = _wordsCount
                    .OrderByDescending(pair => pair.Value)
                    .Take(_configProvider.GetTopKWordsToFind())
                    .Select(w => new WordCount { Word = w.Key, Count = w.Value })
                    .ToList();

                string resultStr = JsonConvert.SerializeObject(result, Formatting.Indented);
                await Console.Out.WriteLineAsync(resultStr);
                _logger.LogInfo(nameof(ExecuteAsync), resultStr);
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

            return result;
        }

        //TODO: extract thread safe counter to another component
        private void IncrementJobsCount()
        {
            lock (_locker)
            {
                _jobsCount++;
            }
        }

        private void ResetJobsCount()
        {
            lock (_locker)
            {
                _jobsCount = 0;
            }
        }

        private async Task PerformWordsCountJobs(ConcurrentQueue<CountEssayWordsJob> jobsQueue)
        {
            while (jobsQueue.Count > 0)
            {
                await PeridicWaitToAvoidRateLimit();
                await WaitForCircuitBreakerToCloseIfNeeded();

                if (jobsQueue.TryDequeue(out CountEssayWordsJob job))
                {
                    try
                    {
                        if (!_circuitBreaker.IsOpen())
                        {
                            int initialWait = _random.Next(0, 59);
                            await Task.Delay(TimeSpan.FromSeconds(initialWait));

                            if (!_circuitBreaker.IsOpen())
                            {
                                IncrementJobsCount();
                                await CountWords(job);
                                await Task.Delay((60 - initialWait) * 1000);
                            }
                        }
                        else
                        {
                            jobsQueue.Enqueue(job);
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        await HandleHttpRequestException(jobsQueue, job, ex);
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

        private async Task CountWords(CountEssayWordsJob job)
        {
            string essayContent = await _singleEssayProvider.GetEssayContentAsync(job.EssayUri);
            string[] tokens = essayContent.Split(" ");

            foreach (string token in tokens)
            {
                if (await _wordsValidator.IsValidAsync(token))
                {
                    _wordsCount.AddOrUpdate(token, 1, (existingKey, existingValue) => existingValue + 1);
                }
            }

            _logger.LogInfo(nameof(PerformWordsCountJobs), $"Successfully counted word from essay {job.EssayUri}");
        }

        private async Task WaitForCircuitBreakerToCloseIfNeeded()
        {
            while (_circuitBreaker.IsOpen())
            {
                await Task.Delay(1000);
            }
        }

        private async Task PeridicWaitToAvoidRateLimit()
        {
            if (_jobsCount >= 100)//TODO: make it configurable
            {
                await _circuitBreaker.OpenForIntervalAsync(10_000);
                ResetJobsCount();
            }
        }

        private async Task HandleHttpRequestException(ConcurrentQueue<CountEssayWordsJob> jobsQueue, CountEssayWordsJob job, HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInfo(nameof(PerformWordsCountJobs), "Opened CB after 404.");
                _logger.LogError(nameof(PerformWordsCountJobs), $"Failed fetching essay content, not retrying, {job.EssayUri}, Exception: {ex.GetType()}, {ex.Message}");
                await _circuitBreaker.OpenForIntervalAsync(15_000);
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
