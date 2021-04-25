using PolyPaint.VueModels;
using System.Windows.Controls;

namespace PolyPaint.Vues
{
    /// <summary>
    /// Interaction logic for GameFlowTransition.xaml
    /// </summary>
    public partial class GameFlowTransition : UserControl
    {
        public GameFlowTransition()
        {
            DataContext = new GameFlowTransitionViewModel();
            InitializeComponent();
        }
    }
}
