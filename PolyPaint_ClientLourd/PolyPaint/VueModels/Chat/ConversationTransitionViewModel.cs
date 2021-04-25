using PolyPaint.Models;
using PolyPaint.Server;
using PolyPaint.Services;
using PolyPaint.Vues;
using PolyPaint.Vues.Chat;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PolyPaint.VueModels.Chat
{

   
    class ConversationTransitionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
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


        private UserControl currentView;

        public ConversationTransitionViewModel()
        {
            if (GameService.Instance.IsInGame)
                CurrentView = new ChatView();
            else
            CurrentView = new ConversationView();

            MessagingService.CurrentSelectionChangedEvent += LoadMessages;
            ChatViewModel.GoBackToConversationViewEventHandler += ShowConversationView;
            CreateChannelViewModel.TaskFinishedEvent += ShowConversationView;
            GameService.Instance.CurrentGameUpdated += OnGameUpdated;
            WebSocket.Instance.ReceivedGameEnded += OnGameEnded;
        }

        public void OnGameEnded(string message)
        {
            ShowConversationView();
        }

        public void OnGameUpdated()
        {
            if (GameService.Instance.IsInGame)
            {
                WebSocket.Instance.GetConversation(GameService.Instance.CurrentGame.CID );
            }
            else
            {
                ShowConversationView();
            }
        }


        private void ShowConversationView()
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate

            {
                if (CurrentView is ConversationView)
                    return;

                CurrentView = new ConversationView();
            });
        }

        private void ShowChatView()
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate

            {
                if (CurrentView is ChatView)
                    return;

                CurrentView = new ChatView();
            });
        }

        private void LoadMessages(Conversation conversation)
        {
            ShowChatView();
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

   
}
