using InformationRetrievalManager.Core;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Functionality to manage data as a inverted index.
    /// </summary>
    public sealed class InvertedIndex : IInvertedIndex
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;
        private readonly IFileManager _fileManager;

        #endregion

        #region Private Members

        /// <summary>
        /// Vocabulary of the inverted index.
        /// </summary>
        /// <remarks>
        ///     SortedDictionary { Key=term, Value=posting list },
        ///     posting list aka Dictionary { Key=DocumentId, Value=TermInfo obj }
        /// </remarks>
        private readonly SortedDictionary<string, Dictionary<long, TermInfo>> _vocabulary = new SortedDictionary<string, Dictionary<long, TermInfo>>();

        #endregion

        #region Interface Properties

        /// <inheritdoc/>
        public string Name { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public InvertedIndex(string name, IFileManager fileManager = null, ILogger logger = null)
        {
            // TODO: check name allowed characters
            Name = name;

            _fileManager = fileManager;
            _logger = logger;
        }

        #endregion

        #region Interface Methods

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <remarks>O(n^2)</remarks>
        public IReadOnlyDictionary<string, IReadOnlyDictionary<long, IReadOnlyTermInfo>> GetReadOnlyVocabulary()
        {
            return _vocabulary.ToDictionary(o => o.Key, o => (IReadOnlyDictionary<long, IReadOnlyTermInfo>)o.Value.ToDictionary(x => x.Key, x => (IReadOnlyTermInfo)x.Value));
        }

        /// <inheritdoc/>
        public void Put(string term, long documentId)
        {
            if (string.IsNullOrEmpty(term) || documentId < 0)
                throw new ArgumentNullException("Invalid parameters for indexing!");

            // Check if the term already exists...
            if (_vocabulary.ContainsKey(term))
            {
                // If the document is already registered for the term...
                if (_vocabulary[term].ContainsKey(documentId))
                {
                    _vocabulary[term][documentId].Frequency++;
                }
                // Otherwise, create a new record for the document...
                else
                {
                    _vocabulary[term].Add(documentId, new TermInfo(frequency: 1));
                }
            }
            // Otherwise, create a new record...
            else
            {
                _vocabulary.Add(term, new Dictionary<long, TermInfo>()
                {
                    { documentId, new TermInfo(frequency: 1) }
                });
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        ///     <see cref="_fileManager"/> must exists, otherwise <see langword="true"/> on return.
        /// </remarks>
        public bool Load()
        {
            if (_fileManager == null)
                return true;

            bool ok = false;

            // Deserialize
            var result = _fileManager.DeserializeObjectFromBinFileAsync($"{Constants.IndexDataStorageDir}/{Name}.idx").Result;
            short status = result.Item1;
            object obj = result.Item2;

            // Check deserialization result
            if (status == 2)
                _logger?.LogWarningSource($"Failed to load index '{Name}'. Corrupted data.");
            else if (status != 0)
                _logger?.LogWarningSource($"Failed to load index '{Name}'. The index does not exist.");

            if (status == 0 && obj != null)
            {
                // Try to load vocabulary
                var newVocabulary = obj as SortedDictionary<string, Dictionary<long, TermInfo>>;
                if (newVocabulary != null)
                {
                    ok = true;

                    _vocabulary.Clear();
                    foreach (var item in newVocabulary)
                        _vocabulary.Add(item.Key, item.Value);
                }
                else
                    _logger?.LogWarningSource($"Failed to load index '{Name}'. Inconsistent data.");
            }
            else
                _logger?.LogWarningSource($"Failed to load index '{Name}'. Data does not exist.");

            return ok;
        }

        /// <inheritdoc/>
        /// <remarks>
        ///     <see cref="_fileManager"/> must exists, otherwise <see langword="true"/> on return.
        /// </remarks>
        public bool Save()
        {
            if (_fileManager == null)
                return true;

            bool ok = false;

            // Serialize
            short status = _fileManager.SerializeObjectToBinFileAsync(_vocabulary, $"{Constants.IndexDataStorageDir}/{Name}.idx").Result;

            // Check serialization result
            if (status == 0)
                ok = true;
            else if (status == 2)
                _logger?.LogWarningSource($"Failed to save index '{Name}'. Invalid data.");
            else
                _logger?.LogWarningSource($"Failed to save index '{Name}'. The index failed to write data in a file.");

            // Clear vocabulary from memory if it is saved successfully
            if (ok)
                _vocabulary.Clear();

            return ok;
        }

        #endregion
    }
}
