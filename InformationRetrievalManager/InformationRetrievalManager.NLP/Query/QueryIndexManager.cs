using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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

        #region Private Members (Models)

        /// <summary>
        /// Data checksum of last queried data
        /// </summary>
        private byte[] _lastDataChecksum = null;

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
        public async Task<int[]> QueryAsync(string query, IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> data, QueryModelType modelType)
        {
            // Lock the task.
            return await AsyncLock.LockResultAsync(nameof(QueryIndexManager) + nameof(QueryAsync), async () =>
            {
                return Array.Empty<int>();
            });
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
