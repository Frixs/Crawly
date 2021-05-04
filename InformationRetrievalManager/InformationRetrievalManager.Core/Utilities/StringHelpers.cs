using System.Text.RegularExpressions;

namespace InformationRetrievalManager.Core
{
    /// <summary>
    /// Helpers for the <see cref="string"/> class
    /// </summary>
    public static class StringHelpers
    {
        /// <summary>
        /// Format the string as a shorten version with possible 3 dots at the end.
        /// </summary>
        /// <param name="value">The string</param>
        /// <param name="length">Allowed length</param>
        /// <returns></returns>
        public static string ShortenWithDots(string value, int length)
        {
            if (value.Length > length)
                return value.Substring(0, length) + "...";
            return value;
        }

        /// <summary>
        /// Format pascal case into readable form - adds spaces between words
        /// </summary>
        /// <param name="value">The input string</param>
        /// <returns>Formatted output string</returns>
        public static string FormatPascalCase(string value)
        {
            return Regex.Replace(value, "(\\B[A-Z])", " $1");
        }


        /// <summary>
        /// Remove new lines from the input string.
        /// </summary>
        /// <param name="value">The input string</param>
        /// <param name="replacement">Replacement for the newline.</param>
        /// <returns>String without newlines.</returns>
        public static string ReplaceNewLines(string value, string replacement)
        {
            return Regex.Replace(value, "(\\r\\n)|(\\n)|(\\r)", replacement);
        }
    }
}
