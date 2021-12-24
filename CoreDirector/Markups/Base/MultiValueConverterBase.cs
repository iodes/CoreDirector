using System.Globalization;

namespace CoreDirector.Markups
{
    public abstract class MultiValueConverterBase<TTo> : MultiValueConverterCore
    {
        public abstract TTo Convert(object[] values, object parameter, CultureInfo culture);
        public abstract object[] ConvertBack(TTo value, object parameter, CultureInfo culture);
    }

    public abstract class MultiValueConverterBase<TFrom1, TFrom2, TTo> : MultiValueConverterCore
    {
        public abstract TTo Convert(TFrom1 value1, TFrom2 value2, object parameter, CultureInfo culture);
        public abstract object[] ConvertBack(TTo value, object parameter, CultureInfo culture);
    }

    public abstract class MultiValueConverterBase<TFrom1, TFrom2, TFrom3, TTo> : MultiValueConverterCore
    {
        public abstract TTo Convert(TFrom1 value1, TFrom2 value2, TFrom3 value3, object parameter, CultureInfo culture);
        public abstract object[] ConvertBack(TTo value, object parameter, CultureInfo culture);
    }

    public abstract class MultiValueConverterBase<TFrom1, TFrom2, TFrom3, TFrom4, TTo> : MultiValueConverterCore
    {
        public abstract TTo Convert(TFrom1 value1, TFrom2 value2, TFrom3 value3, TFrom4 value4, object parameter, CultureInfo culture);
        public abstract object[] ConvertBack(TTo value, object parameter, CultureInfo culture);
    }

    public abstract class MultiValueConverterBase<TFrom1, TFrom2, TFrom3, TFrom4, TFrom5, TTo> : MultiValueConverterCore
    {
        public abstract TTo Convert(TFrom1 value1, TFrom2 value2, TFrom3 value3, TFrom4 value4, TFrom5 value5, object parameter, CultureInfo culture);
        public abstract object[] ConvertBack(TTo value, object parameter, CultureInfo culture);
    }
}