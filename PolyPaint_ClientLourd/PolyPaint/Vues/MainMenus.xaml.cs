using PolyPaint.Server;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PolyPaint.Vues;
using PolyPaint.Vues.Account;
using PolyPaint.VueModels;
using PolyPaint.Vues.Chat;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PolyPaint.Utils;
using PolyPaint.Utils.Localization;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using System;
using PolyPaint.Services;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Logique d'interaction pour MainMenus.xaml
    /// </summary>
    public partial class MainMenus : UserControl
    {
        public MainMenus()
        {
            DataContext = new MainMenusVueModele();
            InitializeComponent();


            MainTransitions.OnChatDetached += DetachChat;

        }


        private void DetachChat()
        {
            if (MessagingService.Instance.IsChatDetached)
            {
                dockPannelConversationTransition.Visibility = Visibility.Collapsed;
                dockPannelGameFlowTransition.SetValue(Grid.ColumnProperty, 0);
                dockPannelGameFlowTransition.SetValue(Grid.ColumnSpanProperty, 2);
            }
            else
            {
                dockPannelGameFlowTransition.SetValue(Grid.ColumnSpanProperty, 1);
                dockPannelConversationTransition.Visibility = Visibility.Visible;
            }



        }

        private void OnDetachedChatClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //_detachedChatWindow = null;
            

            
            MessagingService.Instance.IsChatDetached = false;
        }

        private void ProfilRedirectLink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (GameService.Instance.IsInGame)
                GameService.Instance.LeaveCurrentGame();
            MainTransitions.TransitionPageControl.ShowPage(new GamificationView());
        }

        private void Logout(object sender, MouseButtonEventArgs e)
        {
            var dataContext = DataContext as MainMenusVueModele;
            if (dataContext != null)
            {
                dataContext.DisconnectCommand.Execute(new { });
            }
        }
        private void UserProfile(object sender, MouseButtonEventArgs e)
        {
            if (GameService.Instance.IsInGame)
                GameService.Instance.LeaveCurrentGame();
            
            if(MessagingService.Instance.IsChatDetached)
                MainTransitions.TransitionPageControl.ShowPage(new GamificationView());
        }
        private void Tutorial(object sender, MouseButtonEventArgs e)
        {
            if (GameService.Instance.IsInGame)
                GameService.Instance.LeaveCurrentGame();
            if (MessagingService.Instance.IsChatDetached)

                MainTransitions.TransitionPageControl.ShowPage(new Tutorial());
        }

        private void Home(object sender, MouseButtonEventArgs e)
        {
            if (GameService.Instance.IsInGame)
                GameService.Instance.LeaveCurrentGame();
            if (MessagingService.Instance.IsChatDetached)

                MainTransitions.TransitionPageControl.ShowPage(new Home());
        }

        private void Mode(string mode)
        {
            if (mode == "Light")
            {
                // White (255, 255, 255)
                // Page.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
            if (mode == "Dark")
            {
                // Black - Gray (52, 52, 61)
                // Page.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
            }

        }

        private void ModeChangeChecked(object sender, RoutedEventArgs e)
        {
            Global.mode = "Dark";
            Mode(Global.mode);

            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            Console.WriteLine(theme.PrimaryMid.Color.ToString());

            Color color = (Color)ColorConverter.ConvertFromString("#FF2196F3");

            theme.SetBaseTheme(Theme.Dark);
            theme.SetPrimaryColor(Colors.Teal);
            theme.SetSecondaryColor(Colors.Teal);
            //theme.PrimaryMid = new ColorPair(Colors.Brown, Colors.White);

            paletteHelper.SetTheme(theme);
        }

        private void ModeChangeUnChecked(object sender, RoutedEventArgs e)
        {
            Global.mode = "Light";
            Mode(Global.mode);



            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            Console.WriteLine(theme.PrimaryMid.Color.ToString());

            theme.SetBaseTheme(Theme.Light);
            theme.SetPrimaryColor(Colors.Teal);
            theme.SetSecondaryColor(Colors.Teal);
            //theme.PrimaryMid = new ColorPair(Colors.Brown, Colors.White);

            paletteHelper.SetTheme(theme);
        }
    }
}
