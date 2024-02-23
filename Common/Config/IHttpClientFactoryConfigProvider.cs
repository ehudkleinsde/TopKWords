namespace Common.Config
{
    public interface IHttpClientFactoryConfigProvider
    {
        /// <summary>
        /// Returns the capacity of the HttpClient pool.
        /// </summary>
        int GetHttpClientPoolCapacity();
    }
}
