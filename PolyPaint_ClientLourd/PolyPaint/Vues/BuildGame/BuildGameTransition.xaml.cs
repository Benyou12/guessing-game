using PolyPaint.VueModels.BuildGame;
using System.Windows.Controls;

namespace PolyPaint.Vues.BuildGame
{
    /// <summary>
    /// Logique d'interaction pour BuildGameTransition.xaml
    /// </summary>
    public partial class BuildGameTransition : UserControl
    {
        public BuildGameTransition()
        {
            DataContext = new BuildGameTransitionViewModel();
            InitializeComponent();
        }
    }
}
