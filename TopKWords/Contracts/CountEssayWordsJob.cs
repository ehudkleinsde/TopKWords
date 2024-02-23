namespace TopKWords.Contracts
{
    internal class CountEssayWordsJob
    {
        public Uri EssayUri { get; set; }
        public int Retry { get; set; }
    }
}
