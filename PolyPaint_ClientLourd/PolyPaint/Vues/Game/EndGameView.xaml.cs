using PolyPaint.Models.Lobby;
using PolyPaint.Models.Socket;
using PolyPaint.Services;
using PolyPaint.VueModels.Lobby;
using PolyPaint.Vues.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolyPaint.Vues.Lobby
{
    /// <summary>
    /// Logique d'interaction pour EndGameView.xaml
    /// </summary>

    public partial class EndGameView : UserControl
    {
        

        public EndGameView()
        {
            DataContext = new EndGameViewModel();
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainTransitions.TransitionPageControl.ShowPage(new MainMenus());
        }
    }
}
