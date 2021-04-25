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
using PolyPaint.Vues;
using PolyPaint.Vues.Account;
using PolyPaint.VueModels;
using PolyPaint.Vues.Chat;
using PolyPaint.Utils;
using PolyPaint.Utils.Localization;
using PolyPaint.Vues.BuildGame;
using System.Timers;
using PolyPaint.Server;
using PolyPaint.Services;
using PolyPaint.Models;

namespace PolyPaint.Vues.Account
{
    /// <summary>
    /// Logique d'interaction pour Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public User User { get { return AppContext.AppContextSingleton.Instance.AppContext.CurrentConnectedUser; } }
        public Home()
        {
            DataContext = this;
            InitializeComponent();

            if (Global.language == "en-US")
                cboxLangue.SelectedIndex = 1;
            else
                cboxLangue.SelectedIndex = 0;
        }

        public void NavigateToUsers(object o, RoutedEventArgs e)
        {
            MainTransitions.TransitionPageControl.ShowPage(new PublicProfiles());
        }

        public void NavigateToLobby(object o, RoutedEventArgs e)
        {
            MainTransitions.TransitionPageControl.ShowPage(new MainMenus());
        }

        public void NavigateToMessages(object o, RoutedEventArgs e)
        {
            MainTransitions.TransitionPageControl.ShowPage(new ConversationTransitionView());
        }

        public void NaviateToGameCreation(object o, RoutedEventArgs e)
        {
            MainTransitions.TransitionPageControl.ShowPage(new BuildGameWindow());
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
