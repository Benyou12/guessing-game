using PolyPaint.VueModels.Chat;
using System.Windows.Controls;
using PolyPaint.Utils;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using PolyPaint.Server;
using System.Windows;

namespace PolyPaint.Vues.Chat
{
    /// <summary>
    /// Interaction logic for ChannelView.xaml
    /// </summary>
    public partial class ConversationView : UserControl
    {
        public ConversationView()
        {
            DataContext = new ConversationViewModel();
            InitializeComponent();
            Mode(Global.mode);
        }
        private void Mode(string mode)
        {
            if (mode == "Light")
            {
                // White
                //Page.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
               // Boite.Background =  new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //SearchText.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                // Red
                //SearchText.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));

                // Black
                //itemsCtrl.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            if (mode == "Dark")
            {
                // Black - Gray
                //Page.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
               // Boite.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
                //SearchText.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
                //itemsCtrl.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));

                // White
                //itemsCtrl.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //SearchText.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }

        private void DetachChat(object sender, RoutedEventArgs e)
        {
            WebSocket.Instance.DetachChat();
        }
    }
}
