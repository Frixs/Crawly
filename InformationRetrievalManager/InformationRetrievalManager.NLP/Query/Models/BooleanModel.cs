using InformationRetrievalManager.Core;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private int[] _queryResults = Array.Empty<int>();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logger">Logger instance - not required, if not defined, the logger does log nothing</param>
        public BooleanModel(ILogger logger = null)
        {
            _logger = logger;
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        public void CalculateData(IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> data)
        {
            if (data == null)
                throw new ArgumentNullException("Data not specified!");

            // There is no need for any calculation.

            // Log it
            _logger?.LogDebugSource("Data has been successfully calculated.");
        }

        /// <inheritdoc/>
        public void CalculateQuery(string query, IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> data, IndexProcessingConfiguration processingConfiguration)
        {
            if (query == null || data == null)
                throw new ArgumentNullException("Data not specified!");

            // (Re)Initialize the values
            _queryResults = Array.Empty<int>();

            // Set of all document IDs found among terms
            HashSet<int> documents = new HashSet<int>();

            // Get all document IDs
            foreach (var term in data)
            {
                foreach (var termDocument in term.Value)
                {
                    if (termDocument.Key < 0)
                        continue;

                    documents.Add(termDocument.Key);
                }
            }

            // Query parser
            var parser = new QueryBooleanExpressionParser();
            string[] queryTokens = parser.Tokenize(query);

            // Parse query
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
                List<int> results = new List<int>();
                foreach (var documentId in documents)
                {
                    if (queryParsed.Evaluate(new DocumentTermEvaluator(documentId, data, processingConfiguration)))
                        // document accepted
                        results.Add(documentId);
                }

                // Sort
                _queryResults = results.OrderBy(o => o).ToArray();

                // Log it
                _logger?.LogDebugSource("Query has been successfully calculated and data prepared.");
            }
        }

        /// <inheritdoc/>
        public int[] CalculateBestMatch(int select = 0)
        {
            // The calculations are made in the query method due to parameter limitations.

            if (select > 0)
                return _queryResults.Take(select).ToArray();
            return _queryResults;
        }

        #endregion

        #region Document-Term Evaluator Class

        /// <summary>
        /// Wrapper for <see cref="QueryBooleanExpressionParser"/> to ease evaluation of expressions.
        /// </summary>
        public class DocumentTermEvaluator
        {
            private readonly IndexProcessing _processing;
            public readonly int documentId;
            public readonly IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> data;

            /// <summary>
            /// Default constructor
            /// </summary>
            /// <param name="documentId">Document ID for which the evaluation is made.</param>
            /// <param name="data">Documents data(<see cref="InvertedIndex._vocabulary"/>)</param>
            public DocumentTermEvaluator(int documentId, IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> data, IndexProcessingConfiguration processingConfiguration)
            {
                if (documentId < 0)
                    throw new ArgumentNullException("Invalid document ID!");
                if (data == null)
                    throw new ArgumentNullException("Data not specified!");
                if (processingConfiguration == null)
                    throw new ArgumentNullException("Processing configuration not specified!");

                // Process the query and indexate it for vocabulary
                var processing = new IndexProcessing("__boolean",
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
                    if (data.ContainsKey(term))
                    {
                        bool foundTerm = false;
                        foreach (var termDocument in data[term])
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
    }
}
