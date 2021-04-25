using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using PolyPaint.VueModels;
using System.Windows.Controls;
using PolyPaint.Utils;
using PolyPaint.AppContext;

namespace PolyPaint
{
    /// <summary>
    /// Logique d'interaction pour FenetreDessin.xaml
    /// </summary>
    public partial class DrawingCanvasWindow : UserControl
    {
      private  System.Media.SoundPlayer PencilSound = new System.Media.SoundPlayer(@"..\..\Resources\Sound\PencilWriting.wav");
       
        public DrawingCanvasWindow()
        {
            InitializeComponent();
            DataContext = new DrawingVueModel();
            Mode(Global.mode);
            ResizeInkCanvas();

            // Events
            SizeChanged += OnWindowSizeChanged;

            if (DrawingServiceSingleton.Instance.DrawingService.IsCreatingGame)
            {
                TopBar.Visibility = Visibility.Collapsed;
            } else
            {
                TopBar.Visibility = Visibility.Visible;
            }
            
        }

        private void Canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)

        {
            PencilSound.Stop();
            ((DrawingVueModel)this.DataContext).MouseUp.Execute(e.GetPosition((IInputElement)sender));
        }

        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           
            ((DrawingVueModel)this.DataContext).MouseDown.Execute(e.GetPosition((IInputElement)sender));
            PencilSound.Play();
        }

        private void Canvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((DrawingVueModel)this.DataContext).MouseMove.Execute(e.GetPosition((IInputElement)sender));
        }

        private void SetBlackColor(object o, RoutedEventArgs e)
        {
            ((DrawingVueModel)this.DataContext).SelectedColor = "#000000";
        }

        private void SetRedColor(object o, RoutedEventArgs e)
        {
            ((DrawingVueModel)this.DataContext).SelectedColor = "#e74c3c";
        }

        private void SetGreenColor(object o, RoutedEventArgs e)
        {
            ((DrawingVueModel)this.DataContext).SelectedColor = "#2ecc71";
        }

        private void SetBlueColor(object o, RoutedEventArgs e)
        {
            ((DrawingVueModel)this.DataContext).SelectedColor = "#3498db";
        }

        private void SetYellowColor(object o, RoutedEventArgs e)
        {
            ((DrawingVueModel)this.DataContext).SelectedColor = "#f1c40f";
        }

        private void SetPinkColor(object o, RoutedEventArgs e)
        {
            ((DrawingVueModel)this.DataContext).SelectedColor = "#be2edd";
        }

        protected void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeInkCanvas();
        }

        private void ResizeInkCanvas()
        {
            //colonne.Width = new GridLength(Math.Max(canvasContainer.ActualWidth, 32f));
            //ligne.Height = new GridLength(Math.Max(canvasContainer.ActualHeight, 32f));
        }

        public void Mode(string mode)
        {
            if (mode == "Light")
            {
                // White
                //Dashboard.Background= new SolidColorBrush(Color.FromRgb(255, 255, 255));
               // WordToDrawLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
               // WordToDraw.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            }
            if (mode == "Dark")
            {
                // White
                //Dashboard.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
              //  WordToDrawLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
              //  WordToDraw.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

    }
}
