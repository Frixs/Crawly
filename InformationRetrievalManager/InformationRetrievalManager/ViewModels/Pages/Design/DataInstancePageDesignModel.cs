namespace InformationRetrievalManager
{
    public class DataInstancePageDesignModel : DataInstancePageViewModel
    {
        #region New Instance Getter (Design)

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public static DataInstancePageDesignModel Instance => new DataInstancePageDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataInstancePageDesignModel()
        {
            DataLoaded = true;
            CrawlerInWork = true;
        }

        #endregion
    }
}
