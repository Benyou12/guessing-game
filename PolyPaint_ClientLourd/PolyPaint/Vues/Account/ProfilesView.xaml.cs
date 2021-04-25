using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PolyPaint.Server;
using PolyPaint.Services;
using PolyPaint.Utils;
using PolyPaint.Utils.Localization;

namespace PolyPaint.Vues.Account
{
    /// <summary>
    /// Logique d'interaction pour ProfilesView.xaml
    /// </summary>
    public partial class ProfilesView : UserControl
    {
        public ProfilesView()
        {
            InitializeComponent();
            InitializeComponent();
            Mode(Global.mode);
            if (Global.mode == "Light")
                cboxMode.SelectedIndex = 1;
            else
                cboxMode.SelectedIndex = 0;
            if (Global.language == "en-US")
                cboxLangue.SelectedIndex = 1;
            else
                cboxLangue.SelectedIndex = 0;
          

            profileImageBtn.ImageSource = new BitmapImage(new System.Uri(AppContext.AppContextSingleton.Instance.AppContext.CurrentConnectedUser.ProfileImgURL));

        }
        private void Logout(object sender, MouseButtonEventArgs e)
        {
            Authentification.SignOut();
            MainTransitions.TransitionPageControl.ShowPage(new LoginView());
        }
        private void UserProfile(object sender, MouseButtonEventArgs e)
        {
            MainTransitions.TransitionPageControl.ShowPage(new GamificationView());
        }
        private void Tutorial(object sender, MouseButtonEventArgs e)
        {
            MainTransitions.TransitionPageControl.ShowPage(new Tutorial());
        }
        private void Home(object sender, MouseButtonEventArgs e)
        {
            MainTransitions.TransitionPageControl.ShowPage(new Home());
        }
        private void Mode(string mode)
        {
            if (mode == "Light")
            {
                // White
                Page.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                Rectangle.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));

             
            }
            if (mode == "Dark")
            {
                // Black - Gray
                Page.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
                Rectangle.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
            }
        }
        private void LanguageChange(object sender, SelectionChangedEventArgs e)
        {

            if (cboxLangue.Text == "French" || cboxLangue.Text == "Français")
            {
                TranslationSource.Instance.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                Global.language = "en-US";
            }

            else if (cboxLangue.Text == "Anglais" || cboxLangue.Text == "English")
            {
                TranslationSource.Instance.CurrentCulture = new System.Globalization.CultureInfo("fr-Fr");
                Global.language = "fr-Fr";
            }
        }
        private void ModeChange(object sender, SelectionChangedEventArgs e)
        {
            if (cboxMode.Text == "Dark" || cboxMode.Text == "Sombre")
            {
                Global.mode = "Light";
                Mode(Global.mode);
            }
            else if (cboxMode.Text == "Light" || cboxMode.Text == "Claire")
            {
                Global.mode = "Dark";
                Mode(Global.mode);
            }
        }

    }
}
