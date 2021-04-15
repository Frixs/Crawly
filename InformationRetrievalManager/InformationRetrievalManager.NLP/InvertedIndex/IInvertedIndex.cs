using InformationRetrievalManager.Core;
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
        ///     <para>
        ///         Loads indexated data from file storage into the memory storage (overrides the data currently loaded in the memory).
        ///     </para>
        ///     <para>
        ///         <see cref="IInvertedIndex"/> always has an option to define <see cref="ITaskManager"/>. 
        ///         If the manager is defined, the method loads data from file storage and overrides the data stored in-memory. 
        ///         If the load method fails, the in-memory data remains the same.
        ///         On the other hand, if the manager is not defined (<see cref="null"/>) the method does nothing and the in-memory data remains the same.
        ///     </para>
        /// </summary>
        /// <returns>Returns <see langword="true"/> on successful load, otherwise <see langword="false"/>.</returns>
        bool Load();

        /// <summary>
        ///     <para>
        ///         Saves the into a file storage and clear the memory storaged data.
        ///     </para>
        ///     <para>
        ///         <see cref="IInvertedIndex"/> always has an option to define <see cref="ITaskManager"/>. 
        ///         If the manager is defined, the method saves data to file storage and clears the data stored in-memory. 
        ///         If the save method fails, the in-memory data remains the same.
        ///         On the other hand, if the manager is not defined (<see cref="null"/>) the method does nothing and the in-memory data remains the same.
        ///     </para>
        /// </summary>
        /// <returns>Returns <see langword="true"/> on successful save, otherwise <see langword="false"/>.</returns>
        bool Save();
    }
}
