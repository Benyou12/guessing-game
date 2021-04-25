using PolyPaint.AppContext;
using PolyPaint.Models;
using PolyPaint.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.VueModels.Account
{
    class GamificationViewModel : INotifyPropertyChanged
    { 

        private User _user { get; set; }
        public User CurrentConnectedUser
        {
            get {
                return _user;
            }
            set
            {
                _user = value;
                NotifyPropertyChanged();
            }
        }

        public GamificationViewModel()
        {
            _user = AppContext.AppContextSingleton.Instance.AppContext.CurrentConnectedUser;
            WebSocket.Instance.ReceivedUser += OnReceiveUser;

            WebSocket.Instance.RefereshUser();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void OnReceiveUser(User user)
        {
            CurrentConnectedUser = user;
        }
    }
}
