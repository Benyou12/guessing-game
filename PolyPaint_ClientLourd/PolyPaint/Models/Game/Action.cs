using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Models.Game
{
    public class GameAction
    {
        [JsonProperty(PropertyName= "game_id")]
        public string GameID { get; set; }

        [JsonProperty(PropertyName = "round_id")]
        public string RoundID { get; set; }

        [JsonProperty(PropertyName = "team_id")]
        public string TeamID { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string UserID { get; set; }

        [JsonProperty(PropertyName = "payload")]
        public ActionWordGuess Payload { get; set; }

        [JsonProperty(PropertyName = "type")]
        public GAME_ACTIONS Type { get; set; }
    }

    public struct ActionWordGuess
    {
        [JsonProperty(PropertyName = "word")]
        public string Word { get; set; }
    }

    public enum GAME_ACTIONS
    {
        REQUEST_HINT = 0,
        GUESS_WORD = 1,
        ROUND_WON = 2,
        ADD_PLAYER = 3,
        START = 4,
        END = 5,
        START_ROUND = 6,
        END_ROUND = 7
    }
}
