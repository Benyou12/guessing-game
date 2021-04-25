using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using PolyPaint.Models.Stats;
using PolyPaint.Utils;

namespace PolyPaint.Converters
{
    public class BadgeDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var badge = (Badge)value;

            if (Global.language == "en-US")
                return badge.BadgeDescriptionEN;

            return badge.BadgeDescriptionFR;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

    public class BadgeImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var badge = (Badge)value;

            if (Global.language == "en-US")
                return badge.BadgeImgEN;

            return badge.BadgeImgFR;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

    public class BadgeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var badge = (Badge)value;

            if (Global.language == "en-US")
                return badge.BadgesNameEN;

            return badge.BadgeNameFR;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }
}
