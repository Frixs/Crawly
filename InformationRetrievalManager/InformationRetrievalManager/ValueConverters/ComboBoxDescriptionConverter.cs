using System;
using System.Globalization;
using InformationRetrievalManager.Core;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Invert value into combo box value.
    /// </summary>
    public class ComboBoxDescriptionConverter : BaseValueConverter<ComboBoxDescriptionConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(Enum))
                return ((Enum)value).GetDescription();
            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
