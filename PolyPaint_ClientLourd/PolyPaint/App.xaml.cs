using PolyPaint.Server;
using PolyPaint.Server.SocketHelpers;
using PolyPaint.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Quobject.EngineIoClientDotNet.Modules;

namespace PolyPaint
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Authentification.SignOut();

            WebSocket.Instance.Terminate();
            MessagingService.Instance.Terminate();
            GameService.Instance.Terminate();
            
        }


        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
#if (DEBUG)


#else
            MessageBox.Show(Utils.Global.language == "en-US"
                ? "Oops, something went wrong , The application will restart"
                : "Oops, Nous avons rencontre un probleme. L'application va restart");

            if (GameService.Instance.IsInGame)
                GameService.Instance.LeaveCurrentGame();

            WebSocket.Instance.Disconnect();
            System.Windows.Forms.Application.Restart();
            Current.Shutdown();
            e.SetObserved() ;
#endif
        }


        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
#if (DEBUG)

            e.Handled = false;

#else
            MessageBox.Show(Utils.Global.language == "en-US"
                ? "Oops, something went wrong , The application will restart"
                : "Oops, Nous avons rencontre un probleme. L'application va restart");

            if (GameService.Instance.IsInGame)
                GameService.Instance.LeaveCurrentGame();

            WebSocket.Instance.Disconnect();
            System.Windows.Forms.Application.Restart();
            Current.Shutdown();
            e.Handled = true;
#endif
        }
    }
}
