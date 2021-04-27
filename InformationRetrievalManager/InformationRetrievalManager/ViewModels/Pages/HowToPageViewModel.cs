using System.Windows.Input;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for <see cref="HowToPage"/>
    /// </summary>
    public class HowToPageViewModel : BaseViewModel
    {
        #region Commands

        /// <summary>
        /// The command to go to the another page.
        /// </summary>
        public ICommand GoToHomePageCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HowToPageViewModel()
        {
            // Create commands.
            GoToHomePageCommand = new RelayCommand(GoToHomePageCommandRoutine);
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
    }
}
