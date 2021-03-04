using InformationRetrievalManager.Crawler;
using Ixs.DNA;
using System;
using System.Threading.Tasks;
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
        public HomePageViewModel(ICrawlerManager crawlerManager) : this()
        {
            _crawlerManager = crawlerManager ?? throw new ArgumentNullException(nameof(crawlerManager));
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
            //await RunCommandAsync(() => StartStopAllFlag, async () => await StartStopAll(true));
            // HACK: crawler starter
            var crawler = await _crawlerManager.GetCrawlerAsync("bdo-sea");
            crawler.Start();

            await Task.Delay(1);
        }

        #endregion
    }
}
