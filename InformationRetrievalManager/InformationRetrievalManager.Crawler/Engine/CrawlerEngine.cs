using HtmlAgilityPack;
using InformationRetrievalManager.Core;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

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
        public string Identifier { get; private set; } //; ctor

        /// <inheritdoc/>
        public bool IsCurrentlyCrawlingFlag { get; private set; } //; ctor

        /// <inheritdoc/>
        public short CrawlingProgressPct { get; private set; } //; ctor

        /// <inheritdoc/>
        public int StartPageNo { get; set; } = 1;

        /// <inheritdoc/>
        public int MaxPageNo { get; set; } = 1;

        /// <inheritdoc/>
        public int PageNoModifier { get; set; } = 1;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">It makes an unique identifier among all crawlers in this system</param>
        public CrawlerEngine(string name)
        {
            Identifier = name ?? throw new ArgumentNullException(nameof(name));
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
            _taskManager.RunAndForget(Process);

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
            _logger.LogTraceSource($"Crawler '{Identifier}' has finished.");
        }

        /// <summary>
        /// Process method of crawling
        /// HACK: Might be a good idea to move this into a completely separate class of engine processing
        /// </summary>
        private void Process()
        {
            const string hrefKeyword = "href";
            const string defaultArticleLink = "#";
            // TODO - move it on better location
            const string dataStorageSubfolder = "storage/";

            // Log it
            _logger.LogTraceSource($"Crawler '{Identifier}' started processing.");

            string compoundUrl = "https://www.sea.playblackdesert.com/News/Notice?boardType=2&Page={0}";

            HtmlWeb web = new HtmlWeb();
            HashSet<string> urls = new HashSet<string>();
            string urlsFilename = $"urls_{Identifier}_{compoundUrl.GetHashCode()}_{StartPageNo}_{MaxPageNo}_{PageNoModifier}.txt";

            // Check if we have already scanned urls...
            if (File.Exists(dataStorageSubfolder + urlsFilename))
            {
                foreach (var line in _fileManager.ReadLines(dataStorageSubfolder + urlsFilename))
                    urls.Add(line);
            }
            // Otherwise, scan the website...
            else
            {
                // Go through the pages of articles...
                for (int i = StartPageNo; i <= MaxPageNo; i += PageNoModifier)
                {
                    HtmlDocument doc = web.Load(compoundUrl.Replace("{0}", i.ToString()));

                    // Go through the specific page...
                    foreach (var item in doc.DocumentNode.SelectNodes("//article[@class='content']//ul[@class='thumb_nail_list']//a"))
                    {
                        // Check for cancelation
                        if (_cancelationFlag)
                            break;

                        if (item.HasAttributes)
                        {
                            // TODO: continue in processing .... now we have page links
                            Console.WriteLine(item.GetAttributeValue(hrefKeyword, defaultArticleLink));
                            // TODO save file
                            //urls.Add();
                        }
                    }

                    // Check for cancelation
                    if (_cancelationFlag)
                        break;
                }
            }

            // Finish the processing
            Finish();
        }

        #endregion
    }
}
