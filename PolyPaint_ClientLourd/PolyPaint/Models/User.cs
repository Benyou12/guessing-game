
using Newtonsoft.Json;
using PolyPaint.Models.Stats;
using System.Collections.Generic;

namespace PolyPaint.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "uid")]
        public string UID { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "profileImgUrl")]
        public string ProfileImgURL { get; set; }

        [JsonProperty(PropertyName = "auth_history")]
        public List<UserAuthStats> UserAuthStats { get; set; }

        [JsonProperty(PropertyName = "game_stats")]
        public UserGameStats UserGameStats { get; set; }

        [JsonProperty(PropertyName = "game_history")]
        public List<UserGameStatsHistory> UserGameStatsHistory { get; set; }

        [JsonProperty(PropertyName = "gamification")]
        public UserGamification UserGamification { get; set; }

        [JsonIgnore]
        public int BadgesEarned
        {
            get
            {
                return UserGamification.Badges.Count;
            }
        }

        [JsonIgnore]
        public UserAuthStats LatestAuthenticationStats
        {
            get
            {
                if (UserAuthStats.Count > 0)
                {
                    return UserAuthStats[UserAuthStats.Count - 1];
                }

                return new UserAuthStats();
            }
        }

        public User()
        {
            UID = string.Empty;
            Email = string.Empty;
            Username = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            ProfileImgURL = string.Empty;
            UserAuthStats = new List<UserAuthStats>();
            UserGameStats = new UserGameStats();
            UserGameStatsHistory = new List<UserGameStatsHistory>();
            UserGamification = new UserGamification();
        }

    }
}
