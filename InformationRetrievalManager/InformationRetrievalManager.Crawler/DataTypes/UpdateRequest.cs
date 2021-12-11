using System;

namespace InformationRetrievalManager.Crawler
{
    /// <summary>
    /// Data required for crawler engine to update existing crawler data.
    /// </summary>
    public class UpdateRequest
    {
        #region Public Properties

        /// <summary>
        /// File path of the file that has to be updated.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Mode of the update.
        /// </summary>
        public UpdateMode Mode { get; }

        /// <summary>
        /// Parameter of <see cref="Mode"/> for value <see cref="UpdateMode.Timestamp"/>.
        /// </summary>
        public DateTime ParameterTimestamp { get; }

        /// <summary>
        /// Parameter of <see cref="Mode"/> for value <see cref="UpdateMode.Title"/>.
        /// </summary>
        public string ParameterTitle { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        private UpdateRequest(string filePath, UpdateMode mode)
        {
            FilePath = filePath;
            Mode = mode;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UpdateRequest(string filePath, UpdateMode mode, DateTime parameter)
            : this(filePath, mode)
        {
            ParameterTimestamp = parameter;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UpdateRequest(string filePath, UpdateMode mode, string parameter)
            : this(filePath, mode)
        {
            ParameterTitle = parameter;
        }

        #endregion
    }
}
