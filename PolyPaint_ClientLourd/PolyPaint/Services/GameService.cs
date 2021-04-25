using PolyPaint.AppContext;
using PolyPaint.Models.Lobby;
using PolyPaint.Server;
using PolyPaint.Server.SocketHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolyPaint.Services
{
    public sealed class GameService : IService
    {
        private static readonly Lazy<GameService>
            lazy = new Lazy<GameService>(() => new GameService());

        public static GameService Instance { get { return lazy.Value; } }
        public Game CurrentGame { get; set; }
        public bool IsInGame { get; set; }
        public Player CurrentPlayer { get; set; }
        public Team CurrentPlayerTeam { get; set; }

        public delegate void UpdateGameEventHandler();
        public event UpdateGameEventHandler CurrentGameUpdated;

        public GameService()
        {
            CurrentGame = new Game();
            IsInGame = false;
        }

        public void InitEventListeners()
        {
            WebSocket.Instance.ReceivedJoinGame += UpdateCurrentGame;
            WebSocket.Instance.ReceivedGameEnded += GameEnded;
            WebSocket.Instance.ReceivedCreateNewGame += OnReceivedNewGame;
        }

        public void Initialize()
        {           
            InitEventListeners();
        }

        public void Terminate()
        {
            CurrentGame = new Game();
            IsInGame = false;
            CurrentPlayer = new Player();
            CurrentPlayerTeam = new Team();
        }

        public void GameEnded(string message)
        {
            MessageBox.Show(message);
        }

        private void OnReceivedNewGame(Game game)
        {
            if (game.TeamOne.PlayerOne.UID == AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID)
            {
                CurrentGame = game;
                UpdateCurrentGame(game);
            }
        }

        public void LeaveCurrentGame()
        {
            GameSocketRequest.LeaveGame(CurrentGame.GID, AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID);
            CurrentGame = new Game();
        }

        public void AddVirtualPlayer()
        {
            GameSocketRequest.AddVirtualPlayer(CurrentGame.GID);
        }

        public void SendGuess(string word)
        {
            GameSocketRequest.GuessWord(AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID, 
                CurrentGame.GID, 
                CurrentGame.CurrentRound.ID, 
                word);
        }

        public void StartGame()
        {
            GameSocketRequest.StartGame(CurrentGame.GID, AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID);
        }

        public void SetPlayer()
        {
            CurrentPlayer = CurrentGame.Players.Find(player => player.UID == AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID);
        }

        public void SetTeam()
        {

            if(CurrentPlayer?.PID == CurrentGame.TeamOne.PlayerOne?.PID || CurrentPlayer?.PID == CurrentGame.TeamOne.PlayerTwo?.PID)
            {
                CurrentPlayerTeam = CurrentGame.TeamOne;
            }
            else
            {
                CurrentPlayerTeam = CurrentGame.TeamTwo;
            }
        }

        private bool IsUserInGame(Game game)
        {
            game.SetPlayers();

            foreach (var player in game.Players)
            {
                if (player.UID == AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID)
                    return true;
            }

            return false;
        }

        public void UpdateCurrentGame(Game game)
        {
            game.SetPlayers();

            if(game.UpdatedActions == GAME_UPDATE.UserQuit &&  !IsUserInGame(game))
            {
                CurrentGame = new Game();
                IsInGame = false;
                CurrentGameUpdated.Invoke();

                return;
            }

            if (CurrentGame.CID == string.Empty)
                CurrentGame = game;

            else
            { 
                if(CurrentGame.GID == game.GID)
                    CurrentGame.Copy(game);
                else
                {
                    CurrentGame = game;
                }
            }

            SetPlayer();
            SetTeam();

            IsInGame = CurrentGame.CID != string.Empty;

            CurrentGameUpdated.Invoke();

        }


    }
}
