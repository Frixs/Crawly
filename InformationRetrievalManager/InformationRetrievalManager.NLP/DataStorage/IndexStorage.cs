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
                                    .Where(o => !Path.GetFileName(o).StartsWith("_") && o.EndsWith(".idx")).ToArray()
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
        public string[] GetFiles(string iid)
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
                                    .Where(o => o.EndsWith(".idx")).ToArray()
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
        public void DeleteIndexedDataFiles(string iid, DateTime fileTimestamp)
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
                            var filesToDelete = Directory.GetFiles(dirs[i]).Where(o => Path.GetFileName(o).StartsWith("_") && o.Contains(MakeFilenameTimestamp(fileTimestamp))).ToList();
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
        public void UpdateFilenames(string iid, DateTime oldFileTimestamp, DateTime newFileTimestamp)
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

        /// <inheritdoc/>
        public void SerializeIndexedDataIntoFile<T>(string iid, DateTime fileTimestamp, string typeIdentifier, T obj)
        {
            if (iid == null || typeIdentifier == null)
                throw new ArgumentNullException("Index ID or type ID is not defined!");

            // Serialize
            short status = _fileManager.SerializeObjectToBinFileAsync(obj, $"{Constants.IndexDataStorageDir}/{iid}/_{iid}_{MakeFilenameTimestamp(fileTimestamp)}_{typeIdentifier}.idx").Result;

            // Check serialization result
            if (status == 0)
                _logger?.LogDebugSource($"Successfully saved indexed data of '{iid}' ('{typeIdentifier}').");
            else if (status == 2)
                _logger?.LogWarningSource($"Failed to save index '{iid}' ('{typeIdentifier}'). Invalid data.");
            else
                _logger?.LogWarningSource($"Failed to save index '{iid}' ('{typeIdentifier}'). The index failed to write data in a file.");
        }

        /// <inheritdoc/>
        public byte DeserializeIndexedDataFromFile<T>(string iid, DateTime fileTimestamp, string typeIdentifier, out T result)
        {
            if (iid == null || typeIdentifier == null)
                throw new ArgumentNullException("Index ID or type ID is not defined!");

            T obj = default;

            // Deserialize
            var fileResult = _fileManager.DeserializeObjectFromBinFileAsync<T>($"{Constants.IndexDataStorageDir}/{iid}/_{iid}_{MakeFilenameTimestamp(fileTimestamp)}_{typeIdentifier}.idx").Result;
            short status = fileResult.ResultStatus;
            obj = fileResult.ResultData;
            // Check deserialization result
            if (status == 0)
                _logger?.LogDebugSource($"Successfully loaded indexed data of '{iid}' ('{typeIdentifier}').");
            else if (status == 1)
            { /* file does not exist */ }
            else if (status == 2)
                _logger?.LogWarningSource($"Failed to load index '{iid}' ('{typeIdentifier}'). Corrupted data.");
            else
                _logger?.LogWarningSource($"Failed to load index '{iid}' ('{typeIdentifier}'). Unknown error.");

            // Result
            if (status == 0)
            {
                result = obj;
                return 0;
            }
            else
            {
                result = default;
                return 1;
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
