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
        /// </summary>
        /// <param name="term">The term</param>
        /// <param name="documentId">Document ID</param>
        /// <exception cref="ArgumentNullException">If term is null or empty or document ID is not positive number.</exception>
        void Put(string term, int documentId);
    }
}
