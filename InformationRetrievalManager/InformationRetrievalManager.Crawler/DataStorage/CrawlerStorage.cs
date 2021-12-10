using InformationRetrievalManager.Core;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// Boxing for managing crawler data
    /// </summary>
    public sealed class CrawlerStorage : ICrawlerStorage
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;

        #endregion

        #region Private Members

        /// <summary>
        /// Filename base for HTML parsed data
        /// </summary>
        private const string _contentHtmlFilename = "html";

        /// <summary>
        /// Filename base for minified parsed data
        /// </summary>
        private const string _contentMinFilename = "min";

        /// <summary>
        /// Filename base for normal (tidy) based data
        /// </summary>
        private const string _contentTidyFilename = "tidy";

        /// <summary>
        /// Filename base for organized data
        /// </summary>
        private const string _dataFilename = "data";

        /// <summary>
        /// Offset (byte size) of the dated data that are being updated.
        /// </summary>
        /// <remarks>
        ///     -1 to set it to init state
        /// </remarks>
        private long _updateRequestDatedDataoffset = -1;

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
        public async Task SaveAsync(ICrawlerEngine crawler, string url, string title, string category, DateTime timestamp, 
            string contentHtml, string contentTextMin, string contentText)
        {
            if (crawler == null)
                throw new ArgumentNullException("Crawler is not defined!");

            if (!crawler.IsCurrentlyCrawling || !crawler.IsSiteSet)
                return;

            string crawledDataDirPath = $"{Constants.CrawlerDataStorageDir}/{crawler.GenerateCrawlerSiteIdentificationToken()}";

            // Check if the dir exists...
            if (!Directory.Exists(crawledDataDirPath))
                Directory.CreateDirectory(crawledDataDirPath);

            // Save data into files
            // HTML
            await _fileManager.WriteTextToFileAsync(
                url + Environment.NewLine + contentHtml + Environment.NewLine + Environment.NewLine + Environment.NewLine, 
                $"{crawledDataDirPath}/{MakeFilename(_contentHtmlFilename, crawler.CrawlingTimestamp)}",
                true
                );
            // Minified
            await _fileManager.WriteTextToFileAsync(
                url + Environment.NewLine + contentTextMin + Environment.NewLine + Environment.NewLine + Environment.NewLine, 
                $"{crawledDataDirPath}/{MakeFilename(_contentMinFilename, crawler.CrawlingTimestamp)}",
                true
                );
            // Tidyfied
            await _fileManager.WriteTextToFileAsync(
                url + Environment.NewLine + contentText + Environment.NewLine + Environment.NewLine + Environment.NewLine, 
                $"{crawledDataDirPath}/{MakeFilename(_contentTidyFilename, crawler.CrawlingTimestamp)}",
                true
                );

            // Prepare data for serialization...
            var model = new CrawlerDataModel
            {
                Title = title,
                SourceUrl = url,
                Category = category,
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

        /// <inheritdoc/>
        public void SaveUpdate(ICrawlerEngine crawler, string url, string title, string category, DateTime timestamp, 
            string contentText, string filePath)
        {
            if (crawler == null)
                throw new ArgumentNullException("Crawler is not defined!");

            if (!crawler.IsCurrentlyCrawling || !crawler.IsSiteSet)
                return;

            string crawledDataDirPath = $"{Constants.CrawlerDataStorageDir}/{crawler.GenerateCrawlerSiteIdentificationToken()}";
            string dataFilename = MakeFilename(_dataFilename, crawler.CrawlingTimestamp, "json");

            // Check if the dir exists...
            if (!Directory.Exists(crawledDataDirPath))
                Directory.CreateDirectory(crawledDataDirPath);

            // Create the base of JSON file, if the file does not exist...
            if (!File.Exists($"{crawledDataDirPath}/{dataFilename}"))
                // .. create it from the dated one
                File.Copy(filePath, $"{crawledDataDirPath}/{dataFilename}");

            // Init update request values
            if (_updateRequestDatedDataoffset < 0)
                _updateRequestDatedDataoffset = new FileInfo(filePath).Length;

            // Prepare data for serialization...
            var model = new CrawlerDataModel
            {
                Title = title,
                SourceUrl = url,
                Category = category,
                Timestamp = timestamp,
                Content = "yyy"//contentText
            };
            string json = JsonConvert.SerializeObject(model);

            // Append to JSON file
            using (var fs = new FileStream($"{crawledDataDirPath}/{dataFilename}", FileMode.Open))
            {
                // TODO !!! does not work
                var jsonByted = System.Text.Encoding.UTF8.GetBytes($"{json},");
                fs.Seek(1 - _updateRequestDatedDataoffset, SeekOrigin.End); // We are expecting, the first character is the array bracket
                fs.Write(jsonByted, 0, jsonByted.Length); // Include a leading comma character if required
                //fs.Write(System.Text.Encoding.UTF8.GetBytes("]"), 0, 1);
                fs.Seek(0, SeekOrigin.End);
                fs.SetLength(fs.Position);
            }
        }

        /// <inheritdoc/>
        public string[] GetAllDataFiles(string cid)
        {
            if (cid == null)
                throw new ArgumentNullException("Crawler ID is not defined!");

            var result = new List<string>();

            try
            {
                if (Directory.Exists(Constants.CrawlerDataStorageDir))
                {
                    // Get all crawler directories...
                    string[] dirs = Directory.GetDirectories(Constants.CrawlerDataStorageDir);
                    for (int i = 0; i < dirs.Length; ++i)
                        // Find the one specific for the searched crawler...
                        if (Path.GetFileName(dirs[i]).StartsWith(cid))
                        {
                            result.AddRange(
                                Directory.GetFiles(dirs[i])
                                );
                        }
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorSource($"{ex.GetType()}: {ex.Message}");
            }

            return result.ToArray();
        }

        /// <inheritdoc/>
        public void DeleteDataFiles(string cid, DateTime fileTimestamp)
        {
            if (cid == null)
                throw new ArgumentNullException("Crawler ID is not defined!");

            try
            {
                if (Directory.Exists(Constants.CrawlerDataStorageDir))
                {
                    // Get all crawler directories...
                    string[] dirs = Directory.GetDirectories(Constants.CrawlerDataStorageDir);
                    for (int i = 0; i < dirs.Length; ++i)
                        // Find the one specific for the searched crawler...
                        if (Path.GetFileName(dirs[i]).StartsWith(cid))
                        {
                            var filesToDelete = Directory.GetFiles(dirs[i]).Where(o => o.Contains(MakeFilename("", fileTimestamp, ""))).ToList();
                            foreach (var file in filesToDelete)
                                File.Delete(file);
                        }
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorSource($"{ex.GetType()}: {ex.Message}");
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
