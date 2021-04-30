using InformationRetrievalManager.Core;
using InformationRetrievalManager.Crawler;
using InformationRetrievalManager.Relational;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        private readonly ITaskManager _taskManager;
        private readonly IUnitOfWork _uow;
        private readonly ICrawlerManager _crawlerManager;

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

        /// <summary>
        /// Collection of available index files.
        /// </summary>
        private List<CrawlerFileInfo> _indexFileSelection; //; ctor

        #endregion

        #region Public Properties

        /// <summary>
        /// Indicates if the data are already loaded into the VM (once the values changes to <see langword="true"/>).
        /// </summary>
        public bool DataLoaded { get; protected set; }

        /// <summary>
        /// Property for <see cref="_dataInstance"/>.
        /// </summary>
        public DataInstanceDataModel DataInstance => _dataInstance;

        /// <summary>
        /// Entry selection of available data files.
        /// </summary>
        public ComboEntryViewModel<CrawlerFileInfo> DataFileEntry { get; protected set; }

        /// <summary>
        /// Entry selection of available indexed data files.
        /// </summary>
        public ComboEntryViewModel<CrawlerFileInfo> IndexFileEntry { get; protected set; }

        /// <summary>
        /// Entry for query
        /// </summary>
        public TextEntryViewModel QueryEntry { get; protected set; }

        /// <summary>
        /// Crawler processing progress feedback message to user.
        /// </summary>
        public string CrawlerProgress { get; protected set; }

        /// <summary>
        /// Error string as a feedback to the user.
        /// </summary>
        public string FormErrorString { get; protected set; }

        /// <summary>
        /// Crawler progress feedback message (1-5)
        /// </summary>
        public string[] CrawlerProgressMsgs { get; protected set; } = new string[5];

        /// <summary>
        /// Crawler progress page url (1-5)
        /// </summary>
        public string[] CrawlerProgressUrls { get; protected set; } = new string[5];

        #endregion

        #region Command Flags

        /// <summary>
        /// Indicates if crawler is currently processing
        /// </summary>
        public bool CrawlerInWork
        {
            get => CrawlerInWorkFlag || _crawlerEngine != null || (_crawlerEngine != null && _crawlerEngine.IsCurrentlyCrawling);
            set => CrawlerInWorkFlag = value;
        }

        /// <summary>
        /// Command flag for crawler controls
        /// </summary>
        private bool CrawlerInWorkFlag { get; set; }

        /// <summary>
        /// Indicates if index processing is currently in work
        /// </summary>
        public bool IndexProcessingInWorkFlag { get; set; }

        /// <summary>
        /// Indicates if query is currently searcheing
        /// </summary>
        public bool QueryInWorkFlag { get; set; }

        /// <summary>
        /// Command flag for opening process for opening web pages
        /// </summary>
        private bool OpenWebpageFlag { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to go to the another page.
        /// </summary>
        public ICommand GoToHomePageCommand { get; set; }

        /// <summary>
        /// The command to open URL according to the command parameter.
        /// </summary>
        public ICommand OpenUrlCommand { get; set; }

        /// <summary>
        /// The command to start crawler processing.
        /// </summary>
        public ICommand StartCrawlerCommand { get; set; }

        /// <summary>
        /// The command to start crawler cancel.
        /// </summary>
        public ICommand CancelCrawlerCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DataInstancePageViewModel()
        {
            // Create commands.
            GoToHomePageCommand = new RelayCommand(GoToHomePageCommandRoutine);
            OpenUrlCommand = new RelayParameterizedCommand((parameter) => OpenUrlCommandRoutineAsync(parameter));
            StartCrawlerCommand = new RelayCommand(async () => await StartCrawlerCommandRoutineAsync());
            CancelCrawlerCommand = new RelayCommand(async () => await CancelCrawlerCommandRoutineAsync());

            // Create data selection with its entry.
            _dataFileSelection = new List<CrawlerFileInfo>() { new CrawlerFileInfo("< Select Data File >", null) };
            DataFileEntry = new ComboEntryViewModel<CrawlerFileInfo>
            {
                Label = null,
                Description = "Please, select data for index processing from the selection of crawled data.",
                Validation = null,
                Value = _dataFileSelection[0],
                ValueList = _dataFileSelection,
                DisplayMemberPath = nameof(CrawlerFileInfo.Label)
            };

            // Create index selection with its entry
            _indexFileSelection = new List<CrawlerFileInfo>() { new CrawlerFileInfo("< Select Index File >", null) };
            IndexFileEntry = new ComboEntryViewModel<CrawlerFileInfo>
            {
                Label = null,
                Description = "Please, select an index for querying from the selection of indexed data.",
                Validation = null,
                Value = _indexFileSelection[0],
                ValueList = _indexFileSelection,
                DisplayMemberPath = nameof(CrawlerFileInfo.Label)
            };

            // Create query entry
            QueryEntry = new TextEntryViewModel
            {
                Label = null,
                Description = null,
                Validation = null,
                Value = null,
                Placeholder = "Ask Me Here",
                MaxLength = 155
            };
        }

        /// <summary>
        /// DI constructor
        /// </summary>
        public DataInstancePageViewModel(ILogger logger, ITaskManager taskManager, IUnitOfWork uow, ICrawlerManager crawlerManager)
            : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _taskManager = taskManager ?? throw new ArgumentNullException(nameof(taskManager));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _crawlerManager = crawlerManager ?? throw new ArgumentNullException(nameof(crawlerManager));
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
                _taskManager.RunAndForget(() => LoadAsync(id));
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

        /// <summary>
        /// Open URL according to the <paramref name="parameter"/>.
        /// </summary>
        /// <param name="parameter">The URL</param>
        private async Task OpenUrlCommandRoutineAsync(object parameter)
        {
            await RunCommandAsync(() => OpenWebpageFlag, async () =>
            {
                string url = parameter as string;

                if (!string.IsNullOrEmpty(url) && url.IsURL())
                    System.Diagnostics.Process.Start(url);

                await Task.Delay(1);
            });
        }

        /// <summary>
        /// Command Routine : Start crawler processing
        /// </summary>
        private async Task StartCrawlerCommandRoutineAsync()
        {
            if (_crawlerEngine != null)
                return;

            await RunCommandAsync(() => CrawlerInWorkFlag, async () =>
            {
                // Initialize the crawler
                _crawlerEngine = new CrawlerEngine(_dataInstance.Id.ToString());
                _crawlerEngine.SetControls(
                    _dataInstance.CrawlerConfiguration.SiteAddress,
                    _dataInstance.CrawlerConfiguration.SiteSuffix,
                    _dataInstance.CrawlerConfiguration.StartPageNo,
                    _dataInstance.CrawlerConfiguration.MaxPageNo,
                    _dataInstance.CrawlerConfiguration.PageNoModifier,
                    _dataInstance.CrawlerConfiguration.SearchInterval,
                    _dataInstance.CrawlerConfiguration.SiteUrlArticlesXPath,
                    _dataInstance.CrawlerConfiguration.SiteArticleContentAreaXPath,
                    _dataInstance.CrawlerConfiguration.SiteArticleTitleXPath,
                    _dataInstance.CrawlerConfiguration.SiteArticleCategoryXPath,
                    _dataInstance.CrawlerConfiguration.SiteArticleDateTimeXPath,
                    new DatetimeParseData(
                        _dataInstance.CrawlerConfiguration.SiteArticleDateTimeFormat,
                        new CultureInfo(_dataInstance.CrawlerConfiguration.SiteArticleDateTimeCultureInfo))
                    );
                CrawlerProgress = string.Empty;
                UpdateCrawlerEvents(_crawlerEngine);

                // Start the crawler
                _crawlerEngine.Start();

                OnPropertyChanged(nameof(CrawlerInWork));
                await Task.Delay(1);
            });
        }

        /// <summary>
        /// Command Routine : Cancel crawler processing
        /// </summary>
        private async Task CancelCrawlerCommandRoutineAsync()
        {
            if (_crawlerEngine == null)
                return;

            await RunCommandAsync(() => CrawlerInWorkFlag, async () =>
            {
                _crawlerEngine.Cancel();

                await Task.Delay(1);
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads necessary data structures according to data instance <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ID</param>
        private async Task LoadAsync(long id)
        {
            // Load data instance
            _dataInstance = _uow.DataInstances.Get(o => o.Id == id,
                includeProperties: new string[] { nameof(DataInstanceDataModel.CrawlerConfiguration), nameof(DataInstanceDataModel.IndexProcessingConfiguration) })
                .FirstOrDefault();

            // If the data instance does not exist...
            if (_dataInstance == null)
                // Move the user back...
                GoToHomePageCommandRoutine();
            OnPropertyChanged(nameof(DataInstance));

            // Get crawler engine (if there is running one)
            _crawlerEngine = await _crawlerManager.GetCrawlerAsync(_dataInstance.Id.ToString());
            if (_crawlerEngine != null)
                UpdateCrawlerEvents(_crawlerEngine);

            // Flag up data load is done
            DataLoaded = true;
        }

        /// <summary>
        /// Update crawler events in this view model.
        /// </summary>
        /// <param name="crawler">The crawler</param>
        private void UpdateCrawlerEvents(ICrawlerEngine crawler)
        {
            crawler.OnStartProcessEvent += (s, e) =>
            {
                CrawlerProgress = "Starting...";
                Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(CrawlerProgress)));
            };
            crawler.OnProcessProgressEvent += (s, e) =>
            {
                for (int i = CrawlerProgressMsgs.Length - 1; i >= 0; --i)
                {
                    if (i == 0)
                    {
                        CrawlerProgressMsgs[i] = e.CrawlingProgressMsg;
                        CrawlerProgressUrls[i] = e.CrawlingProgressUrl;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(CrawlerProgressMsgs[i - 1]))
                            continue;
                        else
                        {
                            CrawlerProgressMsgs[i] = CrawlerProgressMsgs[i - 1];
                            CrawlerProgressUrls[i] = CrawlerProgressUrls[i - 1];
                        }
                    }
                }

                CrawlerProgress = $"{e.CrawlingProgressPct}%";

                Application.Current.Dispatcher.Invoke(() =>
                {
                    OnPropertyChanged(nameof(CrawlerProgressMsgs));
                    OnPropertyChanged(nameof(CrawlerProgressUrls));
                    OnPropertyChanged(nameof(CrawlerProgress));
                });
            };
            crawler.OnFinishProcessEvent += (s, e) =>
            {
                CrawlerProgress = "Done!";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    OnPropertyChanged(nameof(CrawlerProgress));

                    _crawlerEngine = null;

                    OnPropertyChanged(nameof(CrawlerInWork));

                    for (int i = 0; i < CrawlerProgressMsgs.Length; ++i)
                        CrawlerProgressMsgs[i] = CrawlerProgressUrls[i] = string.Empty;
                });
            };
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
