using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace UserInterface.Pages.SliderPage
{
    class TimeSpanToStringFormatConverter : System.Windows.Data.IValueConverter
    {
        /// <summary>
        /// Converts a time span to a player friendly display format.
        /// </summary>
        /// <param name="value">The time span that needs to be converted</param>
        /// <param name="targetType">not used</param>
        /// <param name="parameter">not used</param>
        /// <param name="culture">not event touched</param>
        /// <returns>a string represention of the time span</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is TimeSpan))
            {
                return "00:00";
            }

            return ((TimeSpan)value).ToString(@"m\:ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
