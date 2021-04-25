using PolyPaint.AppContext;
using PolyPaint.Models.Lobby;
using PolyPaint.Server;
using PolyPaint.Server.SocketHelpers;
using PolyPaint.Services;
using PolyPaint.Utils;
using PolyPaint.VueModels.Lobby;
using PolyPaint.Vues;
using PolyPaint.Vues.DrawingCanvas;
using PolyPaint.Vues.Lobby;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PolyPaint.VueModels
{


    public class GameFlowTransitionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private GameService gameService;
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

        public GameFlowTransitionViewModel()
        {

            DrawingServiceSingleton.Instance.DrawingService.IsCreatingGame = false;

            CurrentView = new SelectOrCreateGame();

            GameService.Instance.CurrentGameUpdated += new GameService.UpdateGameEventHandler(RefreshVue);
            gameService = GameService.Instance;


        }

        private void RefreshVue()
        {
            switch (gameService.CurrentGame.State)
            {
                case GAME_STATE.Lobby:
                    ShowLobby();
                    break;
                case GAME_STATE.ScoreBoard:
                    ShowScoreBoard();
                    break;
                case GAME_STATE.Round:
                    ShowCanvas();
                    break;
                case GAME_STATE.Reply:
                    break;
                case GAME_STATE.Completed:
                    ShowEndGame();
                    break;
                default:
                    ShowMainPage();
                    break;
            }
        }


        private void ShowEndGame()
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                if (CurrentView is EndGameView)
                    return;
                ChangePage(new EndGameView());
            });
        }

        private void ChangePage(UserControl page)
        {
            CurrentView = page;
        }

        public void ShowMainPage()
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                if (CurrentView is SelectOrCreateGame)
                    return;
                ChangePage(new SelectOrCreateGame());
            });
        }

        private void ShowLobby()
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                if (CurrentView is FinalizeTeamTraining)
                    return;
                ChangePage(new FinalizeTeamTraining());
            });          

        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            GameSocketRequest.StartRound(gameService.CurrentGame.GID, AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID,
                gameService.CurrentGame.CurrentRound.ID);
            AppContextSingleton.Instance.AppContext.Timer.Enabled = false;
        }

        private void ShowCanvas()
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                if (GameService.Instance.CurrentPlayer.PID == GameService.Instance.CurrentGame.CurrentRound.PlayerDrawginID)
                {
                    if (CurrentView is DrawingCanvasWindow)
                        return;
                    ChangePage(new DrawingCanvasWindow());
                }
                else
                {
                    if (CurrentView is GuesserCanvasWindow)
                        return;

                    ChangePage(new GuesserCanvasWindow());
                }
            });
            
        }

        private void ShowScoreBoard()
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                if (CurrentView is ScoreBoardVue)
                    return;
                ChangePage(new ScoreBoardVue());
            });
            AppContextSingleton.Instance.AppContext.Timer.Interval = 10000;
            AppContextSingleton.Instance.AppContext.Timer.Elapsed += Timer_Elapsed;
            AppContextSingleton.Instance.AppContext.Timer.Enabled = false;
        }



        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
