using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PolyPaint.Models;
using Quobject.SocketIoClientDotNet.Client;
using PolyPaint.Models.Socket;
using PolyPaint.Models.Game;
using PolyPaint.Models.Lobby;
using PolyPaint.AppContext;
using PolyPaint.Models.BuildGame;
using GameImage = PolyPaint.Models.BuildGame.GameImage;
using System.Windows;
using PolyPaint.Vues;
using PolyPaint.Vues.Account;
using PolyPaint.Services;

namespace PolyPaint.Server
{
    /* 
        This is a singleton because the connection must be established only once.
    */

    public class Error {
        public int code { get; set; }
        public string message { get; set; }
    }

    public sealed class WebSocket : IService
    {
        private static readonly Lazy<WebSocket>
            lazy = new Lazy<WebSocket>(() => new WebSocket());

        public static WebSocket Instance { get { return lazy.Value; } }

        private static readonly string SocketURL = "https://log3900.lbacreations.com";
        private static readonly string ActionRoute = "action";
        private Socket socket;
        private bool isConnected = false;
        private bool isInitialized = false;

        private WebSocket()
        {
        }

        public void Terminate()
        {
            Disconnect();
        }

        public void Initialize()
        {
            if (isInitialized == false)
            {
                socket = IO.Socket(SocketURL);
                Connect();
                OnReceiveCanvas();
                OnReceiveMessage();
                OnDisconnect();
                OnAuthLogin();
                OnGameEvents();
                OnPingPongEvents();
                OnAuthEvents();
                socket.On("error", data =>
                {
                    if (isConnected) MessageBox.Show((((JObject)data).ToObject<Error>()).message);
                });
            }
        }

        public void JoinRoom()
        {
            if (!string.IsNullOrEmpty(AppContextSingleton.Instance.AppContext.CurrentConnectedUser?.UID) && isInitialized)
            {
                JoinRoom(AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID);
            }
        }

