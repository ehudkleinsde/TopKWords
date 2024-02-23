namespace Logger
{
    public interface ILogger
    {
        void LogInfo(string source, string message);
        void LogWarning(string source, string message);
        void LogError(string source, string message);
        void LogFatalError(string source, string message);
        void Cleanup();
    }
}
