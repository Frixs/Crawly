using InformationRetrievalManager.Core;
using InformationRetrievalManager.Relational;
using System.Windows.Input;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Context for control <see cref="DataInstanceConfigurationView"/>.
    /// </summary>
    public class DataInstanceConfigurationViewContext : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// Ccontext of the <see cref="CrawlerConfigurationForm"/> control.
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

        #region Flags

        public bool CrawlerConfigurationReadOnlyFlag { get; set; }
        public bool ProcessingConfigurationReadOnlyFlag { get; set; }
        public bool DataInstanceNameReadOnlyFlag { get; set; }

        public bool CrawlerConfigurationUpdateCommandFlag { get; set; }
        public bool ProcessingConfigurationUpdateCommandFlag { get; set; }
        public bool DataInstanceNameUpdateCommandFlag { get; set; }
        public bool DataInstanceDeleteCommandFlag { get; set; }

        #endregion

        #region Commands

        public ICommand ToggleEditCrawlerConfigurationReadOnlyCommand { get; set; }
        public ICommand ToggleEditProcessingConfigurationReadOnlyCommand { get; set; }
        public ICommand ToggleEditDataInstanceNameReadOnlyCommand { get; set; }

        public ICommand CrawlerConfigurationUpdateCommand { get; set; }
        public ICommand ProcessingConfigurationUpdateCommand { get; set; }
        public ICommand DataInstanceNameUpdateCommand { get; set; }
        public ICommand DataInstanceDeleteCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataInstanceConfigurationViewContext()
        {
            DataInstanceNameReadOnlyFlag = true;
            DataInstanceNameEntry = new TextEntryViewModel
            {
                IsReadOnly = DataInstanceNameReadOnlyFlag,
                Label = null,
                Description = null,
                Validation = ValidationHelpers.GetPropertyValidateAttribute<DataInstanceDataModel, string, ValidateStringAttribute>(o => o.Name),
                Value = DataInstanceDataModel.Name_DefaultValue,
                Placeholder = "Data Instance Name",
                MaxLength = DataInstanceDataModel.Name_MaxLength
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set values into the context forms.
        /// </summary>
        /// <param name="crawlerConfiguration">Crawler configuration form values.</param>
        /// <param name="processingConfiguration">Processing configuration form values.</param>
        public void Set(CrawlerConfigurationDataModel crawlerConfiguration, IndexProcessingConfigurationDataModel processingConfiguration, string dataInstanceName)
        {
            CrawlerConfigurationReadOnlyFlag = ProcessingConfigurationReadOnlyFlag = DataInstanceNameReadOnlyFlag = true;
            
            CrawlerConfigurationContext.ReadOnly(CrawlerConfigurationReadOnlyFlag).Set(
                crawlerConfiguration.SiteAddress,
                crawlerConfiguration.SiteSuffix,
                crawlerConfiguration.StartPageNo,
                crawlerConfiguration.MaxPageNo,
                crawlerConfiguration.PageNoModifier,
                crawlerConfiguration.SearchInterval,
                crawlerConfiguration.SiteUrlArticlesXPath,
                crawlerConfiguration.SiteArticleContentAreaXPath,
                crawlerConfiguration.SiteArticleTitleXPath,
                crawlerConfiguration.SiteArticleCategoryXPath,
                crawlerConfiguration.SiteArticleDateTimeXPath,
                crawlerConfiguration.SiteArticleDateTimeFormat,
                crawlerConfiguration.SiteArticleDateTimeCultureInfo
                );
            ProcessingConfigurationContext.ReadOnly(ProcessingConfigurationReadOnlyFlag).Set(
                processingConfiguration.Language,
                processingConfiguration.CustomRegex,
                string.Join(IndexProcessingConfiguration.CustomStopWords_Separator.ToString(), processingConfiguration.CustomStopWords),
                processingConfiguration.ToLowerCase,
                processingConfiguration.RemoveAccentsBeforeStemming,
                processingConfiguration.RemoveAccentsAfterStemming
                );
            DataInstanceNameEntry.IsReadOnly = DataInstanceNameReadOnlyFlag;
            DataInstanceNameEntry.Value = dataInstanceName;
        }

        #endregion
    }
}
