using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PolyPaint.Models.Stats
{
    public class UserAuthStats
    {
        [JsonProperty(PropertyName = "isLogin")]
        public bool IsLoggedIn { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public long TimeStamp { get; set; }

        public UserAuthStats()
        {
            IsLoggedIn = false;
            TimeStamp = 0;
        }
    }
}
