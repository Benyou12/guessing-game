using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Models.Game
{
    public class Round
    {
        [JsonProperty(PropertyName= "_id", Required = Required.AllowNull)]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "team", Required = Required.AllowNull)]
        public string TeamID { get; set; }

        [JsonProperty(PropertyName = "player1_guessing_id", Required = Required.AllowNull)]
        public string PlayerOneGuessingString { get; set; }

        [JsonProperty(PropertyName = "player2_guessing_id", Required = Required.AllowNull)]
        public string PlayerTwoGuessingString { get; set; }

        [JsonProperty(PropertyName = "player_drawing_id", Required = Required.AllowNull)]
        public string PlayerDrawginID { get; set; }

        [JsonProperty(PropertyName = "team_win_id", Required = Required.AllowNull)]
        public string TeamWinID { get; set; }

        [JsonProperty(PropertyName = "points_won", Required = Required.AllowNull)]
        public int PointWon { get; set; }

        [JsonProperty(PropertyName = "canvas", Required = Required.AllowNull)]
        public Canvas Canvas { get; set; }

        [JsonProperty(PropertyName = "game_img",Required=Required.AllowNull)]
        public GameImage GameImage { get; set; }

        [JsonProperty(PropertyName = "startTimestamp", Required = Required.AllowNull)]
        public long StartTimeStamp { get; set; }

        [JsonProperty(PropertyName = "endTimestamp", Required = Required.AllowNull)]
        public long? EndTimeStamp { get; set; }

        public Round()
        {
            GameImage = new GameImage();
            Canvas = new Canvas();
            PointWon = 0;
            ID = string.Empty;
            PlayerOneGuessingString = string.Empty;
            PlayerTwoGuessingString = string.Empty;
            StartTimeStamp = 0;
            EndTimeStamp = 0;
            TeamID = string.Empty;
            TeamWinID = string.Empty;
        }

    }
}
