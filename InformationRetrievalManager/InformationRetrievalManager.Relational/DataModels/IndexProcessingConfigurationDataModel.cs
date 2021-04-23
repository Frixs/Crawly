using InformationRetrievalManager.Core;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// The data model representing processing configuration of indexation that is set in seach <<see cref="DataInstanceDataModel"/>.
    /// </summary>
    public class IndexProcessingConfigurationDataModel : IndexProcessingConfiguration
    {
        #region Properties (Keys / Relations)

        /// <summary>
        /// Primary Key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Foreign Key for <see cref="DataInstance"/>
        /// </summary>
        public long DataInstanceId { get; set; }

        /// <summary>
        /// Reference to ONE data instance // Fluent API
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public DataInstanceDataModel DataInstance { get; set; }

        #endregion
    }
}
