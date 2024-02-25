using ClientFactory;
using ContentExtraction;
using Logger;
using System.Diagnostics;

namespace EssaysProvider.SingleEssay
{
    public class HttpEssayContentProvider : ISingleEssayProvider
    {
        private IHttpClientFactory _httpClientFactory;
        private ILogger _logger;
        private IContentExtractor _contentExtractor;
        public HttpEssayContentProvider(IHttpClientFactory httpClientFactory, ILogger logger, IContentExtractor contentExtractor)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _contentExtractor = contentExtractor;
        }

        public async Task<string> GetEssayContentAsync(Uri essaysUri)
        {
            HttpClient client = await _httpClientFactory.GetHttpClientAsync();
            string content = null;

            try
            {
                content = await client.GetStringAsync(essaysUri);
                content = _contentExtractor.Extract(content);
            }
            catch (Exception ex)
            {
                _logger.LogError(nameof(GetEssayContentAsync), $"Failed extracting content from {essaysUri}, Exception: {ex.GetType()}, {ex.Message}");
                _httpClientFactory.ReturnClient(client);
                throw;
            }

            _httpClientFactory.ReturnClient(client);

            return content;
        }
    }
}