using System.Collections.ObjectModel;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Context for control <see cref="QueryDataResultView"/>.
    /// </summary>
    public class QueryDataResultViewContext : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Data for view presentation of this context.
        /// </summary>
        public ObservableCollection<Result> Data { get; set; } = new ObservableCollection<Result>();

        /// <summary>
        /// Number of total documents that were searched for the data.
        /// </summary>
        public long TotalDocuments { get; set; } = -1;

        /// <summary>
        /// Number of documents found in total.
        /// </summary>
        public long FoundDocuments { get; set; } = -1;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public QueryDataResultViewContext()
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears the data into default.
        /// </summary>
        public void ClearData()
        {
            TotalDocuments = -1;
            FoundDocuments = -1;
            Data.Clear();
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
