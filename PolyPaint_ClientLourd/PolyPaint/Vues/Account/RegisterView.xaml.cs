using Microsoft.Win32;
using PolyPaint.VueModels.Account;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using PolyPaint.Utils;
using PolyPaint.Utils.Localization;

namespace PolyPaint.Vues.Account
{
    /// <summary>
    /// Logique d'interaction pour RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            DataContext = new RegisterViewModel();
            InitializeComponent();
            if (Global.language == "en-US")
                cboxLangue.SelectedIndex = 1;
            else
                cboxLangue.SelectedIndex = 0;
        }

        private void LoginRedirectLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(delegate
            {
                MainTransitions.TransitionPageControl.ShowPage(new LoginView());
            });
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
    }
}
