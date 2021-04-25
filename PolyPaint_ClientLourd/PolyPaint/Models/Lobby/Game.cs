using Newtonsoft.Json;
using PolyPaint.Models.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PolyPaint.Models.Lobby
{


    public class Game : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty(PropertyName = "_id")]
        public string GID { get; set; }

        [JsonProperty(PropertyName = "conversation_id")]
        public string CID { get; set; }

        [JsonProperty(PropertyName = "state")]
        public GAME_STATE State { get; set; }

        [JsonProperty(PropertyName = "isReady")]
        public bool IsReady { get; set; }

        [JsonProperty(PropertyName = "teamOne")]
        public Team TeamOne { get; set; }

        [JsonProperty(PropertyName = "teamTwo")]
        public Team TeamTwo { get; set; }

        [JsonProperty(PropertyName ="actions")]
        public List<GameAction> Actions { get; set; }

        [JsonProperty(PropertyName ="rounds", NullValueHandling = NullValueHandling.Include)]
        public List<Round> Rounds { get; set; }

        [JsonProperty(PropertyName ="max_round")]
        public int MaxRound { get; set; }

        [JsonProperty(PropertyName = "updateAction")]
        public GAME_UPDATE UpdatedActions { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        private List<Player> players;
        [JsonIgnore]
        public List<Player> Players
        {
            get
            {
                return players;
            }
            set
            {
                players = value;
                NotifyPropertyChanged();
            }
        }

        [JsonIgnore]
        public Round CurrentRound { get
            {
                if(Rounds == null)
                    return new Round();
                if (Rounds.Count > 0)
                    return Rounds[Rounds.Count -1];
                else {
                    return new Round();
                }
            }
        }


        public Game()
        {
            CID = string.Empty;
            GID = string.Empty;
            State = GAME_STATE.None;
            UpdatedActions = GAME_UPDATE.Default;
            IsReady = false;
            TeamOne = new Team();
            TeamTwo = new Team();
            Players = new List<Player>();
            Rounds = new List<Round>();
            SetPlayers();
        }

        public override bool Equals(object obj)
        {
            var game = obj as Game;

            if (game == null)
            {
                return false;
            }

            return GID.Equals(game.GID);
        }
        public override int GetHashCode()
        {
            return GID.GetHashCode();
        }

        public void Copy(Game game)
        {
            TeamTwo =  game.TeamTwo;
            TeamOne = game.TeamOne;
            IsReady = game.IsReady;
            Actions = game.Actions;
            Rounds = game.Rounds;
            State = game.State;


            SetPlayers();
        }


        public void SetPlayers()
        {
            var tempPlayers = new List<Player>();

            if (TeamOne.PlayerOne != null)
            {
                tempPlayers.Add(TeamOne.PlayerOne);
            }
            if (TeamOne.PlayerTwo != null)
            {
                tempPlayers.Add(TeamOne.PlayerTwo);
            }
            if (TeamTwo.PlayerOne != null)
            {
                tempPlayers.Add(TeamTwo.PlayerOne);
            }
            if (TeamTwo.PlayerTwo != null)
            {
                tempPlayers.Add(TeamTwo.PlayerTwo);
            }

            Players = tempPlayers;
        }
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum GAME_STATE
    {
        Lobby =0,
        ScoreBoard =1,
        Round = 2,
        Reply = 3,
        Completed = 4,
        None = 5
    }

    public enum GAME_UPDATE
    {
        Default = 0,
        Created = 1,
        Updated = 2,
        UserJoin = 3,
        UserQuit = 4,
        VirtualJoin = 5,
        ActionFlow = 6
    }
}
