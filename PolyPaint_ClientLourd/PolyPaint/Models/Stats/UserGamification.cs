using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Models.Stats
{
    public class UserGamification
    {
        [JsonProperty(PropertyName = "badges")]
        public List<UserBadge> Badges { get; set; }

        [JsonProperty(PropertyName = "level")]
        public Level Level { get; set; }

        [JsonProperty(PropertyName = "points")]
        public double Points { get; set; }

        public UserGamification()
        {
            Badges = new List<UserBadge>();
            Level = new Level();
            Points = 0;
        }
    }

    public struct UserBadge
    {

        [JsonProperty(PropertyName = "timestamp")]
        public long TimeStamp { get; set; }

        [JsonProperty(PropertyName = "game_id")]
        public string GameID { get; set; }

        [JsonProperty(PropertyName = "badge")]
        public Badge Badge { get; set; }
    }

    public struct Badge
    {
        [JsonProperty(PropertyName = "badge_id")]
        public string BadgeID { get; set; }

        [JsonProperty(PropertyName = "name_fr")]
        public string BadgeNameFR { get; set; }

        [JsonProperty(PropertyName = "name_en")]
        public string BadgesNameEN { get; set; }

        [JsonProperty(PropertyName = "img_fr")]
        public string BadgeImgFR { get; set; }

        [JsonProperty(PropertyName = "img_en")]
        public string BadgeImgEN { get; set; }

        [JsonProperty(PropertyName = "desc_fr")]
        public string BadgeDescriptionFR { get; set; }

        [JsonProperty(PropertyName = "desc_en")]
        public string BadgeDescriptionEN { get; set; }

        [JsonProperty(PropertyName = "points")]
        public long Points { get; set; }
    }

    public struct Level
    {
        [JsonProperty(PropertyName = "number")]
        public long Number { get; set; }

        [JsonProperty(PropertyName = "name_fr")]
        public string NameFR { get; set; }

        [JsonProperty(PropertyName = "name_en")]
        public string NameEN { get; set; }

        [JsonProperty(PropertyName = "img")]
        public string IMG { get; set; }

        [JsonProperty(PropertyName = "min_points")]
        public long MinPoints { get; set; }

    }

}
