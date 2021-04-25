using PolyPaint.Models;
using PolyPaint.Models.Lobby;
using PolyPaint.Services;

namespace PolyPaint.AppContext
{
    /// <summary>
    /// This class should contains all the information that is shared in all the app
    /// </summary>
    public class AppContext : IService
    {
        public User CurrentConnectedUser { get; set; }
        public System.Timers.Timer Timer { get; set; }
        public string CurrentChatRoom { get; set; }
        public Game CurrentGame { get; set; }
        public AppContext()
        {
            CurrentConnectedUser = new User();
            Timer = new System.Timers.Timer();
            CurrentGame = new Game();
        }

        public void Initialize()
        {

        }

        public void Terminate()
        {
            CurrentConnectedUser = new User();
            CurrentChatRoom = string.Empty;
            CurrentGame = new Game();
        }
    }
}
