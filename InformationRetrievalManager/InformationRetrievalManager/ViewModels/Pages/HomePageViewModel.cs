using InformationRetrievalManager.Core;
using InformationRetrievalManager.Crawler;
using System;
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

        #endregion

        #region Private Members

        private ICrawlerEngine _crawler;

        #endregion

        #region Public Properties

        /// <summary>
        /// UNDONE
        /// </summary>
        public string CrawlerProcessProgress { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to go to the another page.
        /// </summary>
        public ICommand GoToHowToPageCommand { get; set; }

        /// <summary>
        /// UNDONE
        /// </summary>
        public ICommand StartCrawlerCommand { get; set; }

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
        }

        /// <summary>
        /// DI constructor
        /// </summary>
        public HomePageViewModel(ICrawlerManager crawlerManager, ITaskManager taskManager) : this()
        {
            _crawlerManager = crawlerManager ?? throw new ArgumentNullException(nameof(crawlerManager));
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
        /// UNDONE
        /// </summary>
        /// <returns></returns>
        private async Task StartCrawlerCommandRoutineAsync()
        {
            if (_crawler == null)
                return;

            //await RunCommandAsync(() => StartStopAllFlag, async () => await StartStopAll(true));
            // HACK: crawler starter
            _crawler.Start();

            await Task.Delay(1);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load data
        /// </summary>
        private async Task LoadAsync()
        {
            // HACK: crawler starter
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
