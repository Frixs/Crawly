using System;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Inverted Index interface
    /// </summary>
    public interface IInvertedIndex : IReadOnlyInvertedIndex
    {
        /// <summary>
        /// Put a term into a inverted index vocabulary.
        /// Throws <see cref="ArgumentNullException"/> if term is null or empty or document ID is not positive number.
        /// </summary>
        /// <param name="term">The term</param>
        /// <param name="documentId">Document ID</param>
        void Put(string term, int documentId);
    }
}
