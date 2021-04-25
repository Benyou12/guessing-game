using MaterialDesignThemes.Wpf;
using PolyPaint.AppContext;
using PolyPaint.Models;
using PolyPaint.Models.Game;
using PolyPaint.Server;
using PolyPaint.Services;
using PolyPaint.Utils;
using PolyPaint.Utils.Localization;
using PolyPaint.Vues.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PolyPaint.Vues;

namespace PolyPaint.VueModels.Chat
{
    class ConversationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public delegate void CurrentSelectionChanged(Conversation conversation);
        public static CurrentSelectionChanged CurrentSelectionChangedEvent = delegate { };
        public RelayCommand<object> NewChannelCommand { get; set; }
        public RelayCommand<object> ClearFilterCommand { get; set; }

        public ObservableCollection<Conversation> Conversations {
            get{ return MessagingService.Instance.Conversations; }
        }

        public string ConversationFilter { get { return MessagingService.Instance.ConversationFilter; }
            set
            {
                MessagingService.Instance.ConversationFilter = value;
            }
        }

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

        public ConversationViewModel()
        { 
            NewChannelCommand = new RelayCommand<object>(CreateConversation);
            ClearFilterCommand = new RelayCommand<object>(ClearFilter);
            CanUserDetachChat = !MessagingService.Instance.IsChatDetached &&
                               MainTransitions.TransitionPageControl.CurrentPage is MainMenus;

            GetConversations();
            MainTransitions.OnChatDetached += MainTransitions_OnChatDetached;
        }

        private void MainTransitions_OnChatDetached()
        {
            CanUserDetachChat = !MessagingService.Instance.IsChatDetached &&
                                MainTransitions.TransitionPageControl.CurrentPage is MainMenus;
        }

        public Conversation CurrentConversation
        {
            get { return MessagingService.Instance.CurrentConversation; }
            set
            {
                MessagingService.Instance.CurrentConversation = value;
                NotifyPropertyChanged();
            }
        }

        private void CreateConversation(object o)
        {
            var message = TranslationSource.Instance["CreateNewChannelMessage"];
            var confirmBtnMsg = TranslationSource.Instance["ConfirmBtnContent"];
            var denyBtnMsg = TranslationSource.Instance["DenyBtnContent"];

            var dlg = new ChatGenericDialogBoxVue(message, true, confirmBtnMsg, denyBtnMsg);
            dlg.ShowDialog();
        }

        private void ClearFilter(object o)
        {
            ConversationFilter = string.Empty;
            GetConversations();
        }

        private void GetConversations()
        {
            if (MessagingService.Instance.IsChatDetached)
                return ;

            CurrentConversation = null; 
            MessagingService.Instance.GetUserConversations();
        }

        private void SetDetachedChat()
        {
            CanUserDetachChat = !MessagingService.Instance.IsChatDetached;
        }


        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
