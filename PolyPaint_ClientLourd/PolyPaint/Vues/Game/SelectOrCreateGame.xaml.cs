using PolyPaint.VueModels.Lobby;
using System.Windows.Controls;
using PolyPaint.Utils;
using System.Windows.Media;
namespace PolyPaint.Vues.Lobby
{
    /// <summary>
    /// Logique d'interaction pour SelectOrCreateGroup.xaml
    /// </summary>
    public partial class SelectOrCreateGame : UserControl
    {
        public SelectOrCreateGame()
        {
            InitializeComponent();
            DataContext = new SelectOrCreateGameViewModel();
            Mode(Global.mode);
        }
        private void Mode(string mode)
        {
            if (mode == "Light")
            {
                // White
                //Page.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //Rectangle.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //JoinGroupLabel.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //ListBoxgames.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                // Black
                //JoinGroupText.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
              
            }
            if (mode == "Dark")
            {
                // Black - Gray
                //Page.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
               //Rectangle.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
                //JoinGroupLabel.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
                //ListBoxgames.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
                // White
                //JoinGroupText.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
  
            }
        }


    }
}
