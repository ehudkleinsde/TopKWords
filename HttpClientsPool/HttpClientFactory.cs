using Common.Config;
using Logger;
using System.Collections.Concurrent;

namespace ClientFactory
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private ILogger _logger;
        private IHttpClientFactoryConfigProvider _configProvider;
        private ConcurrentBag<HttpClient> _clients;
        private int _capacity;
        private SemaphoreSlim _semaphore;

        public HttpClientFactory(ILogger logger,
            IHttpClientFactoryConfigProvider configProvider)
        {
            _logger = logger;
            _clients = new();
            _capacity = configProvider.GetHttpClientPoolCapacity();
            _semaphore = new(_capacity, _capacity);
        }

        public async Task<HttpClient> GetHttpClientAsync()
        {
            await _semaphore.WaitAsync();

            if (!_clients.TryTake(out HttpClient client))
            {
                return new();
            }
            
            return client;
        }

        public void ReturnClient(HttpClient client)
        {
            _clients.Add(client);
            _semaphore.Release();
        }
    }
}
