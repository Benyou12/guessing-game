using PolyPaint.AppContext;
using PolyPaint.VueModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PolyPaint.Vues.DrawingCanvas
{
    /// <summary>
    /// Interaction logic for GuesserCanvasWindow.xaml
    /// </summary>
    public partial class GuesserCanvasWindow : UserControl
    {
        public GuesserCanvasWindow()
        {
            DataContext = new GuessingVueModel();
            InitializeComponent();
            //DataContext = new VueModel();
            ResizeInkCanvas();

            // Events
            SizeChanged += OnWindowSizeChanged;
            surfaceDessin.IsHitTestVisible = false;

            if (DrawingServiceSingleton.Instance.DrawingService.IsCreatingGame)
                guessInputStack.Visibility = Visibility.Collapsed;

            if (DrawingServiceSingleton.Instance.DrawingService.IsCreatingGame)
            {
                TopBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                TopBar.Visibility = Visibility.Visible;
            }
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
    }
}
