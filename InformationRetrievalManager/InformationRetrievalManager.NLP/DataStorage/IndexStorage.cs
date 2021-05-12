using InformationRetrievalManager.Core;
using Ixs.DNA;
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
    public sealed class IndexStorage : IIndexStorage
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
        public string[] GetIndexFiles(string iid)
        {
            if (iid == null)
                throw new ArgumentNullException("Index ID is not defined!");

            var result = new List<string>();

            try
            {
                if (Directory.Exists(Constants.IndexDataStorageDir))
                {
                    // Get all index directories...
                    string[] dirs = Directory.GetDirectories(Constants.IndexDataStorageDir);
                    for (int i = 0; i < dirs.Length; ++i)
                        // Find the one specific for the searched index...
                        if (Path.GetFileName(dirs[i]).Equals(iid))
                        {
                            result.AddRange(
                                Directory.GetFiles(dirs[i])
                                    .Where(o => !o.StartsWith("_") && o.EndsWith(".idx")).ToArray()
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
        public void DeleteFiles(string iid, DateTime fileTimestamp)
        {
            if (iid == null)
                throw new ArgumentNullException("Index ID is not defined!");

            try
            {
                if (Directory.Exists(Constants.IndexDataStorageDir))
                {
                    // Get all index directories...
                    string[] dirs = Directory.GetDirectories(Constants.IndexDataStorageDir);
                    for (int i = 0; i < dirs.Length; ++i)
                        // Find the one specific for the searched index...
                        if (Path.GetFileName(dirs[i]).Equals(iid))
                        {
                            var filesToDelete = Directory.GetFiles(dirs[i]).Where(o => o.Contains(MakeFilenameTimestamp(fileTimestamp))).ToList();
                            foreach (var file in filesToDelete)
                                File.Delete(file);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                _logger.LogErrorSource($"{ex.GetType()}: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public void UpdateIndexFilename(string iid, DateTime oldFileTimestamp, DateTime newFileTimestamp)
        {
            if (iid == null)
                throw new ArgumentNullException("Index ID is not defined!");

            try
            {
                if (Directory.Exists(Constants.IndexDataStorageDir))
                {
                    // Get all index directories...
                    string[] dirs = Directory.GetDirectories(Constants.IndexDataStorageDir);
                    for (int i = 0; i < dirs.Length; ++i)
                        // Find the one specific for the searched index...
                        if (Path.GetFileName(dirs[i]).Equals(iid))
                        {
                            var filesToDelete = Directory.GetFiles(dirs[i]).Where(o => o.Contains(MakeFilenameTimestamp(oldFileTimestamp))).ToList();
                            foreach (var file in filesToDelete)
                                File.Move(file, file.Replace(MakeFilenameTimestamp(oldFileTimestamp), MakeFilenameTimestamp(newFileTimestamp)));
                            break;
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
