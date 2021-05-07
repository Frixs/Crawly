using MessagePack;
using System;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// <see cref="IReadOnlyDocumentInfo"/> + ability to set values.
    /// </summary>
    [MessagePackObject]
    public sealed class DocumentInfo : IReadOnlyDocumentInfo
    {
        #region Public Properties

        /// <inheritdoc/>
        [Key(nameof(DocumentInfo) + nameof(Timestamp))]
        public DateTime Timestamp { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor with property sets
        /// </summary>
        /// <param name="timestamp">The timestamp of this document info.</param>
        public DocumentInfo(DateTime timestamp)
        {
            Timestamp = timestamp;
        }

        #endregion
    }
}
