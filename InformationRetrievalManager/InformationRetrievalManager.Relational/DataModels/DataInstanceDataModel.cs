using System.Collections.Generic;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// The data model representing created instances for data processing by user.
    /// </summary>
    public class DataInstanceDataModel
    {
        #region Properties (Keys / Relations)

        /// <summary>
        /// Primary Key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Reference to crawler configuration
        /// </summary>
        /// <remarks>
        ///     Keep track of all connectors in data models,
        ///     to keep database design in proper functinality of cascade deletion.
        /// </remarks>
        public CrawlerConfigurationDataModel CrawlerConfiguration { get; set; }

        /// <summary>
        /// Reference to index processing configuration
        /// </summary>
        /// <remarks>
        ///     Keep track of all connectors in data models,
        ///     to keep database design in proper functinality of cascade deletion.
        /// </remarks>
        public IndexProcessingConfigurationDataModel IndexProcessingConfiguration { get; set; }

        /// <summary>
        /// Reference to MANY indexed documents // Fluent API
        /// ---
        /// E.g. You can use this list to put as many indexed documents into this list 
        /// while creation a new data instance to create new indexed documents associated to this data instance at the same time during commit
        /// </summary>
        public ICollection<IndexedDocumentDataModel> IndexedDocuments { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Data instance name
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}
