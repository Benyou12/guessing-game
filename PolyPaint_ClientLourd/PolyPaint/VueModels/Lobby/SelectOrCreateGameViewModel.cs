using PolyPaint.AppContext;
using PolyPaint.Models.Lobby;
using PolyPaint.Server;
using PolyPaint.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PolyPaint.VueModels.Lobby
{
    class SelectOrCreateGameViewModel
    {
        public RelayCommand<object> JoinCommand { get; set; }
        public RelayCommand<object> CreateNewGameCommand { get; set; }

        public ObservableCollection<Game> Games { get; set; }
        private readonly Dictionary<string,Game> GamesByID;
        private bool joinGameClick;

        public delegate void GameJoined();
        public static GameJoined OnGameJoined = delegate { };

        public SelectOrCreateGameViewModel()
        {
            IsLoading = true;
            HasNoGames = false;
            JoinCommand = new RelayCommand<object>(JoinGame);
            CreateNewGameCommand = new RelayCommand<object>(CreateNewGame);

            Games = new ObservableCollection<Game>();
            GamesByID = new Dictionary<string,Game>();

            joinGameClick = false;

            InitEventListeners();
            GetAtiveGames();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading == value) return;
                _isLoading = value;
                NotifyPropertyChanged();
            }
        }

        private bool _noGames;
        public bool HasNoGames
        {
            get { return _noGames; }
            set
            {
                if (_noGames == value) return;
                _noGames = value;
                NotifyPropertyChanged();
            }
        }


        private void GetAtiveGames()
        {
            WebSocket.Instance.GetActiveGames();
        }

        private void InitEventListeners()
        {
            WebSocket.Instance.ReceivedActiveGames += new WebSocket.ReceivedActiveGamesEventHandler(ReceivedActiveGames);
            WebSocket.Instance.ReceivedCreateNewGame += new WebSocket.ReceivedCreateNewGameEventHandler(ReceivedCreateNewGame);
        }

        private void ReceivedOnJoinGame(Game game)
        {
            if (joinGameClick){
                joinGameClick = false;
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    AppContextSingleton.Instance.AppContext.CurrentGame = game;
                });
            }
            else {
                SetGamesList(game);
            }
        }

        private void ReceivedCreateNewGame(Game game)
        {
            HasNoGames = false;
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                SetGamesList(game);
            });
        }

        private void ReceivedActiveGames(List<Game> games)
        {
            IsLoading = false;
            HasNoGames = games.Count == 0;
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                Games.Clear();
                GamesByID.Clear();    

                foreach (var game in games)
                    SetGamesList(game);
            });
        }

        private void JoinGame(object o)
        {
            if (o != null)
            {
                if (o is Game game)
                {
                    joinGameClick = true;
                    WebSocket.Instance.JoinGame(AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID, game.GID);
                }

            }
        }

        private void CreateNewGame(object o)
        {
            WebSocket.Instance.CreateNewGame(AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID);
        }

        private void SetGamesList(Game game)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                if (game == null)
                    return;

                game.SetPlayers();

                if (GamesByID.ContainsKey(game.GID))
                {
                    GamesByID[game.GID].Copy(game);
                }
                else
                {
                    Games.Add(game);
                    GamesByID.Add(game.GID, game);
                }
            });
        }

    }

  
}
