namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// <see cref="IReadOnlyTermInfo"/> + ability to set values.
    /// </summary>
    public class TermInfo : IReadOnlyTermInfo
    {
        /// <inheritdoc/>
        public int Frequency { get; set; } = -1;
    }
}
