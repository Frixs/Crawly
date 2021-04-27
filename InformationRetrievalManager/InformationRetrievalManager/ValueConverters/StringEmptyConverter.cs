using System;
using System.Globalization;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Check if string is empty converter to boolean. 
    /// Defined parameter inverts the result.
    /// </summary>
    public class StringEmptyConverter : BaseValueConverter<StringEmptyConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter == null 
                ? string.IsNullOrWhiteSpace((string)value) 
                : !string.IsNullOrWhiteSpace((string)value);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
