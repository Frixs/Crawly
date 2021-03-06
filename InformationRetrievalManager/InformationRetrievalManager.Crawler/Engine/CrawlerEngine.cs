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
        public string SiteArticleContentAreaXPath { get; private set; } = "";

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
            CrawlingProgressPct = 0;

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
        public bool SetControls(string siteAddress, string siteSuffix, int startPageNo, int maxPageNo, int pageNoModifier, int searchInterval, string siteUrlArticlesXPath, string siteArticleContentAreaXPath)
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
        /// </summary>
        private async Task ProcessAsync()
        {
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

            const short processPctValue = 25;

            const string hrefKeyword = "href";
            const string defaultArticleLink = "#";

            bool anyInvalidLinks = false;
            string compoundUrl = SiteAddress + SiteSuffix;
            string urlsFilename = $"urls_{NameIdentifier}_{compoundUrl.GetHashCode()}_{StartPageNo}_{MaxPageNo}_{PageNoModifier}_{DateTime.UtcNow.Year}_{DateTime.UtcNow.Month}_{DateTime.UtcNow.Day}_{DateTime.UtcNow.Hour}.txt";
            string urlsFilePath = $"{Constants.DataStorageDir}/{urlsFilename}";

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
                    HtmlDocument doc = web.Load(compoundUrl.Replace("{0}", i.ToString()));
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
        /// Process scanned URLs
        /// </summary>
        /// <param name="web">Web HtmlAgility instance</param>
        /// <param name="urls">The URLs</param>
        private async Task ProcessUrlsAsync(HtmlWeb web, HashSet<string> urls)
        {
            const short processPctValue = 75;

            short currentPctProgress = 0;
            short previousPctProgress = 0;

            string compoundUrl = SiteAddress + SiteSuffix;
            string dirPath = $"{Constants.DataStorageDir}/{NameIdentifier}_{compoundUrl.GetHashCode()}";
            string htmlFilename = $"html_{DateTime.UtcNow.Year}_{DateTime.UtcNow.Month}_{DateTime.UtcNow.Day}_{DateTime.UtcNow.Hour}_{DateTime.UtcNow.Minute}_{DateTime.UtcNow.Second}.txt";
            string textFilename = $"text_{DateTime.UtcNow.Year}_{DateTime.UtcNow.Month}_{DateTime.UtcNow.Day}_{DateTime.UtcNow.Hour}_{DateTime.UtcNow.Minute}_{DateTime.UtcNow.Second}.txt";
            string tidytextFilename = $"tidytext_{DateTime.UtcNow.Year}_{DateTime.UtcNow.Month}_{DateTime.UtcNow.Day}_{DateTime.UtcNow.Hour}_{DateTime.UtcNow.Minute}_{DateTime.UtcNow.Second}.txt";

            int i = 0;
            foreach (var item in urls)
            {
                string url = item;

                // If the link is actually only the absolute path...
                if (!url.Contains(SiteAddress))
                    url = SiteAddress + url;

                // Check if the url is actual URL now...
                if (url.IsURL())
                {
                    // Load the document
                    HtmlDocument doc = web.Load(url);
                    // Log it
                    _logger.LogTraceSource($"Crawler '{NameIdentifier}' is currently processing URL '{web.ResponseUri}'.");

                    var content = doc.DocumentNode.SelectNodes(SiteArticleContentAreaXPath).FirstOrDefault();
                    if (content != null)
                    {
                        string html = content.InnerHtml;
                        string minifiedText = MinifyText(content.InnerText);
                        string tidyText = TidyfyText(content.InnerText);

                        // Check if the dir exists...
                        if (!Directory.Exists(dirPath))
                            Directory.CreateDirectory(dirPath);

                        // Save data into files
                        await _fileManager.WriteTextToFileAsync(url + Environment.NewLine + html + Environment.NewLine, $"{dirPath}/{htmlFilename}", true);
                        await _fileManager.WriteTextToFileAsync(url + Environment.NewLine + minifiedText + Environment.NewLine, $"{dirPath}/{textFilename}", true);
                        await _fileManager.WriteTextToFileAsync(url + Environment.NewLine + tidyText + Environment.NewLine, $"{dirPath}/{tidytextFilename}", true);

                        // Calculate progress pct
                        currentPctProgress = Convert.ToInt16(Math.Round(i / (double)(urls.Count / 100.0) * (processPctValue / 100.0)));
                        CrawlingProgressPct += (short)(currentPctProgress - previousPctProgress);
                        previousPctProgress = currentPctProgress;
                        Console.WriteLine(CrawlingProgressPct);
                    }

                    await Task.Delay(SearchInterval);
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
