using HtmlAgilityPack;
using InformationRetrievalManager.Core;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// Crawler engineú scrapes the web data and parse them into files
    /// TODO: Make category and timestamp optional
    /// </summary>
    public class CrawlerEngine : ICrawlerEngine
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;
        private readonly ITaskManager _taskManager;
        private readonly ICrawlerStorage _crawlerStorage;

        #endregion

        #region Private Members

        /// <summary>
        /// It is used to cancel the crawler processing
        /// </summary>
        private bool _cancelationFlag = false;

        /// <summary>
        /// Private member for <see cref="CrawlingProgressPct"/>
        /// </summary>
        private short _crawlingProgressPct; //; ctor

        #endregion

        #region Interface Properties

        /// <inheritdoc/>
        public string NameIdentifier { get; private set; } //; ctor

        /// <inheritdoc/>
        public bool IsCurrentlyCrawling { get; private set; } //; ctor

        /// <inheritdoc/>
        public short CrawlingProgressPct
        {
            get => _crawlingProgressPct;
            private set
            {
                _crawlingProgressPct = value;
                if (value > 0)
                    OnProcessProgressEvent?.Invoke(this, new CrawlerEngineEventArgs { CrawlingProgressPct = CrawlingProgressPct });
            }
        }

        /// <inheritdoc/>
        public DateTime CrawlingTimestamp { get; private set; }

        /// <inheritdoc/>
        public int StartPageNo { get; private set; } = 1;

        /// <inheritdoc/>
        public int MaxPageNo { get; private set; } = 1;

        /// <inheritdoc/>
        public int PageNoModifier { get; private set; } = 1;

        /// <inheritdoc/>
        public string SiteAddress { get; private set; } = "";

        /// <inheritdoc/>
        public string SiteSuffix { get; private set; } = "";

        /// <inheritdoc/>
        public string FullSiteAddress => SiteAddress + SiteSuffix;

        /// <inheritdoc/>
        public bool IsSiteSet => !FullSiteAddress.IsNullOrEmpty();

        /// <inheritdoc/>
        public string SiteUrlArticlesXPath { get; private set; } = "";

        /// <inheritdoc/>
        public string SiteArticleContentAreaXPath { get; private set; } = "";

        /// <inheritdoc/>
        public string SiteArticleTitleXPath { get; private set; } = "";

        /// <inheritdoc/>
        public string SiteArticleCategoryXPath { get; private set; } = "";

        /// <inheritdoc/>
        public string SiteArticleDateTimeXPath { get; private set; } = "";

        /// <inheritdoc/>
        public DatetimeParseData SiteArticleDateTimeParseData { get; private set; } = new DatetimeParseData();

        /// <inheritdoc/>
        public int SearchInterval { get; private set; } = 1000;

        /// <inheritdoc/>
        public bool InterruptOnError { get; set; } = false;

        #endregion

        #region Interface Events

        /// <inheritdoc/>
        public event EventHandler OnStartProcessEvent;

        /// <inheritdoc/>
        public event EventHandler OnFinishProcessEvent;

        /// <inheritdoc/>
        public event EventHandler<CrawlerEngineEventArgs> OnProcessProgressEvent;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">It makes an unique identifier among all crawlers in this system</param>
        public CrawlerEngine(string name)
        {
            NameIdentifier = name ?? throw new ArgumentNullException(nameof(name));
            IsCurrentlyCrawling = false;
            CrawlingProgressPct = -1;

            // HACK: DI injections
            _logger = FrameworkDI.Logger ?? throw new ArgumentNullException(nameof(_logger));
            _fileManager = CoreDI.File ?? throw new ArgumentNullException(nameof(_fileManager));
            _taskManager = CoreDI.Task ?? throw new ArgumentNullException(nameof(_taskManager));
            _crawlerStorage = Framework.Service<ICrawlerStorage>() ?? throw new ArgumentNullException(nameof(_crawlerStorage));
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public string GenerateCrawlerSiteIdentificationToken()
        {
            int hc = FullSiteAddress.GetHashCode();
            return $"{NameIdentifier}_{(hc < 0 ? "1" : "0-")}{hc}";
        }

        /// <inheritdoc/>
        public bool Start()
        {
            // First, make sure the process is not running in this crawler...
            // ... the same check should be in process method due to separate process running and default value set in there
            if (IsCurrentlyCrawling)
                return false;

            // Raise the start event
            OnStartProcessEvent?.Invoke(null, EventArgs.Empty);

            // Run the process of crawling...
            _taskManager.RunAndForget(ProcessAsync);

            return true;
        }

        /// <inheritdoc/>
        public bool Cancel()
        {
            if (!IsCurrentlyCrawling)
                return false;

            _cancelationFlag = true;
            return true;
        }

        /// <inheritdoc/>
        public bool SetControls(
            string siteAddress, string siteSuffix,
            int startPageNo, int maxPageNo, int pageNoModifier,
            int searchInterval,
            string siteUrlArticlesXPath,
            string siteArticleContentAreaXPath,
            string siteArticleTitleXPath,
            string siteArticleCategoryXPath,
            string siteArticleDateTimeXPath, DatetimeParseData siteArticleDateTimeParseData
            )
        {
            // TODO: validate data
            SiteAddress = siteAddress;
            SiteSuffix = siteSuffix;
            StartPageNo = startPageNo;
            MaxPageNo = maxPageNo;
            PageNoModifier = pageNoModifier;
            SearchInterval = searchInterval;
            SiteUrlArticlesXPath = siteUrlArticlesXPath;
            SiteArticleContentAreaXPath = siteArticleContentAreaXPath;
            SiteArticleTitleXPath = siteArticleTitleXPath;
            SiteArticleCategoryXPath = siteArticleCategoryXPath;
            SiteArticleDateTimeXPath = siteArticleDateTimeXPath;
            SiteArticleDateTimeParseData = siteArticleDateTimeParseData;

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finish up crawler processing and put the crawler into a default state
        /// </summary>
        private void Finish()
        {
            // Turn off the flag once the crawling is finished
            IsCurrentlyCrawling = false;
            CrawlingProgressPct = -1;
            CrawlingTimestamp = default;
            _cancelationFlag = false;

            // Raise the finish event
            OnFinishProcessEvent?.Invoke(null, EventArgs.Empty);

            // Clean the event handlers
            OnStartProcessEvent = null;
            OnFinishProcessEvent = null;
            OnProcessProgressEvent = null;

            // Log it
            _logger.LogInformationSource($"Crawler '{NameIdentifier}' has finished.");
        }

        /// <summary>
        /// Process method of crawling
        /// </summary>
        private async Task ProcessAsync()
        {
            // Check if the process is not already running in this crawler...
            if (IsCurrentlyCrawling)
                return;
            // Init/start the crawling process
            /*
             *  It would be more clean to have this in Start method instead, 
             *  but this makes us sure, the crawler cannot throttle down due to separate process start failure.
             */
            IsCurrentlyCrawling = true;
            CrawlingProgressPct = 0;
            CrawlingTimestamp = DateTime.UtcNow;

            // Log it
            _logger.LogInformationSource($"Crawler '{NameIdentifier}' started processing.");

            // Check the basis...
            if (IsSiteSet)
            {
                HtmlWeb web = new HtmlWeb();

                // Get URLs
                HashSet<string> urls = await GetUrlsAsync(web);

                // Crawl the data
                await ProcessUrlsAsync(web, urls);
            }
            // Otherwise, we are unable to start the process...
            else
            {
                // Log it
                _logger.LogWarningSource($"Crawler '{NameIdentifier}' was unable to proceed with the process.");
            }

            // Finish the processing (it is important to call it at the end here)
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

            const short processPctValue = 20;

            const string hrefKeyword = "href";
            const string defaultArticleLink = "#";

            bool anyInvalidLinks = false;
            string urlsFilename = $"urls_{GenerateCrawlerSiteIdentificationToken()}_{StartPageNo}_{MaxPageNo}_{PageNoModifier}_{DateTime.UtcNow.Year}_{DateTime.UtcNow.Month}_{DateTime.UtcNow.Day}_{DateTime.UtcNow.Hour}.txt";
            string urlsFilePath = $"{Constants.CrawlerDataStorageDir}/{urlsFilename}";

            // Check if we have already scanned urls...
            if (File.Exists(urlsFilePath))
            {
                foreach (var line in _fileManager.ReadLines(urlsFilePath))
                    result.Add(line);

                // Calculaate progress pct
                CrawlingProgressPct = processPctValue;

                // Log it
                _logger.LogTraceSource($"Crawler '{NameIdentifier}' has loaded scanned URLs from the file storage.");
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

                    // Load the document
                    HtmlDocument doc = web.Load(FullSiteAddress.Replace("{0}", i.ToString()));
                    // Log it
                    _logger.LogTraceSource($"Crawler '{NameIdentifier}' is currently scanning '{web.ResponseUri}'.");

                    // Go through the specific page...
                    foreach (var item in doc.DocumentNode.SelectNodes(SiteUrlArticlesXPath))
                    {
                        // Check for cancelation
                        if (_cancelationFlag)
                            break;

                        if (item.HasAttributes)
                        {
                            string link = item.GetAttributeValue(hrefKeyword, defaultArticleLink);
                            result.Add(link);
                        }
                    }

                    // Calculate progress pct
                    CrawlingProgressPct = Convert.ToInt16(
                        (((i - StartPageNo) / PageNoModifier) + 1) / (double)((MaxPageNo - StartPageNo + PageNoModifier) / PageNoModifier) * processPctValue
                        );

                    await Task.Delay(SearchInterval);
                }

                // Save it to the file
                await _fileManager.WriteLinesToFileAsync(result.ToList(), urlsFilePath, false);

                // If invalid links are present...
                if (anyInvalidLinks)
                    _logger.LogInformationSource($"Crawler '{NameIdentifier}' found invalid links during scanning!");

                // Log it
                _logger.LogTraceSource($"Crawler '{NameIdentifier}' successfully finished the scanning process.");
            }

            return result;
        }

        /// <summary>
        /// Process scanned URLs and save them
        /// </summary>
        /// <param name="web">Web HtmlAgility instance</param>
        /// <param name="urls">The URLs</param>
        private async Task ProcessUrlsAsync(HtmlWeb web, HashSet<string> urls)
        {
            const short processPctValue = 80;

            short currentPctProgress = 0;
            short previousPctProgress = 0;

            int i = 0;
            foreach (var item in urls)
            {
                string url = item;

                // Check for cancelation
                if (_cancelationFlag)
                    break;

                // If the link is actually only the absolute path...
                if (!url.Contains(SiteAddress))
                    url = SiteAddress + url;

                // Check if the url is actual URL now...
                if (url.IsURL())
                {
                    // Load the document
                    HtmlDocument doc = web.Load(url);
                    // Log it
                    _logger.LogDebugSource($"Crawler '{NameIdentifier}' is currently processing URL '{web.ResponseUri}'.");

                    HtmlNode title = null, category = null, datetime = null, content = null;
                    if (string.IsNullOrEmpty(SiteArticleTitleXPath)) title = doc.DocumentNode.SelectNodes(SiteArticleTitleXPath).FirstOrDefault();
                    if (string.IsNullOrEmpty(SiteArticleCategoryXPath)) category = doc.DocumentNode.SelectNodes(SiteArticleCategoryXPath).FirstOrDefault();
                    if (string.IsNullOrEmpty(SiteArticleDateTimeXPath)) datetime = doc.DocumentNode.SelectNodes(SiteArticleDateTimeXPath).FirstOrDefault();
                    if (string.IsNullOrEmpty(SiteArticleContentAreaXPath)) content = doc.DocumentNode.SelectNodes(SiteArticleContentAreaXPath).FirstOrDefault();

                    // Make sure we found all needed HTML...
                    if (title != null
                        && category != null
                        && datetime != null
                        && content != null)
                    {
                        DateTime timestamp;
                        // Check fi the datetime is parsable...
                        if (DateTime.TryParseExact(datetime.InnerText.Trim(), SiteArticleDateTimeParseData.Format, SiteArticleDateTimeParseData.CultureInfo, System.Globalization.DateTimeStyles.None, out timestamp))
                        {
                            // Save data
                            await _crawlerStorage.SaveAsync(
                                crawler: this,
                                url: url,
                                title: MinifyText(title.InnerText).Trim(),
                                category: MinifyText(category.InnerText).Trim(),
                                timestamp: timestamp,
                                contentHtml: TidyfyText(content.InnerHtml),
                                contentTextMin: MinifyText(content.InnerText),
                                contentText: TidyfyText(content.InnerText)
                                );
                        }
                        else
                        {
                            _logger.LogTraceSource($"Crawler '{NameIdentifier}' cannot parse the article's datetime according to attached formatting!");

                            if (InterruptOnError)
                                break;
                        }
                    }
                    // Otherwise, catch the issue...
                    else
                    {
                        string invalidNodeNames = $"{(title == null ? $" {nameof(title)}" : "")}{(category == null ? $" {nameof(category)}" : "")}{(datetime == null ? $" {nameof(datetime)}" : "")}{(content == null ? $" {nameof(content)}" : "")}";
                        _logger.LogTraceSource($"Crawler '{NameIdentifier}' cannot find html node(s):{invalidNodeNames}");

                        if (InterruptOnError)
                            break;
                    }

                    // Calculate progress pct
                    currentPctProgress = Convert.ToInt16(Math.Round(i / (double)(urls.Count / 100.0) * (processPctValue / 100.0)));
                    CrawlingProgressPct += (short)(currentPctProgress - previousPctProgress);
                    previousPctProgress = currentPctProgress;

                    await Task.Delay(SearchInterval);
                }
                else
                {
                    _logger.LogTraceSource($"Crawler '{NameIdentifier}' indicates a wrong URL! '{url}'");

                    if (InterruptOnError)
                        break;
                }

                ++i;
            }
        }

        /// <summary>
        /// Minify the text into a compact form
        /// </summary>
        /// <param name="text">The text</param>
        /// <returns>Minified text</returns>
        private string MinifyText(string text)
        {
            return Regex.Replace(text.Trim(), @"\s+", " ");
        }

        /// <summary>
        /// Make the text more tidy! Remove all non necessary blank lines nad whitespaces
        /// </summary>
        /// <param name="text">The text</param>
        /// <returns>The tidy text</returns>
        private string TidyfyText(string text)
        {
            return Regex.Replace(Regex.Replace(text.Trim(), @"(^\p{Zs}*\r\n){2,}", "\r\n", RegexOptions.Multiline), @" +", " ");
        }

        #endregion
    }
}
