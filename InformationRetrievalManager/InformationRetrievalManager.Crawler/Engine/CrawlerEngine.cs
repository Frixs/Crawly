using HtmlAgilityPack;
using InformationRetrievalManager.Core;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// Crawler engineú scrapes the web data and parse them into files
    /// </summary>
    public class CrawlerEngine : ICrawlerEngine
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;
        private readonly ITaskManager _taskManager;

        #endregion

        #region Private Members

        /// <summary>
        /// It is used to cancel the crawler processing
        /// </summary>
        private bool _cancelationFlag = false;

        #endregion

        #region Interface Properties

        /// <inheritdoc/>
        public string NameIdentifier { get; private set; } //; ctor

        /// <inheritdoc/>
        public bool IsCurrentlyCrawlingFlag { get; private set; } //; ctor

        /// <inheritdoc/>
        public short CrawlingProgressPct { get; private set; } //; ctor

        /// <inheritdoc/>
        public int StartPageNo { get; private set; } = 1;

        /// <inheritdoc/>
        public int MaxPageNo { get; private set; } = 1;

        /// <inheritdoc/>
        public int PageNoModifier { get; private set; } = 1;

        /// <inheritdoc/>
        public string SiteAddress { get; private set; } = null;

        /// <inheritdoc/>
        public string SiteSuffix { get; private set; } = null;

        /// <inheritdoc/>
        public string SiteUrlArticlesXPath { get; private set; } = "";

        /// <inheritdoc/>
        public int SearchInterval { get; private set; } = 1000;

        #endregion

        #region Public Properties

        /// <summary>
        /// Basic check/indication if the site is set / ready to use
        /// </summary>
        public bool IsSiteSet => !SiteAddress.IsNullOrEmpty() && !SiteSuffix.IsNullOrEmpty();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">It makes an unique identifier among all crawlers in this system</param>
        public CrawlerEngine(string name)
        {
            NameIdentifier = name ?? throw new ArgumentNullException(nameof(name));
            IsCurrentlyCrawlingFlag = false;
            CrawlingProgressPct = -1;

            // HACK: DI injections
            _logger = FrameworkDI.Logger ?? throw new ArgumentNullException(nameof(_logger));
            _fileManager = CoreDI.File ?? throw new ArgumentNullException(nameof(_fileManager));
            _taskManager = CoreDI.Task ?? throw new ArgumentNullException(nameof(_taskManager));
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public bool Start()
        {
            if (IsCurrentlyCrawlingFlag)
                return false;
            IsCurrentlyCrawlingFlag = true;

            // Run the process of crawling...
            _taskManager.RunAndForget(ProcessAsync);

            return true;
        }

        /// <inheritdoc/>
        public bool Cancel()
        {
            if (!IsCurrentlyCrawlingFlag)
                return false;

            _cancelationFlag = true;
            return true;
        }

        /// <inheritdoc/>
        public bool SetControls(string siteAddress, string siteSuffix, int startPageNo, int maxPageNo, int pageNoModifier, int searchInterval, string siteUrlArticlesXPath)
        {
            // TODO: validate data
            SiteAddress = siteAddress;
            SiteSuffix = siteSuffix;
            StartPageNo = startPageNo;
            MaxPageNo = maxPageNo;
            PageNoModifier = pageNoModifier;
            SearchInterval = searchInterval;
            SiteUrlArticlesXPath = siteUrlArticlesXPath;

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finish up crawler processing
        /// </summary>
        private void Finish()
        {
            // Turn off the flag once the crawling is finished
            IsCurrentlyCrawlingFlag = false;
            _cancelationFlag = false;

            // Log it
            _logger.LogInformationSource($"Crawler '{NameIdentifier}' has finished.");
        }

        /// <summary>
        /// Process method of crawling
        /// HACK: Might be a good idea to move this into a completely separate class of engine processing
        /// </summary>
        private async Task ProcessAsync()
        {
            // Log it
            _logger.LogInformationSource($"Crawler '{NameIdentifier}' started processing.");

            // Check the basis...
            if (IsSiteSet)
            {
                HtmlWeb web = new HtmlWeb();

                HashSet<string> urls = await GetUrlsAsync(web);

                // TODO
            }
            // Otherwise, we are unable to start the process...
            else
            {
                // Log it
                _logger.LogInformationSource($"Crawler '{NameIdentifier}' was unable to proceed with the process.");
            }

            // Finish the processing
            Finish();
        }

        /// <summary>
        /// Get urls by crawling it or by loading it from already crawled data source
        /// </summary>
        /// <param name="web">Web HtmlAgility instance</param>
        /// <returns>Set of the URLs</returns>
        private async Task<HashSet<string>> GetUrlsAsync(HtmlWeb web)
        {
            HashSet<string> result = new HashSet<string>();

            const string hrefKeyword = "href";
            const string defaultArticleLink = "#";

            string compoundUrl = SiteAddress + SiteSuffix;
            string urlsFilename = $"urls_{NameIdentifier}_{compoundUrl.GetHashCode()}_{StartPageNo}_{MaxPageNo}_{PageNoModifier}_{DateTime.Today.Year}_{DateTime.Today.Month}_{DateTime.Today.Day}.txt";
            string urlsFilePath = $"{Constants.DataStorageDir}/{urlsFilename}";

            // Check if we have already scanned urls...
            if (File.Exists(urlsFilePath))
            {
                foreach (var line in _fileManager.ReadLines(urlsFilePath))
                    result.Add(line);
            }
            // Otherwise, scan the website...
            else
            {
                // Go through the pages of articles...
                for (int i = StartPageNo; i <= MaxPageNo; i += PageNoModifier)
                {
                    // Check for cancelation
                    if (_cancelationFlag)
                        break;

                    HtmlDocument doc = web.Load(compoundUrl.Replace("{0}", i.ToString()));

                    // Go through the specific page...
                    foreach (var item in doc.DocumentNode.SelectNodes(SiteUrlArticlesXPath))
                    {
                        // Check for cancelation
                        if (_cancelationFlag)
                            break;

                        if (item.HasAttributes)
                            result.Add(item.GetAttributeValue(hrefKeyword, defaultArticleLink));
                    }

                    await Task.Delay(SearchInterval);
                }

                // Save it to the file
                await _fileManager.WriteLinesToFileAsync(result.ToList(), urlsFilePath, false);
            }

            return result;
        }

        #endregion
    }
}
