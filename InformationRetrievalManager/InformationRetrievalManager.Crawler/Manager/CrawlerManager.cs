using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// Base crawler manager
    /// </summary>
    public sealed class CrawlerManager : ICrawlerManager
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;

        #endregion

        #region Private Members

        /// <summary>
        ///  Dict of all crawlers the manager serves
        /// </summary>
        private Dictionary<string, ICrawlerEngine> _crawlers = new Dictionary<string, ICrawlerEngine>();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CrawlerManager(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public async Task<bool> AddCrawlerAsync(ICrawlerEngine crawler)
        {
            if (crawler == null)
                return false;

            // Lock the task
            return await AsyncLock.LockResultAsync(nameof(CrawlerManager), () =>
            {
                _crawlers.Add(crawler.NameIdentifier, crawler);
                _logger.LogTraceSource($"Crawler '{crawler.NameIdentifier}' has been added to the crawler manager successfully.");
                return true;
            });
        }

        /// <inheritdoc/>
        public async Task<ICrawlerEngine> GetCrawlerAsync(string cid)
        {
            if (cid == null)
                return null;

            // Lock the task
            return await AsyncLock.LockResultAsync(nameof(CrawlerManager), () =>
            {
                if (_crawlers.ContainsKey(cid))
                    return _crawlers[cid];
                return null;
            });
        }

        /// <inheritdoc/>
        public async Task<bool> RemoveCrawlerAsync(string cid)
        {
            if (cid == null)
                return false;

            // Lock the task
            return await AsyncLock.LockResultAsync(nameof(CrawlerManager), () =>
            {
                bool result = _crawlers.Remove(cid);

                if (result) _logger.LogTraceSource($"Crawler '{cid}' has been removed from the crawler manager.");
                else _logger.LogTraceSource($"Crawler '{cid}' could not be removed from the crawler manager.");

                return result;
            });
        }

        #endregion
    }
}
