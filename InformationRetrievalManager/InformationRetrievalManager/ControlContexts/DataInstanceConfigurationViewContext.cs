using InformationRetrievalManager.Core;
using InformationRetrievalManager.Relational;

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

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DataInstanceConfigurationViewContext()
        {
            DataInstanceNameEntry = new TextEntryViewModel
            {
                IsReadOnly = true,
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
        public void Set(CrawlerConfigurationDataModel crawlerConfiguration, IndexProcessingConfigurationDataModel processingConfiguration)
        {
            CrawlerConfigurationContext.ReadOnly(true).Set(
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
            ProcessingConfigurationContext.ReadOnly(true).Set(
                processingConfiguration.Language,
                processingConfiguration.CustomRegex,
                string.Join(IndexProcessingConfiguration.CustomStopWords_Separator.ToString(), processingConfiguration.CustomStopWords),
                processingConfiguration.ToLowerCase,
                processingConfiguration.RemoveAccentsBeforeStemming,
                processingConfiguration.RemoveAccentsAfterStemming
                );
        }

        #endregion
    }
}
