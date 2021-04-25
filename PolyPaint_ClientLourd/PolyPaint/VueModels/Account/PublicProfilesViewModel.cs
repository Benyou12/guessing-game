using PolyPaint.AppContext;
using PolyPaint.Models;
using PolyPaint.Server;
using PolyPaint.Utils;
using PolyPaint.Vues;
using PolyPaint.Vues.Account;
using PolyPaint.Vues.Chat;
using PolyPaint.Vues.Lobby;
using System;
using PolyPaint.Vues.Account;
using System.Windows;
using System.Windows.Controls;
using PolyPaint.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace PolyPaint.VueModels.Account
{
    class PublicProfilesViewModel: INotifyPropertyChanged
    {
        public ObservableCollection<User> Users { get; set; }

        public PublicProfilesViewModel()
        {
            Users = new ObservableCollection<User>();
            WebSocket.Instance.ReceivedAllUsers += OnAllUsersReceived;
            WebSocket.Instance.GetAllUsers();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnAllUsersReceived(List<User> users)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            });
        }

    }
}
