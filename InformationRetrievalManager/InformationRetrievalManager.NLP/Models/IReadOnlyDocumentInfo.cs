using System;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Model made as a wrapper for document data that are intedent to be stored as <see cref="InvertedIndex"/>.
    /// </summary>
    public interface IReadOnlyDocumentInfo
    {
        /// <summary>
        /// Document timestamp
        /// </summary>
        DateTime Timestamp { get; }
    }
}
