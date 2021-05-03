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
                Label = null,
                Description = null,
                Validation = ValidationHelpers.GetPropertyValidateAttribute<DataInstanceDataModel, string, ValidateStringAttribute>(o => o.Name),
                Value = DataInstanceDataModel.Name_DefaultValue,
                Placeholder = "Data Instance Name",
                MaxLength = DataInstanceDataModel.Name_MaxLength
            };
        }

        #endregion
    }
}
