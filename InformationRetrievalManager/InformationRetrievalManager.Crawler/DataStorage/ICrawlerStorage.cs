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
        ///  It is not possible to save data if the crawler has not set site information.
        /// </summary>
        /// <param name="crawler">The crawler</param>
        /// <param name="url">Currently saved data's URL</param>
        /// <param name="title">Title</param>
        /// <param name="category">Category</param>
        /// <param name="timestamp">Datetime of the saved article</param>
        /// <param name="contentHtml">HTML version of content</param>
        /// <param name="contentTextMin">Minimized version of content</param>
        /// <param name="contentText">Normal (tidy) version of content</param>
        /// <exception cref="ArgumentNullException">Crawler is not defined.</exception>
        Task SaveAsync(ICrawlerEngine crawler, string url, string title, string category, DateTime timestamp, string contentHtml, string contentTextMin, string contentText);

        /// <summary>
        /// Find all data files related to specific crawler. 
        /// It is not possible to get the data if the crawler is currently processing.
        /// </summary>
        /// <param name="cid">The crawler identifier</param>
        /// <returns>Array of all data files (file paths).</returns>
        /// <exception cref="ArgumentNullException">Crawler ID is not defined.</exception>
        /// <exception cref="Exception">Unknown error - related to IO.</exception>
        /// <remarks>
        ///     Make sure to check the crawler is not crawling at the moment of getting data files.
        /// </remarks>
        string[] GetAllDataFiles(string cid);

        /// <summary>
        /// Delete files according to crawler identifier and file timestmap.
        /// </summary>
        /// <param name="cid">The crawler identifier.</param>
        /// <param name="fileTimestamp">The file timestamp.</param>
        /// <exception cref="ArgumentNullException">Crawler ID is not defined.</exception>
        /// <exception cref="Exception">Unknown error - related to IO.</exception>
        /// <remarks>
        ///     Make sure to check the crawler is not crawling at the moment of deleting data files.
        /// </remarks>
        void DeleteDataFiles(string cid, DateTime fileTimestamp);

        #endregion
    }
}
