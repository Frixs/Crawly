using System;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Wrapper to keep crawler data file information bound together.
    /// </summary>
    public class DataFileInfo
    {
        #region Public Properties

        /// <summary>
        /// Label representing the data file.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Filepath of the data file.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Filepath of the data file.
        /// </summary>
        public DateTime CreatedAt { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="label">Label of the file.</param>
        /// <param name="filePath">Filepath of the file.</param>
        /// <param name="createdAt">Created at date time of the file..</param>
        public DataFileInfo(string label, string filePath, DateTime createdAt)
        {
            Label = label;
            FilePath = filePath;
            CreatedAt = createdAt;
        }

        #endregion
    }
}
