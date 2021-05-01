using InformationRetrievalManager.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Manager interface for querying data above indexed data.
    /// </summary>
    public interface IQueryIndexManager
    {
        /// <summary>
        /// Query documents from the most relevant to the least.
        /// </summary>
        /// <param name="query">The query</param>
        /// <param name="data"><see cref="InvertedIndex._vocabulary"/> - data used for query</param>
        /// <param name="modelType">Type of the model to use for querying.</param>
        /// <param name="configuration">Processing configuration to use for the query index processing.</param>
        /// <param name="select">Limit number of records to select (0 to ignore limit).</param>
        /// <returns>Array of document IDs sorted from the most relevant to the least.</returns>
        /// <exception cref="ArgumentNullException">Invalid parameters.</exception>
        Task<long[]> QueryAsync(string query, IReadOnlyDictionary<string, IReadOnlyDictionary<long, IReadOnlyTermInfo>> data, QueryModelType modelType, IndexProcessingConfiguration configuration, int select);

        /// <summary>
        /// Resets model data that are saved from previous query calls. 
        /// After reset, next called query is forced to recalculate everything.
        /// </summary>
        void ResetLastModelData();
    }
}
