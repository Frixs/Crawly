using System;
using System.Collections;
using System.Globalization;

namespace InformationRetrievalManager
{
    /// <summary>
    /// Has collection any items - returns bool
    /// </summary>
    public class CollectionHasAnyItemsConverter : BaseValueConverter<CollectionHasAnyItemsConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ICollection)value).Count > 0;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
