using PolyPaint.Models.Lobby;
using PolyPaint.Services;
using PolyPaint.Utils.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PolyPaint.Converters
{
    class PlayerRoleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value != null)
            {
                Player player = (Player)value;
                if (player.Role ==  "DRAWER")
                {
                    return TranslationSource.Instance["RoleDrawer"];
                }
                else if (player.Role == "GUESSER")
                    return TranslationSource.Instance["RoleGuesser"];
                else
                    return TranslationSource.Instance["RoleNone"];
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }
}
