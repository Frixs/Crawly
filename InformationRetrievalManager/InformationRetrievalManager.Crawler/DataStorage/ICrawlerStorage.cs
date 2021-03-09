using System;
using System.Threading.Tasks;

namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// Crawler storage interface for manipulating with saved crawler data
    /// </summary>
    public interface ICrawlerStorage
    {
        #region Methods

        /// <summary>
        /// Saves crawled data into specific files.
        /// It is not possible to save data if the crawler is not currently processing any data that could be saved.
        /// </summary>
        /// <param name="crawler">The crawler</param>
        /// <param name="url">Currently saved data's URL</param>
        /// <param name="title">Title</param>
        /// <param name="timestamp">Datetime of the saved article</param>
        /// <param name="contentHtml">HTML version of content</param>
        /// <param name="contentTextMin">Minimized version of content</param>
        /// <param name="contentText">Normal (tidy) version of content</param>
        Task SaveAsync(ICrawlerEngine crawler, string url, string title, DateTime timestamp, string contentHtml, string contentTextMin, string contentText);

        #endregion
    }
}
