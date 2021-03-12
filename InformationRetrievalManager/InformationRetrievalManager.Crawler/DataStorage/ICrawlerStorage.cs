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

        /// <summary>
        /// Find all data files related to specific crawler. 
        /// It is not possible to get the data if the crawler is currently processing.
        /// </summary>
        /// <param name="crawler">The crawler</param>
        /// <returns>Array of all data files (file paths) - if the crawler is processing, it returns <see langword="null"/>.</returns>
        string[] GetDataFiles(ICrawlerEngine crawler);

        #endregion
    }
}
