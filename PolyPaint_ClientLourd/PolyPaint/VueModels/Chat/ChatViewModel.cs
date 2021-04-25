using PolyPaint.Models;
using PolyPaint.Models.Socket;
using PolyPaint.Server;
using PolyPaint.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PolyPaint.AppContext;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PolyPaint.Services;
using PolyPaint.Vues;
using PolyPaint.Vues.Chat.Dialogs;

namespace PolyPaint.VueModels.Chat
{
    class ChatViewModel : INotifyPropertyChanged
    {
        public RelayCommand<object> SendMessageCommand { get; set; }
        public RelayCommand<object> GoToConversationsCommand { get; set; }
        public RelayCommand<object> AddUserCommand { get; set; }
        public RelayCommand<object> ShowUserHistoryCommand { get; set; }
        public RelayCommand<object> LeaveConversationCommand { get; set; }

        public delegate void GoBackToConversationEvent();
        public static GoBackToConversationEvent GoBackToConversationViewEventHandler = delegate { };

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Message> Messages { get
            {
                return MessagingService.Instance.Messages;
            }
        }
        public Message UserMessage { get; set; }


        public Conversation CurrentConversation
        {
            get { return MessagingService.Instance.CurrentConversation; }

        }

        public bool CanUserLeaveConversation { get; set; }
        public bool CanUserAddOtherPlayer { get; set; }
        public bool CanUserGoBack { get; set; }
        public bool CanShowHistory { get; set; }

        private bool _canUserDetachChat;

        public bool CanUserDetachChat
        {
            get { return _canUserDetachChat; }
            set
            {
                if (_canUserDetachChat == value)
                    return;
                _canUserDetachChat = value;
                NotifyPropertyChanged();
            }
        }

        public ChatViewModel()
        {
            SendMessageCommand = new RelayCommand<object>(SendMessage);
            GoToConversationsCommand = new RelayCommand<object>(GoToConversations);
            AddUserCommand = new RelayCommand<object>(AddUser);
            LeaveConversationCommand = new RelayCommand<object>(LeaveConversation);
            ShowUserHistoryCommand = new RelayCommand<object>(ShowChatHistory);
            CanUserDetachChat = !MessagingService.Instance.IsChatDetached &&
                                MainTransitions.TransitionPageControl.CurrentPage is MainMenus;

            UserMessage = new Message();
            MainTransitions.OnChatDetached += MainTransitions_OnChatDetached;

            InitializeView();

        }

        private void MainTransitions_OnChatDetached()
        {
            CanUserDetachChat = !MessagingService.Instance.IsChatDetached &&
                                MainTransitions.TransitionPageControl.CurrentPage is MainMenus;
        }

        private void InitializeView()
        {
            CanUserAddOtherPlayer = (CurrentConversation?.CID != "general") && (!GameService.Instance.IsInGame);
            CanUserGoBack = !GameService.Instance.IsInGame; 
            CanUserLeaveConversation = (CurrentConversation?.CID != "general") && (!GameService.Instance.IsInGame);
            CanShowHistory = !GameService.Instance.IsInGame && MessagingService.Instance.CanShowUserHistory();
        }

        public void ShowChatHistory(object o)
        {
            MessagingService.Instance.ShowHistory();
        }

        private void AddUser(object o)
        {
            var dlg = new AddUserChatDialog();
            dlg.ShowDialog();
        }

        public void GoToConversations(object o )
        {
            GoBackToConversationViewEventHandler.Invoke();
        }

        public void LeaveConversation(object o)
        {
            MessagingService.Instance.LeaveConversation();
            GoBackToConversationViewEventHandler.Invoke();

        }

        public void SendMessage(object o)
        {
            if (!CanBeSend())
                return;

            MessagingService.Instance.SendMessage(UserMessage.Text);
            ResetTextBox();
        }

        private void ResetTextBox()
        {
            UserMessage.Text = string.Empty;
        }

        private bool CanBeSend()
        {
            return UserMessage.Text.Trim() != "";
        }


        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
