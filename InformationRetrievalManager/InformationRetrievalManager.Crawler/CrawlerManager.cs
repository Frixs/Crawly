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
    public class CrawlerManager : ICrawlerManager
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
            return await AsyncLock.LockResultAsync(nameof(CrawlerManager), async () =>
            {
                _crawlers.Add(crawler.Identifier, crawler);
                _logger.LogDebugSource($"Crawler '{crawler.Identifier}' has been added to the crawler manager successfully.");
                return true;
            });
        }

        /// <inheritdoc/>
        public async Task<ICrawlerEngine> GetCrawlerAsync(string cid)
        {
            if (cid == null)
                return null;

            // Lock the task
            return await AsyncLock.LockResultAsync(nameof(CrawlerManager), async () =>
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
            return await AsyncLock.LockResultAsync(nameof(CrawlerManager), async () =>
            {
                bool result = _crawlers.Remove(cid);

                if (result) _logger.LogDebugSource($"Crawler '{cid}' has been removed from the crawler manager.");
                else _logger.LogDebugSource($"Crawler '{cid}' could not be removed from the crawler manager.");

                return result;
            });
        }

        #endregion
    }
}
