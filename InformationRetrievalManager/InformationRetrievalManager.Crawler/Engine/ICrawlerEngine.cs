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
        /// <remarks>Should not be changeable.</remarks>
        string NameIdentifier { get; }

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
        /// The base size address
        /// </summary>
        /// <remarks>Can contain wildchars to be replaced with page no. - more in <see cref="SetControls"/></remarks>
        string SiteAddress { get; }

        /// <summary>
        /// The suffix of the site address for more specific selection of page articles
        /// </summary>
        /// <remarks>Can contain wildchars to be replaced with page no. - more in <see cref="SetControls"/></remarks>
        string SiteSuffix { get; }

        /// <summary>
        /// XPath to select URLs of articles
        /// </summary>
        string SiteUrlArticlesXPath { get; }

        /// <summary>
        /// Start page number for crawling
        /// </summary>
        int StartPageNo { get; }

        /// <summary>
        /// Maximal page number for crawling
        /// </summary>
        int MaxPageNo { get; }
        
        /// <summary>
        /// Page number modifier value
        /// </summary>
        int PageNoModifier { get; }

        /// <summary>
        /// Delay between search tasks (ms)
        /// </summary>
        int SearchInterval { get; }

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

        /// <summary>
        /// Set control items
        /// </summary>
        /// <param name="siteAddress">Site address</param>
        /// <param name="siteSuffix">Site suffix</param>
        /// <param name="startPageNo">Start/minimal page number</param>
        /// <param name="maxPageNo">Maximal/the last page number</param>
        /// <param name="pageNoModifier">Modifier which is used to increment the page number</param>
        /// <param name="searchInterval">Search interval to do not load the web servers too much</param>
        /// <param name="siteUrlArticlesXPath">XPath for searching URL article links</param>
        /// <returns>
        ///     <see langword="true"/> on successful set.
        ///     <see langword="false"/> on failure.
        /// </returns>
        /// <remarks>
        ///     <paramref name="siteAddress"/> and <paramref name="siteSuffix"/> may contain wildchars '{0}' that defines the place to replace it with real page number
        /// </remarks>
        bool SetControls(string siteAddress, string siteSuffix, int startPageNo, int maxPageNo, int pageNoModifier, int searchInterval, string siteUrlArticlesXPath);

        #endregion
    }
}
