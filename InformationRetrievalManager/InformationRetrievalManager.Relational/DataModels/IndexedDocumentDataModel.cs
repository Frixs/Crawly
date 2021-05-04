using InformationRetrievalManager.Core;
using System;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// The data model for already indexed documents to leave a reference for querying.
    /// </summary>
    [ValidableModel(typeof(IndexedDocumentDataModel))]
    public class IndexedDocumentDataModel
    {
        #region Limit Constants

        public static readonly bool Title_IsRequired = true;
        public static readonly short Title_MaxLength = 350;
        public static readonly string Title_DefaultValue = "";

        public static readonly bool Category_AllowNull = true;
        public static readonly bool Category_IsRequired = false;
        public static readonly short Category_MaxLength = 50;
        public static readonly string Category_DefaultValue = "";

        public static readonly bool SourceUrl_IsRequired = true;
        public static readonly short SourceUrl_MaxLength = 999;
        public static readonly string SourceUrl_CanContainRegex = CrawlerConfigurationDataModel.SiteAddress_CanContainRegex;
        public static readonly string SourceUrl_DefaultValue = "";

        public static readonly bool Content_IsRequired = true;
        public static readonly short Content_MaxLength = 255;
        public static readonly string Content_DefaultValue = "";

        #endregion

        #region Properties (Keys / Relations)

        /// <summary>
        /// Primary Key
        /// </summary>
        [ValidateIgnore]
        public long Id { get; set; }

        /// <summary>
        /// Foreign Key for <see cref="DataInstance"/>
        /// </summary>
        [ValidateIgnore]
        public long? DataInstanceId { get; set; }

        /// <summary>
        /// Reference to ONE data instance // Fluent API
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        [ValidateIgnore]
        public DataInstanceDataModel DataInstance { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Document title
        /// </summary>
        [ValidateString(nameof(Title), typeof(IndexedDocumentDataModel),
            pIsRequired: nameof(Title_IsRequired),
            pMaxLength: nameof(Title_MaxLength))]
        public string Title { get; set; }

        /// <summary>
        /// Document category
        /// </summary>
        /// <remarks>
        ///     Value can be <see langword="null"/>.
        /// </remarks>
        [ValidateString(nameof(Category), typeof(IndexedDocumentDataModel),
            pAllowNull: nameof(Category_AllowNull),
            pIsRequired: nameof(Category_IsRequired),
            pMaxLength: nameof(Category_MaxLength))]
        public string Category { get; set; }

        /// <summary>
        /// Document timestamp
        /// </summary>
        /// <remarks>
        ///     Value <see cref="DateTime.MinValue"/> = not set.
        /// </remarks>
        [ValidateIgnore]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Document's source URL
        /// </summary>
        [ValidateString(nameof(SourceUrl), typeof(CrawlerConfigurationDataModel),
            pIsRequired: nameof(SourceUrl_IsRequired),
            pMaxLength: nameof(SourceUrl_MaxLength),
            pCanContainRegex: nameof(SourceUrl_CanContainRegex))]
        public string SourceUrl { get; set; }

        /// <summary>
        /// Document's content
        /// </summary>
        /// <remarks>
        ///     The max length is made just for content preview, not the entire content.
        /// </remarks>
        [ValidateString(nameof(Content), typeof(IndexedDocumentDataModel),
            pIsRequired: nameof(Content_IsRequired),
            pMaxLength: nameof(Content_MaxLength))]
        public string Content { get; set; }

        #endregion
    }
}
