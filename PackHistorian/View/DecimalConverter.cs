using System;
using System.Globalization;

namespace PackTracker.View
{
    internal class DecimalConverter : AbstractValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.TryParse(value.ToString(), out var dec) ? dec.ToString("n0", this.GetCultureInfo()) : value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
