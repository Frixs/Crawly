using InformationRetrievalManager.Core;
using System;

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
        /// <remarks>Cannot be <see cref="null"/>. Should not be changeable. Should be valid for file/dir names.</remarks>
        string NameIdentifier { get; }

        /// <summary>
        /// Indicates if the engine is currently crawling data
        /// </summary>
        bool IsCurrentlyCrawling { get; }

        /// <summary>
        /// Percentage progress of current crawling
        /// </summary>
        /// <remarks>
        ///     If the crowling is not running currently, the result is '-1'
        /// </remarks>
        short CrawlingProgressPct { get; }

        /// <summary>
        /// Indicates the start of processing as a timestamp that could be useful to identify specific processing. 
        /// If the processing is not running the value is set to <see cref="default"/>
        /// </summary>
        DateTime CrawlingTimestamp { get; }

        /// <summary>
        /// The base size address
        /// </summary>
        /// <remarks>Cannot be <see cref="null"/>. Can contain wildchars to be replaced with page no. - more in <see cref="SetControls"/></remarks>
        string SiteAddress { get; }

        /// <summary>
        /// The suffix of <see cref="SiteAddress"/> address for more specific selection of page articles
        /// </summary>
        /// <remarks>Cannot be <see cref="null"/>. Can contain wildchars to be replaced with page no. - more in <see cref="SetControls"/></remarks>
        string SiteSuffix { get; }

        /// <summary>
        /// Return combination of <see cref="SiteAddress"/> and <see cref="SiteSuffix"/>.
        /// </summary>
        string FullSiteAddress { get; }

        /// <summary>
        /// Basic check/indication if the <see cref="FullSiteAddress"/> is set / ready to use
        /// </summary>
        bool IsSiteSet { get; }

        /// <summary>
        /// XPath to select URLs of articles
        /// </summary>
        string SiteUrlArticlesXPath { get; }

        /// <summary>
        /// XPath to content of the article
        /// </summary>
        string SiteArticleContentAreaXPath { get; }

        /// <summary>
        /// XPath to title of the article
        /// </summary>
        string SiteArticleTitleXPath { get; }

        /// <summary>
        /// XPath to category of the article
        /// </summary>
        string SiteArticleCategoryXPath { get; }

        /// <summary>
        /// XPath to date-time of the article
        /// </summary>
        string SiteArticleDateTimeXPath { get; }

        /// <summary>
        /// Date-time related parsing data (<see cref="SiteArticleDateTimeXPath"/>)
        /// </summary>
        DatetimeParseData SiteArticleDateTimeParseData { get; }

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

        /// <summary>
        /// Indicates if the crawler should interrupt processing on any error (TRUE) (e.g. failed to parse html) or simple continue and ignore it (FALSE)
        /// </summary>
        bool InterruptOnError { get; set; }

        #endregion

        #region Events

        /// <summary>
        /// Event raised on process start
        /// </summary>
        /// <remarks>
        ///     After each crawler processing, the event is meant to be set default
        /// </remarks>
        event EventHandler OnStartProcessEvent;

        /// <summary>
        /// Event raised on process finish
        /// </summary>
        /// <remarks>
        ///     After each crawler processing, the event is meant to be set default
        /// </remarks>
        event EventHandler OnFinishProcessEvent;

        /// <summary>
        /// Event raised each time process progress moves
        /// </summary>
        /// <remarks>
        ///     After each crawler processing, the event is meant to be set default
        /// </remarks>
        event EventHandler<CrawlerEngineEventArgs> OnProcessProgressEvent;

        #endregion

        #region Methods

        /// <summary>
        /// Generates string identifier based on <see cref="NameIdentifier"/> and <see cref="FullSiteAddress"/>
        /// </summary>
        /// <returns>Return string identifier token.</returns>
        /// <remarks>
        ///     Dirname base that identifies the crawler's data. 
        /// </remarks>
        string GenerateCrawlerSiteIdentificationToken();

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
        /// <param name="siteArticleContentAreaXPath">XPath for content of the article</param>
        /// <param name="siteArticleTitleXPath">XPath for title of the article</param>
        /// <param name="siteArticleCategoryXPath">XPath for category of the article</param>
        /// <param name="siteArticleDateTimeXPath">XPath for date-time of creation of the article</param>
        /// <param name="siteArticleDateTimeParseData">Parse data need for parsing the date-time</param>
        /// <returns>
        ///     <see langword="true"/> on successful set.
        ///     <see langword="false"/> on failure.
        /// </returns>
        /// <remarks>
        ///     <paramref name="siteAddress"/> and <paramref name="siteSuffix"/> may contain wildchars '{0}' that defines the place to replace it with real page number
        /// </remarks>
        bool SetControls(
            string siteAddress, string siteSuffix, 
            int startPageNo, int maxPageNo, int pageNoModifier, 
            int searchInterval, 
            string siteUrlArticlesXPath, 
            string siteArticleContentAreaXPath, 
            string siteArticleTitleXPath,
            string siteArticleCategoryXPath, 
            string siteArticleDateTimeXPath, DatetimeParseData siteArticleDateTimeParseData
            );

        #endregion
    }
}
