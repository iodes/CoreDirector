using System;
using System.Globalization;
using System.Windows.Data;

namespace CoreDirector.Markups
{
    public abstract class ValueConverterBase<IFrom, ITo> : MarkupSupport, IValueConverter
    {
        public abstract ITo Convert(IFrom value, object parameter, CultureInfo culture);

        public abstract IFrom ConvertBack(ITo value, object parameter, CultureInfo culture);

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((IFrom)value, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((ITo)value, parameter, culture);
        }
    }
}
