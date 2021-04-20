using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Definition of vector space model TF-IDF
    /// </summary>
    internal sealed class TfIdf : IQueryModel
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;

        #endregion

        #region Private Members

        /// <summary>
        /// Calculated IDF for each term based on the last run of <see cref="CalculateData"/>.
        /// </summary>
        private Dictionary<string, double> _termIdf = null;

        /// <summary>
        /// Calculated document vectors based on the last run of <see cref="CalculateData"/>.
        /// </summary>
        private Dictionary<int, double[]> _documentVectors = null;

        /// <summary>
        /// Calculated query vector based on the last run of <see cref="CalculateQuery"/>.
        /// </summary>
        private double[] _queryVector = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logger">Logger instance - not required, if not defined, the logger does log nothing</param>
        public TfIdf(ILogger logger = null)
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

            // (Re)Initialize the values
            _termIdf = new Dictionary<string, double>();
            _documentVectors = null;

            // Set of all document IDs found among terms
            HashSet<int> documents = new HashSet<int>();

            // Calculate term IDF
            foreach (var term in data)
            {
                var termDocCount = 0;
                // Get no. of documents the term is located in.
                foreach (var termDocument in term.Value)
                {
                    if (termDocument.Key < 0)
                        continue;

                    termDocCount++;
                    documents.Add(termDocument.Key);
                }

                // The IDF calculation
                _termIdf[term.Key] = Math.Log(documents.Count / termDocCount, 10);
            }

            // Create array for document vectors
            Dictionary<int, double[]> docVectors = new Dictionary<int, double[]>();

            // Go through documents and create document vectors...
            foreach (var documentId in documents)
                docVectors[documentId] = CalculateDocumentVector(data, documentId);

            // Save the vectors
            _documentVectors = docVectors;

            // Log it
            _logger?.LogDebugSource("Data has been successfully calculated into vectors.");
        }

        /// <inheritdoc/>
        public void CalculateQuery(string query, IndexProcessingConfigurationDataModel processingConfiguration)
        {
            if (query == null)
                throw new ArgumentNullException("Data not specified!");

            if (_termIdf == null)
                throw new InvalidOperationException("Term IDF map is not defined!");
            if (_documentVectors == null)
                throw new InvalidOperationException("Document vectors are not defined!");

            // (Re)Initialize the values
            _queryVector = null;

            // Process the query and indexate it for vocabulary
            var processing = new IndexProcessing("__tfidf",
                new Tokenizer(processingConfiguration.CustomRegex),
                new Stemmer(processingConfiguration.Language),
                new StopWordRemover(processingConfiguration.Language, processingConfiguration.CustomStopWords),
                fileManager: null,
                logger: null,
                processingConfiguration.ToLowerCase,
                processingConfiguration.RemoveAccentsBeforeStemming,
                processingConfiguration.RemoveAccentsAfterStemming
                );
            var data = processing.IndexText(query);

            _queryVector = CalculateDocumentVector(data, 0);

            // Log it
            _logger?.LogDebugSource("Query has been successfully calculated into vector.");
        }

        /// <inheritdoc/>
        public int[] CalculateBestMatch(int select = 0)
        {
            var documentVectors = _documentVectors;
            var queryVector = _queryVector;

            if (select < 0)
                throw new InvalidCastException($"Parameter '{nameof(select)}' cannot be negative number!");

            if (documentVectors == null)
                throw new InvalidOperationException("Document vectors are not defined!");
            if (queryVector == null)
                throw new InvalidOperationException("Query vector is not defined!");

            // Calculate cosine similarity for each document...
            List<(int, double)> results = new List<(int, double)>();
            foreach (var document in documentVectors)
                results.Add(
                    (document.Key, CalculateCosSimilarity(queryVector, document.Value))
                    );

            // Sort and return result
            results.Sort((x, y) => y.Item2.CompareTo(x.Item2));
            if (select > 0)
                return results.Select(o => o.Item1).Take(select).ToArray();
            return results.Select(o => o.Item1).ToArray();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Calculate cosine similarity for 2 specific vectors.
        /// </summary>
        /// <param name="queryVector">Query vector</param>
        /// <param name="documentVector">Document vector</param>
        /// <returns>Cosine similarity result</returns>
        /// <exception cref="ArgumentNullException">Missing query or document vector reference.</exception>
        private double CalculateCosSimilarity(double[] queryVector, double[] documentVector)
        {
            if (queryVector == null || documentVector == null)
                throw new ArgumentNullException("Vectors not specified!");

            double queryPowSum = 0;
            foreach (var value in queryVector)
                queryPowSum += value * value;
            double querySum = Math.Sqrt(queryPowSum);

            double documentPowSum = 0;
            foreach (var value in documentVector)
                documentPowSum += value * value;
            double documentSum = Math.Sqrt(documentPowSum);

            double x = 0;
            for (int i = 0; i < queryVector.Length; i++)
                x += queryVector[i] * documentVector[i];

            return x / (documentSum * querySum);
        }

        /// <summary>
        /// Calculate space vector for the inputing data based on the <paramref name="documentId"/>.
        /// </summary>
        /// <param name="data">The data</param>
        /// <param name="documentId">The ID</param>
        /// <returns>Document vector from <paramref name="data"/> based on <paramref name="documentId"/>.</returns>
        private double[] CalculateDocumentVector(IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> data, int documentId)
        {
            List<double> docVector = new List<double>();
            foreach (var term in data)
            {
                // Take only the terms located in the document...
                if (term.Value.ContainsKey(documentId))
                {
                    // Calculate term TF-IDF
                    double tf = term.Value[documentId].Frequency; // Term Frequency
                    double ntf = tf > 0 ? 1 + Math.Log(tf, 10) : 0; // 1 + log(TF) --- if TF == 0 => 0
                    double tfidf = _termIdf.ContainsKey(term.Key) ? ntf * _termIdf[term.Key] : 0; // If the term does not exist yet (query call), TF-IDF of the term is 0
                    docVector.Add(tfidf);
                }
            }

            return docVector.ToArray();
        }

        #endregion
    }
}
