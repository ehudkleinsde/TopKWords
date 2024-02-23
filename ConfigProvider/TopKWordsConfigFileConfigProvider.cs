using Common.Config;
using Newtonsoft.Json;

namespace TopKWordsConfigProvider
{
    public class TopKWordsConfigFileConfigProvider : ITopKWordsConfigProvider, IEssaysProviderConfigProvider, IHttpClientFactoryConfigProvider, IWordsBankConfigProvider
    {
        private string _configFilePath;
        private TopKWordsConfig _topKWordsConfig;

        //TODO: consider using a logger for incorrect config values, e.g., invalid Uris, or deserialization issues.
        //(not added due to cross reference between logger and config provider)
        public TopKWordsConfigFileConfigProvider(string configFilePath)
        {
            _configFilePath = configFilePath;

            if (!File.Exists(configFilePath))
            {
                throw new FileNotFoundException($"Config file not found, {configFilePath}");
            }

            string jsonString = File.ReadAllText(_configFilePath);
            _topKWordsConfig = JsonConvert.DeserializeObject<TopKWordsConfig>(jsonString);
        }

        public int GetMaxRequestsPerMinute()
        {
            return _topKWordsConfig.MaxRequestsPerMinute;
        }

        public Uri GetEssaysListUri()
        {
            return _topKWordsConfig.EssaysListAbsoluteUri;
        }

        public int GetHttpClientPoolCapacity()
        {
            return _topKWordsConfig.HttpClientPoolCapacity;
        }

        public string GetLogFilePath()
        {
            return _topKWordsConfig.LogFilePath;
        }

        public int GetMaxRetriesForFetchingEssayContent()
        {
            return _topKWordsConfig.MaxRetriesForFetchingEssayContent;
        }

        public int GetTopKWordsToFind()
        {
            return _topKWordsConfig.TopKWords;
        }

        public Uri GetWordsBankUri()
        {
            return _topKWordsConfig.WordsBankUri;
        }
    }
}
