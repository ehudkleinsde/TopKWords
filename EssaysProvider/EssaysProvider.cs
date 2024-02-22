using EssaysProvider.Config;
using System.Collections.Concurrent;

namespace EssaysProvider
{
    internal class EssaysProvider : IEssaysProvider
    {
        private bool _isInit;
        private EssaysProviderConfig _config;
        
        public EssaysProvider()
        {
            _isInit = false;
        }
        public async Task<ConcurrentQueue<Uri>> GetEssaysAsync()
        {
            if(!_isInit) { throw new InvalidOperationException("Provider is not initialized. Call InitAsync for initialization."); }

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(_config.EssaysListUri);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();

            return PopulateQueue(content);
        }

        public async Task InitAsync(EssaysProviderConfig config)
        {
            if(config == null) throw new ArgumentNullException("config");

            _config = config;
            _isInit = true;
        }

        private ConcurrentQueue<Uri> PopulateQueue(string content)
        {
            if(content == null || content == string.Empty)
            {
                throw new ArgumentException(content, "content");
            }

            ConcurrentQueue<Uri> result = new ConcurrentQueue<Uri>();
            string[] lines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            foreach(string line in lines)
            {

            }

        }
    }
}
