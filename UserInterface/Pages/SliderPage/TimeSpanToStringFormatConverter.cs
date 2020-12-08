using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace UserInterface.Pages.SliderPage
{
    class TimeSpanToStringFormatConverter : System.Windows.Data.IValueConverter
    {
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
