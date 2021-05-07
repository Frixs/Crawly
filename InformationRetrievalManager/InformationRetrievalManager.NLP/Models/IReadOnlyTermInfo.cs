namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Model made as a wrapper for term data that are intedent to be stored as <see cref="InvertedIndex"/>.
    /// </summary>
    public interface IReadOnlyTermInfo
    {
        /// <summary>
        /// Frequency of the term in the document.
        /// </summary>
        int Frequency { get; }
    }
}
