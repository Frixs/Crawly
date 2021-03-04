using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;

namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// UNDONE
    /// </summary>
    public class CrawlerEngine : ICrawlerEngine
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;

        #endregion

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
        /// <param name="name">It makes an unique identifier among all crawlers in this system</param>
        public CrawlerEngine(string name)
        {
            Identifier = name ?? throw new ArgumentNullException(nameof(name));
            IsCurrentlyCrawlingFlag = false;
            CrawlingProgressPct = -1;

            _logger = FrameworkDI.Logger ?? throw new ArgumentNullException(nameof(_logger)); // HACK: DI injection
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
