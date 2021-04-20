using System;
using System.Collections.Generic;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Query model interface
    /// </summary>
    public interface IQueryModel
    {
        /// <summary>
        /// Calculates model for documents
        /// </summary>
        /// <param name="data"><see cref="InvertedIndex._vocabulary"/></param>
        /// <exception cref="ArgumentNullException">Missing data reference.</exception>
        void CalculateData(IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> data);

        /// <summary>
        /// Calculates model for query
        /// </summary>
        /// <param name="query"></param>
        /// <exception cref="ArgumentNullException">Missing query reference.</exception>
        /// <exception cref="InvalidOperationException">Term IDF map is not defined or document vectors are not defined!. <see cref="CalculateData"/> must be called beforehand.</exception>
        void CalculateQuery(string query);

        /// <summary>
        /// Calculate best matching documents by the query.
        /// </summary>
        /// <returns>Sorted array of document IDs from the best matching to the least.</returns>
        int[] CalculateBestMatch();
    }
}
