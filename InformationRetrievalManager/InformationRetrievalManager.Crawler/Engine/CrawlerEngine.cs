using HtmlAgilityPack;
using InformationRetrievalManager.Core;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
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

            // Log it
            _logger.LogTraceSource($"Crawler '{Identifier}' started processing.");

            HtmlWeb web = new HtmlWeb();
            // Go through the pages of articles...
            for (int i = StartPageNo; i <= MaxPageNo; i += PageNoModifier)
            {
                HtmlDocument doc = web.Load("https://www.sea.playblackdesert.com/News/Notice?boardType=2&Page=" + i);

                // Go through the specific page...
                foreach (var item in doc.DocumentNode.SelectNodes("//article[@class='content']//ul[@class='thumb_nail_list']//a"))
                {
                    // TODO: continue in processing .... now we have page links
                    if (item.HasAttributes)
                        Console.WriteLine(item.GetAttributeValue(hrefKeyword, defaultArticleLink));
                }
            }

            // Finish the processing
            Finish();
        }

        #endregion
    }
}
