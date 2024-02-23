namespace EssaysProvider.EssaysList
{
    public interface IEssaysListProvider
    {
        /// <summary>
        /// Returns a list of essays Uris.
        /// </summary>
        Task<List<Uri>> GetEssaysListAsync();
    }
}
