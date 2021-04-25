using System.Threading.Tasks;
using Newtonsoft.Json;
using PolyPaint.Models;
using PolyPaint.AppContext;
using System.IO;
using PolyPaint.Services;

namespace PolyPaint.Server
{
    public class Authentification
    {

        private static readonly string LoginURL = "/auth/login";
        private static readonly string LogoutURL = "/auth/logout";
        private static readonly string RegisterURL = "/user";

        /// <summary>
        /// This method is goin to authentificate the user 
        /// </summary>
        /// <param name="input"> contains all the information required to authentificate the user</param>
        /// <returns>Will return a response model </returns>
        public static ResponseModel SignIn(LogInRequestModel input)
        {           
            var task = Task.Run(async () => await RequestHandler.Post(LoginURL, input));

            var result = task.Result;

            if (result.JSON != null)
            {
                var user = JsonConvert.DeserializeObject<User>(result.JSON);
                AppContextSingleton.Instance.AppContext.CurrentConnectedUser = user;
                if (AppContextSingleton.Instance.AppContext.CurrentConnectedUser?.UserGamification?.Badges != null)
                {
                    AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UserGamification.Badges.Reverse();
                    AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UserGameStatsHistory.Reverse();
                    AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UserAuthStats.Reverse();
                }
                    

                // Initialize the socket connection
                WebSocket.Instance.Initialize();

                WebSocket.Instance.LoginCompleted();
            }

            return result;
        }

        public static void SignOut()
        {
            var uid = AppContextSingleton.Instance?.AppContext?.CurrentConnectedUser?.UID;

            if (uid != null)
            {
                Task.Run(async () => await RequestHandler.Post(LogoutURL, new { uid }));
            }
            

            if (GameService.Instance.IsInGame)
                GameService.Instance.LeaveCurrentGame();

            WebSocket.Instance.Terminate();
            MessagingService.Instance.Terminate();
            GameService.Instance.Terminate();

            WebSocket.Instance.LogoutCompleted();


        }

        /// <summary>
        /// This method will register the user to the database 
        /// </summary>
        /// <param name="input"> contains all the information required to register the user</param>
        /// <returns></returns>
        public static ResponseModel SignUp(RegisterRequestModel input)
        {
            if (File.Exists(input.LocalImageURI))
            {
                var url = UploadImage(input.LocalImageURI);
                input.ProfileImgUrl = url;
            }
            else
            {
                if (input.ProfileImgUrl == string.Empty)
                {
                    input.ProfileImgUrl = "https://firebasestorage.googleapis.com/v0/b/polychat-9c90e.appspot.com/o/default_profile_picture.png?alt=media";
                }
            }           

            var task = Task.Run(async () => await RequestHandler.Post(RegisterURL, input));

            var result = task.Result;

            if (result.JSON != null)
            {
                var user = JsonConvert.DeserializeObject<User>(result.JSON);
                AppContextSingleton.Instance.AppContext.CurrentConnectedUser = user;
                if (AppContextSingleton.Instance.AppContext.CurrentConnectedUser?.UserGamification?.Badges != null)
                {
                    AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UserGamification.Badges.Reverse();
                    AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UserGameStatsHistory.Reverse();
                    AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UserAuthStats.Reverse();
                }

                WebSocket.Instance.JoinRoom();

                WebSocket.Instance.LogoutCompleted();
            }
           
            return result;

        }
        public static string UploadImage(string imageURI)
        {
            var task = Task.Run(async () => await RequestHandler.PostFile(imageURI));
            var result = task.Result;

            return result;

        }
    }
}
