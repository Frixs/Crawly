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
        #region Private Members

        /// <summary>
        /// Vocabulary of the inverted index.
        /// </summary>
        /// <remarks>
        ///     SortedDictionary { Key=term, Value=posting list },
        ///     posting list aka Dictionary { Key=DocumentId, Value=TermInfo obj }
        /// </remarks>
        private readonly SortedDictionary<string, Dictionary<int, TermInfo>> _vocabulary = new SortedDictionary<string, Dictionary<int, TermInfo>>();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public InvertedIndex()
        {
        }

        #endregion

        #region Interface Methods

        /// <inheritdoc/>
        /// <remarks>O(n^2)</remarks>
        public IReadOnlyDictionary<string, IReadOnlyDictionary<int, IReadOnlyTermInfo>> GetReadOnlyVocabulary()
        {
            return _vocabulary.ToDictionary(o => o.Key, o => (IReadOnlyDictionary<int, IReadOnlyTermInfo>)o.Value.ToDictionary(x => x.Key, x => (IReadOnlyTermInfo)x.Value));
        }

        /// <inheritdoc/>
        public void Put(string term, int documentId)
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
                _vocabulary.Add(term, new Dictionary<int, TermInfo>()
                {
                    { documentId, new TermInfo(frequency: 1) }
                });
            }
        }

        #endregion
    }
}
