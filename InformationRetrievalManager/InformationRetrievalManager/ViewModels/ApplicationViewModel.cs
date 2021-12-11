using InformationRetrievalManager.Core;
using InformationRetrievalManager.Relational;
using Ixs.DNA;
using PropertyChanged;
using System.Windows;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The application state as a view model.
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// <see cref="ApplicationPage"/> index limit definition for form pages
        /// </summary>
        private readonly int _formPagesStartIndex = 1000;

        #endregion

        #region Public Properties

        /// <summary>
        /// Application publisher name.
        /// Represents name of directory in Start Menu, where the application deploys its shortcuts.
        /// ---
        /// This const is duplicate of the value from ClickOnce installation settings (Publish->Options)!!!
        /// Do not forget to change the value on both places!!!
        /// </summary>
        public string PublisherName { get; } = "Tomas Frixs";

        /// <summary>
        /// Application Title/Name.
        /// What user can see in the app UI, e.g.
        /// </summary>
        public string ApplicationName { get; private set; } = "Crawly";

        /// <summary>
        /// Application version. 
        /// What user can see in the app UI, e.g.
        /// </summary>
        public string ApplicationVersion { get; private set; } = "v 2 . 0";

        /// <summary>
        /// Window default title.
        /// Default name which is set at application start.
        /// </summary>
        public string WindowTitleDefault { get; private set; } //; ctor

        /// <summary>
        /// Window title - dynamic name - postfix is changing based on opened page.
        /// ---
        /// Set in <see cref="SetWindowTitlePostfixOnly"/>.
        /// </summary>
        public string WindowTitle { get; private set; } //;

        /// <summary>
        /// Window title change only postfix.
        /// </summary>
        [DoNotNotify]
        public string SetWindowTitlePostfixOnly
        {
            set => WindowTitle = value.Length > 0 ? WindowTitleDefault + " : " + value : WindowTitleDefault;
        }

        /// <summary>
        /// The current page of the application.
        /// </summary>
        public ApplicationPage CurrentPage { get; private set; } //;

        /// <summary>
        /// The view model to use for the current page when the <see cref="CurrentPage"/> changes.
        /// NOTE: This is not a love up-to-date view model of the current page.
        ///       It is simply used to set the view model of the current page at the time it chages.
        ///       In other words, we can pass data to the new page.
        /// </summary>
        public BaseViewModel CurrentPageViewModel { get; set; } //;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor / Deisgn mode
        /// </summary>
        public ApplicationViewModel()
        {
        }

        /// <summary>
        /// Constructor to initialize app properties.
        /// Very important to call constructor in this way to properly initialize your app
        /// <para>
        ///     It is used as alternative in .NET Standard for DesignerProperties.GetIsInDesignMode 
        ///     To leave default constructor for designer and tthis call for our initialization
        /// </para>
        /// </summary>
        /// <param name="ensureInitialization">Fake out constructor to have choice of constructor call</param>
        public ApplicationViewModel(bool ensureInitialization)
        {
            // Set default window title by application name.
            WindowTitleDefault = ApplicationName;

            // Set Window page postfix.
            SetWindowTitlePostfixOnly = CurrentPage > 0 ? CurrentPage.GetDescription() : "";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Navigates to the specified page.
        /// </summary>
        /// <param name="page">Desired page to go to.</param>
        /// <param name="viewModel">The view model, if any, to set explicitly to the new page.</param>
        public void GoToPage(ApplicationPage page, BaseViewModel viewModel = null)
        {
            // Defaule page to switch to
            var switchPage = page;
            // Default VM to switch
            var switchViewModel = viewModel;
            
            // Get numerical value of the page
            int pageVal = (int)page;

            // If pae differs from the current page...
            if (CurrentPage != switchPage)
            {
                // Switch page
                SwitchPage(switchPage, switchViewModel);

                // Set window title page name
                if (pageVal < _formPagesStartIndex)
                    SetWindowTitlePostfixOnly = pageVal > 0 ? switchPage.GetDescription() : "";
            }
        }

        /// <summary>
        /// Exit application command to close the application.
        /// E.g. Use <see cref="App.Application_Exit(object, ExitEventArgs)"/> (Windows WPF) to process routines on application exit in.
        /// </summary>
        public void Exit()
        {
            // Save application state before closure
            SaveStateData();

            // Shutdown.
            Application.Current.Shutdown();

            // Close all windows - Shutdown() already do that.
            //for (int intCounter = Application.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
            //    Application.Current.Windows[intCounter].Close();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Switch page
        /// </summary>
        /// <param name="page">The page to go to.</param>
        /// <param name="viewModel">The view model, if any, to set explicitly to the new page.</param>
        private void SwitchPage(ApplicationPage page, BaseViewModel viewModel = null)
        {
            // Set the view model.
            CurrentPageViewModel = viewModel;

            // Set the current page.
            CurrentPage = page;

            // Fire off a CurrentPage chaned event.
            OnPropertyChanged(nameof(CurrentPage));
        }

        /// <summary>
        /// Saves the state data
        /// </summary>
        private void SaveStateData()
        {
            var uof = Framework.Service<IUnitOfWork>();

            // Get main window size
            var mainWindowSize = Framework.Service<IUIManager>().GetMainWindowSize();

            // Save
            uof.ApplicationState.Insert(new ApplicationStateDataModel
            {
                MainWindowSizeX = mainWindowSize.X,
                MainWindowSizeY = mainWindowSize.Y
            });
            uof.SaveChanges();
        }

        #endregion
    }
}
