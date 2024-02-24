namespace Common.Config
{
    public interface IWordsBankConfigProvider
    {
        /// <summary>
        /// Get Uri for fetching words bank.
        /// </summary>
        Uri GetWordsBankUri();
    }
}
