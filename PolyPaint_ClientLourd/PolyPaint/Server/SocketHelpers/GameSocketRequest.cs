using PolyPaint.Models.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Server.SocketHelpers
{
    class GameSocketRequest
    {
        public static void CreateGame(string uid)
        {

        }

        public static void AddVirtualPlayer(string gameID)
        {
            var content = new SocketAction
            {
                Route = "game.patch.addVirtual",
                Payload = new { _id = gameID }
            };

            WebSocket.Instance.Emit("action", content);
        }

        public static void JoinGame(string uid, string gid)
        {
            var content = new SocketAction
            {
                Route = "game.patch.join",
                Payload = new { _id = gid, uid }
            };

            WebSocket.Instance.Emit("Action", content);
        }

        public static void GuessWord(string userID,string gameID,string roundID, string word)
        {
            var action = new Models.Game.GameAction()
            {
                UserID = userID,
                GameID = gameID,
                Type = Models.Game.GAME_ACTIONS.GUESS_WORD,
                RoundID = roundID,
                Payload = new Models.Game.ActionWordGuess()
                {
                    Word = word
                }
            };

            var content = new SocketAction
            {
                Route = "game.patch.action",
                Payload = action
            };
            WebSocket.Instance.Emit("action", content);
        }

        public static void StartGame(string gameID, string userID)
        {
            var action = new Models.Game.GameAction()
            {
                UserID = userID,
                GameID = gameID,
                Type = Models.Game.GAME_ACTIONS.START
            };

            var content = new SocketAction
            {
                Route = "game.patch.action",
                Payload = action
            };
            WebSocket.Instance.Emit("action", content);

        }

        public static void LeaveGame(string gid, string uid)
        {
            var content = new SocketAction
            {
                Route = "game.patch.quit",
                Payload = new { _id = gid, uid }
            };

            WebSocket.Instance.Emit("action", content);
        }

            public static void StartRound(string gameID,string userID, string roundID)
        {
                var action = new Models.Game.GameAction()
                {
                    UserID = userID,
                    GameID = gameID,
                    Type = Models.Game.GAME_ACTIONS.START_ROUND,
                    RoundID = roundID
                };

                var content = new SocketAction
                {
                    Route = "game.patch.action",
                    Payload = action
                };
                WebSocket.Instance.Emit("action", content);
            }

        public static void RequestHint(string gameID, string userID, string roundID)
        {
            var action = new Models.Game.GameAction()
            {
                UserID = userID,
                GameID = gameID,
                Type = Models.Game.GAME_ACTIONS.REQUEST_HINT,
                RoundID = roundID
            };

            var content = new SocketAction
            {
                Route = "",
                Payload = action
            };
            WebSocket.Instance.Emit("Action", content);
        }
    }
}
