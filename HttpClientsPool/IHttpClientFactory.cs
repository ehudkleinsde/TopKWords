namespace ClientFactory
{
    public interface IHttpClientFactory
    {
        /// <summary>
        /// Get HttpClient from pool.
        /// </summary>
        Task<HttpClient> GetHttpClientAsync();
        /// <summary>
        /// Return a client to the pool.
        /// </summary>
        /// <param name="client">The client to return.</param>
        void ReturnClient(HttpClient client);
    }
}
