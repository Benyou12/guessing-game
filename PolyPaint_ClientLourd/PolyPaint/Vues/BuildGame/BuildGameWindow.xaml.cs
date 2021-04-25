using PolyPaint.Models.BuildGame;
using PolyPaint.VueModels.BuildGame;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using PolyPaint.Utils;
using PolyPaint.Utils.Localization;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PolyPaint.Vues;
using PolyPaint.Vues.Account;
namespace PolyPaint.Vues.BuildGame
{
    /// <summary>
    /// Logique d'interaction pour BuildGameWindow.xaml
    /// </summary>
    public partial class BuildGameWindow : UserControl
    {
        public BuildGameWindow()
        {
            DataContext = new BuildGameViewModel();
            InitializeComponent();
            cbDrawingModes.ItemsSource = Enum.GetValues(typeof(DRAWING_MODES)).Cast<DRAWING_MODES>();
            cbGameDifficulty.ItemsSource = Enum.GetValues(typeof(GAME_DIFFICULTY)).Cast<GAME_DIFFICULTY>();
            cbWordLanguage.ItemsSource = Enum.GetValues(typeof(LANG)).Cast<LANG>();
        }

        private void SetSelectedItemBinding(string enumName)
        {
            Binding cmbBinding = new Binding
            {
                Mode = BindingMode.TwoWay,
                Path = new PropertyPath(enumName),
            };
            cbDrawingDirections.SetBinding(ComboBox.SelectedItemProperty, cmbBinding);
            cbDrawingDirections.IsEnabled = true;
        }

        private void ComboBoxDrawingModes_DropDownClosed(object sender, EventArgs e)
        {
            var elem = (sender as ComboBox).SelectedItem;
            if (elem != null)
            {
                var item = (DRAWING_MODES)elem;
                if (item == DRAWING_MODES.PANORAMIC)
                {
                    cbDrawingDirections.ItemsSource = Enum.GetValues(typeof(PANORAMIC_DRAWING_DIRECTION)).Cast<PANORAMIC_DRAWING_DIRECTION>();
                    SetSelectedItemBinding("PanoramicDrawingDirection");
                    cbDrawingDirections.SelectedItem = PANORAMIC_DRAWING_DIRECTION.FROM_LEFT_TO_RIGHT;
                }
                else if (item == DRAWING_MODES.CENTERED)
                {
                    cbDrawingDirections.ItemsSource = Enum.GetValues(typeof(CENTER_DRAWING_DIRECTION)).Cast<CENTER_DRAWING_DIRECTION>();
                    SetSelectedItemBinding("CenterDrawingDirection");
                    cbDrawingDirections.SelectedItem = CENTER_DRAWING_DIRECTION.INSIDE_TO_OUTSIDE;
                }
                else
                {
                    cbDrawingDirections.ItemsSource = null;
                    cbDrawingDirections.IsEnabled = false;
                }
            }   
        }
        private void Mode(string mode)
        {
            if (mode == "Light")
            {
                // White
                //Page.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //  Rectangle.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //btnPrevious.Source = new BitmapImage(new Uri(@"..\..\Resources\Button\PreviousBlack.png", UriKind.Relative));
                //btnNext.Source = new BitmapImage(new Uri(@"..\..\Resources\Button\NextBlack.png", UriKind.Relative));

                
                // Black
                //BuildGameDrawingDirection.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                //BuildGameDrawingMode.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                //BuildGameDifficultyLevel.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                //BuildGameIndice.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                //BuildGamePath.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                //BuildGameWord.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                //BuildGameWordLanguage.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
            if (mode == "Dark")
            {
                // Black - Gray
                //Page.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
                //   Rectangle.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));

                
                //btnPrevious.Source = new BitmapImage (new Uri(@"..\..\Resources\Button\PreviousWhite.png", UriKind.Relative));
                //btnNext.Source = new BitmapImage(new Uri(@"..\..\Resources\Button\NextWhite.png", UriKind.Relative));
                // White
                //BuildGameDrawingDirection.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //BuildGameDrawingMode.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //BuildGameDifficultyLevel.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //BuildGameIndice.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //BuildGamePath.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //BuildGameWord.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                //BuildGameWordLanguage.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }
        private void Logout(object sender, MouseButtonEventArgs e)
        {
            MainTransitions.TransitionPageControl.ShowPage(new PolyPaint.Vues.Account.LoginView());
        }
        private void UserProfile(object sender, MouseButtonEventArgs e)
        {
            MainTransitions.TransitionPageControl.ShowPage(new GamificationView());
        }
        private void Tutorial(object sender, MouseButtonEventArgs e)
        {
            MainTransitions.TransitionPageControl.ShowPage(new Tutorial());
        }
        private void Home(object sender, MouseButtonEventArgs e)
        {
            MainTransitions.TransitionPageControl.ShowPage(new Home());
        }
    }
}
