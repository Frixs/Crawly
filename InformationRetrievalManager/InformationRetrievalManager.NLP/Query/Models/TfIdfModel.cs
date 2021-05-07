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
    /// Definition of vector space model TF-IDF
    /// </summary>
    internal sealed class TfIdfModel : IQueryModel
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
        private Dictionary<long, (uint Position, double Value)[]> _documentVectors = null;
        
        /// <summary>
        /// Calculated query vector based on the last run of <see cref="CalculateQuery"/>.
        /// </summary>
        private (uint Position, double Value)[] _queryVector = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="logger">Logger instance - not required, if not defined, the logger does log nothing</param>
        public TfIdfModel(ILogger logger = null)
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

            // (Re)Initialize the values
            _termIdf = new Dictionary<string, double>();
            _documentVectors = null;

            // Calculate term IDF
            long i = 0;
            foreach (var term in data.Vocabulary)
            {
                i++;
                var termDocCount = 0;
                // Get no. of documents the term is located in.
                foreach (var termDocument in term.Value)
                {
                    // Check for cancelation
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    if (termDocument.Key < 0)
                        continue;

                    termDocCount++;
                }

                // Check for cancelation
                if (cancellationToken.IsCancellationRequested)
                    break;

                // The IDF calculation
                _termIdf[term.Key] = Math.Log(data.Documents.Count / termDocCount, 10);
                setProgressMessage?.Invoke($"data IDF calculation - {i}/{data.Vocabulary.Count}");
            }

            // Create array for document vectors
            var docVectors = new Dictionary<long, (uint Position, double Value)[]>();

            // Go through documents and create document vectors...
            i = 0;
            foreach (var documentId in data.Documents.Keys)
            {
                i++;
                // Check for cancelation
                if (cancellationToken.IsCancellationRequested)
                    break;

                docVectors[documentId] = CalculateDocumentVector(data, documentId, cancellationToken);
                setProgressMessage?.Invoke($"calculating data vectors - {i}/{data.Documents.Count}");
            }

            // Save the vectors
            _documentVectors = docVectors;

            // Log it
            _logger?.LogDebugSource("Data has been successfully calculated into vectors.");
            setProgressMessage?.Invoke("data done");
        }

        /// <inheritdoc/>
        public void CalculateQuery(InvertedIndex.ReadOnlyData data, string query, IndexProcessingConfiguration processingConfiguration, Action<string> setProgressMessage = null, CancellationToken cancellationToken = default)
        {
            if (query == null || data == null)
                throw new ArgumentNullException("Data not specified!");

            if (_termIdf == null)
                throw new InvalidOperationException("Term IDF map is not defined!");
            if (_documentVectors == null)
                throw new InvalidOperationException("Document vectors are not defined!");

            // (Re)Initialize the values
            _queryVector = null;

            setProgressMessage?.Invoke("calculating query vector");

            // Process the query and indexate it for vocabulary
            var processing = new IndexProcessing("__tfidf",
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
            var queryData = processing.IndexText(query);

            _queryVector = CalculateDocumentVector(CreateQueryData(queryData, data), 0, cancellationToken);

            // Log it
            _logger?.LogDebugSource("Query has been successfully calculated into vector.");
            setProgressMessage?.Invoke("query done");
        }

        /// <inheritdoc/>
        public long[] CalculateBestMatch(InvertedIndex.ReadOnlyData data, int select, out long foundDocuments, Action<string> setProgressMessage = null, CancellationToken cancellationToken = default)
        {
            var documentVectors = _documentVectors;
            var queryVector = _queryVector;

            if (data == null)
                throw new ArgumentNullException("Data not specified!");

            if (select < 0)
                throw new InvalidCastException($"Parameter '{nameof(select)}' cannot be negative number!");

            if (documentVectors == null)
                throw new InvalidOperationException("Document vectors are not defined!");
            if (queryVector == null)
                throw new InvalidOperationException("Query vector is not defined!");

            // Calculate cosine similarity for each document...
            var results = new SortedSet<(long DocumentId, DateTime DocumentTimestamp, double Relevance)>(new DocumentComparer());
            long i = 0;
            foreach (var document in documentVectors) // Key = document ID
            {
                i++;
                // Check for cancelation
                if (cancellationToken.IsCancellationRequested)
                    break;

                results.Add(
                    (document.Key, data.Documents[document.Key].Timestamp, CalculateCosSimilarity(queryVector, document.Value))
                    );
                setProgressMessage?.Invoke($"calculating cos-similarity - {i}/{documentVectors.Count}");
            }

            // Sort and return result
            setProgressMessage?.Invoke("retrieving results");
            foundDocuments = results.Count;
            if (select > 0)
                return results.Take(select).Select(o => o.DocumentId).ToArray();
            return results.Select(o => o.DocumentId).ToArray();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Calculate cosine similarity for 2 specific vectors.
        /// </summary>
        /// <param name="queryVector">Query vector</param>
        /// <param name="documentVector">Document vector</param>
        /// <returns>Cosine similarity result of document relevance.</returns>
        /// <exception cref="ArgumentNullException">Missing query or document vector reference.</exception>
        /// <remarks>
        ///     The vectors are created by <see cref="CalculateDocumentVector"/> and this vector calculation is based on the process of creation.
        /// </remarks>
        private double CalculateCosSimilarity((uint Position, double Value)[] queryVector, (uint Position, double Value)[] documentVector)
        {
            if (queryVector == null || documentVector == null)
                throw new ArgumentNullException("Vectors not specified!");

            // Query SUM calculation
            double queryPowSum = 0;
            foreach (var (Index, Value) in queryVector)
                queryPowSum += Value * Value;
            double querySum = Math.Sqrt(queryPowSum);

            // Document SUM calculation
            double documentPowSum = 0;
            foreach (var (Index, Value) in documentVector)
                documentPowSum += Value * Value;
            double documentSum = Math.Sqrt(documentPowSum);

            // X calculation
            double x = 0;
            int qIdx = 0; // array index of query vector values
            int dIdx = 0; // array index of document vector values
            while (true)
            {
                // If one of the vectors already reached its end...
                if (qIdx >= queryVector.Length || dIdx >= documentVector.Length)
                    // ...break the calculation, no need for further calculation due to zero multiplication
                    break;

                // Get the current positions
                var qPos = queryVector[qIdx].Position;
                var dPos = documentVector[dIdx].Position;

                // Calculate only if vector positions match
                if (qPos == dPos)
                    x += queryVector[qIdx].Value * documentVector[dIdx].Value;

                // Balance the vector positions
                if (qPos > dPos)
                    dIdx++;
                else
                    qIdx++;
            }

            // Make the final calculation
            return x / (documentSum * querySum);
        }

        /// <summary>
        /// Calculate space vector for the inputing data based on the <paramref name="documentId"/>.
        /// </summary>
        /// <param name="data">The data</param>
        /// <param name="documentId">The ID</param>
        /// <param name="cancellationToken">Cancellation token for interrupting the process.</param>
        /// <returns>
        ///     Document vector made from <paramref name="data"/> based on <paramref name="documentId"/>
        ///     represented with tuple where (position indexes are in ASC order): 
        ///         1=(term position index), 
        ///         2=(vector position value)
        /// </returns>
        /// <remarks>
        ///     Vector stores only non-zero values to optimize the TF-IDF calculation process.
        /// </remarks>
        private (uint Position, double Value)[] CalculateDocumentVector(InvertedIndex.ReadOnlyData data, long documentId, CancellationToken cancellationToken = default)
        {
            var docVector = new List<(uint Position, double Value)>();
            uint i = 0;
            foreach (var term in data.Vocabulary)
            {
                // Check for cancelation
                if (cancellationToken.IsCancellationRequested)
                    break;

                // Take only the terms located in the document...
                if (term.Value.ContainsKey(documentId))
                {
                    // Calculate term TF-IDF
                    double tf = term.Value[documentId].Frequency; // Term Frequency
                    double ntf = tf > 0 ? 1 + Math.Log(tf, 10) : 0; // 1 + log(TF) --- if TF == 0 => 0
                    double tfidf = _termIdf.ContainsKey(term.Key) ? ntf * _termIdf[term.Key] : 0; // If the term does not exist (query call), TF-IDF of the term is 0
                    docVector.Add((i, tfidf));
                }
                // Otherwise, it is 0...

                i++;
            }

            return docVector.ToArray();
        }

        /// <summary>
        /// Map <paramref name="query"/> vocabulary into <paramref name="documents"/> representing documents vocabulary.
        /// </summary>
        /// <param name="query">The query data</param>
        /// <param name="documents">The document data</param>
        /// <returns>Query vocabulary in scope of document vocabulary terms.</returns>
        private InvertedIndex.ReadOnlyData CreateQueryData(InvertedIndex.ReadOnlyData query, InvertedIndex.ReadOnlyData documents)
        {
            var result = new InvertedIndex.Data();
            
            // Go through the document vocabulary...
            foreach (var term in documents.Vocabulary)
            {
                // Create default value for posting list
                var postingList = new Dictionary<long, TermInfo>();

                // If the query vocabulary contains the same term as the documents have...
                if (query.Vocabulary.ContainsKey(term.Key))
                    // ...take the posting list from the query
                    foreach (var doc in query.Vocabulary[term.Key])
                        postingList.Add(doc.Key, (TermInfo)doc.Value);

                // Add posting list to the final result
                result.Vocabulary.Add(term.Key, postingList);
            }

            return new InvertedIndex.ReadOnlyData(result.Vocabulary, documents.Documents);
        }

        #endregion

        #region Comparer Class

        /// <summary>
        /// Document comparer to skip necessary sorting
        /// </summary>
        private class DocumentComparer : IComparer<(long DocumentId, DateTime DocumentTimestamp, double Relevance)>
        {
            public int Compare((long DocumentId, DateTime DocumentTimestamp, double Relevance) x, (long DocumentId, DateTime DocumentTimestamp, double Relevance) y)
            {
                // Firstly by relevance
                int result = y.Relevance.CompareTo(x.Relevance);

                // Then by document timestamp
                if (result == 0)
                    result = y.DocumentTimestamp.CompareTo(x.DocumentTimestamp);

                // Then by document ID
                if (result == 0)
                    result = x.DocumentId.CompareTo(y.DocumentId);

                return result;
            }
        }

        #endregion
    }
}
