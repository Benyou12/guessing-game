using PolyPaint.VueModels.Lobby;
using System.Windows.Controls;
using PolyPaint.Utils;
using System.Windows.Media;
namespace PolyPaint.Vues.Lobby
{
    /// <summary>
    /// Logique d'interaction pour FinalizeTeamTraining.xaml
    /// </summary>
    public partial class FinalizeTeamTraining : UserControl
    {
        public FinalizeTeamTraining()
        {
            InitializeComponent();
            DataContext = new FinalizeTeamTrainingViewModel();
            Mode(Global.mode);
        }
        private void Mode(string mode)
        {
            if (mode == "Light")
            {
                // White
                //Page.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //Rectangle.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //WaitingPlayersBorder.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //Players.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //ListBoxPlayers.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));

                //// Black
                //WaitingPlayersLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
 
            }
            if (mode == "Dark")
            {
                // Black - Gray
                //Page.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
                //Rectangle.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
                //WaitingPlayersBorder.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
                //Players.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
                //ListBoxPlayers.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
                //// White
                //WaitingPlayersLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));


            }
        }
    }
}
