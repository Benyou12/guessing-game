using PolyPaint.Models.Game;
using PolyPaint.Models.Lobby;
using PolyPaint.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace PolyPaint.VueModels.Lobby
{
    public class ScoreBoardVueModel
    {
        public Game CurrentGame { get { return GameService.Instance.CurrentGame; } }
        public Team TeamOne { get { return CurrentGame.TeamOne; } }
        public Team TeamTwo { get { return CurrentGame.TeamTwo; } }
        public Round NextRound { get { return GameService.Instance.CurrentGame.Rounds.Last(); } }

        public ScoreBoardVueModel()
        {

        }

    }
}
