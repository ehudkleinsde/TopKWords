namespace TopKWordsConfigProvider
{
    public interface ITopKWordsConfigProvider
    {
        /// <summary>
        /// Get the path for a log file.
        /// </summary>
        string GetLogFilePath();
    }
}
