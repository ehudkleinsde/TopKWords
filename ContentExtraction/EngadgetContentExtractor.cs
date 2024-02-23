namespace ContentExtraction
{
    public class EngadgetContentExtractor : IContentExtractor
    {
        public string Extract(string content)
        {
            return Extract("<article", "</article", content);
        }

        private string Extract(string startTag, string endTag, string content)
        {
            var startIndex = content.IndexOf(startTag, 0, StringComparison.OrdinalIgnoreCase) + startTag.Length;
            var endIndex = content.IndexOf(endTag, startIndex, StringComparison.OrdinalIgnoreCase);

            return content.Substring(startIndex, endIndex - startIndex);
        }
    }
}
