using Serilog;
using TopKWordsConfigProvider;

namespace Logger
{
    public class SeriLogger : ILogger
    {
        private Serilog.ILogger _logger;
        private IConfigProvider _configProvider;
        public SeriLogger(IConfigProvider configProvider)
        {
            _configProvider = configProvider;

            if (!Directory.Exists(_configProvider.GetLogFilePath()))
            {
                Directory.CreateDirectory(_configProvider.GetLogFilePath());
            }

            _logger = new LoggerConfiguration()
                .WriteTo.File(_configProvider.GetLogFilePath()+"\\log.txt")
                .CreateLogger();
        }

        public void Finalize()
        {
            Log.CloseAndFlush();
        }

        public void LogError(string message)
        {
            _logger.Error(message);
        }

        public void LogFatalError(string message)
        {
            _logger.Fatal(message);
        }

        public void LogInfo(string message)
        {
            _logger.Information(message);
        }

        public void LogWarning(string message)
        {
            _logger.Warning(message);
        }
    }
}
