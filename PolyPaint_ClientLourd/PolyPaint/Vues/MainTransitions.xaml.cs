using MaterialDesignThemes.Wpf;
using PolyPaint.Services;
using PolyPaint.Utils;
using PolyPaint.Utils.Transitions;
using PolyPaint.VueModels;
using PolyPaint.Vues.Account;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PolyPaint.Server;
using PolyPaint.Vues.Chat;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Logique d'interaction pour Main.xaml
    /// Lien utile pour comprendre les transitions entre les pages
    /// https://www.codeproject.com/Articles/197132/Simple-WPF-Page-Transitions
    /// </summary>
    public partial class MainTransitions : Window
    {
        Stack<UserControl> pages = new Stack<UserControl>();
        public static TransitionPage TransitionPageControl;
        private Window _detachedChatWindow;

        public delegate void ChatDetachedEventHandler();
        public static event ChatDetachedEventHandler OnChatDetached =  delegate {};
        public MainTransitions()
        {
            DataContext = new MainMenusVueModele();
            InitializeComponent();
            TransitionPageControl = transitionPageControl;
            TransitionPageControl.ShowPage(new LoginView());

            if (Global.mode == "Dark")
            {
                var paletteHelper = new PaletteHelper();
                ITheme theme = paletteHelper.GetTheme();

                theme.SetBaseTheme(Theme.Dark);
                theme.SetPrimaryColor(Colors.Teal);
                theme.SetSecondaryColor(Colors.Teal);
                //theme.PrimaryMid = new ColorPair(Colors.Brown, Colors.White);

                paletteHelper.SetTheme(theme);
            }
            else
            {

            }

            _detachedChatWindow = null;
            WebSocket.Instance.DetachChatEvent += DetachChat;
        }

        public void DetachChat()
        {
            if (MessagingService.Instance.IsChatDetached)
                return;

            if (_detachedChatWindow == null)
            {
                _detachedChatWindow = new Window();
                _detachedChatWindow.Closing += OnDetachedChatClosing;
                _detachedChatWindow.Content = new ConversationTransitionView();
            }


            _detachedChatWindow.Height = 700;
            _detachedChatWindow.Width = 400;


            _detachedChatWindow.Show();
            MessagingService.Instance.IsChatDetached = true;
            OnChatDetached.Invoke();
        }

        private void OnDetachedChatClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _detachedChatWindow = null;
            MessagingService.Instance.IsChatDetached = false;
            OnChatDetached.Invoke();
        }

        public void NavigateToUsers(object o, RoutedEventArgs e)
        {
            _detachedChatWindow?.Close();
            TransitionPageControl.ShowPage(new PublicProfiles());
        }

        public void NavigateToMessages(object o, RoutedEventArgs e)
        {
            _detachedChatWindow?.Close();
            TransitionPageControl.ShowPage(new MainMenus());
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            _detachedChatWindow?.Close();
            var dataContext = DataContext as MainMenusVueModele;
            if (dataContext != null)
            {
                dataContext.DisconnectCommand.Execute(new { });
            }
        }
        private void UserProfile(object sender, RoutedEventArgs e)
        {
            _detachedChatWindow?.Close();

            if (GameService.Instance.IsInGame)
                GameService.Instance.LeaveCurrentGame();
            TransitionPageControl.ShowPage(new GamificationView());
        }
        private void Tutorial(object sender, RoutedEventArgs e)
        {
            _detachedChatWindow?.Close();

            if (GameService.Instance.IsInGame)
                GameService.Instance.LeaveCurrentGame();
            TransitionPageControl.ShowPage(new Tutorial());
        }

        private void Home(object sender, RoutedEventArgs e)
        {
            _detachedChatWindow?.Close();

            if (GameService.Instance.IsInGame)
                GameService.Instance.LeaveCurrentGame();
            TransitionPageControl.ShowPage(new Home());
        }

        private void ModeChangeChecked(object sender, RoutedEventArgs e)
        {
            Global.mode = "Dark";

            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            Console.WriteLine(theme.PrimaryMid.Color.ToString());

            Color color = (Color)ColorConverter.ConvertFromString("#FF2196F3");

            theme.SetBaseTheme(Theme.Dark);
            theme.SetPrimaryColor(Colors.Teal);
            theme.SetSecondaryColor(Colors.Teal);

            paletteHelper.SetTheme(theme);
        }

        private void ModeChangeUnChecked(object sender, RoutedEventArgs e)
        {
            Global.mode = "Light";

            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();

            Console.WriteLine(theme.PrimaryMid.Color.ToString());

            theme.SetBaseTheme(Theme.Light);
            theme.SetPrimaryColor(Colors.Teal);
            theme.SetSecondaryColor(Colors.Teal);

            paletteHelper.SetTheme(theme);
        }

        private void UIElement_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var listbox = sender as ListBox;
            var selectedOption = listbox?.SelectedItem as TextBlock;

            if (selectedOption.Name == "userProfile")
            {
                UserProfile(new { }, new RoutedEventArgs());
            }
            else if (selectedOption.Name == "tutorial")
            {
                Tutorial(new { }, new RoutedEventArgs());
            }
            else
            {
                Logout(new { }, new RoutedEventArgs());
            }
        }
    }
}
