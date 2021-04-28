using InformationRetrievalManager.Core;
using InformationRetrievalManager.Relational;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
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

        /// <summary>
        /// Property for input field to set data instance name.
        /// </summary>
        public TextEntryViewModel DataInstanceNameEntry { get; set; } //; ctor

        #endregion

        #region Command Flags

        /// <summary>
        /// Flag to set the netire form into disabled mode if <see langword="true"/>.
        /// </summary>
        public bool FormProcessingFlag { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to go to the another page.
        /// </summary>
        public ICommand GoToHomePageCommand { get; set; }

        /// <summary>
        /// The command to create the data instance from form values
        /// </summary>
        public ICommand CreateCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CreateDataInstancePageViewModel()
        {
            // Create commands.
            GoToHomePageCommand = new RelayCommand(GoToHomePageCommandRoutine);
            CreateCommand = new RelayCommand(async () => await CreateCommandRoutineAsync());

            DataInstanceNameEntry = new TextEntryViewModel
            {
                Label = null,
                Description = null,
                Validation = ValidationHelpers.GetPropertyValidateAttribute<DataInstanceDataModel, string, ValidateStringAttribute>(o => o.Name),
                Value = DataInstanceDataModel.Name_DefaultValue,
                Placeholder = "Name Your Data Instance",
                MaxLength = DataInstanceDataModel.Name_MaxLength
            };
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

        /// <summary>
        /// Create data instance process
        /// </summary>
        private async Task CreateCommandRoutineAsync()
        {
            await RunCommandAsync(() => FormProcessingFlag, async () =>
            {
                await Task.Delay(1000);
            });
        }

        #endregion
    }
}
