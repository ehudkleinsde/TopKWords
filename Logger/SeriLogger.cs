using Serilog;
using TopKWordsConfigProvider;

namespace Logger
{
    public class SeriLogger : ILogger
    {
        private Serilog.ILogger _logger;
        private ITopKWordsConfigProvider _configProvider;
        public SeriLogger(ITopKWordsConfigProvider configProvider)
        {
            _configProvider = configProvider;

            if (!Directory.Exists(_configProvider.GetLogFilePath()))
            {
                Directory.CreateDirectory(_configProvider.GetLogFilePath());
            }

            _logger = new LoggerConfiguration()
                .WriteTo.File(_configProvider.GetLogFilePath()+"\\log.txt", flushToDiskInterval: TimeSpan.FromSeconds(1))
                .CreateLogger();
        }

        public void Cleanup()
        {
            Log.CloseAndFlush();
        }

        public void LogError(string source, string message)
        {
            _logger.Error($"{source} : {message}");
        }

        public void LogFatalError(string source, string message)
        {
            _logger.Fatal($"{source} : {message}");
        }

        public void LogInfo(string source, string message)
        {
            _logger.Information($"{source} : {message}");
        }

        public void LogWarning(string source, string message)
        {
            _logger.Warning($"{source} : {message}");
        }
    }
}
