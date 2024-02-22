using EssaysProvider.Config;
using System.Collections.Concurrent;

namespace EssaysProvider
{
    internal interface IEssaysProvider
    {
        /// <summary>
        /// Inits the provider. Idemnpotent. Must be called in order to use the provider.
        /// </summary>
        /// <param name="config">The provider's config.</param>
        Task InitAsync(EssaysProviderConfig config);
        /// <summary>
        /// Returns a concurrent queue of essays Uris.
        /// </summary>
        /// <returns></returns>
        Task<ConcurrentQueue<Uri>> GetEssaysAsync();
    }
}
