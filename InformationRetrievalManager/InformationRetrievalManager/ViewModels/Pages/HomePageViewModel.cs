using InformationRetrievalManager.Core;
using InformationRetrievalManager.Relational;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for <see cref="HomePage"/>
    /// </summary>
    public class HomePageViewModel : BaseViewModel
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;
        private readonly IUnitOfWork _uow;
        private readonly ITaskManager _taskManager;

        #endregion

        #region Private Members

        //private ICrawlerEngine _crawler;
        //private IndexProcessingConfiguration _processingConfiguration = new IndexProcessingConfiguration
        //{
        //    Language = ProcessingLanguage.EN,
        //    ToLowerCase = true,
        //    RemoveAccentsBeforeStemming = true,
        //};

        #endregion

        #region Public Properties

        /// <summary>
        /// Indicates if the data are already loaded into the VM (once the values changes to <see langword="true"/>).
        /// </summary>
        public bool DataLoaded { get; set; }

        /// <summary>
        /// Collection of data instances created in this app by the user
        /// </summary>
        public ObservableCollection<DataInstanceDataModel> DataInstances { get; set; } = new ObservableCollection<DataInstanceDataModel>();

        //public bool IsCurrentlyCrawlingFlag { get; set; }

        //public string CrawlerProcessProgress { get; set; }

        //public string DataProcessingStatus { get; set; }

        //public string Query { get; set; }

        //public string QueryStatus { get; set; }

        #endregion

        #region Command Flags

        //public bool ProcessingCommandFlag { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to go to the another page.
        /// </summary>
        public ICommand GoToHowToPageCommand { get; set; }

        /// <summary>
        /// Command to open page with the specific data instance
        /// </summary>
        public ICommand GoToDataInstanceCommand { get; set; }

        /// <summary>
        /// Command to create a new data instance
        /// </summary>
        public ICommand CreateNewDataInstanceCommand { get; set; }

        //public ICommand StartCrawlerCommand { get; set; }

        //public ICommand CancelCrawlerCommand { get; set; }

        //public ICommand StartProcessingCommand { get; set; }

        //public ICommand RunQueryCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HomePageViewModel()
        {
            // Create commands.
            GoToHowToPageCommand = new RelayCommand(GoToHowToPageCommandRoutine);
            GoToDataInstanceCommand = new RelayParameterizedCommand((parameter) => GoToDataInstancePageCommandRoutine(parameter));
            CreateNewDataInstanceCommand = new RelayCommand(GoToCreateDataInstancePageCommandRoutine);
            //StartCrawlerCommand = new RelayCommand(async () => await StartCrawlerCommandRoutineAsync());
            //CancelCrawlerCommand = new RelayCommand(async () => await CancelCrawlerCommandRoutineAsync());
            //StartProcessingCommand = new RelayCommand(async () => await StartProcessingCommandRoutineAsync());
            //RunQueryCommand = new RelayCommand(async () => await RunQueryCommandRoutineAsync());
        }

        /// <summary>
        /// DI constructor
        /// </summary>
        public HomePageViewModel(ILogger logger, IUnitOfWork uow, ITaskManager taskManager)
            : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _taskManager = taskManager ?? throw new ArgumentNullException(nameof(taskManager));

            // HACK: crawler starter
            _taskManager.RunAndForget(LoadAsync);
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// Command Routine : Go To Page
        /// </summary>
        private void GoToHowToPageCommandRoutine()
        {
            DI.ViewModelApplication.GoToPage(ApplicationPage.HowTo);
        }

        /// <summary>
        /// Command Routine : Go To Page
        /// </summary>
        private void GoToDataInstancePageCommandRoutine(object parameter)
        {
            DI.ViewModelApplication.GoToPage(ApplicationPage.DataInstance,
                Framework.Service<DataInstancePageViewModel>().Init(long.Parse(parameter.ToString()))
                );
        }

        /// <summary>
        /// Command Routine : Go To Page
        /// </summary>
        private void GoToCreateDataInstancePageCommandRoutine()
        {
            DI.ViewModelApplication.GoToPage(ApplicationPage.CreateDataInstance);
        }

        //private async Task StartCrawlerCommandRoutineAsync()
        //{
        //    if (_crawler == null)
        //        return;

        //    //await RunCommandAsync(() => StartStopAllFlag, async () => await StartStopAll(true));
        //    await LoadAsync();
        //    _crawler.Start();
        //    IsCurrentlyCrawlingFlag = true;

        //    await Task.Delay(1);
        //}

        //private async Task CancelCrawlerCommandRoutineAsync()
        //{
        //    if (_crawler == null)
        //        return;

        //    //await RunCommandAsync(() => StartStopAllFlag, async () => await StartStopAll(true));
        //    _crawler.Cancel();
        //    IsCurrentlyCrawlingFlag = false;

        //    await Task.Delay(1);
        //}

        //private async Task StartProcessingCommandRoutineAsync()
        //{
        //    await RunCommandAsync(() => ProcessingCommandFlag, async () =>
        //    {
        //        var filePaths = _crawlerStorage.GetDataFiles(_crawler);
        //        if (filePaths != null)
        //        {
        //            var dataFilePath = filePaths.FirstOrDefault(o => o.Contains(".json"));
        //            if (dataFilePath != null)
        //            {
        //                if (!_crawler.IsCurrentlyCrawling)
        //                {
        //                    // Deserialize JSON directly from a file
        //                    using (StreamReader file = File.OpenText(dataFilePath))
        //                    {
        //                        JsonSerializer serializer = new JsonSerializer();
        //                        CrawlerDataModel[] data = (CrawlerDataModel[])serializer.Deserialize(file, typeof(CrawlerDataModel[]));

        //                        List<IndexDocument> docs = new List<IndexDocument>();
        //                        for (int i = 0; i < data.Length; ++i)
        //                            docs.Add(new IndexDocument(i, data[i].Title, data[i].SourceUrl, data[i].Category, data[i].Timestamp, data[i].Content));

        //                        // HACK - start index processing
        //                        DataProcessingStatus = "Indexing...";
        //                        var processing = new IndexProcessing("my_index", _processingConfiguration, _fileManager, _logger);
        //                        await _taskManager.Run(() =>
        //                        {
        //                            processing.IndexDocuments(docs.ToArray(), true);
        //                            _logger.LogDebugSource("Index processing done!");
        //                            DataProcessingStatus = "Done! Data has been indexed into a binary file.";
        //                        });
        //                    }
        //                }
        //                else
        //                {
        //                    DataProcessingStatus = "Cannot process data during crawling!";
        //                }
        //            }
        //            else
        //            {
        //                DataProcessingStatus = "No data found! Use crawler to get data first.";
        //            }
        //        }

        //        await Task.Delay(1);
        //    });
        //}

        //private async Task RunQueryCommandRoutineAsync()
        //{
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
        //}

        #endregion

        #region Private Methods

        /// <summary>
        /// Load initial configuration/data
        /// </summary>
        private async Task LoadAsync()
        {
            //_crawler = await _crawlerManager.GetCrawlerAsync("bdo-naeu");
            //// Set the events
            ////     - Raise the property changed in the UI thread (crawler is running in a different assembly on a separate thread)
            //_crawler.OnStartProcessEvent += (s, e) =>
            //{
            //    CrawlerProcessProgress = "Starting...";
            //    Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(CrawlerProcessProgress)));
            //};
            //_crawler.OnProcessProgressEvent += (s, e) =>
            //{
            //    CrawlerProcessProgress = $"{e.CrawlingProgressPct}%";
            //    Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(CrawlerProcessProgress)));
            //};
            //_crawler.OnFinishProcessEvent += (s, e) =>
            //{
            //    CrawlerProcessProgress = "Done!";
            //    Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(CrawlerProcessProgress)));
            //};

            // Load data instances
            DataInstances = new ObservableCollection<DataInstanceDataModel>(
                _uow.DataInstances.Get(null, q => q.OrderBy(o => o.Name))
                );

            DataLoaded = true;
            await Task.Delay(1);
        }

        #endregion
    }
}
