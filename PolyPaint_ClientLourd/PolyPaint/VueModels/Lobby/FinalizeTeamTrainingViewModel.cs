using PolyPaint.AppContext;
using PolyPaint.Models;
using PolyPaint.Models.Lobby;
using PolyPaint.Server;
using PolyPaint.Server.SocketHelpers;
using PolyPaint.Services;
using PolyPaint.Utils;
using PolyPaint.Vues;
using PolyPaint.Vues.Account;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PolyPaint.VueModels.Lobby
{
    class FinalizeTeamTrainingViewModel: INotifyPropertyChanged
    {
        public RelayCommand<object> DisconnectCommand { get; set; }
        public RelayCommand<object> StartTheGameCommand { get; set; }
        public RelayCommand<object> LeaveLobbyCommand { get; set; }
        public RelayCommand<object> AddVirtualPlayerCommand { get; set; }
        private bool canUserStartGame;
        public bool CanUserStartGame
        {
            get { return canUserStartGame; }
            set
            {
                if (canUserStartGame == value)
                    return;
                canUserStartGame = value;
                NotifyPropertyChanged();
            }
        }

        private bool _canAddVirtual;
        public bool CanAddVirtual
        {
            get { return _canAddVirtual; }
            set
            {
                if (_canAddVirtual == value) return;
                _canAddVirtual = value;
                NotifyPropertyChanged();
            }
        }

        private bool _loadingGame;
        public bool IsLoadingGame
        {
            get { return _loadingGame; }
            set
            {
                if (_loadingGame == value) return;
                _loadingGame = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public Game CurrentGame { get { return GameService.Instance.CurrentGame; } }

        public delegate void StartGameEventHadler();
        public static StartGameEventHadler OnGameStarted = delegate { };

        public FinalizeTeamTrainingViewModel()
        {
            StartTheGameCommand = new RelayCommand<object>(StartTheGame);
            AddVirtualPlayerCommand = new RelayCommand<object>(AddVirtualPlayer);
            LeaveLobbyCommand = new RelayCommand<object>(LeaveLobby);

            InitEventListeners();
            UpdateGame();
        }

        private void InitEventListeners()
        {
            GameService.Instance.CurrentGameUpdated += new GameService.UpdateGameEventHandler(UpdateGame);
        }

        private void UpdateGame()
        {
            if (GameService.Instance.CurrentGame.IsReady)
            {
                if (CurrentGame.Players.Count >= 2 && CurrentGame.Players.Count < 4)
                {
                    if(CurrentGame.TeamOne.PlayerOne.PID == GameService.Instance.CurrentPlayer?.PID)
                        CanAddVirtual = true;
                }

                if (CurrentGame.Players.Count == 4)
                {
                    CanAddVirtual = false;
                    if (CurrentGame.TeamOne.PlayerOne.PID == GameService.Instance.CurrentPlayer?.PID)
                        CanUserStartGame = true;
                }
                
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void StartTheGame(object o)
        {
            IsLoadingGame = true;
            GameService.Instance.StartGame();
        }

        private void LeaveLobby(object o)
        {
            GameService.Instance.LeaveCurrentGame();
        }

        private void AddVirtualPlayer(object o)
        {
            GameService.Instance.AddVirtualPlayer();
        }

       

    }
}
