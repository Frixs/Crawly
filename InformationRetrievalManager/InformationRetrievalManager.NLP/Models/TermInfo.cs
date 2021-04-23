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
        public TermInfo(int frequency)
        {
            Frequency = frequency;
        }

        #endregion
    }
}
