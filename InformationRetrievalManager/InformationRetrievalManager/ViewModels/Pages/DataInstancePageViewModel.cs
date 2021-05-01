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
        public ComboEntryViewModel<DataFileInfo> DataFileEntry { get; protected set; }

        /// <summary>
        /// Entry selection of available indexed data files.
        /// </summary>
        public ComboEntryViewModel<DataFileInfo> IndexFileEntry { get; protected set; }

        /// <summary>
        /// Entry for query
        /// </summary>
        public TextEntryViewModel QueryEntry { get; protected set; }

        /// <summary>
        /// Crawler processing progress feedback message to user.
        /// </summary>
        public string CrawlerProgress { get; protected set; }

        /// <summary>
        /// Index processing progress feedback message to user.
        /// </summary>
        public string IndexProcessingProgress { get; protected set; }

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
        public DataInstancePageViewModel(ILogger logger, ITaskManager taskManager, IFileManager fileManager, IUnitOfWork uow, ICrawlerManager crawlerManager, ICrawlerStorage crawlerStorage, IIndexStorage indexStorage)
            : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _taskManager = taskManager ?? throw new ArgumentNullException(nameof(taskManager));
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _crawlerManager = crawlerManager ?? throw new ArgumentNullException(nameof(crawlerManager));
            _crawlerStorage = crawlerStorage ?? throw new ArgumentNullException(nameof(crawlerStorage));
            _indexStorage = indexStorage ?? throw new ArgumentNullException(nameof(indexStorage));
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
                                Content = StringHelpers.ShortenWithDots(data[i].Content, 160)
                            };

                            // Validate, if no errors...
                            if (ValidationHelpers.ValidateModel(model).Count == 0)
                            {
                                _uow.IndexedDocuments.Insert(model);
                                _uow.Commit();

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
                            IndexProcessingProgress = "Successfully indexed the documents!";

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
                Console.WriteLine("TODO");

                await Task.Delay(1);
            });

            //    // TODO : we need to have a currently processing index name list to be able to say when we are able to touch the data 
            //    // (as a feature update while multiple processing will run and we want to query only the instances that are not indexing/processing atm and visa/versa).

            //    await RunCommandAsync(() => ProcessingCommandFlag, async () =>
            //    {
            //        var ii = new InvertedIndex("my_index", _fileManager, _logger);

            //        QueryStatus = "Searching...";

            //        int[] results = Array.Empty<int>();

            //        await _taskManager.Run(async () =>
            //        {
            //            ii.Load();
            //            results = await _queryIndexManager.QueryAsync(Query, ii.GetReadOnlyVocabulary(), QueryModelType.Boolean, _processingConfiguration, 10);
            //        });

            //        QueryStatus = "Results: [" + string.Join(",", results) + "]";

            //        await Task.Delay(1);
            //    });
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

            var filePaths = _crawlerStorage.GetDataFiles(_dataInstance.Id.ToString());
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

            // Load data files
            LoadDataFiles(true);
            // Load index files
            LoadIndexFiles(true);

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
    }
}
