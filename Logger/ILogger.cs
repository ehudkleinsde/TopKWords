namespace Logger
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogFatalError(string message);
        void Finalize();
    }
}
