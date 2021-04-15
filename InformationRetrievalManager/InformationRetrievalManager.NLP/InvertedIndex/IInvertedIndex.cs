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

        /// <summary>
        /// Loads indexated data from file storage into the memory storage (overrides the data currently loaded in the memory).
        /// </summary>
        /// <returns>Returns <see langword="true"/> on successful load, otherwise <see langword="false"/>.</returns>
        bool Load();

        /// <summary>
        /// Saves the into a file storage and clear the memory storaged data.
        /// </summary>
        /// <returns>Returns <see langword="true"/> on successful save, otherwise <see langword="false"/>.</returns>
        bool Save();
    }
}
