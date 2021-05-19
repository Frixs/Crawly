using System;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Abstraction above the storage interface more generalized for indexed data calculation serialization.
    /// </summary>
    public interface IIndexStorageIndexedSerializable
    {
        /// <summary>
        /// Serialize indexed data (calculations) into a file (cache).
        /// </summary>
        /// <typeparam name="T">Data type that is serialized.</typeparam>
        /// <param name="iid">Identifier of the index.</param>
        /// <param name="fileTimestamp">Index timestamp.</param>
        /// <param name="typeIdentifier">Identifier that defines the file name for the data.</param>
        /// <param name="obj">The data.</param>
        void SerializeIndexedDataIntoFile<T>(string iid, DateTime fileTimestamp, string typeIdentifier, T obj);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Data type that is serialized.</typeparam>
        /// <param name="iid">Identifier of the index.</param>
        /// <param name="fileTimestamp">Index timestamp.</param>
        /// <param name="typeIdentifier">Identifier that defines the file name for the data.</param>
        /// <param name="result">Deserialized data (or <see langword="default"/>).</param>
        /// <returns>Status: 0 = OK, 1 = Failed to deserialize</returns>
        byte DeserializeIndexedDataFromFile<T>(string iid, DateTime fileTimestamp, string typeIdentifier, out T result);
    }

    /// <summary>
    /// Index storage interface for manipulating with saved index data
    /// </summary>
    public interface IIndexStorage : IIndexStorageIndexedSerializable
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
        /// Find all files related to index.
        /// </summary>
        /// <param name="iid">The index identifier.</param>
        /// <returns>Array of all index files (file paths).</returns>
        /// <exception cref="ArgumentNullException">Identifier is not defined.</exception>
        /// <exception cref="Exception">Unknown error - related to IO.</exception>
        string[] GetFiles(string iid);

        /// <summary>
        /// Delete files according to index identifier and file timestmap.
        /// </summary>
        /// <param name="iid">The index identifier.</param>
        /// <param name="fileTimestamp">The file timestamp.</param>
        /// <exception cref="ArgumentNullException">Identifier is not defined.</exception>
        /// <exception cref="Exception">Unknown error - related to IO.</exception>
        void DeleteFiles(string iid, DateTime fileTimestamp);

        /// <summary>
        /// Delete files according to index identifier and file timestmap but only the indexed data (calculations etc.).
        /// </summary>
        /// <param name="iid">The index identifier.</param>
        /// <param name="fileTimestamp">The file timestamp.</param>
        /// <exception cref="ArgumentNullException">Identifier is not defined.</exception>
        /// <exception cref="Exception">Unknown error - related to IO.</exception>
        void DeleteIndexedDataFiles(string iid, DateTime fileTimestamp);

        /// <summary>
        /// Update index filename
        /// </summary>
        /// <param name="iid">The index identifier.</param>
        /// <param name="oldFileTimestamp">The old timestamp (file to change).</param>
        /// <param name="newFileTimestamp">The updated timestamp.</param>
        /// <exception cref="ArgumentNullException">Identifier is not defined.</exception>
        /// <exception cref="Exception">Unknown error - related to IO.</exception>
        void UpdateFilenames(string iid, DateTime oldFileTimestamp, DateTime newFileTimestamp);
    }
}
