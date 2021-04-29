namespace InformationRetrievalManager
{
    /// <summary>
    /// Wrapper to keep crawler data file information bound together.
    /// </summary>
    public class CrawlerFileInfo
    {
        #region Public Properties

        /// <summary>
        /// Label representing the data file.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Filepath of the data file.
        /// </summary>
        public string FilePath { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="label">Label of the file.</param>
        /// <param name="filePath">Filepath of the file.</param>
        public CrawlerFileInfo(string label, string filePath)
        {
            Label = label;
            FilePath = filePath;
        }

        #endregion
    }
}
