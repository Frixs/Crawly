namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// Base crawler manager
    /// </summary>
    public class CrawlerManager : ICrawlerManager
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CrawlerManager()
        {
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public bool AddCrawler(ICrawlerEngine crawler)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public ICrawlerEngine GetCrawler(string cid)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        public bool RemoveCrawler(string cid)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
