using Common.Config;
using Logger;

namespace WordValidation
{
    public class WordsBank : IWordsBank
    {
        private IWordsBankConfigProvider _configProvider;
        private ILogger _logger;

        private bool _isInit;

        private HashSet<string> _words;
        public WordsBank(IWordsBankConfigProvider configProvider, ILogger logger)
        {
            _configProvider = configProvider;
            _logger = logger;
            _words = new();
        }

        public async Task<bool> IsWordInBank(string word)
        {
            if (!_isInit)
            {
                //TODO: create new exception type
                throw new Exception("Words bank not initialized");
            }

            return _words.Contains(word);
        }

        public async Task Init()
        {
            HttpClient client = new HttpClient();
            string content;

            try
            {
                HttpResponseMessage response = await client.GetAsync(_configProvider.GetWordsBankUri());
                response.EnsureSuccessStatusCode();
                content = await response.Content.ReadAsStringAsync();
                string[] words = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                foreach(string word in words)
                {
                    _words.Add(word);
                }

                _isInit = true;
            }
            catch (Exception ex)
            {
                _logger.LogFatalError(nameof(Init), $"Unable to get words bank. Exception: {ex.GetType()}, {ex.Message}");
                throw;
            }

            _logger.LogInfo(nameof(Init), $"Got {_words.Count} words in word bank.");
        }
    }
}
