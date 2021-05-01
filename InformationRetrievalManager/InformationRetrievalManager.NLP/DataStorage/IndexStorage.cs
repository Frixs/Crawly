using InformationRetrievalManager.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Singleton boxing for managing index data
    /// </summary>
    public sealed class IndexStorage
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public IndexStorage(ILogger logger, IFileManager fileManager)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public string[] GetDataFiles(string cid)
        {
            if (cid == null)
                throw new ArgumentNullException("Index ID is not defined!");

            var result = new List<string>();

            if (Directory.Exists(Constants.IndexDataStorageDir))
            {
                // Get all crawler directories...
                string[] dirs = Directory.GetDirectories(Constants.IndexDataStorageDir);
                for (int i = 0; i < dirs.Length; ++i)
                    // Find the one specific for the searched index...
                    if (Path.GetFileName(dirs[i]).Equals(cid))
                    {
                        result.AddRange(
                            Directory.GetFiles(dirs[i])
                                .Where(o => o.EndsWith(".idx")).ToArray()
                            );
                    }
            }

            return result.ToArray();
        }

        /// <inheritdoc/>
        public void DeleteDataFiles(string cid, DateTime fileTimestamp)
        {
            if (cid == null)
                throw new ArgumentNullException("Index ID is not defined!");

            if (Directory.Exists(Constants.IndexDataStorageDir))
            {
                // Get all crawler directories...
                string[] dirs = Directory.GetDirectories(Constants.IndexDataStorageDir);
                for (int i = 0; i < dirs.Length; ++i)
                    // Find the one specific for the searched index...
                    if (Path.GetFileName(dirs[i]).Equals(cid))
                    {
                        var filesToDelete = Directory.GetFiles(dirs[i]).Where(o => o.Contains(MakeFilenameTimestamp(fileTimestamp))).ToList();
                        foreach (var file in filesToDelete)
                            File.Delete(file);
                    }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates filename timestamp.
        /// </summary>
        /// <returns>The filename timestamp</returns>
        private string MakeFilenameTimestamp(DateTime timestmap)
        {
            return $"{timestmap.Year}_{timestmap.Month}_{timestmap.Day}_{timestmap.Hour}_{timestmap.Minute}_{timestmap.Second}";
        }

        #endregion
    }
}
