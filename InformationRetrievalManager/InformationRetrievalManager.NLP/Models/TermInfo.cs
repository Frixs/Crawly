using System;

namespace InformationRetrievalManager.NLP
{
    /// <summary>
    /// <see cref="IReadOnlyTermInfo"/> + ability to set values.
    /// </summary>
    [Serializable]
    public class TermInfo : IReadOnlyTermInfo
    {
        #region Public Properties

        /// <inheritdoc/>
        public int Frequency { get; set; } = -1;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks>
        ///     Should not be used outside of serialization/deserialization !!!
        /// </remarks>
        public TermInfo() { }

        /// <summary>
        /// Constructor with property sets
        /// </summary>
        /// <param name="frequency">The frequency of this term info.</param>
        public TermInfo(int frequency)
        {
            Frequency = frequency;
        }

        #endregion
    }
}
