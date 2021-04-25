using PolyPaint.AppContext;
using PolyPaint.Vues.BuildGame;
using PolyPaint.Vues.DrawingCanvas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace PolyPaint.VueModels.BuildGame
{
    public class BuildGameTransitionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly List<UserControl> pages;
        private UserControl currentView;
        private int currentViewIndex;

        public UserControl CurrentView
        {
            get { return currentView; }
            set
            {
                if (currentView == value)
                    return;
                currentView = value;
                NotifyPropertyChanged();
            }
        }

        public BuildGameTransitionViewModel()
        {
            DrawingServiceSingleton.Instance.DrawingService.IsCreatingGame = true;

            pages = new List<UserControl>()
            {
                new DrawingCanvasWindow(),
                new GuesserCanvasWindow(),
                new GoogleSearchView()
            };

            currentViewIndex = 0;
            CurrentView = pages[currentViewIndex];

            BuildGameViewModel.OnPreviousViewCommand += GoPrevious;
            BuildGameViewModel.OnNextViewCommand += GoNext;
            BuildGameViewModel.OnGuesserCanvasWindowView += GoToGuesserCanvasWindow;
            GoogleSearchViewModel.OnGoogleSearchView += GoToGoogleSearchView;
            BuildGameViewModel.OnDrawView += GoToDrawView;
        }

        private void GoToGuesserCanvasWindow()
        {
            currentViewIndex = 1;
            CurrentView = pages[currentViewIndex];
        }

        private void GoToGoogleSearchView()
        {
            currentViewIndex = 2;
            CurrentView = pages[currentViewIndex];
        }

        private void GoToDrawView()
        {
            currentViewIndex = 0;
            CurrentView = pages[currentViewIndex];
        }

        private void GoNext()
        {
            currentViewIndex = currentViewIndex == (pages.Count - 1) ? 0 : ++currentViewIndex;
            CurrentView = pages[currentViewIndex];
        }

        private void GoPrevious()
        {
            currentViewIndex = currentViewIndex == 0 ? (pages.Count - 1) : --currentViewIndex;
            CurrentView = pages[currentViewIndex];
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
