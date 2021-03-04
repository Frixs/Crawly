namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// UNDONE
    /// </summary>
    interface ICrawlerEngine
    {
        #region Properties

        /// <summary>
        /// Indicates if the engine is currently crawling data
        /// </summary>
        bool IsCurrentlyCrawlingFlag { get; }

        /// <summary>
        /// Percentage progress of current crawling
        /// </summary>
        /// <remarks>
        ///     If the crowling is not running currently, the result is '-1'
        /// </remarks>
        short CrawlingProgressPct { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Start the crawling process
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> = Started successfully.
        ///     <see langword="false"/> = Unable to start because it is already running.
        /// </returns>
        bool Start();

        #endregion
    }
}
