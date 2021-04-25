using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PolyPaint.Converters
{
    class LoginStatsDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                bool isLoggedIn = (bool)value;
                if (isLoggedIn)
                {
                    return "#e74c3c";
                }

            }

            return "#000000";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }
}

