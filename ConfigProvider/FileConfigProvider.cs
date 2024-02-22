using Newtonsoft.Json;

namespace TopKWordsConfigProvider
{
    public class FileConfigProvider : IConfigProvider
    {
        private string _configFilePath;
        private TopKWordsConfig _topKWordsConfig;
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