        private void Connect() {
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                isInitialized = true;
            });
            socket.On(Socket.EVENT_RECONNECT, () =>
            {
                JoinRoom();
            });
        }

        public void OnDisconnect()
        {
            socket.On(Socket.EVENT_DISCONNECT, () =>
            {
                isConnected = false;
                Console.WriteLine("Socket disconnected.");
            });
        }


        public void Disconnect()
        {
            socket.Disconnect();
            socket = IO.Socket(SocketURL);
            isInitialized = false;
            Console.WriteLine("Disconnected the user from the socket");
            while (isConnected) ;
        }

        public void Emit(string eventName, object content)
        {
            var arr = new object[] { JsonConvert.SerializeObject(content) };
            socket.Emit(eventName, arr);
        }

        public void JoinRoom(string room) {
            var content = new JoinRoomRequest() { UID = room };
            Emit("join-room", content);
            socket.On("room-joined", (data) =>
            {
                isConnected = true;
                Console.WriteLine("Joined room on the server: " + data.ToString());
            });
        }

        /*
        * Send event to the server 
        */

        public void GetAllConversations()
        {
            var content = new SocketAction
            {
                Route = "conversation.get.search",
                Payload = new { }
            };
            Emit(ActionRoute, content);
        }

        public void GetAllUserConversations(string uid)
        {
            var content = new SocketAction {
                Route = "conversation.get.all",
                Payload = new { uid }
            };
            Emit(ActionRoute, content);
        }

        public void GetConversation(string cid)
        {
            var content = new SocketAction
            {
                Route = "conversation.get.one",
                Payload = new { cid }
            };
            Emit(ActionRoute, content);
        }

        public void CreateConversation(string[] uids, string convName)
        {
            var content = new SocketAction
            {
                Route = "conversation.post.create",
                Payload = new { uids, convName }
            };
            Emit(ActionRoute, content);
        }

        public void GetConversationMessages(string cid)
        {
            var content = new SocketAction
            {
                Route = "message.get.all",
                Payload = new { cid }
            };
            Emit(ActionRoute, content);
        }

        public void CreateCanvas(List<string> uids)
        {
            var content = new SocketAction
            {
                Route = "canvas.post.create",
                Payload = new
                {
                    uids,
                }
            };
            Emit(ActionRoute, content);
        }

        public void CreateStroke(Path path)
        {
            var content = new SocketAction
            {
                Route = "canvas.patch.stroke",
                Payload = path
            };
            Emit(ActionRoute, content);
        }

        public void CreateConversationMessage(string cid, string uid, string text)
        {
            var content = new SocketAction
            {
                Route = "message.post.create",
                Payload = new {
                    cid,
                    message = new
                    {
                        text,
                        user = new {
                            uid
                        }
                    }
                }
            };
            Emit(ActionRoute, content);
        }

        public void GetAllUsers()
        {
            var content = new SocketAction
            {
                Route = "user.get.all",
                Payload = new { }
            };
            Emit(ActionRoute, content);
        }


        public void AddUserToConversation(string cid, string uid)
        {
            var content = new SocketAction
            {
                Route = "conversation.patch.addUser",
                Payload = new {
                    cid,
                    uid
                }
            };
            Emit(ActionRoute, content);
        }

        public void InviteUserToConversation(string cid, string uid)
        {
            var content = new SocketAction
            {
                Route = "conversation.post.invite",
                Payload = new
                {
                    cid,
                    uid
                }
            };
            Emit(ActionRoute, content);
        }

        public void LeaveConversation(string cid, string uid)
        {
            var content = new SocketAction
            {
                Route = "conversation.patch.removeUser",
                Payload = new
                {
                    cid,
                    uid
                }
            };
            Emit(ActionRoute, content);
        }


        public void GetActiveGames() {
            var content = new SocketAction
            {
                Route = "game.get.active",
                Payload = new { }
            };
            Emit(ActionRoute, content);
        }

        public void CreateNewGame(string uid)
        {
            var content = new SocketAction
            {
                Route = "game.post.create",
                Payload = new { uid }
            };
            Emit(ActionRoute, content);
        }

        public void JoinGame(string uid, string gid)
        {
            var content = new SocketAction
            {
                Route = "game.patch.join",
                Payload = new { _id = gid, uid }
            };
            Emit(ActionRoute, content);
        }

        public void CreateGameImage(GameImage gameImage)
        {
            var content = new SocketAction
            {
                Route = "gameImage.post.create",
                Payload = new {
                    hints = gameImage.Hints,
                    word = gameImage.Word,
                    lang = gameImage.WordLanguage,
                    difficulty = gameImage.Difficulty,
                    canvas = gameImage.Canvas,
                    drawing_mode = gameImage.DrawingMode
                }
            };
            Emit(ActionRoute, content);
        }

        public void GetAllConnectedUsers()
        {
            var content = new SocketAction
            {
                Route = "user.get.online",
                Payload = { }
            };

            Emit(ActionRoute, content);
        }

        public void RefereshUser()
        {
            if (isConnected && AppContext.AppContextSingleton.Instance.AppContext?.CurrentConnectedUser != null)
            {
                var content = new SocketAction
                {
                    Route = "user.get.one",
                    Payload = new { uid = AppContext.AppContextSingleton.Instance.AppContext?.CurrentConnectedUser.UID }
                };

                Emit(ActionRoute, content);
            }
        }

        /*
        * Receive events from the server 
        */

        //conversation.search
        public delegate void ReceivedUpdatedConversationEventHandler(Conversation conversation);
        public event ReceivedUpdatedConversationEventHandler ReceivedUpdatedConversation;

        //conversation.search
        public delegate void ReceivedConversationsSearchEeventHandler(List<Conversation> conveersations);
        public event ReceivedConversationsSearchEeventHandler ReceivedConversationSearch;

        // conversation.all
        public delegate void ReceivedConversationsEventHandler(List<Conversation> conversations);
        public event ReceivedConversationsEventHandler ReceivedConversations;

        // conversation.one & conversation.new
        public delegate void ReceivedConversationEventHandler(Conversation conversation);
        public event ReceivedConversationEventHandler ReceivedNewConversation;

        public delegate void ReceivedOneConversationEventHandler(Conversation conversation);
        public event ReceivedOneConversationEventHandler ReceivecOneConversation;

        // message.all
        public delegate void ReceivedMessagesEventHandler(List<Message> messages);
        public event ReceivedMessagesEventHandler ReceivedMessages;

        // message.new
        public delegate void ReceivedNewMessageEventHandler(NewMessage message);
        public event ReceivedNewMessageEventHandler ReceivedNewMessage;

        // user.conversation
        public delegate void ReceivedConversationUsersEventHandler(List<User> users);
        public event ReceivedConversationUsersEventHandler ReceivedConversationUsers;

        // canvas.updated
        public delegate void ReceivedUpdatedCanvasEventHandler(Path path);
        public event ReceivedUpdatedCanvasEventHandler ReceivedNewStroke = delegate { };

        public delegate void ReceivedUserInformationEventHandler(OAuthLoginResponse user);
        public event ReceivedUserInformationEventHandler ReceivedUserInformation = delegate { };

        public delegate void ReceivedAllOnlineUsersEventHandler(List<User> users);
        public event ReceivedAllOnlineUsersEventHandler ReceivedAllOnlineUsers = delegate { };

        public delegate void ReceivedConversationInvitationEventHandler(ConversationInviteResponse invitationResponse);
        public event ReceivedConversationInvitationEventHandler ReceivedConversationInvitation = delegate { };

        public delegate void ReceivedAllUsersEventHandler(List<User> users);
        public event ReceivedAllUsersEventHandler ReceivedAllUsers = delegate { };

        public delegate void ReceivedUserEventHandler(User user);
        public event ReceivedUserEventHandler ReceivedUser = delegate { };

        private void OnAuthLogin()
        {
            socket.On("auth.oauth", data =>
            {
                ReceivedUserInformation(((JObject)data).ToObject<OAuthLoginResponse>());
            });
        }


        public void OnReceiveCanvas()
        {
            socket.On("canvas.stroke.updated", data =>
            {
                Console.WriteLine("received new stroke");
                if (isConnected) ReceivedNewStroke(((JObject)data).ToObject<Path>());
            });

        }

        public void OnReceiveGameEvent()
        {

        }

        public void OnReceiveMessage()
        {

            socket.On("conversation.all", data =>
            {
                Console.WriteLine(data.GetType());
                if (isConnected) ReceivedConversations(((JArray)data).ToObject<List<Conversation>>());
            });
            socket.On("conversation.one", data =>
            {
                if (isConnected) ReceivecOneConversation(((JObject)data).ToObject<Conversation>());
            });
            socket.On("conversation.new", data =>
            {
                if (isConnected) ReceivedNewConversation(((JObject)data).ToObject<Conversation>());
            });
            socket.On("message.all", data =>
            {
                if (isConnected) ReceivedMessages(((JArray)data).ToObject<List<Message>>());
            });
            socket.On("message.new", data =>
            {
                Console.WriteLine(data.ToString());
                if (isConnected) ReceivedNewMessage(((JObject)data).ToObject<NewMessage>());
            });
            socket.On("user.conversation", data =>
            {
                if (isConnected) ReceivedConversationUsers(((JArray)data).ToObject<List<User>>());
            });
            socket.On("conversation.search", data =>
            {
                if (isConnected) ReceivedConversationSearch(((JArray)data).ToObject<List<Conversation>>());
            });
            socket.On("conversation.updated", data =>
            {
                if (isConnected) ReceivedUpdatedConversation(((JObject)data).ToObject<Conversation>());
            });
            socket.On("user.online", data =>
            {
                if (isConnected) ReceivedAllOnlineUsers(((JArray)data).ToObject<List<User>>());
            });

            socket.On("user.all", data =>
            {
                if (isConnected) ReceivedAllUsers(((JArray)data).ToObject<List<User>>());
            });

            socket.On("conversation.invite", data =>
            {
                if (isConnected) ReceivedConversationInvitation(((JObject)data).ToObject<ConversationInviteResponse>());
            });


            socket.On("user.one", data =>
            {
                if (isConnected && data != null) {
                    var user = ((JObject)data).ToObject<User>();
                    if (user != null)
                    {
                        if (user.UserGamification.Badges != null)
                        {
                            AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UserGamification.Badges.Reverse();
                            AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UserGameStatsHistory.Reverse();
                            AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UserAuthStats.Reverse();
                        }

                        AppContext.AppContextSingleton.Instance.AppContext.CurrentConnectedUser = user;

                        ReceivedUser(user);
                    }
                };
            });

        }

        public delegate void ReceivedActiveGamesEventHandler(List<Game> games);
        public event ReceivedActiveGamesEventHandler ReceivedActiveGames = delegate { };

        public delegate void ReceivedCreateNewGameEventHandler(Game game);
        public event ReceivedCreateNewGameEventHandler ReceivedCreateNewGame = delegate { };

        public delegate void ReceivedJoinGameEventHandler(Game game);
        public event ReceivedJoinGameEventHandler ReceivedJoinGame = delegate { };

        public delegate void ReceivedGameImageEventHandler(GameImage gameImage);
        public event ReceivedGameImageEventHandler ReceivedGameImage;

        public delegate void GameEndedEventHandler(string message);
        public event GameEndedEventHandler ReceivedGameEnded = delegate { };

        public void OnGameEvents()
        {
            socket.On("game.active", data =>
            {
                if (isConnected) {
                    ReceivedActiveGames(((JArray)data).ToObject<List<Game>>());
                }
            });
            socket.On("game.created", data =>
            {
                Console.WriteLine("GAME CREATED: " + ((JObject)data).ToObject<Game>().GID);
                if (isConnected) ReceivedCreateNewGame(((JObject)data).ToObject<Game>());
            });
            socket.On("game.updated", data =>
            {
                Console.WriteLine("GAME UPDATED: " + data.ToString());
                if (isConnected) {
                    var test = ((JObject)data).ToString();
                    ReceivedJoinGame(((JObject)data).ToObject<Game>()); 
                }
            });
            socket.On("game.canceled", data =>
            {
                Console.WriteLine("GAME CANCELED: " + data.ToString());
                if (isConnected)
                {
                    // TODO: Afficher un message et g�rer les �v�nements game.deleted et conversation.deleted
                    ReceivedGameEnded(data.ToString());
                    Console.WriteLine("(PLACEHOLDER)" + data.ToString());
                }
            });
            socket.On("gameImage.created", data =>
            {
                Console.WriteLine("GAME IMAGE CREATED: " + data.ToString()); 
                if (isConnected) ReceivedGameImage(((JObject)data).ToObject<GameImage>());
            });
        }

        /**
         * This function keeps the sokect connection alive.
         */
        public void OnPingPongEvents()
        {
            socket.On("ping1", data =>
            {
                socket.Emit("pong1", data);
            });
         
        }

        public delegate void ReceivedDeconnexionEventHandler();
        public event ReceivedDeconnexionEventHandler ReceivedDeconnexion;

        public void OnAuthEvents()
        {
            socket.On("auth.logout", data =>
            {
                ReceivedDeconnexion();
            });
        }

        /* Warning! This is a hack: it's not legit but it works
         i am adding some more dangerous stuff
             */
        public delegate void DetachChatEventHandler();
        public event DetachChatEventHandler DetachChatEvent;

        public void DetachChat()
        {
            if(MainTransitions.TransitionPageControl.CurrentPage is MainMenus)
                DetachChatEvent();
        }

        public delegate void ReceivedLoginEvent();
        public event ReceivedLoginEvent Login;

        public void LoginCompleted()
        {
            Login();
        }

        public delegate void ReceivedLogoutEvent();
        public event ReceivedLogoutEvent Logout;

        public void LogoutCompleted()
        {
            Logout();
        }
    }
}
