
using InformationRetrievalManager.Relational;
using System.Collections.ObjectModel;

namespace InformationRetrievalManager
{
    public class HomePageDesignModel : HomePageViewModel
    {
        #region New Instance Getter (Design)

        /// <summary>
        /// Create a new instance of this class.
        /// </summary>
        public static HomePageDesignModel Instance => new HomePageDesignModel();

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HomePageDesignModel()
        {
            DataInstances = new ObservableCollection<DataInstanceDataModel>
            {
                new DataInstanceDataModel
                {
                    Id = 0,
                    Name = "my_crawler"
                },
                new DataInstanceDataModel
                {
                    Id = 1,
                    Name = "aBBA"
                },
                new DataInstanceDataModel
                {
                    Id = 2,
                    Name = "other_things"
                },
                new DataInstanceDataModel
                {
                    Id = 3,
                    Name = "all_should_Be_lowercase"
                },
                new DataInstanceDataModel
                {
                    Id = 4,
                    Name = "data"
                },
                new DataInstanceDataModel
                {
                    Id = 5,
                    Name = "lorem_ipsum"
                },
            };
        }

        #endregion
    }
}
