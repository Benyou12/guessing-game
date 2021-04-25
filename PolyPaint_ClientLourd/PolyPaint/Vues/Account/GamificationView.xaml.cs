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
using PolyPaint.VueModels.Account;
using PolyPaint.Server;

namespace PolyPaint.Vues.Account
{
    /// <summary>
    /// Logique d'interaction pour GamificationView.xaml
    /// </summary>
    public partial class GamificationView : UserControl
    {
        public GamificationView()
        {
            DataContext = new GamificationViewModel();
            InitializeComponent();

            //UserImage.ImageSource = new BitmapImage(new System.Uri(AppContext.AppContextSingleton.Instance.AppContext.CurrentConnectedUser.ProfileImgURL));
            //Username.Content = AppContext.AppContextSingleton.Instance.AppContext.CurrentConnectedUser.Username;
            //FirstName.Content = AppContext.AppContextSingleton.Instance.AppContext.CurrentConnectedUser.FirstName;
            //LastName.Content = AppContext.AppContextSingleton.Instance.AppContext.CurrentConnectedUser.LastName;
            //Email.Content = AppContext.AppContextSingleton.Instance.AppContext.CurrentConnectedUser.Email;
        }
        private void BackToMainMenuRedirectLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainTransitions.TransitionPageControl.ShowPage(new MainMenus());
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

    }
}
