using InformationRetrievalManager.Crawler;
using InformationRetrievalManager.Relational;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for <see cref="DataInstancePage"/>
    /// </summary>
    public class DataInstancePageViewModel : BaseViewModel
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;

        #endregion

        #region Private Members

        /// <summary>
        /// Data instance managed by this view model.
        /// </summary>
        private DataInstanceDataModel _dataInstance; //;

        /// <summary>
        /// Crawler engined currently used.
        /// </summary>
        private ICrawlerEngine _crawlerEngine = null;

        /// <summary>
        /// Collection of available data files for indexation.
        /// </summary>
        private List<CrawlerFileInfo> _dataFileSelection; //; ctor

        #endregion

        #region Public Properties

        /// <summary>
        /// Indicates if the data are already loaded into the VM (once the values changes to <see langword="true"/>).
        /// </summary>
        public bool DataLoaded { get; set; }

        /// <summary>
        /// Property for <see cref="_dataInstance"/>.
        /// </summary>
        public DataInstanceDataModel DataInstance => _dataInstance;

        /// <summary>
        /// Entry selection of available data files.
        /// </summary>
        public ComboEntryViewModel<CrawlerFileInfo> DataFileEntry { get; set; }

        /// <summary>
        /// Error string as a feedback to the user.
        /// </summary>
        public string FormErrorString { get; set; }

        #endregion

        #region Command Flags

        /// <summary>
        /// Indicates if crawler is currently processing
        /// </summary>
        public bool CrawlerInWorkFlag { get; set; }

        /// <summary>
        /// Indicates if index processing is currently in work
        /// </summary>
        public bool IndexProcessingInWorkFlag { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to go to the another page.
        /// </summary>
        public ICommand GoToHomePageCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DataInstancePageViewModel()
        {
            // Create commands.
            GoToHomePageCommand = new RelayCommand(GoToHomePageCommandRoutine);

            // Create data selection with its entry.
            _dataFileSelection = new List<CrawlerFileInfo>() { new CrawlerFileInfo("Select Data File", null) };
            DataFileEntry = new ComboEntryViewModel<CrawlerFileInfo>
            {
                Label = null,
                Description = "Please, select data for index processing from the selection of crawled data.",
                Validation = null,
                Value = _dataFileSelection[0],
                ValueList = _dataFileSelection,
                DisplayMemberPath = nameof(CrawlerFileInfo.Label)
            };
        }

        /// <summary>
        /// DI constructor
        /// </summary>
        public DataInstancePageViewModel(ILogger logger, IUnitOfWork uow)
            : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        /// <summary>
        /// Initialize view model with specific Data Instance ID.
        /// </summary>
        /// <param name="id">The ID</param>
        /// <returns>Returns self for chaining.</returns>
        /// <exception cref="ArgumentException">Invalid ID range.</exception>
        /// <exception cref="InvalidOperationException">If the data instance is already set.</exception>
        public DataInstancePageViewModel Init(long id)
        {
            if (id < 0)
                throw new ArgumentException(nameof(id));

            // If the value is not set yet...
            if (_dataInstance == null)
                Load(id);
            // Otherwise, it is already set...
            else
                throw new InvalidOperationException(nameof(_dataInstance));

            return this;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Command Routine : Go To Page
        /// </summary>
        private void GoToHomePageCommandRoutine()
        {
            DI.ViewModelApplication.GoToPage(ApplicationPage.Home);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads necessary data structures according to data instance <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ID</param>
        private void Load(long id)
        {
            // Load data instance
            _dataInstance = _uow.DataInstances.Get(o =>o.Id == id, 
                includeProperties: new string[] { nameof(DataInstanceDataModel.CrawlerConfiguration), nameof(DataInstanceDataModel.IndexProcessingConfiguration) })
                .FirstOrDefault();

            // If the data instance does not exist...
            if (_dataInstance == null)
                // Move the user back...
                GoToHomePageCommandRoutine();

            // Flag up data load is done
            DataLoaded = true;
        }

        /// <summary>
        /// Update data file selection and its entry.
        /// </summary>
        /// <param name="data">New data selection list (<see langword="null"/> clears just the list).</param>
        /// <remarks>
        ///     Method expects to already have 1 item (first) that represents default selected item in <see cref="_dataFileSelection"/>.
        /// </remarks>
        private void UpdateDataFileSelection(List<CrawlerFileInfo> data)
        {
            var newData = new List<CrawlerFileInfo>();
            newData.Add(_dataFileSelection[0]);

            // Clear previous data selection
            _dataFileSelection.Clear();

            if (data != null)
                newData.AddRange(data);

            // Update the file selection
            _dataFileSelection = newData;

            // Update the file selection entry
            DataFileEntry.ValueList = _dataFileSelection;
            DataFileEntry.Value = _dataFileSelection[0]; // Default selected value
        }

        #endregion
    }
}
