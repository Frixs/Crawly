namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// UNDONE
    /// </summary>
    class CrawlerEngine : ICrawlerEngine
    {
        #region Interface Properties

        /// <inheritdoc/>
        public bool IsCurrentlyCrawlingFlag { get; private set; }

        /// <inheritdoc/>
        public short CrawlingProgressPct { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CrawlerEngine()
        {
            IsCurrentlyCrawlingFlag = false;
            CrawlingProgressPct = -1;
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public bool Start()
        {
            if (IsCurrentlyCrawlingFlag)
                return false;
            IsCurrentlyCrawlingFlag = true;

            // TODO: add process

            return true;
        }

        #endregion
    }
}
