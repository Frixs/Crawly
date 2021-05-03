using System.Collections.ObjectModel;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Context for control <see cref="QueryDataResult"/>.
    /// </summary>
    public class QueryDataResultContext
    {
        #region Public Properties

        /// <summary>
        /// Data for view presentation of this context.
        /// </summary>
        public ObservableCollection<Result> Data { get; set; } = new ObservableCollection<Result>();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public QueryDataResultContext()
        {

        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Result data wrapper for GUI
        /// </summary>
        public class Result
        {
            public string Title { get; set; }
            public string Category { get; set; }
            public string Timestamp { get; set; }
            public string SourceUrl { get; set; }
            public string Content { get; set; }
        }

        #endregion
    }
}
