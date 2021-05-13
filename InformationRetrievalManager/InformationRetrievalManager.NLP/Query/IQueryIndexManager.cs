using InformationRetrievalManager.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Manager interface for querying data above indexed data.
    /// </summary>
    public interface IQueryIndexManager
    {
        /// <summary>
        /// Query documents from the most relevant to the least. The process is run in separate thread.
        /// </summary>
        /// <param name="query">The query</param>
        /// <param name="index">Reference to the index of which data are queried.</param>
        /// <param name="modelType">Type of the model to use for querying.</param>
        /// <param name="configuration">Processing configuration to use for the query index processing.</param>
        /// <param name="select">Limit number of records to select (0 to ignore limit).</param>
        /// <param name="setProgressMessage">Action to retrieve progress data ("what is going on during processing").</param>
        /// <param name="cancellationToken">Cancellation token for interrupting the process.</param>
        /// <returns>
        ///     Tuple:
        ///         1. Status (0 = OK, 1 = Index load failure)
        ///         2. Tuple:
        ///             1: Array of document IDs sorted from the most relevant to the least  (not <see langword="null"/>). 
        ///             2: No. of ofund documents. 
        ///             3. No. of total searched documents.
        /// </returns>
        /// <exception cref="ArgumentNullException">Invalid parameters.</exception>
        Task<(byte Status, (long[] Data, long FoundDocuments, long TotalDocuments) Result)> QueryAsync(string query, IInvertedIndex index, QueryModelType modelType, IndexProcessingConfiguration configuration, int select, Action<string> setProgressMessage, CancellationToken cancellationToken);

        /// <summary>
        /// Resets model data that are saved from previous query calls. 
        /// After reset, next called query is forced to recalculate everything.
        /// </summary>
        void ResetLastModelData();
    }
}
