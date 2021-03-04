namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// Interface for crawler engines
    /// </summary>
    public interface ICrawlerEngine
    {
        #region Properties

        /// <summary>
        /// represents the crawler and makes it unique identifier for a certain crawler
        /// </summary>
        string Identifier { get; }

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

        /// <summary>
        /// Start page number for crawling
        /// </summary>
        int StartPageNo { get; set; }

        /// <summary>
        /// Maximal page number for crawling
        /// </summary>
        int MaxPageNo { get; set; }
        
        /// <summary>
        /// Page number modifier value
        /// </summary>
        int PageNoModifier { get; set; }

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

        /// <summary>
        /// Cancel the crawling process (if it is in progress)
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> = Canceled successfully.
        ///     <see langword="false"/> = Unable to cancel because it is not running.
        /// </returns>
        bool Cancel();

        // TODO: site, site_suffix, site_page, xpaths...
        //bool SetControls();

        #endregion
    }
}
