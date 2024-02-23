namespace WordsBank
{
    public class WordsBank : IWordsBank
    {
        private HashSet<string> _words;
        public WordsBank()
        {
            
        }

        public bool IsWordInBank(string word)
        {
            return _words.Contains(word);
        }

        private async Task<List<Uri>> GetEssaysListAsync()
        {
            HttpClient client = new HttpClient();
            string content;
            List<Uri> essaysQueue;

            try
            {
                HttpResponseMessage response = await client.GetAsync(_essaysListUri);
                response.EnsureSuccessStatusCode();
                content = await response.Content.ReadAsStringAsync();
                essaysQueue = PopulateQueue(content);
            }
            catch (Exception ex)
            {
                _logger.LogFatalError(nameof(GetEssaysListAsync), $"Unable to get essays list. Exception: {ex.GetType()}, {ex.Message}");
                throw;
            }

            _logger.LogInfo(nameof(GetEssaysListAsync), $"Got {essaysQueue.Count} essays.");

            return essaysQueue;
        }
    }
}
