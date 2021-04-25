using PolyPaint.Models;
using PolyPaint.Server;
using PolyPaint.Services;
using PolyPaint.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.VueModels.Chat.Dialogs
{
    class AddUserChatDialogVueModel : INotifyPropertyChanged
    {
        public ObservableCollection<User> Users { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public RelayCommand<object> AddUserRelayCommand { get; set; }

        private readonly HashSet<string> userInvitedUIDS;

        public string UserFilter
        {
            get
            {
                return userFilter;
            }
            set
            {
                if (userFilter == value)
                    return;

                userFilter = value;
                RefreshUser();
            }
        }
        private string userFilter;

        public AddUserChatDialogVueModel()
        {
            Users = new ObservableCollection<User>();
            AddUserRelayCommand = new RelayCommand<object>(AddUserToConversation);
            UserFilter = string.Empty;
            userInvitedUIDS = new HashSet<string>();

            WebSocket.Instance.ReceivedAllOnlineUsers += OnOnlineUserReceived;
            WebSocket.Instance.GetAllConnectedUsers();
        }

        public void RefreshUser()
        {
            WebSocket.Instance.GetAllConnectedUsers();
        }


        public void AddUserToConversation(object o)
        {
            if (o != null)
            {
                if (o is User user)
                {
                    userInvitedUIDS.Add(user.UID);
                    MessagingService.Instance.InviteUserToConversation(user);
                    RefreshUser();
                }

            }
        }

        public void OnOnlineUserReceived(List<User> users)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                Users.Clear();
                foreach (var user in users)
                {
                    if(user.Username.Contains(UserFilter) || string.IsNullOrEmpty(user.Username))
                        if (!MessagingService.Instance.CurrentConversation.UIDS.Contains(user.UID) && !userInvitedUIDS.Contains(user.UID))
                             Users.Add(user);
                }
            });
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
