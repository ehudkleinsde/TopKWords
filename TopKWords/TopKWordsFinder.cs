using EssaysProvider.EssaysList;
using EssaysProvider.SingleEssay;
using Logger;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using TopKWords.Contracts;
using TopKWordsConfigProvider;

namespace TopKWords
{
    internal class TopKWordsFinder
    {
        private ILogger _logger;
        private ITopKWordsConfigProvider _configProvider;
        private IEssaysListProvider _essaysProvider;
        private ISingleEssayProvider _singleEssayProvider;

        private ConcurrentDictionary<string, int> _wordsCount;

        public TopKWordsFinder(ILogger logger,
            ITopKWordsConfigProvider configProvider,
            IEssaysListProvider essaysProvider,
            ISingleEssayProvider singleEssayProvider)
        {
            _logger = logger;

            //TODO: consider keeping the config obj and not the providers
            _configProvider = configProvider;
            _essaysProvider = essaysProvider;
            _singleEssayProvider = singleEssayProvider;

            _wordsCount = new();
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInfo(nameof(ExecuteAsync), "Start");

            try
            {
                List<Uri> essaysList = await _essaysProvider.GetEssaysListAsync();
                ConcurrentQueue<CountEssayWordsJob> jobsQueue = CreateJobsQueue(essaysList);
                List<Task> workers = Enumerable.Range(0, _configProvider.GetMaxRequestsPerMinute()).Select(_ => PerformWordsCountJobs(jobsQueue)).ToList();
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
            Random random = new();
            _logger.LogInfo(nameof(PerformWordsCountJobs), $"Start, Thread id {Thread.CurrentThread.ManagedThreadId}");

            while (jobsQueue.Count > 0)
            {
                if (jobsQueue.TryDequeue(out CountEssayWordsJob job))
                {
                    try
                    {
                        int initialWait = random.Next(0, 59);
                        await Task.Delay(initialWait * 1000);

                        string essayContent = await _singleEssayProvider.GetEssayContentAsync(job.EssayUri);
                        string[] tokens = essayContent.Split(" ");

                        foreach (string token in tokens)
                        {
                            _wordsCount.AddOrUpdate(token, 1, (existingKey, existingValue) => existingValue + 1);
                        }

                        await Task.Delay((60 - initialWait) * 1000);

                        _logger.LogInfo(nameof(PerformWordsCountJobs), $"Successfully counted word from essay {job.EssayUri}");
                    }
                    catch (HttpRequestException ex)
                    {
                        if (job.Retry < _configProvider.GetMaxRetriesForFetchingEssayContent())
                        {
                            _logger.LogError(nameof(PerformWordsCountJobs),
                                $"Failed fetching essay content, retryable exception, retry number: {job.Retry}, " +
                                $"retries left: {_configProvider.GetMaxRetriesForFetchingEssayContent() - job.Retry}, {job.EssayUri}, Exception: {ex.GetType()}, {ex.Message}");

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
                        _logger.LogError(nameof(PerformWordsCountJobs), $"Failed fetching essay content, nor retrying, {job.EssayUri}, Exception: {ex.GetType()}, {ex.Message}");
                    }
                }
            }

            _logger.LogInfo(nameof(PerformWordsCountJobs), $"End, Thread id {Thread.CurrentThread.ManagedThreadId}");
        }

        private ConcurrentQueue<CountEssayWordsJob> CreateJobsQueue(List<Uri> essaysList)
        {
            ConcurrentQueue<CountEssayWordsJob> jobs = new();

            foreach (Uri uri in essaysList.Take(1))
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
