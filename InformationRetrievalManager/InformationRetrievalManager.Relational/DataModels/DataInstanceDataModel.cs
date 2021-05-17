using InformationRetrievalManager.Core;
using System.Collections.Generic;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// The data model representing created instances for data processing by user.
    /// </summary>
    [ValidableModel(typeof(DataInstanceDataModel))]
    public class DataInstanceDataModel
    {
        #region Limit Constants

        public static readonly bool Name_IsRequired = true;
        public static readonly ushort Name_MaxLength = 15;
        public static readonly string Name_CanContainRegex = @"^([a-zA-Z0-9])+$";
        public static readonly string Name_DefaultValue = "";

        #endregion

        #region Properties (Keys / Relations)

        /// <summary>
        /// Primary Key
        /// </summary>
        [ValidateIgnore]
        public long Id { get; set; }

        /// <summary>
        /// Reference to crawler configuration
        /// </summary>
        /// <remarks>
        ///     Keep track of all connectors in data models,
        ///     to keep database design in proper functinality of cascade deletion.
        /// </remarks>
        [ValidateIgnore]
        public CrawlerConfigurationDataModel CrawlerConfiguration { get; set; }

        /// <summary>
        /// Reference to index processing configuration
        /// </summary>
        /// <remarks>
        ///     Keep track of all connectors in data models,
        ///     to keep database design in proper functinality of cascade deletion.
        /// </remarks>
        [ValidateIgnore]
        public IndexProcessingConfigurationDataModel IndexProcessingConfiguration { get; set; }

        /// <summary>
        /// Reference to MANY indexed file references // Fluent API
        /// ---
        /// E.g. You can use this list to put as many indexed file references into this list 
        /// while creation a new data instance to create new indexed file references associated to this data instance at the same time during commit
        /// </summary>
        [ValidateIgnore]
        public ICollection<IndexedFileReferenceDataModel> IndexedFileReferences { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Data instance name
        /// </summary>
        [ValidateString(nameof(Name), typeof(DataInstanceDataModel),
            pIsRequired: nameof(Name_IsRequired),
            pMaxLength: nameof(Name_MaxLength),
            pCanContainRegex: nameof(Name_CanContainRegex))]
        public string Name { get; set; }

        #endregion
    }
}
