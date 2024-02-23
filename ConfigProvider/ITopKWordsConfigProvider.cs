namespace TopKWordsConfigProvider
{
    public interface ITopKWordsConfigProvider
    {
        /// <summary>
        /// Get the path for a log file.
        /// </summary>
        string GetLogFilePath();
        /// <summary>
        /// Get how many requests per minute are allowed.
        /// </summary>
        int GetMaxRequestsPerMinute();
        int GetMaxRetriesForFetchingEssayContent();
        int GetTopKWordsToFind();
    }
}
