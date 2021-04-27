namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Encapsulates an error from the <see cref="DataValidationResult"/>.
    /// </summary>
    public class DataValidationError
    {
        /// <summary>
        /// Gets or sets the code for this error.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the description for this error.
        /// </summary>
        public string Description { get; set; }
    }
}
