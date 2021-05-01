using System;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Index storage interface for manipulating with saved index data
    /// </summary>
    public interface IIndexStorage
    {
        /// <summary>
        /// Find all index files.
        /// </summary>
        /// <param name="iid">The index identifier.</param>
        /// <returns>Array of all index files (file paths).</returns>
        /// <exception cref="ArgumentNullException">Identifier is not defined.</exception>
        string[] GetDataFiles(string iid);

        /// <summary>
        /// Delete files according to index identifier and file timestmap.
        /// </summary>
        /// <param name="iid">The index identifier.</param>
        /// <param name="fileTimestamp">The file timestamp.</param>
        /// <exception cref="ArgumentNullException">Identifier is not defined.</exception>
        void DeleteIndexFiles(string iid, DateTime fileTimestamp);
    }
}
