using InformationRetrievalManager.Core;
using InformationRetrievalManager.Relational;
using Ixs.DNA;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
        private readonly IUnitOfWork _uow;

        #endregion

        #region Public Properties

        /// <summary>
        /// Context of the <see cref="CrawlerConfigurationForm"/> control.
        /// </summary>
        public CrawlerConfigurationFormContext CrawlerConfigurationContext { get; } = new CrawlerConfigurationFormContext();

        /// <summary>
        /// Context of the <see cref="ProcessingConfigurationForm"/> control.
        /// </summary>
        public ProcessingConfigurationFormContext ProcessingConfigurationContext { get; } = new ProcessingConfigurationFormContext();

        /// <summary>
        /// Property for input field to set data instance name.
        /// </summary>
        public TextEntryViewModel DataInstanceNameEntry { get; set; } //; ctor

        /// <summary>
        /// Error string as a feedback to the user.
        /// </summary>
        public string FormErrorString { get; set; }

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
        public CreateDataInstancePageViewModel(ILogger logger, IUnitOfWork uow)
            : this()
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
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
                // Re-initialize state values
                FormErrorString = null;

                // Create data models
                var crawlerConfiguration = new CrawlerConfigurationDataModel
                {
                    SiteAddress = CrawlerConfigurationContext.SiteAddressEntry.Value,
                    SiteSuffix = CrawlerConfigurationContext.SiteSuffixEntry.Value,
                    StartPageNo = CrawlerConfigurationContext.StartPageNoEntry.Value,
                    MaxPageNo = CrawlerConfigurationContext.MaxPageNoEntry.Value,
                    PageNoModifier = CrawlerConfigurationContext.PageNoModifierEntry.Value,
                    SearchInterval = CrawlerConfigurationContext.SearchIntervalEntry.Value,
                    SiteUrlArticlesXPath = CrawlerConfigurationContext.SiteUrlArticlesXPathEntry.Value,
                    SiteArticleContentAreaXPath = CrawlerConfigurationContext.SiteArticleContentAreaXPathEntry.Value,
                    SiteArticleTitleXPath = CrawlerConfigurationContext.SiteArticleTitleXPathEntry.Value,
                    SiteArticleCategoryXPath = CrawlerConfigurationContext.SiteArticleCategoryXPathEntry.Value,
                    SiteArticleDateTimeXPath = CrawlerConfigurationContext.SiteArticleDateTimeXPathEntry.Value,
                    SiteArticleDateTimeFormat = CrawlerConfigurationContext.SiteArticleDateTimeFormatEntry.Value,
                    SiteArticleDateTimeCultureInfo = CrawlerConfigurationContext.SiteArticleDateTimeCultureInfoEntry.Value
                };
                var processingConfiguration = new IndexProcessingConfigurationDataModel
                {
                    Language = ProcessingConfigurationContext.LanguageEntry.Value,
                    CustomRegex = ProcessingConfigurationContext.CustomRegexEntry.Value,
                    CustomStopWords = new HashSet<string>(ProcessingConfigurationContext.CustomStopWordsEntry.Value.Split(IndexProcessingConfiguration.CustomStopWords_Separator)),
                    ToLowerCase = ProcessingConfigurationContext.ToLowerCaseEntry.Value,
                    RemoveAccentsBeforeStemming = ProcessingConfigurationContext.RemoveAccentsBeforeStemmingEntry.Value,
                    RemoveAccentsAfterStemming = ProcessingConfigurationContext.RemoveAccentsAfterStemmingEntry.Value,
                };
                var dataInstance = new DataInstanceDataModel
                {
                    Name = DataInstanceNameEntry.Value,
                    CrawlerConfiguration = crawlerConfiguration,
                    IndexProcessingConfiguration = processingConfiguration
                };

                // Validate data
                var validationResults = ValidationHelpers.ValidateModel(crawlerConfiguration);
                validationResults.Concat(ValidationHelpers.ValidateModel(processingConfiguration));
                validationResults.Concat(ValidationHelpers.ValidateModel(dataInstance));

                // Additional validation steps
                // Culture info must be valid...
                try
                {
                    _ = new CultureInfo(crawlerConfiguration.SiteArticleDateTimeCultureInfo);
                }
                catch
                {
                    validationResults.Add(new DataValidationError
                    {
                        Code = nameof(crawlerConfiguration.SiteArticleDateTimeCultureInfo),
                        Description = "Invalid date-time culture."
                    }); // TODO localization
                }
                // Data instance name must be unique...
                if (_uow.DataInstances.Get(o => o.Name.Equals(dataInstance.Name)).Any())
                {
                    validationResults.Add(new DataValidationError
                    {
                        Code = nameof(dataInstance.Name),
                        Description = "Data Instance Name already exists."
                    }); // TODO localization
                }

                // If any errors...
                if (validationResults.Count > 0)
                {
                    FormErrorString = validationResults.AggregateErrors();
                }
                // Otherwise valid results...
                else
                {
                    // Insert
                    _uow.DataInstances.Insert(dataInstance);
                    _uow.SaveChanges();

                    // Log it
                    _logger.LogInformationSource($"Data Instance '{dataInstance.Name}' successfully created!");

                    // Move to the newly created data instance's page
                    GoToDataInstancePage(dataInstance.Id);
                }

                await Task.Delay(1);
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Go to Data instance page
        /// </summary>
        /// <param name="id">ID of the data instance.</param>
        private void GoToDataInstancePage(long id)
        {
            DI.ViewModelApplication.GoToPage(ApplicationPage.DataInstance,
                Framework.Service<DataInstancePageViewModel>().Init(id)
                );
        }

        #endregion
    }
}
