using Hearthstone_Deck_Tracker;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PackTracker.View
{
    internal abstract class AbstractValueConverter : IValueConverter
    {
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        protected CultureInfo GetCultureInfo()
        {
            var cult = null as CultureInfo;
            try
            {
                var lang = Config.Instance.Localization.ToString().Insert(2, "-");

                return new CultureInfo(lang);
            }
            catch (CultureNotFoundException)
            {
                return CultureInfo.InstalledUICulture;
            }
        }
    }
}
