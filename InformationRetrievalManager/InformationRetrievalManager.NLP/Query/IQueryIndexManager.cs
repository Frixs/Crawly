using System;
using System.Collections.Generic;

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
        /// <param name="data">Data used for the query</param>
        /// <param name="modelType">Type of the model to use for querying.</param>
        /// <returns>Array of document IDs sorted from the most relevant to the least.</returns>
        int[] Query(string query, IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> data, QueryModelType modelType);
    }
}
