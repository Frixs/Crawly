using InformationRetrievalManager.Core;
using System;
using System.Threading;

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
        /// <param name="data">Documents data reference(<see cref="InvertedIndex._data"/>) from the query manager.</param>
        /// <param name="setProgressMessage">Action to retrieve progress data ("what is going on during processing").</param>
        /// <param name="cancellationToken">Cancellation token for interrupting the process.</param>
        /// <exception cref="ArgumentNullException">Missing data reference.</exception>
        void CalculateData(InvertedIndex.ReadOnlyData data, Action<string> setProgressMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Calculates model for query
        /// </summary>
        /// <param name="data">Documents data reference(<see cref="InvertedIndex._data"/>) from the query manager.</param>
        /// <param name="query">The query</param>
        /// <param name="processingConfiguration">Processing configuration to use for the query index processing</param>
        /// <param name="setProgressMessage">Action to retrieve progress data ("what is going on during processing").</param>
        /// <param name="cancellationToken">Cancellation token for interrupting the process.</param>
        /// <exception cref="ArgumentNullException">Missing query reference.</exception>
        /// <exception cref="InvalidOperationException">Missing parameters. <see cref="CalculateData"/> must be called beforehand.</exception>
        void CalculateQuery(InvertedIndex.ReadOnlyData data, string query, IndexProcessingConfiguration processingConfiguration, Action<string> setProgressMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Calculate best matching documents by the query.
        /// </summary>
        /// <param name="data">Documents data reference(<see cref="InvertedIndex._data"/>) from the query manager.</param>
        /// <param name="select">Limit number of records to select (0 to ignore limit).</param>
        /// <param name="foundDocuments">Number of documents that was found regardless of <paramref name="select"/>.</param>
        /// <param name="setProgressMessage">Action to retrieve progress data ("what is going on during processing").</param>
        /// <param name="cancellationToken">Cancellation token for interrupting the process.</param>
        /// <returns>Sorted array of document IDs from the best matching to the least.</returns>
        /// <exception cref="InvalidOperationException">Missing parameters. <see cref="CalculateData"/> and <see cref="CalculateQuery"/> must be called beforehand.</exception>
        long[] CalculateBestMatch(InvertedIndex.ReadOnlyData data, int select, out long foundDocuments, Action<string> setProgressMessage, CancellationToken cancellationToken);
    }
}
