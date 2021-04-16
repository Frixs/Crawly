using Ixs.DNA;
using System.Globalization;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Hold the data neccessary for date-time parsing
    /// </summary>
    public class DatetimeParseData
    {
        #region Public Properties

        /// <summary>
        /// DateTime format according to c# specification
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Culture info related to the date time
        /// </summary>
        public CultureInfo CultureInfo { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="format">Sets the format according to C# specification - default value = 'yyyy-MM-dd HH:mm'</param>
        /// <param name="cultureInfo">The culture info related to the date-time</param>
        public DatetimeParseData(string format = null, CultureInfo cultureInfo = null)
        {
            // Set format
            // TODO: check if the format is valid
            if (!format.IsNullOrEmpty())
                Format = format;
            else
                Format = "yyyy-MM-dd HH:mm";

            // Set culture info
            if (cultureInfo != null)
                CultureInfo = cultureInfo;
            else
                CultureInfo = CultureInfo.CurrentCulture;
        }

        #endregion
    }
}
