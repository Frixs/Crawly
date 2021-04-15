﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Handles reading/writing and querying the file system.
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Deserialize object from binary file
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>
        ///     Tuple that consists of status code and the deserialized object (it is null on error).
        ///     Status code:
        ///         0=OK, 
        ///         1=IO exception, 
        ///         2=Serialization error,
        /// </returns>
        Task<(short, object)> DeserializeObjectFromBinFileAsync(string path);

        /// <summary>
        /// Serialize object into binary file
        /// </summary>
        /// <param name="obj">The object</param>
        /// <param name="path">File path</param>
        /// <returns>
        ///     Status code:
        ///         0=OK, 
        ///         1=IO exception, 
        ///         2=Serialization error,
        /// </returns>
        Task<short> SerializeObjectToBinFileAsync(object obj, string path);

        /// <summary>
        /// Writes the text to the specified file.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="path">The path of the file to write to.</param>
        /// <param name="append">If true, writes the text to the end of the file, otherwise overrides any existing file.</param>
        Task WriteTextToFileAsync(string text, string path, bool append = false);

        /// <summary>
        /// Writes lines to the specified file.
        /// </summary>
        /// <param name="lines">The lines to write.</param>
        /// <param name="path">The path of the file to write to.</param>
        /// <param name="append">If true, writes the text to the end of the file, otherwise overrides any existing file.</param>
        Task WriteLinesToFileAsync(List<string> lines, string path, bool append = false);

        /// <summary>
        /// Count lines in the specified file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        long LineCount(string path);

        /// <summary>
        /// Read all lines from specified file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        List<string> ReadLines(string path);

        /// <summary>
        /// Check if the file is in use or not.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        bool IsInUse(FileInfo file);

        /// <summary>
        /// Normalizing a path based on the current operating system.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns></returns>
        string NormalizedPath(string path);

        /// <summary>
        /// Resolves any relative elements of the path to absolute.
        /// </summary>
        /// <param name="path">The path to resolve.</param>
        /// <returns></returns>
        string ResolvePath(string path);
    }
}
