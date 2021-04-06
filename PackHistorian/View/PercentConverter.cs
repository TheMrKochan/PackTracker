using System;
using System.Globalization;

namespace PackTracker.View
{
    internal class PercentConverter : AbstractValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.TryParse(value.ToString(), out var percent) ? percent.ToString("p1", this.GetCultureInfo()) : value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
