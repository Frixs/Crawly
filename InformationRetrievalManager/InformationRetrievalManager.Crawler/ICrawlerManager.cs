using System.Threading.Tasks;

namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// Interface for crawler manager
    /// </summary>
    public interface ICrawlerManager
    {
        /// <summary>
        /// Adds a new crawler engine into the manager
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> = Successfully added. 
        ///     <see langword="false"/> = Failed to add.
        /// </returns>
        Task<bool> AddCrawlerAsync(ICrawlerEngine crawler);

        /// <summary>
        /// Removes crawler engine based on CID from the manager
        /// </summary>
        /// <param name="cid">Crawler identifier</param>
        /// <returns>
        ///     <see langword="true"/> = Successfully removed. 
        ///     <see langword="false"/> = Failed to remove.
        /// </returns>
        Task<bool> RemoveCrawlerAsync(string cid);

        /// <summary>
        /// Gets the crawler engine based on CID.
        /// </summary>
        /// <param name="cid">Crawler identifier</param>
        /// <returns>Crawler Engine or null on failure</returns>
        Task<ICrawlerEngine> GetCrawlerAsync(string cid);
    }
}
