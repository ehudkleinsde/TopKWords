using Logger;

namespace TopKWords
{
    internal class TopKWordsFinder
    {
        private ILogger _logger;
        public TopKWordsFinder(ILogger logger)
        {
            _logger = logger;
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInfo("yolo");
            Finalize();
        }

        private void Finalize()
        {
            _logger.Finalize();
        }
    }
}
