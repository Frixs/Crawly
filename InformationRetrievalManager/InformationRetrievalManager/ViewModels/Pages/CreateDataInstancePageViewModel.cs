using Microsoft.Extensions.Logging;
using System;
using System.Windows.Input;

namespace InformationRetrievalManager
{
    /// <summary>
    /// The view model for <see cref="CreateDataInstancePage"/>
    /// </summary>
    public class CreateDataInstancePageViewModel : BaseViewModel
    {
        #region Private Members (Injects)

        private readonly ILogger _logger;

        #endregion

        #region Public Properties

        /// <summary>
        /// Context of the <see cref="CrawlerConfigurationForm"/> control.
        /// </summary>
        public CrawlerConfigurationFormContext CrawlerConfigurationContext { get; set; } = new CrawlerConfigurationFormContext();

        /// <summary>
        /// Context of the <see cref="ProcessingConfigurationForm"/> control.
        /// </summary>
        public ProcessingConfigurationFormContext ProcessingConfigurationContext { get; set; } = new ProcessingConfigurationFormContext();

        #endregion

        #region Command Flags

        #endregion

        #region Commands

        /// <summary>
        /// The command to go to the another page.
        /// </summary>
        public ICommand GoToHomePageCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CreateDataInstancePageViewModel()
        {
            // Create commands.
            GoToHomePageCommand = new RelayCommand(GoToHomePageCommandRoutine);
        }

        /// <summary>
        /// DI constructor
        /// </summary>
        public CreateDataInstancePageViewModel(ILogger logger)
            : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
