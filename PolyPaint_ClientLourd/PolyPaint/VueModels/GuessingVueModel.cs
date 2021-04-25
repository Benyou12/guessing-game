using PolyPaint.AppContext;
using PolyPaint.Models;
using PolyPaint.Models.Game;
using PolyPaint.Models.Lobby;
using PolyPaint.Server;
using PolyPaint.Services;
using PolyPaint.Utils;
using PolyPaint.VueModels.BuildGame;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;

namespace PolyPaint.VueModels
{
    class GuessingVueModel : INotifyPropertyChanged
    {
        public Game CurrentGame { get { return GameService.Instance.CurrentGame; } }
        public Team TeamOne { get { return CurrentGame.TeamOne; } }
        public Team TeamTwo { get { return CurrentGame.TeamTwo; } }
        public Round NextRound { get { return GameService.Instance.CurrentGame.Rounds.Last(); } }

        private Editor editor = new Editor();
        private DrawingService drawingService;
        public StrokeCollection Lines { get; set; } 

        private Timer Timer { get; set; }
        private int Index { get; set; }
        private string userGuessWordInput;

        public RelayCommand<object> SendGuessCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public string UserGuessWordInput
        {
            get { return userGuessWordInput; }
            set
            {
                if (userGuessWordInput == value)
                    return;

                userGuessWordInput = value;
                NotifyPropertyChanged();
            }
        }

        private bool canUserGuess;

        public bool CanUserGuess
        {
            get
            {
                return canUserGuess;
            }
            set
            {
                if (canUserGuess == value)
                    return;

                canUserGuess = value;
                NotifyPropertyChanged();
            }
        }

        private readonly string GuesserString = "GUESSER";

        public GuessingVueModel()
        {

            drawingService = DrawingServiceSingleton.Instance.DrawingService.IsCreatingGame ? DrawingServiceSingleton.Instance.DrawingService : new DrawingService();

            
            if (!DrawingServiceSingleton.Instance.DrawingService.IsCreatingGame)
            {
                //If the user is not creating a game use new editor
                drawingService.Editor = editor;
                drawingService.Lines = editor.lines;
                Lines = drawingService.Lines;
            }
            else
            {
                //If the user is creating a game, use the same editor and lines
                editor = drawingService.Editor;
                Lines = drawingService.Lines;
            }

            CanUserGuess = GameService.Instance.CurrentPlayerTeam?.ID == GameService.Instance.CurrentGame?.CurrentRound?.TeamID;
            CanUserGuess = GameService.Instance.CurrentPlayer?.Role == GuesserString;

            SendGuessCommand = new RelayCommand<object>(SendGuess);
            WebSocket.Instance.ReceivedNewStroke += Redraw;
            GameService.Instance.CurrentGameUpdated += UpdateViewOnGameChanged;

            BuildGameViewModel.OnShowPreviewDrawing += ShowPreview;
        }

        private void ShowPreview()
        {
            if (drawingService.ListOfStrokes.Count <= 0 || !drawingService.IsCreatingGame) return;

            Lines.Clear();
            Index = 0;
            Timer = new Timer();

            if(drawingService.ListOfStrokes.Count <= 10)
                Timer.Interval = 500;
            else if (drawingService.ListOfStrokes.Count <= 100)
                Timer.Interval = 100;
            else
                Timer.Interval = 10;

            Timer.Elapsed += SendStrokeAfterTime;
            Timer.Start();
        }

        public void UpdateViewOnGameChanged()
        {
            CanUserGuess = GameService.Instance.CurrentPlayer.Role == GuesserString;
        }

        private void SendGuess(object sender)
        {
            if (string.IsNullOrEmpty(UserGuessWordInput))
                return;

            GameService.Instance.SendGuess(UserGuessWordInput);
            UserGuessWordInput = string.Empty;
        }


        private void SendStrokeAfterTime(object sender, ElapsedEventArgs e)
        {
            if(drawingService.ListOfStrokes.Count > Index)
            {
                Redraw(drawingService.ListOfStrokes.ElementAt(Index));
                Index++;
            }
            else
            {
                Timer.Dispose();
            }
        }

        private void Redraw(Path path)
        {
            drawingService.Redraw(path);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
