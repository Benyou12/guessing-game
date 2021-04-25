using PolyPaint.Server;
using PolyPaint.Utils;
using PolyPaint.Vues;
using PolyPaint.Vues.Account;
using PolyPaint.Vues.Lobby;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.VueModels.Lobby
{
    class SelectGameModeViewModel
    {
        public RelayCommand<object> DisconnectCommand { get; set; }
        public RelayCommand<object> SelectClassicGameModeCommand { get; set; }

        public SelectGameModeViewModel()
        {
            DisconnectCommand = new RelayCommand<object>(Disconnect);
            SelectClassicGameModeCommand = new RelayCommand<object>(SelectClassicGameMode);
        }

        public void Disconnect(object o)
        {
            Authentification.SignOut();
            MainTransitions.TransitionPageControl.ShowPage(new LoginView());
        }

        public void SelectClassicGameMode(object o)
        {
            //TODO : save the selected game mode

            MainTransitions.TransitionPageControl.ShowPage(new SelectOrCreateGame());
        }

    }
}
