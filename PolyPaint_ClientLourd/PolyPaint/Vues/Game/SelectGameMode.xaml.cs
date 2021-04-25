using PolyPaint.VueModels.Lobby;
using System.Windows.Controls;

namespace PolyPaint.Vues.Lobby
{
    /// <summary>
    /// Logique d'interaction pour SelectGameMode.xaml
    /// </summary>
    public partial class SelectGameMode : UserControl
    {
        public SelectGameMode()
        {
            InitializeComponent();
            DataContext = new SelectGameModeViewModel();
        }
    }
}
