using PolyPaint.Models.BuildGame;
using PolyPaint.VueModels.BuildGame;
using System.Windows.Controls;
using System.Windows.Input;

namespace PolyPaint.Vues.BuildGame
{
    /// <summary>
    /// Logique d'interaction pour GoogleSearchView.xaml
    /// </summary>
    public partial class GoogleSearchView : UserControl
    {
        public delegate void SelectSearchImageEvent(string searchItem);
        public static SelectSearchImageEvent OnSelectSearchImage = delegate { };

        public GoogleSearchView()
        {
            DataContext = new GoogleSearchViewModel();
            InitializeComponent();
        }
        private void GoogleSearchItem_BtnClick(object sender, MouseButtonEventArgs e)
        {
            SearchItem item = (sender as GoogleSearchItemView).DataContext as SearchItem;
            OnSelectSearchImage.Invoke(item.Link);
        }
    }
}
