using System;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// The data model for already indexed documents to leave a reference for querying.
    /// </summary>
    public class IndexedDocumentDataModel
    {
        #region Properties (Keys / Relations)

        /// <summary>
        /// Primary Key
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Foreign Key for <see cref="DataInstance"/>
        /// </summary>
        public long? DataInstanceId { get; set; }

        /// <summary>
        /// Reference to ONE data instance // Fluent API
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public DataInstanceDataModel DataInstance { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// Document title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Document category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Document timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Document's source URL
        /// </summary>
        public string SourceUrl { get; set; }

        #endregion
    }
}
