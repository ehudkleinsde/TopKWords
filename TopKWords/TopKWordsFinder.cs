using EssaysProvider.EssaysList;
using Logger;
using TopKWordsConfigProvider;

namespace TopKWords
{
    internal class TopKWordsFinder
    {
        private ILogger _logger;
        private ITopKWordsConfigProvider _configProvider;
        private IEssaysListProvider _essaysProvider;

        public TopKWordsFinder(ILogger logger, 
            ITopKWordsConfigProvider configProvider,
            IEssaysListProvider essaysProvider)
        {
            _logger = logger;
            _configProvider = configProvider;
            _essaysProvider = essaysProvider;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInfo(nameof(ExecuteAsync), "Start");

            try
            {
                List<Uri> essaysList = await _essaysProvider.GetEssaysListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogFatalError(nameof(ExecuteAsync),$"{ex.GetType()}, {ex.Message}, {ex.StackTrace}");
            }
            finally
            {
                _logger.LogInfo(nameof(ExecuteAsync), "End");
                CleanUp();
            }
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
