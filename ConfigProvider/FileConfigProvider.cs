using EssaysProvider.Config;
using Newtonsoft.Json;

namespace TopKWordsConfigProvider
{
    public class FileConfigProvider : ITopKWordsConfigProvider, IEssaysProviderConfigProvider
    {
        private string _configFilePath;
        private TopKWordsConfig _topKWordsConfig;

        //TODO: consider using a logger for incorrect config values, e.g., invalid Uris, or deserialization issues.
        //(not added due to cross reference between logger and config provider)
        public FileConfigProvider(string configFilePath)
        {
            _configFilePath = configFilePath;

            if (!File.Exists(configFilePath))
            {
                throw new FileNotFoundException($"Config file not found, {configFilePath}");
            }

            string jsonString = File.ReadAllText(_configFilePath);
            _topKWordsConfig = JsonConvert.DeserializeObject<TopKWordsConfig>(jsonString);
        }

        public Uri GetEssaysListUri()
        {
            return _topKWordsConfig.EssaysListAbsoluteUri;
        }

        public string GetLogFilePath()
        {
            return _topKWordsConfig.LogFilePath;
        }

        public int GetTopKWordsToFind()
        {
            return _topKWordsConfig.TopKWords;
        }
    }
}
