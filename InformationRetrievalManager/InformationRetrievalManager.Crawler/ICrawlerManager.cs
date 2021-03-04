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
        bool AddCrawler(ICrawlerEngine crawler);

        /// <summary>
        /// Removes crawler engine based on CID from the manager
        /// </summary>
        /// <param name="cid">Crawler identifier</param>
        /// <returns>
        ///     <see langword="true"/> = Successfully removed. 
        ///     <see langword="false"/> = Failed to remove.
        /// </returns>
        bool RemoveCrawler(string cid);

        /// <summary>
        /// Gets the crawler engine based on CID.
        /// </summary>
        /// <param name="cid">Crawler identifier</param>
        /// <returns>Crawler Engine or null on failure</returns>
        ICrawlerEngine GetCrawler(string cid);
    }
}
