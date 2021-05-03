using InformationRetrievalManager.Core;
using InformationRetrievalManager.Crawler;
using InformationRetrievalManager.NLP;
using InformationRetrievalManager.Relational;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        private readonly IFileManager _fileManager;
        private readonly IUnitOfWork _uow;
        private readonly ICrawlerManager _crawlerManager;
        private readonly ICrawlerStorage _crawlerStorage;
        private readonly IIndexStorage _indexStorage;
        private readonly IQueryIndexManager _queryIndexManager;

        #endregion

        #region Limit Constants

        public static readonly bool QueryEntry_IsRequired = true;

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
        private List<DataFileInfo> _dataFileSelection; //; ctor

        /// <summary>
        /// Collection of available index files.
        /// </summary>
        private List<DataFileInfo> _indexFileSelection; //; ctor

        /// <summary>
        /// Currently selected query model.
        /// </summary>
        private QueryModelType _selectedQueryModel; //; ctor

        #endregion

        #region Public Properties

        /// <summary>
        /// Property for <see cref="_dataInstance"/>.
        /// </summary>
        public DataInstanceDataModel DataInstance => _dataInstance;

        /// <summary>
        /// <see cref="View.Results"/> context
        /// </summary>
        public QueryDataResultViewContext ResultContext { get; } = new QueryDataResultViewContext();

        /// <summary>
        /// <see cref="View.Configuration"/> context
        /// </summary>
        public DataInstanceConfigurationViewContext ConfigurationContext { get; } = new DataInstanceConfigurationViewContext();

        /// <summary>
        /// Entry selection of available data files.
        /// </summary>
        public ComboEntryViewModel<DataFileInfo> DataFileEntry { get; protected set; } //; ctor

        /// <summary>
        /// Entry selection of available indexed data files.
        /// </summary>
        public ComboEntryViewModel<DataFileInfo> IndexFileEntry { get; protected set; } //; ctor

        /// <summary>
        /// Entry for query.
        /// </summary>
        [ValidateString(nameof(QueryEntry), typeof(DataInstancePageViewModel),
            pIsRequired: nameof(QueryEntry_IsRequired))]
        [ValidateBooleanExpressionString(nameof(QueryEntry), typeof(DataInstancePageViewModel),
            pIsRequired: nameof(QueryEntry_IsRequired))]
        public TextEntryViewModel QueryEntry { get; protected set; } //; ctor

        /// <summary>
        /// Entry for limiting query selection.
        /// </summary>
        public IntegerEntryViewModel QuerySelectLimitEntry { get; protected set; } //; ctor

        /// <summary>
        /// Query model radio entry array
        /// </summary>
        public RadioEntryViewModel[] QueryModelEntryArray { get; protected set; } //; ctor

        /// <summary>
        /// Crawler processing progress feedback message to user.
        /// </summary>
        public string CrawlerProgress { get; protected set; }

        /// <summary>
        /// Index processing progress feedback message to user.
        /// </summary>
        public string IndexProcessingProgress { get; protected set; }

        /// <summary>
        /// Crawler progress feedback message (1-5)
        /// </summary>
        public string[] CrawlerProgressMsgs { get; protected set; } = new string[5];

        /// <summary>
        /// Crawler progress page url (1-5)
        /// </summary>
        public string[] CrawlerProgressUrls { get; protected set; } = new string[5];

        /// <summary>
        /// Gives query feedback to the user if error occurrs.
        /// </summary>
        public string QueryErrorString { get; protected set; }

        #endregion

        #region Flags

        /// <summary>
        /// Indicates current view that should be (is) displayed.
        /// </summary>
        public View CurrentView { get; set; } //; ctor init

        /// <summary>
        /// Indicates if the data are already loaded into the VM (once the values changes to <see langword="true"/>).
        /// </summary>
        public bool DataLoaded { get; protected set; }

        /// <summary>
        /// Indicates if crawler is currently processing
        /// </summary>
        public bool CrawlerInWork
        {
            get => CrawlerInWorkFlag || _crawlerEngine != null || (_crawlerEngine != null && _crawlerEngine.IsCurrentlyCrawling);
            set => CrawlerInWorkFlag = value;
        }

        #endregion

        #region Command Flags

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
        /// Command flag for opening process (e.g. files or for opening web pages)
        /// </summary>
        private bool ProcessFlag { get; set; }

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

        /// <summary>
        /// The command to open raw file data.
        /// </summary>
        public ICommand OpenRawDataCommand { get; set; }

        /// <summary>
        /// The command to delete crawler data file.
        /// </summary>
        public ICommand DeleteDataFileCommand { get; set; }

        /// <summary>
        /// The command to start index processing.
        /// </summary>
        public ICommand StartIndexProcessingCommand { get; set; }

        /// <summary>
        /// The command to delete index data file.
        /// </summary>
        public ICommand DeleteIndexFileCommand { get; set; }

        /// <summary>
        /// The command to start index processing.
        /// </summary>
        public ICommand StartQueryCommand { get; set; }

        /// <summary>
        /// The command to change the view based on parameter.
        /// </summary>
        public ICommand ShowViewCommand { get; set; }

        public ICommand ToggleEditCrawlerConfigurationReadOnlyCommand { get; set; }
        public ICommand ToggleEditProcessingConfigurationReadOnlyCommand { get; set; }
        public ICommand ToggleEditDataInstanceNameReadOnlyCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public DataInstancePageViewModel()
        {
            // Create commands.
            GoToHomePageCommand = new RelayCommand(GoToHomePageCommandRoutine);
            OpenUrlCommand = new RelayParameterizedCommand(async (parameter) => await OpenUrlCommandRoutineAsync(parameter));
            StartCrawlerCommand = new RelayCommand(async () => await StartCrawlerCommandRoutineAsync());
            CancelCrawlerCommand = new RelayCommand(async () => await CancelCrawlerCommandRoutineAsync());
            OpenRawDataCommand = new RelayParameterizedCommand(async (parameter) => await OpenRawDataCommandRoutineAsync(parameter));
            DeleteDataFileCommand = new RelayParameterizedCommand(async (parameter) => await DeleteDataFileCommandRoutineAsync(parameter));
            StartIndexProcessingCommand = new RelayCommand(async () => await StartIndexProcessingCommandRoutineAsync());
            DeleteIndexFileCommand = new RelayParameterizedCommand(async (parameter) => await DeleteIndexFileCommandRoutineAsync(parameter));
            StartQueryCommand = new RelayCommand(async () => await StartQueryCommandRoutineAsync());
            ShowViewCommand = new RelayParameterizedCommand((parameter) => ShowViewCommandRoutine(parameter));

            ConfigurationContext.ToggleEditCrawlerConfigurationReadOnlyCommand = new RelayCommand(ToggleEditCrawlerConfigurationReadOnlyCommandRoutine);
            ConfigurationContext.ToggleEditProcessingConfigurationReadOnlyCommand = new RelayCommand(ToggleEditProcessingConfigurationReadOnlyCommandRoutine);
            ConfigurationContext.ToggleEditDataInstanceNameReadOnlyCommand = new RelayCommand(ToggleEditDataInstanceNameReadOnlyCommandRoutine);
            ConfigurationContext.CrawlerConfigurationUpdateCommand = new RelayCommand(async () => await CrawlerConfigurationUpdateCommandRoutineAsync());
            ConfigurationContext.ProcessingConfigurationUpdateCommand = new RelayCommand(async () => await ProcessingConfigurationUpdateCommandRoutineAsync());
            ConfigurationContext.DataInstanceNameUpdateCommand = new RelayCommand(async () => await DataInstanceNameUpdateCommandRoutineAsync());
            ConfigurationContext.DataInstanceDeleteCommand = new RelayCommand(async () => await DataInstanceDeleteCommandRoutineAsync());

            // Create data selection with its entry.
            _dataFileSelection = new List<DataFileInfo>() { new DataFileInfo("< Select Data File >", null, default) };
            DataFileEntry = new ComboEntryViewModel<DataFileInfo>
            {
                Label = null,
                Description = "Please, select data for index processing from the selection of crawled data.",
                Validation = null,
                Value = _dataFileSelection[0],
                ValueList = _dataFileSelection,
                DisplayMemberPath = nameof(DataFileInfo.Label)
            };

            // Create index selection with its entry
            _indexFileSelection = new List<DataFileInfo>() { new DataFileInfo("< Select Index File >", null, default) };
            IndexFileEntry = new ComboEntryViewModel<DataFileInfo>
            {
                Label = null,
                Description = "Please, select an index for querying from the selection of indexed data.",
                Validation = null,
                Value = _indexFileSelection[0],
                ValueList = _indexFileSelection,
                DisplayMemberPath = nameof(DataFileInfo.Label)
            };

            // Create query input entry
            QueryEntry = new TextEntryViewModel
            {
                Label = null,
                Description = Localization.Resource.QueryEntry_Description_TfIdf,
                Validation = null,
                Value = null,
                Placeholder = "Ask Me Here",
                MaxLength = 155
            };

            // Create query select limit query
            QuerySelectLimitEntry = new IntegerEntryViewModel
            {
                Label = null,
                Description = null,
                Validation = null,
                Value = 10,
                MinValue = 1,
                MaxValue = 100
            };

            // Create query model entry(ies)
            _selectedQueryModel = QueryModelType.TfIdf; // Set default selection
            QueryEntry.Validation = new ValidateStringAttribute("Query", typeof(DataInstancePageViewModel), nameof(QueryEntry_IsRequired));
            QueryModelEntryArray = new RadioEntryViewModel[2]
            {
                new RadioEntryViewModel
                {
                    Label = "TF-IDF",
                    Description = null,
                    Validation = null,
                    Value = true, // Set default selection
                    GroupName = nameof(QueryModelEntryArray),
                    CheckAction = async () =>
                    {
                        _selectedQueryModel = QueryModelType.TfIdf;
                        QueryEntry.Description = Localization.Resource.QueryEntry_Description_TfIdf;
                        QueryEntry.Validation = new ValidateStringAttribute("Query", typeof(DataInstancePageViewModel), nameof(QueryEntry_IsRequired));
                        await Task.Delay(1);
                    }
                },
                new RadioEntryViewModel
                {
                    Label = "Boolean",
                    Description = null,
                    Validation = null,
                    Value = false,
                    GroupName = nameof(QueryModelEntryArray),
                    CheckAction = async () =>
                    {
                        _selectedQueryModel = QueryModelType.Boolean;
                        QueryEntry.Description = Localization.Resource.QueryEntry_Description_Boolean;
                        QueryEntry.Validation = new ValidateBooleanExpressionStringAttribute("Query", typeof(DataInstancePageViewModel), nameof(QueryEntry_IsRequired));
                        await Task.Delay(1);
                    }
                }
            };
        }

        /// <summary>
        /// DI constructor
        /// </summary>
        public DataInstancePageViewModel(ILogger logger, ITaskManager taskManager, IFileManager fileManager, IUnitOfWork uow, ICrawlerManager crawlerManager, ICrawlerStorage crawlerStorage, IIndexStorage indexStorage, IQueryIndexManager queryIndexManager)
            : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _taskManager = taskManager ?? throw new ArgumentNullException(nameof(taskManager));
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _crawlerManager = crawlerManager ?? throw new ArgumentNullException(nameof(crawlerManager));
            _crawlerStorage = crawlerStorage ?? throw new ArgumentNullException(nameof(crawlerStorage));
            _indexStorage = indexStorage ?? throw new ArgumentNullException(nameof(indexStorage));
            _queryIndexManager = queryIndexManager ?? throw new ArgumentNullException(nameof(queryIndexManager));
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
                _taskManager.RunAndForget(() => LoadAsync(id, true));
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
            await RunCommandAsync(() => ProcessFlag, async () =>
            {
                string url = parameter as string;

                if (!string.IsNullOrEmpty(url) && url.IsURL())
                    System.Diagnostics.Process.Start(url);

                await Task.Delay(1);
            });
        }

        /// <summary>
        /// Command Routine : Start crawler processing.
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
                await _crawlerManager.AddCrawlerAsync(_crawlerEngine);

                OnPropertyChanged(nameof(CrawlerInWork));
            });
        }

        /// <summary>
        /// Command Routine : Cancel crawler processing.
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

        /// <summary>
        /// Command Routine : Open raw file data
        /// </summary>
        /// <param name="parameter"><see cref="DataFileInfo"/></param>
        private async Task OpenRawDataCommandRoutineAsync(object parameter)
        {
            await RunCommandAsync(() => ProcessFlag, async () =>
            {
                var fileInfo = parameter as DataFileInfo;

                if (fileInfo != null && fileInfo.FilePath != null && File.Exists(fileInfo.FilePath))
                    System.Diagnostics.Process.Start(fileInfo.FilePath);

                await Task.Delay(1);
            });
        }

        /// <summary>
        /// Command Routine : Delete data (crawler) file
        /// </summary>
        /// <param name="parameter"><see cref="DataFileInfo"/></param>
        private async Task DeleteDataFileCommandRoutineAsync(object parameter)
        {
            await RunCommandAsync(() => ProcessFlag, async () =>
            {
                var fileInfo = parameter as DataFileInfo;

                if (fileInfo != null && fileInfo.FilePath != null && File.Exists(fileInfo.FilePath))
                {
                    _crawlerStorage.DeleteDataFiles(_dataInstance.Id.ToString(), fileInfo.CreatedAt);
                    LoadDataFiles(true);
                }

                await Task.Delay(1);
            });
        }

        /// <summary>
        /// Command Routine : Start index processing.
        /// </summary>
        private async Task StartIndexProcessingCommandRoutineAsync()
        {
            await RunCommandAsync(() => IndexProcessingInWorkFlag, async () =>
            {
                IndexProcessingProgress = string.Empty;

                DataFileInfo file = DataFileEntry.Value;
                if (file.FilePath == null) // should not happen - but it is default selection protection
                    return;

                // We do not want to let it process during crawling
                if (CrawlerInWork)
                    return;

                IndexProcessingProgress = "Starting...";

                await _taskManager.Run(async () =>
                {
                    CrawlerDataModel[] data = null;
                    // Deserialize JSON directly from the file
                    using (StreamReader sr = File.OpenText(file.FilePath))
                    {
                        JsonSerializer jsonSerializer = new JsonSerializer();
                        data = (CrawlerDataModel[])jsonSerializer.Deserialize(sr, typeof(CrawlerDataModel[]));
                    }

                    // If any data...
                    if (data != null && data.Length > 0)
                    {
                        _uow.BeginTransaction();

                        IndexProcessingProgress = "Preparing documents...";

                        // Clear all the previous/old indexes first (if any)
                        foreach (var doc in _uow.IndexedDocuments.Get())
                            _uow.IndexedDocuments.Delete(doc);

                        // Prepare documents for indexation
                        bool anyIndexedData = false;
                        List<IndexDocument> docs = new List<IndexDocument>();
                        for (int i = 0; i < data.Length; ++i)
                        {
                            var model = new IndexedDocumentDataModel
                            {
                                DataInstanceId = _dataInstance.Id,
                                Title = data[i].Title,
                                Category = data[i].Category,
                                Timestamp = data[i].Timestamp,
                                SourceUrl = data[i].SourceUrl,
                                Content = StringHelpers.ShortenWithDots(Regex.Replace(data[i].Content.Replace(Environment.NewLine, " "), @"[ ]+", " "), IndexedDocumentDataModel.Content_MaxLength - 3)
                            };

                            // Validate, if no errors...
                            if (ValidationHelpers.ValidateModel(model).Count == 0)
                            {
                                _uow.IndexedDocuments.Insert(model);
                                _uow.SaveChanges();

                                docs.Add(data[i].ToIndexDocument(model.Id));
                                anyIndexedData = true;
                            }
                        }

                        // If any documents are ready for indexation...
                        if (anyIndexedData)
                        {
                            IndexProcessingProgress = "Indexing...";

                            // Indexate documents
                            var processing = new IndexProcessing(_dataInstance.Id.ToString(), DateTime.Now, _dataInstance.IndexProcessingConfiguration, _fileManager, _logger);

                            processing.IndexDocuments(docs.ToArray(), save: true);
                            _logger.LogDebugSource("Index processing done!");
                            IndexProcessingProgress = "Done!";

                            _uow.CommitTransaction();

                            Application.Current.Dispatcher.Invoke(() => LoadIndexFiles(true));
                        }
                        // Otherwise no documents are ready (corrupted)...
                        else
                        {
                            IndexProcessingProgress = "No documents are valid for indexation!";

                            _uow.RollbackTransaction();
                        }
                    }
                    // Otherwise, corrupted data or no data...
                    else
                    {
                        IndexProcessingProgress = "Corrupted data file!";

                        await DeleteDataFileCommandRoutineAsync(file);
                    }
                });
            });
        }

        /// <summary>
        /// Command Routine : Delete data (crawler) file
        /// </summary>
        /// <param name="parameter"><see cref="DataFileInfo"/></param>
        private async Task DeleteIndexFileCommandRoutineAsync(object parameter)
        {
            await RunCommandAsync(() => ProcessFlag, async () =>
            {
                var fileInfo = parameter as DataFileInfo;

                if (fileInfo != null && fileInfo.FilePath != null && File.Exists(fileInfo.FilePath))
                {
                    _indexStorage.DeleteIndexFiles(_dataInstance.Id.ToString(), fileInfo.CreatedAt);
                    LoadIndexFiles(true);
                }

                await Task.Delay(1);
            });
        }

        /// <summary>
        /// Command Routine : Start query request
        /// </summary>
        private async Task StartQueryCommandRoutineAsync()
        {
            await RunCommandAsync(() => QueryInWorkFlag, async () =>
            {
                string query = QueryEntry.Value;
                QueryModelType queryModel = _selectedQueryModel;
                DataFileInfo file = IndexFileEntry.Value;
                int select = QuerySelectLimitEntry.Value;

                // Clear the previous results first
                QueryErrorString = null;
                ResultContext.ClearData();
                // Fire update context property
                OnPropertyChanged(nameof(ResultContext));

                if (string.IsNullOrEmpty(query))
                    return;

                if (file.FilePath == null) // should not happen - but it is default selection protection
                    return;

                // We do not want to let it process during indexation
                if (IndexProcessingInWorkFlag)
                    return;

                var ii = new InvertedIndex(_dataInstance.Id.ToString(), file.CreatedAt, _fileManager, _logger);

                (long[], long, long) queryResult = (Array.Empty<long>(), -1, -1);

                await _taskManager.Run(async () =>
                {
                    ii.Load();
                    queryResult = await _queryIndexManager.QueryAsync(query, ii.GetReadOnlyVocabulary(), queryModel, _dataInstance.IndexProcessingConfiguration, select);
                });

                // Go through the results...
                for (int i = 0; i < queryResult.Item1.Length; ++i)
                {
                    var doc = _uow.IndexedDocuments.GetByID(queryResult.Item1[i]);
                    if (doc != null)
                    {
                        ResultContext.Data.Add(new QueryDataResultViewContext.Result
                        {
                            Title = doc.Title,
                            Category = doc.Category,
                            Timestamp = doc.Timestamp.ToString("yyyy-MM-dd hh:mm"),
                            SourceUrl = doc.SourceUrl,
                            Content = doc.Content
                        });
                    }
                    else
                    {
                        // Something went wrong...
                        QueryErrorString = "Something went wrong during searching.";
                        break;
                    }
                }

                // If no error occurred...
                if (QueryErrorString == null)
                {
                    ResultContext.FoundDocuments = queryResult.Item2;
                    ResultContext.TotalDocuments = queryResult.Item3;
                    // Fire update context property
                    OnPropertyChanged(nameof(ResultContext));

                    // Automatically move the user to the result view
                    CurrentView = View.Results;
                }
            });
        }

        /// <summary>
        /// Change view to the result one.
        /// </summary>
        /// <param name="parameter">Int id specific for <see cref="View"/> or itself the enum.</param>
        private void ShowViewCommandRoutine(object parameter)
        {
            if (parameter.GetType().Equals(typeof(View)))
                CurrentView = (View)parameter;
            else
                CurrentView = (View)Enum.ToObject(typeof(View), int.Parse(parameter.ToString()));

            // If configuration, reload the data...
            if (CurrentView == View.Configuration)
            {
                // Re-initialize state values
                ConfigurationContext.FormErrorString = null;
                // Load data/values into the configuration context
                ConfigurationContext.Set(_dataInstance.CrawlerConfiguration, _dataInstance.IndexProcessingConfiguration, _dataInstance.Name);
            }
        }

        private void ToggleEditCrawlerConfigurationReadOnlyCommandRoutine()
        {
            ConfigurationContext.CrawlerConfigurationReadOnlyFlag = !ConfigurationContext.CrawlerConfigurationReadOnlyFlag;
            ConfigurationContext.CrawlerConfigurationContext.ReadOnly(ConfigurationContext.CrawlerConfigurationReadOnlyFlag);
        }
        private void ToggleEditProcessingConfigurationReadOnlyCommandRoutine()
        {
            ConfigurationContext.ProcessingConfigurationReadOnlyFlag = !ConfigurationContext.ProcessingConfigurationReadOnlyFlag;
            ConfigurationContext.ProcessingConfigurationContext.ReadOnly(ConfigurationContext.ProcessingConfigurationReadOnlyFlag);
        }
        private void ToggleEditDataInstanceNameReadOnlyCommandRoutine()
        {
            ConfigurationContext.DataInstanceNameReadOnlyFlag = !ConfigurationContext.DataInstanceNameReadOnlyFlag;
            ConfigurationContext.DataInstanceNameEntry.IsReadOnly = ConfigurationContext.DataInstanceNameReadOnlyFlag;
        }
        /// <summary>
        /// Update crawler configuration
        /// </summary>
        private async Task CrawlerConfigurationUpdateCommandRoutineAsync()
        {
            await RunCommandAsync(() => ConfigurationContext.CrawlerConfigurationUpdateCommandFlag, async () =>
            {
                // Re-initialize state values
                ConfigurationContext.FormErrorString = null;

                if (CrawlerInWork || IndexProcessingInWorkFlag || QueryInWorkFlag)
                {
                    ConfigurationContext.FormErrorString = "Cannot update configuration during processing!";
                    return;
                }

                string currentSite = _dataInstance.CrawlerConfiguration.SiteAddress + _dataInstance.CrawlerConfiguration.SiteSuffix;
                // Assign updated data
                _dataInstance.CrawlerConfiguration.SiteAddress = ConfigurationContext.CrawlerConfigurationContext.SiteAddressEntry.Value;
                _dataInstance.CrawlerConfiguration.SiteSuffix = ConfigurationContext.CrawlerConfigurationContext.SiteSuffixEntry.Value;
                _dataInstance.CrawlerConfiguration.StartPageNo = ConfigurationContext.CrawlerConfigurationContext.StartPageNoEntry.Value;
                _dataInstance.CrawlerConfiguration.MaxPageNo = ConfigurationContext.CrawlerConfigurationContext.MaxPageNoEntry.Value;
                _dataInstance.CrawlerConfiguration.PageNoModifier = ConfigurationContext.CrawlerConfigurationContext.PageNoModifierEntry.Value;
                _dataInstance.CrawlerConfiguration.SearchInterval = ConfigurationContext.CrawlerConfigurationContext.SearchIntervalEntry.Value;
                _dataInstance.CrawlerConfiguration.SiteUrlArticlesXPath = ConfigurationContext.CrawlerConfigurationContext.SiteUrlArticlesXPathEntry.Value;
                _dataInstance.CrawlerConfiguration.SiteArticleContentAreaXPath = ConfigurationContext.CrawlerConfigurationContext.SiteArticleContentAreaXPathEntry.Value;
                _dataInstance.CrawlerConfiguration.SiteArticleTitleXPath = ConfigurationContext.CrawlerConfigurationContext.SiteArticleTitleXPathEntry.Value;
                _dataInstance.CrawlerConfiguration.SiteArticleCategoryXPath = ConfigurationContext.CrawlerConfigurationContext.SiteArticleCategoryXPathEntry.Value;
                _dataInstance.CrawlerConfiguration.SiteArticleDateTimeXPath = ConfigurationContext.CrawlerConfigurationContext.SiteArticleDateTimeXPathEntry.Value;
                _dataInstance.CrawlerConfiguration.SiteArticleDateTimeFormat = ConfigurationContext.CrawlerConfigurationContext.SiteArticleDateTimeFormatEntry.Value;
                _dataInstance.CrawlerConfiguration.SiteArticleDateTimeCultureInfo = ConfigurationContext.CrawlerConfigurationContext.SiteArticleDateTimeCultureInfoEntry.Value;

                // Validate data
                var validationResults = ValidationHelpers.ValidateModel(_dataInstance.CrawlerConfiguration);

                // Additional validation steps
                // Culture info must be valid...
                try
                {
                    _ = new CultureInfo(_dataInstance.CrawlerConfiguration.SiteArticleDateTimeCultureInfo);
                }
                catch
                {
                    validationResults.Add(new DataValidationError
                    {
                        Code = nameof(_dataInstance.CrawlerConfiguration.SiteArticleDateTimeCultureInfo),
                        Description = "Invalid date-time culture."
                    }); // TODO localization
                }

                // If any errors...
                if (validationResults.Count > 0)
                {
                    _uow.UndoEntityChanges(_dataInstance.CrawlerConfiguration);
                    ConfigurationContext.FormErrorString = validationResults.AggregateErrors();
                }
                // Otherwise valid results...
                else
                {
                    var fileDeletionOk = true;
                    // If the site has changed...
                    if (!currentSite.Equals(ConfigurationContext.CrawlerConfigurationContext.SiteAddressEntry.Value + ConfigurationContext.CrawlerConfigurationContext.SiteSuffixEntry.Value))
                    {
                        // Find and delete all the crawled data files...
                        try
                        {
                            var filePaths = _crawlerStorage.GetAllDataFiles(_dataInstance.Id.ToString());
                            for (int i = 0; i < filePaths.Length; ++i)
                                File.Delete(filePaths[i]);
                        }
                        catch
                        {
                            fileDeletionOk = false;
                        }
                    }

                    // If the file deletion has no errors...
                    if (fileDeletionOk)
                    {
                        // Update
                        _uow.CrawlerConfigurations.Update(_dataInstance.CrawlerConfiguration);
                        _uow.SaveChanges();

                        // Reaload data
                        await LoadAsync(_dataInstance.Id);

                        // Log it
                        _logger.LogInformationSource($"Crawler configuration of '{_dataInstance.Name}' ({_dataInstance.Id}) successfully updated!");
                    }
                    // Otherwise, something went wrong during the file deletion...
                    else
                    {
                        _uow.UndoEntityChanges(_dataInstance.CrawlerConfiguration);
                        ConfigurationContext.FormErrorString = "Something went wrong during clearing old crawled data.";
                    }
                }
            });
        }
        /// <summary>
        /// Update processing configuration
        /// </summary>
        private async Task ProcessingConfigurationUpdateCommandRoutineAsync()
        {
            await RunCommandAsync(() => ConfigurationContext.ProcessingConfigurationUpdateCommandFlag, async () =>
            {
                // Re-initialize state values
                ConfigurationContext.FormErrorString = null;

                if (CrawlerInWork || IndexProcessingInWorkFlag || QueryInWorkFlag)
                {
                    ConfigurationContext.FormErrorString = "Cannot update configuration during processing!";
                    return;
                }

                // Assign updated data
                _dataInstance.IndexProcessingConfiguration.Language = ConfigurationContext.ProcessingConfigurationContext.LanguageEntry.Value;
                _dataInstance.IndexProcessingConfiguration.CustomRegex = ConfigurationContext.ProcessingConfigurationContext.CustomRegexEntry.Value;
                _dataInstance.IndexProcessingConfiguration.CustomStopWords = new HashSet<string>(ConfigurationContext.ProcessingConfigurationContext.CustomStopWordsEntry.Value.Split(IndexProcessingConfiguration.CustomStopWords_Separator));
                _dataInstance.IndexProcessingConfiguration.ToLowerCase = ConfigurationContext.ProcessingConfigurationContext.ToLowerCaseEntry.Value;
                _dataInstance.IndexProcessingConfiguration.RemoveAccentsBeforeStemming = ConfigurationContext.ProcessingConfigurationContext.RemoveAccentsBeforeStemmingEntry.Value;
                _dataInstance.IndexProcessingConfiguration.RemoveAccentsAfterStemming = ConfigurationContext.ProcessingConfigurationContext.RemoveAccentsAfterStemmingEntry.Value;

                // Validate data
                var validationResults = ValidationHelpers.ValidateModel(_dataInstance.IndexProcessingConfiguration);

                // If any errors...
                if (validationResults.Count > 0)
                {
                    _uow.UndoEntityChanges(_dataInstance.IndexProcessingConfiguration);
                    ConfigurationContext.FormErrorString = validationResults.AggregateErrors();
                }
                // Otherwise valid results...
                else
                {
                    var fileDeletionOk = true;
                    // Find and delete all the index files...
                    try
                    {
                        var filePaths = _indexStorage.GetIndexFiles(_dataInstance.Id.ToString());
                        for (int i = 0; i < filePaths.Length; ++i)
                            File.Delete(filePaths[i]);
                    }
                    catch
                    {
                        fileDeletionOk = false;
                    }

                    // If the file deletion has no errors...
                    if (fileDeletionOk)
                    {
                        // Update
                        _uow.IndexProcessingConfigurations.Update(_dataInstance.IndexProcessingConfiguration);
                        _uow.SaveChanges();

                        // Reaload data
                        await LoadAsync(_dataInstance.Id);

                        // Log it
                        _logger.LogInformationSource($"Index processing configuration of '{_dataInstance.Name}' ({_dataInstance.Id}) successfully updated!");
                    }
                    // Otherwise, something went wrong during the file deletion...
                    else
                    {
                        _uow.UndoEntityChanges(_dataInstance.CrawlerConfiguration);
                        ConfigurationContext.FormErrorString = "Something went wrong during clearing old index data.";
                    }
                }
            });
        }
        /// <summary>
        /// Update data instance name
        /// </summary>
        private async Task DataInstanceNameUpdateCommandRoutineAsync()
        {
            await RunCommandAsync(() => ConfigurationContext.DataInstanceNameUpdateCommandFlag, async () =>
            {
                // Re-initialize state values
                ConfigurationContext.FormErrorString = null;

                if (CrawlerInWork || IndexProcessingInWorkFlag || QueryInWorkFlag)
                {
                    ConfigurationContext.FormErrorString = "Cannot update configuration during processing!";
                    return;
                }

                // Assign updated data
                _dataInstance.Name = ConfigurationContext.DataInstanceNameEntry.Value;

                // Validate data
                var validationResults = ValidationHelpers.ValidateModel(_dataInstance);

                // Additional validation steps
                // Data instance name must be unique...
                if (_uow.DataInstances.Get(o => o.Name.Equals(_dataInstance.Name)).Any())
                {
                    validationResults.Add(new DataValidationError
                    {
                        Code = nameof(_dataInstance.Name),
                        Description = "Data Instance Name already exists."
                    }); // TODO localization
                }

                // If any errors...
                if (validationResults.Count > 0)
                {
                    _uow.UndoEntityChanges(_dataInstance);
                    ConfigurationContext.FormErrorString = validationResults.AggregateErrors();
                }
                // Otherwise valid results...
                else
                {
                    // Update
                    _uow.DataInstances.Update(_dataInstance);
                    _uow.SaveChanges();

                    // Reaload data
                    await LoadAsync(_dataInstance.Id);

                    // Log it
                    _logger.LogInformationSource($"Data instance name of '{_dataInstance.Name}' ({_dataInstance.Id}) successfully updated!");
                }
            });
        }
        /// <summary>
        /// Delete the data instance
        /// </summary>
        private async Task DataInstanceDeleteCommandRoutineAsync()
        {
            await RunCommandAsync(() => ConfigurationContext.DataInstanceDeleteCommandFlag, async () =>
            {
                // Re-initialize state values
                ConfigurationContext.FormErrorString = null;

                if (CrawlerInWork || IndexProcessingInWorkFlag || QueryInWorkFlag)
                {
                    ConfigurationContext.FormErrorString = "Cannot update configuration during processing!";
                    return;
                }

                var fileDeletionOk = true;
                // Find and delete all the crawled data files...
                try
                {
                    var crawlerFilePaths = _crawlerStorage.GetAllDataFiles(_dataInstance.Id.ToString());
                    for (int i = 0; i < crawlerFilePaths.Length; ++i)
                        File.Delete(crawlerFilePaths[i]);
                    var indexFilePaths = _indexStorage.GetIndexFiles(_dataInstance.Id.ToString());
                    for (int i = 0; i < indexFilePaths.Length; ++i)
                        File.Delete(indexFilePaths[i]);
                }
                catch
                {
                    fileDeletionOk = false;
                }

                // If the file deletion has no errors...
                if (fileDeletionOk)
                {
                    long id = _dataInstance.Id;
                    string name = _dataInstance.Name;

                    // Delete
                    _uow.DataInstances.Delete(_dataInstance.Id);
                    _uow.SaveChanges();

                    // Go to Home page
                    GoToHomePageCommandRoutine();

                    // Log it
                    _logger.LogInformationSource($"Data instance '{name}' ({id}) successfully deleted!");
                }
                // Otherwise, something went wrong during the file deletion...
                else
                {
                    ConfigurationContext.FormErrorString = "Something went wrong during clearing data files.";
                }

                await Task.Delay(1);
            });
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads data files
        /// </summary>
        /// <param name="resetToDefaultSelection">Indication to reset the selection entry to default value.</param>
        public void LoadDataFiles(bool resetToDefaultSelection = false)
        {
            ushort fileLimit = 50;
            string startsWith = "data_";
            string endsWith = ".json";

            var filePaths = _crawlerStorage.GetAllDataFiles(_dataInstance.Id.ToString());
            var dataFilePaths = filePaths.Where(o => o.EndsWith(endsWith)).ToArray();
            if (dataFilePaths != null)
            {
                var data = new List<DataFileInfo>();
                for (int i = 0; i < dataFilePaths.Length; ++i)
                {
                    string filename = Path.GetFileName(dataFilePaths[i]);
                    try
                    {
                        string dateStr = filename.Substring(startsWith.Length, filename.Length - startsWith.Length - endsWith.Length);

                        var datetime = DateTime.ParseExact(dateStr, "yyyy_M_d_H_m_s", CultureInfo.InvariantCulture);
                        data.Add(new DataFileInfo(datetime.ToString("yyyy-MM-dd (HH:mm:ss)"), dataFilePaths[i], datetime));
                    }
                    catch
                    {
                        // Corrupted filename
                        // skip
                    }
                }

                data.Sort((x, y) => DateTime.Compare(y.CreatedAt, x.CreatedAt));
                if (data.Count > fileLimit)
                    data = data.Take(fileLimit).ToList();
                UpdateDataFileSelection(data, resetToDefaultSelection);
            }
        }

        /// <summary>
        /// Loads index files
        /// </summary>
        /// <param name="resetToDefaultSelection">Indication to reset the selection entry to default value.</param>
        public void LoadIndexFiles(bool resetToDefaultSelection = false)
        {
            ushort fileLimit = 50;
            string startsWith = $"{_dataInstance.Id}_";
            string endsWith = ".idx";

            var filePaths = _indexStorage.GetIndexFiles(_dataInstance.Id.ToString());
            if (filePaths != null)
            {
                var data = new List<DataFileInfo>();
                for (int i = 0; i < filePaths.Length; ++i)
                {
                    string filename = Path.GetFileName(filePaths[i]);
                    try
                    {
                        string dateStr = filename.Substring(startsWith.Length, filename.Length - startsWith.Length - endsWith.Length);

                        var datetime = DateTime.ParseExact(dateStr, "yyyy_M_d_H_m_s", CultureInfo.InvariantCulture);
                        data.Add(new DataFileInfo(datetime.ToString("yyyy-MM-dd (HH:mm:ss)"), filePaths[i], datetime));
                    }
                    catch
                    {
                        // Corrupted filename
                        // skip
                    }
                }

                data.Sort((x, y) => DateTime.Compare(y.CreatedAt, x.CreatedAt));
                if (data.Count > fileLimit)
                    data = data.Take(fileLimit).ToList();
                UpdateIndexFileSelection(data, resetToDefaultSelection);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads necessary data structures according to data instance <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The ID</param>
        /// <param name="loadToMainView">Indicates if the method should load main view.</param>
        private async Task LoadAsync(long id, bool loadToMainView = false)
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

            // Load data files
            LoadDataFiles(true);
            // Load index files
            LoadIndexFiles(true);
            // Load data/values into the configuration context
            ConfigurationContext.Set(_dataInstance.CrawlerConfiguration, _dataInstance.IndexProcessingConfiguration, _dataInstance.Name);

            // Flag up data load is done
            if (loadToMainView) CurrentView = View.Main;
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

                    _ = _crawlerManager.RemoveCrawlerAsync(_crawlerEngine.NameIdentifier);
                    _crawlerEngine = null;

                    OnPropertyChanged(nameof(CrawlerInWork));

                    for (int i = 0; i < CrawlerProgressMsgs.Length; ++i)
                        CrawlerProgressMsgs[i] = CrawlerProgressUrls[i] = string.Empty;

                    // Load data files
                    LoadDataFiles(true);
                });
            };
        }

        /// <summary>
        /// Update data file selection and its entry.
        /// </summary>
        /// <param name="data">New data selection array (<see langword="null"/> just clears the list).</param>
        /// <param name="resetToDefaultSelection">Indication to reset the selection entry to default value.</param>
        /// <remarks>
        ///     Method expects to already have 1 item (first) that represents default selected item in <see cref="_dataFileSelection"/>.
        /// </remarks>
        private void UpdateDataFileSelection(List<DataFileInfo> data, bool resetToDefaultSelection = false)
        {
            var newData = new List<DataFileInfo>();
            newData.Add(_dataFileSelection[0]);

            // Clear previous data selection
            _dataFileSelection.Clear();

            if (data != null)
                newData.AddRange(data);

            // Update the file selection
            _dataFileSelection = newData;

            // Update the file selection entry
            DataFileEntry.ValueList = _dataFileSelection;
            if (resetToDefaultSelection)
                DataFileEntry.Value = _dataFileSelection[0]; // Default selected value
        }

        /// <summary>
        /// Update index file selection and its entry.
        /// </summary>
        /// <param name="data">New index selection array (<see langword="null"/> just clears the list).</param>
        /// <param name="resetToDefaultSelection">Indication to reset the selection entry to default value.</param>
        /// <remarks>
        ///     Method expects to already have 1 item (first) that represents default selected item in <see cref="_indexFileSelection"/>.
        /// </remarks>
        private void UpdateIndexFileSelection(List<DataFileInfo> data, bool resetToDefaultSelection = false)
        {
            var newData = new List<DataFileInfo>();
            newData.Add(_indexFileSelection[0]);

            // Clear previous data selection
            _indexFileSelection.Clear();

            if (data != null)
                newData.AddRange(data);

            // Update the file selection
            _indexFileSelection = newData;

            // Update the file selection entry
            IndexFileEntry.ValueList = _indexFileSelection;
            if (resetToDefaultSelection)
                IndexFileEntry.Value = _indexFileSelection[0]; // Default selected value
        }

        #endregion

        #region Helper View Enum

        /// <summary>
        /// Defines views associated to this view model and GUI.
        /// </summary>
        public enum View
        {
            Main = 0,
            Results = 1,
            Configuration = 2
        }

        #endregion
    }
}
