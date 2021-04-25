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
    class EndGameViewModel : INotifyPropertyChanged
    {
        public static int _numberOfOpeneConfettis = 0;
        public Models.Lobby.Game CurrentGame { get { return GameService.Instance.CurrentGame; } }
        public Team TeamOne { get { return CurrentGame.TeamOne; } }
        public Team TeamTwo { get { return CurrentGame.TeamTwo; } }
        private bool canUserStartGame;
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public EndGameViewModel()
        {
            var bestTeam = TeamOne.Score > TeamTwo.Score ? TeamOne : TeamTwo;
            var currentPlayerTeam = GameService.Instance.CurrentPlayerTeam;
            if (TeamOne.Score == TeamTwo.Score)
                IsEquality = true;

            if (bestTeam.ID == currentPlayerTeam.ID && !IsEquality)
            {
                HasWon = true;
                Window w1 = new Confettis();
                w1.Height = SystemParameters.PrimaryScreenHeight; ;
                w1.Width = SystemParameters.PrimaryScreenWidth;
                if(_numberOfOpeneConfettis > 0 )
                    return;
                _numberOfOpeneConfettis += 1;
                w1.Show();
                w1.Closing += W1_Closing;
            }
            else
            {
                if(!IsEquality)
                    HasLost = true;
            }
        }

        private void W1_Closing(object sender, CancelEventArgs e)
        {
            _numberOfOpeneConfettis -= 1;
        }

        private bool _hasWon;
        public bool HasWon
        {
            get { return _hasWon; }
            set
            {
                if (_hasWon == value) return;
                _hasWon = value;
                NotifyPropertyChanged();
            }
        }

        private bool _hasLost;
        public bool HasLost
        {
            get { return _hasLost; }
            set
            {
                if (_hasLost == value) return;
                _hasLost = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isEquality;
        public bool IsEquality
        {
            get { return _isEquality; }
            set
            {
                if (_isEquality == value) return;
                _isEquality = value;
                NotifyPropertyChanged();
            }
        }       

    }
}
