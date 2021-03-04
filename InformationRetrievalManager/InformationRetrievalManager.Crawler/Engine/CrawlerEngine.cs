using System;

namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// UNDONE
    /// </summary>
    class CrawlerEngine : ICrawlerEngine
    {
        #region Interface Properties

        /// <inheritdoc/>
        public string Identifier { get; private set; } //; ctor

        /// <inheritdoc/>
        public bool IsCurrentlyCrawlingFlag { get; private set; } //; ctor

        /// <inheritdoc/>
        public short CrawlingProgressPct { get; private set; } //; ctor

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name">It makes a unique identifier among all crawlers in this system</param>
        public CrawlerEngine(string name)
        {
            Identifier = name ?? throw new ArgumentNullException(nameof(name));
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
            Console.WriteLine($"TODO: crawler start '{Identifier}'");

            return true;
        }

        #endregion
    }
}
