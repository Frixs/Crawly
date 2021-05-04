using InformationRetrievalManager.Core;
using System;
using System.Collections.Generic;

namespace InformationRetrievalManager.Relational
{
    /// <summary>
    /// The data model for tracking index file (reference that should exist as a file).
    /// </summary>
    [ValidableModel(typeof(IndexedFileReferenceDataModel))]
    public class IndexedFileReferenceDataModel
    {
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

        /// <summary>
        /// Reference to MANY indexed documents // Fluent API
        /// ---
        /// E.g. You can use this list to put as many indexed documents into this list 
        /// while creation a new indexed file reference to create new indexed documents associated to this indexed file reference at the same time during commit
        /// </summary>
        [ValidateIgnore]
        public ICollection<IndexedDocumentDataModel> IndexedDocuments { get; set; }

        #endregion

        #region Properties

        /// <summary>
        /// File index timestamp
        /// </summary>
        [ValidateIgnore]
        public DateTime Timestamp { get; set; }

        #endregion
    }
}
