using InformationRetrievalManager.Core;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Definition of boolean model for querying
    /// </summary>
    internal sealed class BooleanModel : IQueryModel
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;

        #endregion

        #region Private Members

        /// <summary>
        /// Results made by the query based on its data
        /// </summary>
        private SortedSet<(long DocumentId, DateTime DocumentTimestamp)> _queryResults;

        #endregion

        #region Interface Properties

        /// <inheritdoc/>
        public bool DataCalculated { get; private set; }

        /// <inheritdoc/>
        public bool QueryCalculated { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BooleanModel(ILogger logger)
        {
            _logger = logger;
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public void CalculateData(InvertedIndex.ReadOnlyData data, Action<string> setProgressMessage = null, CancellationToken cancellationToken = default)
        {
            if (data == null)
                throw new ArgumentNullException("Data not specified!");

            // Set flag
            DataCalculated = true;

            // Log it
            _logger?.LogDebugSource("Data has been successfully calculated.");
            setProgressMessage?.Invoke("data done");
        }
        
        /// <inheritdoc/>
        public void CalculateQuery(InvertedIndex.ReadOnlyData data, string query, IndexProcessingConfiguration processingConfiguration, Action<string> setProgressMessage = null, CancellationToken cancellationToken = default)
        {
            if (query == null || data == null)
                throw new ArgumentNullException("Data not specified!");

            // (Re)Initialize the values
            _queryResults = new SortedSet<(long DocumentId, DateTime DocumentTimestamp)>();

            // Query parser
            var parser = new QueryBooleanExpressionParser();
            string[] queryTokens = parser.Tokenize(query);

            // Parse query
            setProgressMessage?.Invoke("parsing query");
            QueryBooleanExpressionParser.Node queryParsed = null;
            try
            {
                queryParsed = parser.Parse(queryTokens);
            }
            catch (Exception)
            {
                // Parse error
                _logger?.LogDebugSource("Query failed to parse.");
            }

            // If the query is successfully parsed...
            if (queryParsed != null)
            {
                var results = new SortedSet<(long DocumentId, DateTime DocumentTimestamp)>(new DocumentComparer());
                long i = 0;
                foreach (KeyValuePair<long, IReadOnlyDocumentInfo> doc in data.Documents) // Key = Document ID
                {
                    i++;
                    // Check for cancelation
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    if (queryParsed.Evaluate(new DocumentTermEvaluator(doc.Key, data, processingConfiguration)))
                        // document accepted
                        results.Add((doc.Key, doc.Value.Timestamp));

                    setProgressMessage?.Invoke($"evaluating documents - {i}/{data.Documents.Count}");
                }

                // Assign to query results
                _queryResults = results;

                // Set flag
                QueryCalculated = true;

                // Log it
                _logger?.LogDebugSource("Query has been successfully calculated and data prepared.");
            }
            // Otherwise, something went wrong during query parsing....
            else
            {
                // Log it
                _logger?.LogDebugSource("Query failed to parse.");
            }
            setProgressMessage?.Invoke("query done");
        }

        /// <inheritdoc/>
        public long[] CalculateBestMatch(InvertedIndex.ReadOnlyData data, int select, out long foundDocuments, Action<string> setProgressMessage = null, CancellationToken cancellationToken = default)
        {
            if (data == null)
                throw new ArgumentNullException("Data not specified!");

            if (select < 0)
                throw new InvalidCastException($"Parameter '{nameof(select)}' cannot be negative number!");

            // The calculations are made in the query method due to parameter limitations.
            // Cancellation is not needed here

            setProgressMessage?.Invoke("retrieving results");

            foundDocuments = _queryResults.Count;

            if (select > 0)
                return _queryResults.Take(select).Select(o => o.DocumentId).ToArray();
            return _queryResults.Select(o => o.DocumentId).ToArray();
        }

        /// <inheritdoc/>
        public void LoadCalculations(IIndexStorageIndexedSerializable storage, IReadOnlyInvertedIndex index)
        {
            // no for Boolean model
        }

        /// <inheritdoc/>
        public void SaveCalculations(IIndexStorageIndexedSerializable storage, IReadOnlyInvertedIndex index)
        {
            // no for Boolean model
        }

        #endregion

        #region Document-Term Evaluator Class

        /// <summary>
        /// Wrapper for <see cref="QueryBooleanExpressionParser"/> to ease evaluation of expressions.
        /// </summary>
        public class DocumentTermEvaluator
        {
            private readonly IndexProcessing _processing;
            public readonly long documentId;
            public readonly InvertedIndex.ReadOnlyData data;

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="documentId">Document ID for which the evaluation is made.</param>
            /// <param name="data">Documents data(<see cref="InvertedIndex._data"/>)</param>
            public DocumentTermEvaluator(long documentId, InvertedIndex.ReadOnlyData data, IndexProcessingConfiguration processingConfiguration)
            {
                if (documentId < 0)
                    throw new ArgumentNullException("Invalid document ID!");
                if (data == null)
                    throw new ArgumentNullException("Data not specified!");
                if (processingConfiguration == null)
                    throw new ArgumentNullException("Processing configuration not specified!");

                // Process the query and indexate it for vocabulary
                var processing = new IndexProcessing("__boolean",
                    timestamp: default,
                    new Tokenizer(processingConfiguration.CustomRegex),
                    new Stemmer(processingConfiguration.Language),
                    new StopWordRemover(processingConfiguration.Language, processingConfiguration.CustomStopWords),
                    fileManager: null,
                    logger: null,
                    processingConfiguration.ToLowerCase,
                    processingConfiguration.RemoveAccentsBeforeStemming,
                    processingConfiguration.RemoveAccentsAfterStemming
                    );

                this.documentId = documentId;
                this.data = data;
                _processing = processing;
            }

            /// <summary>
            /// Parse <paramref name="token"/> into terms according to the initial instance configuration
            /// and evaluate each term if it is in <see cref="data"/> under the <see cref="documentId"/>.
            /// </summary>
            /// <param name="token">The token</param>
            /// <returns><see langword="true"/> on success (all terms are located in the document), <see langword="false"/> otherwise</returns>
            /// <exception cref="ArgumentNullException">Missing term reference.</exception>
            public bool EvaluateTerm(string token)
            {
                if (token == null)
                    throw new ArgumentNullException("Token not specified!");

                bool result = true;
                string[] terms = _processing.ProcessText(token);

                // Go through the terms...
                foreach (var term in terms)
                {
                    // Chech if the term exists in the data...
                    if (data.Vocabulary.ContainsKey(term))
                    {
                        bool foundTerm = false;
                        foreach (var termDocument in data.Vocabulary[term])
                        {
                            if (termDocument.Key == documentId)
                            {
                                foundTerm = true;
                                break;
                            }
                        }

                        // Check if the term was found in searched document...
                        if (!foundTerm)
                        {
                            // ... it does not exist in the document, break it and result false
                            result = false;
                            break;
                        }
                    }
                    // Otherwise, term does not exist in any document...
                    else
                    {
                        result = false;
                        break;
                    }
                }

                return result;
            }
        }

        #endregion

        #region Comparer Class

        /// <summary>
        /// Document comparer to skip necessary sorting
        /// </summary>
        private class DocumentComparer : IComparer<(long DocumentId, DateTime DocumentTimestamp)>
        {
            public int Compare((long DocumentId, DateTime DocumentTimestamp) x, (long DocumentId, DateTime DocumentTimestamp) y)
            {
                // By Timestamp
                int result = y.DocumentTimestamp.CompareTo(x.DocumentTimestamp);

                // Then by document ID
                if (result == 0)
                    result = x.DocumentId.CompareTo(y.DocumentId);

                return result;
            }
        }

        #endregion
    }
}
