using System;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Index storage interface for manipulating with saved index data
    /// </summary>
    public interface IIndexStorage
    {
        /// <summary>
        /// Find index files (indexes).
        /// </summary>
        /// <param name="iid">The index identifier.</param>
        /// <returns>Array of all index files (file paths).</returns>
        /// <exception cref="ArgumentNullException">Identifier is not defined.</exception>
        /// <exception cref="Exception">Unknown error - related to IO.</exception>
        string[] GetIndexFiles(string iid);

        /// <summary>
        /// Delete files according to index identifier and file timestmap.
        /// </summary>
        /// <param name="iid">The index identifier.</param>
        /// <param name="fileTimestamp">The file timestamp.</param>
        /// <exception cref="ArgumentNullException">Identifier is not defined.</exception>
        /// <exception cref="Exception">Unknown error - related to IO.</exception>
        void DeleteIndexFiles(string iid, DateTime fileTimestamp);

        /// <summary>
        /// Update index filename
        /// </summary>
        /// <param name="iid">The index identifier.</param>
        /// <param name="oldFileTimestamp">The old timestamp (file to change).</param>
        /// <param name="newFileTimestamp">The updated timestamp.</param>
        /// <exception cref="ArgumentNullException">Identifier is not defined.</exception>
        /// <exception cref="Exception">Unknown error - related to IO.</exception>
        void UpdateIndexFilename(string iid, DateTime oldFileTimestamp, DateTime newFileTimestamp);
    }
}
