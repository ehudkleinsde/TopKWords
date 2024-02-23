namespace EssaysProvider.SingleEssay
{
    public interface ISingleEssayProvider
    {
        /// <summary>
        /// Get the textual content of the essay's web page.
        /// </summary>
        /// <param name="essaysUri">Uri of the essay.</param>
        Task<string> GetEssayContentAsync(Uri essaysUri);
    }
}
