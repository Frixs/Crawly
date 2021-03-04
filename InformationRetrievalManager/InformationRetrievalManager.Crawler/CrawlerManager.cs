using Ixs.DNA;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// Base crawler manager
    /// </summary>
    public class CrawlerManager : ICrawlerManager
    {
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
        public CrawlerManager()
        {
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
                return _crawlers.Remove(cid);
            });
        }

        #endregion
    }
}
