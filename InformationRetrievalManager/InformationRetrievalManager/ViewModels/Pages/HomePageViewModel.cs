using InformationRetrievalManager.Core;
using InformationRetrievalManager.Crawler;
using InformationRetrievalManager.NLP;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for Home page
    /// </summary>
    public class HomePageViewModel : BaseViewModel
    {
        #region Private Members (Injects)

        private readonly ICrawlerManager _crawlerManager;
        private readonly ITaskManager _taskManager;
        private readonly ICrawlerStorage _crawlerStorage;

        #endregion

        #region Private Members

        public ICrawlerEngine _crawler;

        #endregion

        #region Public Properties

        public bool IsCurrentlyCrawlingFlag { get; set; }

        public string CrawlerProcessProgress { get; set; }

        #endregion

        #region Command Flags

        public bool ProcessingCommandFlag { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to go to the another page.
        /// </summary>
        public ICommand GoToHowToPageCommand { get; set; }

        public ICommand StartCrawlerCommand { get; set; }

        public ICommand CancelCrawlerCommand { get; set; }

        public ICommand StartProcessingCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HomePageViewModel()
        {
            // Create commands.
            GoToHowToPageCommand = new RelayCommand(GoToHowToPageCommandRoutine);
            StartCrawlerCommand = new RelayCommand(async () => await StartCrawlerCommandRoutineAsync());
            CancelCrawlerCommand = new RelayCommand(async () => await CancelCrawlerCommandRoutineAsync());
            StartProcessingCommand = new RelayCommand(async () => await StartProcessingCommandRoutineAsync());
        }

        /// <summary>
        /// DI constructor
        /// </summary>
        public HomePageViewModel(ICrawlerManager crawlerManager, ITaskManager taskManager, ICrawlerStorage crawlerStorage) : this()
        {
            _crawlerManager = crawlerManager ?? throw new ArgumentNullException(nameof(crawlerManager));
            _taskManager = taskManager ?? throw new ArgumentNullException(nameof(taskManager));
            _crawlerStorage = crawlerStorage ?? throw new ArgumentNullException(nameof(crawlerStorage));

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

        private async Task StartCrawlerCommandRoutineAsync()
        {
            if (_crawler == null)
                return;

            //await RunCommandAsync(() => StartStopAllFlag, async () => await StartStopAll(true));
            await LoadAsync();
            _crawler.Start();
            IsCurrentlyCrawlingFlag = true;

            await Task.Delay(1);
        }

        private async Task CancelCrawlerCommandRoutineAsync()
        {
            if (_crawler == null)
                return;

            //await RunCommandAsync(() => StartStopAllFlag, async () => await StartStopAll(true));
            _crawler.Cancel();
            IsCurrentlyCrawlingFlag = false;

            await Task.Delay(1);
        }

        private async Task StartProcessingCommandRoutineAsync()
        {
            await RunCommandAsync(() => ProcessingCommandFlag, async () =>
            {
                var filePaths = _crawlerStorage.GetDataFiles(_crawler);
                if (filePaths != null)
                {
                    var dataFilePath = filePaths.FirstOrDefault(o => o.Contains(".json"));
                    if (dataFilePath != null)
                    {
                        // Deserialize JSON directly from a file
                        using (StreamReader file = File.OpenText(dataFilePath))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            CrawlerDataModel[] data = (CrawlerDataModel[])serializer.Deserialize(file, typeof(CrawlerDataModel[]));
                            if (data.Length > 0)
                            {
                                // HACK - start processing
                                var processing = new BasicProcessing(new Tokenizer(), new Stemmer(), new StopWordRemover());
                                processing.Index(data[0].Content);
                            }
                        }
                    }
                }
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load data
        /// </summary>
        private async Task LoadAsync()
        {
            _crawler = await _crawlerManager.GetCrawlerAsync("bdo-sea");
            // Set the events
            //     - Raise the property changed in the UI thread (crawler is running in a different assembly on a separate thread)
            _crawler.OnStartProcessEvent += (s, e) =>
            {
                CrawlerProcessProgress = "Starting...";
                Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(CrawlerProcessProgress)));
            };
            _crawler.OnProcessProgressEvent += (s, e) =>
            {
                CrawlerProcessProgress = $"{e.CrawlingProgressPct}%";
                Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(CrawlerProcessProgress)));
            };
            _crawler.OnFinishProcessEvent += (s, e) =>
            {
                CrawlerProcessProgress = "Done!";
                Application.Current.Dispatcher.Invoke(() => OnPropertyChanged(nameof(CrawlerProcessProgress)));
            };
        }

        #endregion
    }
}
