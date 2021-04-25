using PolyPaint.AppContext;
using PolyPaint.Models;
using PolyPaint.Models.Socket;
using PolyPaint.Server;
using PolyPaint.Utils;
using PolyPaint.Utils.Localization;
using PolyPaint.Vues.Chat;
using PolyPaint.Vues.Chat.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolyPaint.Services
{
    public sealed class MessagingService : INotifyPropertyChanged, IService
    {
        private static readonly Lazy<MessagingService>
            lazy = new Lazy<MessagingService>(() => new MessagingService());

        public static MessagingService Instance { get { return lazy.Value; } }

        public event PropertyChangedEventHandler PropertyChanged;
        public delegate void CurrentSelectionChanged(Conversation conversation);
        public static CurrentSelectionChanged CurrentSelectionChangedEvent = delegate { };
        public ObservableCollection<Message> Messages { get; }
        private readonly HashSet<string> messageUIDS;
        private readonly HashSet<string> _joinedConversation;
        public  Conversation conversation;
        public Conversation CurrentConversation { get
            {
                return conversation;
            }
            set
            {
                if (conversation == value )
                    return;

                conversation = value;

                if (value == null)
                    return;

                LoadConversationMessages(value);
                
            }
        }

        public ObservableCollection<Conversation> Conversations { get; set; }
        public Dictionary<string, Conversation> ConversationByCID { get; set; }
        private readonly Dictionary<string, bool> visibilityByConversationID;

        private string conversationFilter;
        public string ConversationFilter
        {
            get { return conversationFilter; }
            set
            {
                if (conversationFilter == value)
                    return;

                if (value == string.Empty)
                {
                    conversationFilter = value;
                    GetUserConversations();
                }
                else
                {

                    conversationFilter = value;
                    GetAllConversations();
                }

            }
        }

        private bool _isChatDetached { get; set; }

        public bool IsChatDetached
        {
            get { return _isChatDetached; }
            set
            {
                if (_isChatDetached == value)
                    return;
                _isChatDetached = value;
                NotifyPropertyChanged();
            }
        }

        public MessagingService()
        {
            Messages = new ObservableCollection<Message>();
            messageUIDS = new HashSet<string>();
            Conversations = new ObservableCollection<Conversation>();
            ConversationByCID = new Dictionary<string, Conversation>();
            visibilityByConversationID = new Dictionary<string, bool>();
            ConversationFilter = string.Empty;
            _joinedConversation = new HashSet<string>();
        }

        public void Initialize()
        {
            
            InitEventListeners();
        }

        public void Terminate()
        {
            Messages.Clear();
            messageUIDS.Clear();
            Conversations.Clear();
            ConversationByCID.Clear();
            visibilityByConversationID.Clear();
            ConversationFilter = string.Empty;
        }

        public bool CanShowUserHistory()
        {
            if (!visibilityByConversationID.ContainsKey(CurrentConversation.CID))
                return false;
            return !visibilityByConversationID[CurrentConversation.CID];
        }

        private void InitEventListeners()
        {
            WebSocket.Instance.ReceivedMessages += new WebSocket.ReceivedMessagesEventHandler(OnReceivedConversationMessages);
            WebSocket.Instance.ReceivedNewMessage += new WebSocket.ReceivedNewMessageEventHandler(OnReceivedNewMessage);
            WebSocket.Instance.ReceivedUpdatedConversation += new WebSocket.ReceivedUpdatedConversationEventHandler(LoadConversationMessages);

            WebSocket.Instance.ReceivedConversations += new WebSocket.ReceivedConversationsEventHandler(ReceivedUserConversations);
            WebSocket.Instance.ReceivedConversationInvitation += new WebSocket.ReceivedConversationInvitationEventHandler(OnConversationInviteReceived);
            WebSocket.Instance.ReceivedNewConversation += new WebSocket.ReceivedConversationEventHandler(OnReceivedNewConversation);
            WebSocket.Instance.ReceivedConversationSearch += new WebSocket.ReceivedConversationsSearchEeventHandler(OnReceivedAllConversations);
            //WebSocket.Instance.ReceivedUpdatedConversation += new WebSocket.ReceivedUpdatedConversationEventHandler(OnReceivedNewConversation);
            WebSocket.Instance.ReceivecOneConversation += SetCurrentConversation;

        }

        public void OnConversationInviteReceived(ConversationInviteResponse conversationInviteResponse)
        {
            var message = string.Format(TranslationSource.Instance["ConversationInvitationMessage"],conversationInviteResponse.User.Username,conversationInviteResponse.ConvName);
            var confirmBtnMsg = TranslationSource.Instance["ConfirmBtnContent"];
            var denyBtnMesg = TranslationSource.Instance["DenyBtnContent"];

            Application.Current.Dispatcher.BeginInvoke((Action)delegate {

                var dlg = new ChatGenericDialogBoxVue(message, false, confirmBtnMsg, denyBtnMesg);

                if (dlg.ShowDialog() == true)
                {
                    JoinConversartion(conversationInviteResponse.Conversation);
                }
            });
        }

        public void SetCurrentConversation(Conversation conversation)
        {
            CurrentConversation = conversation;
        }

        public void ReceivedUserConversations(List<Conversation> conversations)
        {
            App.Current.Dispatcher.Invoke((Action)delegate

            {
                CurrentConversation = null;
                ConversationFilter = string.Empty;
                Conversations.Clear();
                ConversationByCID.Clear();

                foreach (var conversation in conversations)
                {
                    AddConversationToView(conversation);
                }

                
            });
        }

        public void GetUserConversations()
        {
            var user = AppContextSingleton.Instance.AppContext.CurrentConnectedUser;
            WebSocket.Instance.GetAllUserConversations(user.UID);
        }

        public void OnReceivedAllConversations(List<Conversation> conversations)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate

            {
                CurrentConversation = null;
                Conversations.Clear();
                ConversationByCID.Clear();

                var filteredConverstions = conversations.Where(w => w.ConvName.ToLower().Contains(ConversationFilter.ToLower()));

                foreach (var conversation in filteredConverstions)
                {
                    AddConversationToView(conversation);
                }
            });
        }


        public void GetAllConversations()
        {
            App.Current.Dispatcher.Invoke((Action)delegate

            {
                Conversations.Clear();
                ConversationByCID.Clear();
                CurrentConversation = null;
            });

            
            WebSocket.Instance.GetAllConversations();
        }

        public void OnReceivedNewConversation(Conversation conversation)
        {
            AddConversationToView(conversation);    
        }


        public void OnReceivedConversationMessages(List<Message> messages)
        {

            foreach (var message in messages)
            {
                if (message.Timestamp > AppContextSingleton.Instance.AppContext.CurrentConnectedUser.LatestAuthenticationStats.TimeStamp 
                    || (visibilityByConversationID[CurrentConversation.CID] ))
                {
                    AddMessageToView(message);
                }
                
            }

        }

        public void ShowHistory()
        {
            visibilityByConversationID[CurrentConversation.CID] = true;
            LoadConversationMessages(CurrentConversation);
        }


        public void LoadConversationMessages(Conversation conversation)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate
            {
                messageUIDS.Clear();
                Messages.Clear();
                conversationFilter = string.Empty;
            });

            if (conversation == null || conversation.UpdateAction == ConversationUpdate.UserRemoved)
            {
                GetUserConversations();
                return;
            }
            if (conversation.UIDS.Contains(AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID))
            {
                if (!visibilityByConversationID.ContainsKey(conversation.CID))
                    visibilityByConversationID.Add(conversation.CID, false);

                WebSocket.Instance.GetConversationMessages(conversation.CID);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke((Action)delegate {

                    var message = TranslationSource.Instance["JoinConversationMessage"];
                    var joinBtnMsg = TranslationSource.Instance["JoinChannelDialogJoin"];
                    var denyBtnMesg = TranslationSource.Instance["DenyBtnContent"];

                    //This is not legit, sorry brams
                    if (_joinedConversation.Contains(conversation.CID))
                        return;
                    var dlg = new ChatGenericDialogBoxVue(message, false, joinBtnMsg, denyBtnMesg );

                    if (dlg.ShowDialog() == true)
                    {
                        JoinConversartion(conversation);
                    }
                });
            }

            //CurrentConversation = conversation;
            CurrentSelectionChangedEvent.Invoke(CurrentConversation);
        }

        public void OnReceivedNewMessage(NewMessage message)
        {
            AddMessageToView(message.Message);

            if (message.Message.User.Username == "moderator")
            {
                System.Media.SoundPlayer Moderator = new System.Media.SoundPlayer(@"..\..\Resources\Sound\Moderator.wav");
                Moderator.Play();
            }
        }

        public void SendMessage(string message)
        {
            WebSocket.Instance.CreateConversationMessage(CurrentConversation.CID, AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID,
                message);
        }

        public void JoinConversartion(Conversation conversation)
        {
            if (!_joinedConversation.Contains((conversation.CID)))
                _joinedConversation.Add((conversation.CID));
            CurrentConversation = conversation;
            WebSocket.Instance.AddUserToConversation(conversation.CID, AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID);
        }

        public void InviteUserToConversation(User user)
        {
            WebSocket.Instance.InviteUserToConversation(CurrentConversation.CID, user.UID);
        }

        public void LeaveConversation()
        {
            if(CurrentConversation != null)
                WebSocket.Instance.LeaveConversation(CurrentConversation.CID, AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID);
        }

        public void CreateConversation(string newConversationName)
        {
            WebSocket.Instance.CreateConversation(new string[] { AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID }, newConversationName);
        }

        private void AddMessageToView(Message message)
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate

            {

                if (messageUIDS.Contains(message.MID))
                {
                    return;
                }
                else
                {
                    messageUIDS.Add(message.MID);
                    Messages.Add(message);
                }

            });
        }

        private void AddConversationToView(Conversation conversation)
        {
            if (ConversationByCID.ContainsKey(conversation.CID))
            {
                ConversationByCID[conversation.CID].UIDS = conversation.UIDS;
                if (conversation.Equals(CurrentConversation))
                {
                    CurrentConversation = ConversationByCID[conversation.CID];
                }
            }

            else
            {
                ConversationByCID.Add(conversation.CID, conversation);

                App.Current.Dispatcher.Invoke((Action)delegate

                {
                    Conversations.Add(conversation);

                });
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
