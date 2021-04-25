using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Models.Stats
{
    public class UserGameStats
    {
        [JsonProperty(PropertyName = "rounds_played")]
        public int RoundsPlayed { get; set; }

        [JsonProperty(PropertyName = "victories")]
        public int Victories { get; set; }

        [JsonProperty(PropertyName = "failures")]
        public int Failures { get; set; }

        [JsonProperty(PropertyName = "rounds_avg_time")]
        public double RoundAvgTime { get; set; }

        [JsonProperty(PropertyName = "total_time_played")]
        public double TotalTimePlayed { get; set; }

        [JsonProperty(PropertyName = "total_games_played")]
        public double TotalGamePlayed { get; set; }

        public UserGameStats()
        {
            RoundsPlayed = 0;
            Victories = 0;
            Failures = 0;
            RoundAvgTime = 0;
            TotalTimePlayed = 0;
            TotalGamePlayed = 0;
        }

    }
}
