using System;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Attribute to define constraint range for the numeric property as a additional information.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ConstraintRangeAttribute : Attribute
    {
        #region Public Properties

        /// <summary>
        /// The min value
        /// </summary>
        public int Min { get; }

        /// <summary>
        /// The max value
        /// </summary>
        public int Max { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ConstraintRangeAttribute(int min, int max)
        {
            Min = min;
            Max = max;
        }

        #endregion
    }
}
