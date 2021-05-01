namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// Model containing the data about term in vocabulary in <see cref="InvertedIndex"/>.
    /// </summary>
    public interface IReadOnlyTermInfo
    {
        /// <summary>
        /// Frequency of the term in the document.
        /// </summary>
        int Frequency { get; }
    }
}
