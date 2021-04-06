using Hearthstone_Deck_Tracker;
using System;
using System.Globalization;

namespace PackTracker.View
{
    internal class DateTimeConverter : AbstractValueConverter
    {
        private static Config _config = Config.Instance;
        protected virtual string _format => "G";

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is DateTime ? ((DateTime)value).ToString(this._format, this.GetCultureInfo()) : value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
