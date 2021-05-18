using Ixs.DNA;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for <see cref="HowToPage"/>
    /// </summary>
    public class HowToPageViewModel : BaseViewModel
    {
        #region Command Flags

        /// <summary>
        /// Command flag for opening process (e.g. files or for opening web pages)
        /// </summary>
        public bool ProcessFlag { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to go to the another page.
        /// </summary>
        public ICommand GoToHomePageCommand { get; set; }

        /// <summary>
        /// The command to go to donate website.
        /// </summary>
        public ICommand WikiCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HowToPageViewModel()
        {
            // Create commands.
            GoToHomePageCommand = new RelayCommand(GoToHomePageCommandRoutine);
            WikiCommand = new RelayCommand(async () => await WikiCommandRoutineAsync());
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
        /// Command Routine : Go To Donate website
        /// </summary>
        /// <returns></returns>
        private async Task WikiCommandRoutineAsync()
        {
            await RunCommandAsync(() => ProcessFlag, async () =>
            {
                string url = "https://github.com/Frixs/Crawly/wiki";

                if (!string.IsNullOrEmpty(url) && url.IsURL())
                    System.Diagnostics.Process.Start(url);

                await Task.Delay(1);
            });
        }

        #endregion
    }
}
