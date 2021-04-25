using PolyPaint.Models.Stats;
using PolyPaint.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;

namespace PolyPaint.Converters
{
    /// <summary>
    /// Permet de générer une couleur en fonction de la chaine passée en paramètre.
    /// Par exemple, pour chaque bouton d'un groupe d'options on compare son nom avec l'élément actif (sélectionné) du groupe.
    /// S'il y a correspondance, la bordure du bouton aura une teinte bleue, sinon elle sera transparente.
    /// Cela permet de mettre l'option sélectionnée dans un groupe d'options en évidence.
    /// </summary>
    class BorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value.ToString() == parameter.ToString()) ? "#FF58BDFA" : "#00000000";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

    /// Permet de générer une couleur en fonction de la chaine passée en paramètre.
    /// Par exemple, pour chaque bouton d'un groupe d'option on compare son nom avec l'élément actif (sélectionné) du groupe.
    /// S'il y a correspondance, la couleur de fond du bouton aura une teinte bleue, sinon elle sera transparente.
    /// Cela permet de mettre l'option sélectionnée dans un groupe d'options en évidence.
    class BackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value.ToString() == parameter.ToString()) ? "#3F58BDFA" : "#00000000";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

    /// <summary>
    /// Permet au InkCanvas de définir son mode d'édition en fonction de l'outil sélectionné.
    /// </summary>
    class EditionModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            switch (value)
            {
                case "efface_segment":
                    return InkCanvasEditingMode.EraseByPoint;
                case "efface_trait":
                    return InkCanvasEditingMode.EraseByStroke;
                default:
                    return InkCanvasEditingMode.Ink;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

    class DateTimeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                long valueLong = (long)value;
                TimeSpan time = TimeSpan.FromMilliseconds(valueLong);
                DateTime date = new DateTime(1970, 1, 1) + time;
                //var newTime = TimeZoneInfo.ConvertTimeFromUtc(date, TimeZoneInfo.FindSystemTimeZoneById("Easter Standard Time"));
                date = TimeZone.CurrentTimeZone.ToLocalTime(date);
                string stringDate = date.ToString("MM/dd/yyyy HH:mm:ss");
                return (stringDate);
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

    class ChatPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var user = (Models.User)value;

            if (user.UID == AppContext.AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID)
            {
                return "Right";
            }
            else if (user.UID == "moderator")
            {
                return "Center";
            }
            else
            {
                return "Left";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

    class ChatForegroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var user = (Models.User)value;

            if (user.UID == AppContext.AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID)
            {
                return "#FFF";
            }
            else if (user.UID == "moderator")
            {
                return "#FFF";
            }
            else
            {
                return "#333";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

    class ChatContentConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var message = (Models.Message)value;

            if (message.User.UID.Contains("virtual"))
            {
                var virtualMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.VirtualPlayerMessage>(message.Text);
                if (Global.language == "en-US")
                    return virtualMessage.EnglishMsg;

                return virtualMessage.FrenchMsg;
            }
            if (message.User.UID.Contains("moderator"))
            {
                var moderatorMessage = Newtonsoft.Json.JsonConvert.DeserializeObject<Models.VirtualPlayerMessage>(message.Text);
                if (Global.language == "en-US")
                    return moderatorMessage.EnglishMsg;

                return moderatorMessage.FrenchMsg;
            }
            else
            {
                return message.Text;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

    class ChatBackgroundColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var user = (Models.User)value;

            if (user.UID == AppContext.AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID)
            {
                return "#2796ED";
            }
            else if (user.UID == "moderator")
            {
               return "#F45B69";
            }
            else
            {
                return "#F1F1F1";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

    class ProfileBadgeCount : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var badges = (List<UserBadge>)value;
            return badges.Count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

    class ProfileWins : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var userGameStats = (UserGameStats)value;

            if (userGameStats.RoundsPlayed < 1) return 0 + "%";

            return Math.Round((double)(userGameStats.Victories / userGameStats.RoundsPlayed * 100)) + "%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

    class ProfileTopBadges : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var badges = (List<UserBadge>)value;
            List<object> listBadges = new List<object>();

            for (int i = 0; i < 3; i++)
            {
                if (badges.Count <= i) break;

                if (Global.language == "en-US")
                {
                    listBadges.Add(new
                    {
                        BadgesName = badges[i].Badge.BadgesNameEN,
                        BadgeImg = badges[i].Badge.BadgeImgEN,
                        BadgeDescription = badges[i].Badge.BadgeDescriptionEN
                    });
                } else
                {
                    listBadges.Add(new
                    {
                        BadgesName = badges[i].Badge.BadgeNameFR,
                        BadgeImg = badges[i].Badge.BadgeImgFR,
                        BadgeDescription = badges[i].Badge.BadgeDescriptionFR
                    });
                }
            }

            return listBadges;
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => System.Windows.DependencyProperty.UnsetValue;
    }

}
