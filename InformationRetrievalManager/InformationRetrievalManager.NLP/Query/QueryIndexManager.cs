using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Manager for querying data above idnexed data.
    /// </summary>
    public sealed class QueryIndexManager : IQueryIndexManager
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;

        #endregion

        #region Private Members (Model Data)

        /// <summary>
        /// Model type of last queried data
        /// </summary>
        private QueryModelType? _lastModelType = null;

        /// <summary>
        /// Model reference (if used last time - depends on <see cref="_lastModelType"/>)
        /// </summary>
        private IQueryModel _lastModel = null;

        /// <summary>
        /// Query reference (if used last time - depends on <see cref="_lastModelType"/>)
        /// </summary>
        private string _lastQuery = null;

        /// <summary>
        /// Data checksum of last queried data
        /// </summary>
        private byte[] _lastDataChecksum = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public QueryIndexManager(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public async Task<int[]> QueryAsync(string query, IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> data, QueryModelType modelType, IndexProcessingConfigurationDataModel configuration, int select = 0)
        {
            if (query == null || data == null)
                throw new ArgumentNullException("Query data not specified!");

            if (select < 0)
                throw new InvalidCastException($"Parameter '{nameof(select)}' cannot be negative number!");

            // Lock the task.
            return await AsyncLock.LockResultAsync(nameof(QueryIndexManager) + nameof(QueryAsync), () =>
            {
                var t_query = query;
                var t_data = data;
                var t_modelType = modelType;
                var t_configuration = configuration;

                var dataChecksum = GetDataChecksum(t_data);

                // If the model type is the same as the one from the last query request...
                if (t_modelType == _lastModelType)
                {
                    // If the data are equal to the last used one...
                    if (dataChecksum.SequenceEqual(_lastDataChecksum))
                    {
                        // If so, the data are equal to the last one...
                        // ... check if the query is different...
                        if (!t_query.Equals(_lastQuery))
                            // If so, recalculate query
                            _lastModel.CalculateQuery(t_query, t_data, t_configuration);
                        // Otherwise, there is not need to do anything, the query data are the same as the previous request.
                    }
                    // Otherwise, recalculate everything...
                    else
                    {
                        _lastModel.CalculateData(t_data);
                        _lastModel.CalculateQuery(t_query, t_data, t_configuration);
                    }
                }
                // Otherwise, recalculate the whole model straight away...
                else
                {
                    switch (t_modelType)
                    {
                        case QueryModelType.TfIdf:
                            _lastModel = new TfIdfModel(_logger);
                            break;

                        case QueryModelType.Boolean:
                            _lastModel = new BooleanModel(_logger);
                            break;

                        default:
                            Debugger.Break();
                            _logger.LogCriticalSource("Model is out of range!");
                            break;
                    }

                    _lastModel.CalculateData(t_data);
                    _lastModel.CalculateQuery(t_query, t_data, t_configuration);
                }

                // Save information about last query request
                _lastModelType = t_modelType;
                _lastQuery = t_query;
                _lastDataChecksum = dataChecksum;

                return _lastModel.CalculateBestMatch(select);
            });
        }

        /// <inheritdoc/>
        public void ResetLastModelData()
        {
            _lastModelType = null;
            _lastModel = null;
            _lastQuery = null;
            _lastDataChecksum = null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets checksum of query data
        /// </summary>
        /// <param name="data">The data (<see cref="InvertedIndex._vocabulary"/>)</param>
        /// <returns>Checksum</returns>
        private byte[] GetDataChecksum(IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> data)
        {
            var bf = new BinaryFormatter();
            using (var stream = new MemoryStream())
            using (var md5 = MD5.Create())
            {
                bf.Serialize(stream, data);
                return md5.ComputeHash(stream);
            }
        }

        #endregion
    }
}
