using PolyPaint.Server;
using PolyPaint.Services;
using PolyPaint.Utils;
using PolyPaint.Vues;
using PolyPaint.Vues.Account;
using PolyPaint.Vues.Chat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolyPaint.VueModels
{
    class MainMenusVueModele : INotifyPropertyChanged
    {
        public RelayCommand<object> DisconnectCommand { get; set; }
        // ce element est la guide de mock
        public List<string> ChannelList { get; set; }
        public RelayCommand<object> NewChannelCommand { get; set; }

        public MainMenusVueModele()
        {
            DisconnectCommand = new RelayCommand<object>(Disconnect);
            IsVisible = false;

            WebSocket.Instance.Login += OnLogin;
            WebSocket.Instance.Logout += OnLogout;
            WebSocket.Instance.ReceivedDeconnexion += new WebSocket.ReceivedDeconnexionEventHandler(DiconnectFromSocket);
        }

        private void OnLogin()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                IsVisible = true;
            });
        }

        private void OnLogout()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                IsVisible = false;
            });
        }

        private void DiconnectFromSocket()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                Disconnect(new { });
            });
        }

        public void Disconnect(object o)
        {
            Authentification.SignOut();
            MainTransitions.TransitionPageControl.ShowPage(new LoginView());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isVisible { get; set; }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                _isVisible = value;
                NotifyPropertyChanged();
            }
        }

        

    }
}
