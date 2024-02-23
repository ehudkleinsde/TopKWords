using Common.Config;
using Logger;

namespace ClientFactory
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private ILogger _logger;
        private IHttpClientFactoryConfigProvider _configProvider;
        private Queue<HttpClient> _clients;
        private int _capacity;

        public HttpClientFactory(ILogger logger,
            IHttpClientFactoryConfigProvider configProvider)
        {
            _logger = logger;
            _clients = new();
            _capacity = configProvider.;
        }

        public Task<HttpClient> GetHttpClientAsync()
        {
            throw new NotImplementedException();
        }
    }
}
