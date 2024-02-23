namespace ClientFactory
{
    public interface IHttpClientFactory
    {
        Task<HttpClient> GetHttpClientAsync();
    }
}
