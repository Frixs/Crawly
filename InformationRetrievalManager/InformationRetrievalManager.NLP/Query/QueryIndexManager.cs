using InformationRetrievalManager.Core;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
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

        /// <summary>
        /// Last saved number of total document searched from the document calculation.
        /// </summary>
        private long _lastTotalDocumentCount = 0;

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
        public async Task<(long[], long, long)> QueryAsync(string query, IReadOnlyDictionary<string, IReadOnlyDictionary<long, IReadOnlyTermInfo>> data, QueryModelType modelType, IndexProcessingConfiguration configuration, int select = 0, Action<string> setProgressMessage = null, CancellationToken cancellationToken = default)
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

                long totalDocuments = _lastTotalDocumentCount;
                long foundDocuments = 0;

                setProgressMessage?.Invoke("starting");

                // Get data checksum
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
                            _lastModel.CalculateQuery(t_query, t_data, t_configuration, setProgressMessage, cancellationToken);
                        // Otherwise, there is not need to do anything, the query data are the same as the previous request.
                    }
                    // Otherwise, recalculate everything...
                    else
                    {
                        _lastModel.CalculateData(t_data, out totalDocuments, setProgressMessage, cancellationToken);
                        _lastModel.CalculateQuery(t_query, t_data, t_configuration, setProgressMessage, cancellationToken);
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

                    _lastModel.CalculateData(t_data, out totalDocuments, setProgressMessage, cancellationToken);
                    _lastModel.CalculateQuery(t_query, t_data, t_configuration, setProgressMessage, cancellationToken);
                }

                // Save information about last query request
                _lastModelType = t_modelType;
                _lastQuery = t_query;
                _lastDataChecksum = dataChecksum;
                _lastTotalDocumentCount = totalDocuments;

                return (Results: _lastModel.CalculateBestMatch(select, out foundDocuments, setProgressMessage, cancellationToken), FoundDocuments: foundDocuments, TotalDocuments: totalDocuments);
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
        /// <returns>Checksum or <see langword="null"/> on serialization failure.</returns>
        private byte[] GetDataChecksum(IReadOnlyDictionary<string, IReadOnlyDictionary<long, IReadOnlyTermInfo>> data)
        {
            try
            {
                // Create hashable data
                List<byte> buffer = new List<byte>();
                foreach (var item1 in data)
                {
                    buffer.AddRange(Encoding.ASCII.GetBytes(item1.Key));
                    foreach (var item2 in item1.Value)
                        buffer.AddRange(BitConverter.GetBytes(item2.Key + item2.Value.Frequency));
                }

                using (var md5 = MD5.Create())
                {
                    return md5.ComputeHash(buffer.ToArray());
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}
