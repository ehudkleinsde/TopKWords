namespace Common.Config
{
    public interface IEssaysProviderConfigProvider
    {
        /// <summary>
        /// Get the Uri for the list of essays.
        /// </summary>
        Uri GetEssaysListUri();
    }
}
