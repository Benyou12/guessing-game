using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Models.Stats
{
    public class UserGameStatsHistory
    {
        [JsonProperty(PropertyName = "game_id")]
        public string GameID { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public long TimeStamp { get; set; }

        [JsonProperty(PropertyName = "names")]
        public List<string> Names { get; set; }

        [JsonProperty(PropertyName = "myTeamResult")]
        public int MyTeamResult { get; set; }

        [JsonProperty(PropertyName = "otherTeamResult")]
        public int OtherTeamResult { get; set; }


        public UserGameStatsHistory()
        {
            GameID = string.Empty;
            TimeStamp = 0;
            Names = new List<string>();
            MyTeamResult = 0;
            OtherTeamResult = 0;
        }
    }
}
