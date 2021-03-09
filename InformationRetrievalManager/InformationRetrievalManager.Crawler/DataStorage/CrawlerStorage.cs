using InformationRetrievalManager.Core;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// Singleton boxing for managing crawler data
    /// </summary>
    public class CrawlerStorage : ICrawlerStorage
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;

        #endregion

        #region Private Members

        /// <summary>
        /// Filename base for HTML parsed data
        /// </summary>
        private string _contentHtmlFilename = "html";

        /// <summary>
        /// Filename base for minified parsed data
        /// </summary>
        private string _contentMinFilename = "min";

        /// <summary>
        /// Filename base for normal (tidy) based data
        /// </summary>
        private string _contentTidyFilename = "tidy";

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CrawlerStorage(ILogger logger, IFileManager fileManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public async Task SaveAsync(ICrawlerEngine crawler, string url, string title, string datetime, string contentHtml, string contentTextMin, string contentText)
        {
            if (!crawler.IsCurrentlyCrawlingFlag)
                return;

            string crawledDataDirPath = $"{Constants.CrawlerDataStorageDir}/{crawler.CurrentSiteDataIdentification}";

            // Check if the dir exists...
            if (!Directory.Exists(crawledDataDirPath))
                Directory.CreateDirectory(crawledDataDirPath);

            // Save data into files
            // HTML
            await _fileManager.WriteTextToFileAsync(
                url + Environment.NewLine + contentHtml + Environment.NewLine + Environment.NewLine + Environment.NewLine, $"{crawledDataDirPath}/{MakeFilename(_contentHtmlFilename, crawler.CrawlingTimestamp)}", 
                true
                );
            // Minified
            await _fileManager.WriteTextToFileAsync(
                url + Environment.NewLine + contentTextMin + Environment.NewLine + Environment.NewLine + Environment.NewLine, $"{crawledDataDirPath}/{MakeFilename(_contentMinFilename, crawler.CrawlingTimestamp)}", 
                true
                );
            // Tidyfied
            await _fileManager.WriteTextToFileAsync(
                url + Environment.NewLine + contentText + Environment.NewLine + Environment.NewLine + Environment.NewLine, $"{crawledDataDirPath}/{MakeFilename(_contentTidyFilename, crawler.CrawlingTimestamp)}", 
                true
                );
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates filename based on the base filename
        /// </summary>
        /// <param name="filename">The filename base</param>
        /// <returns>Created filename for use</returns>
        private string MakeFilename(string filename, DateTime datetime)
        {
            return $"{filename}_{datetime.Year}_{datetime.Month}_{datetime.Day}_{datetime.Hour}_{datetime.Minute}_{datetime.Second}.txt";
        }

        #endregion
    }
}
