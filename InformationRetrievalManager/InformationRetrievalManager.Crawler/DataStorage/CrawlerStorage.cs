using InformationRetrievalManager.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

        /// <summary>
        /// Filename base for organized data
        /// </summary>
        private string _dataFilename = "data";

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
        public async Task SaveAsync(ICrawlerEngine crawler, string url, string title, DateTime timestamp, string contentHtml, string contentTextMin, string contentText)
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

            // Prepare data for serialization...
            var model = new CrawlerDataModel
            {
                Title = title,
                Timestamp = timestamp,
                Content = contentText
            };
            string json = JsonConvert.SerializeObject(model);

            // Create the base of JSON file, if the file does not exist...
            if (!File.Exists($"{crawledDataDirPath}/{MakeFilename(_dataFilename, crawler.CrawlingTimestamp, "json")}"))
                await _fileManager.WriteTextToFileAsync("[]", $"{crawledDataDirPath}/{MakeFilename(_dataFilename, crawler.CrawlingTimestamp, "json")}", false);

            // Append to JSON file
            using (var fs = new FileStream($"{crawledDataDirPath}/{MakeFilename(_dataFilename, crawler.CrawlingTimestamp, "json")}", FileMode.Open))
            {
                var jsonByted = System.Text.Encoding.UTF8.GetBytes($"{json},");
                fs.Seek(-1, SeekOrigin.End);
                fs.Write(jsonByted, 0, jsonByted.Length); // Include a leading comma character if required
                fs.Write(System.Text.Encoding.UTF8.GetBytes("]"), 0, 1);
                fs.SetLength(fs.Position); // Only needed if new content may be smaller than old
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates filename based on the base filename
        /// </summary>
        /// <param name="filename">The filename base</param>
        /// <param name="datetime">Datetime to set timestamp</param>
        /// <param name="ext">extension (without the dot)</param>
        /// <returns>Created filename for use</returns>
        private string MakeFilename(string filename, DateTime datetime, string ext = "txt")
        {
            return $"{filename}_{datetime.Year}_{datetime.Month}_{datetime.Day}_{datetime.Hour}_{datetime.Minute}_{datetime.Second}.{ext}";
        }

        #endregion
    }
}
