using PolyPaint.VueModels.Chat;
using System.Windows.Controls;
using System.Windows;

namespace PolyPaint.Vues.Chat
{
    /// <summary>
    /// Interaction logic for ConversationTransitionView.xaml
    /// </summary>
    public partial class ConversationTransitionView : UserControl
    {
        public ConversationTransitionView()
        {
            DataContext = new ConversationTransitionViewModel();
            InitializeComponent();
        }

    }
}
