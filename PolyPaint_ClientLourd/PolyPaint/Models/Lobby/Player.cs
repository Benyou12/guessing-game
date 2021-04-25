using Newtonsoft.Json;

namespace PolyPaint.Models.Lobby
{
    public class Player
    {
        [JsonProperty(PropertyName = "_id")]
        public string PID { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public string UID { get; set; }

        [JsonProperty(PropertyName = "user")]
        public User User { get; set; }

        [JsonProperty(PropertyName = "role")]
        public string Role { get; set; }

        [JsonProperty(PropertyName = "isVirtual")]
        public bool IsVirtual { get; set; }

        
        public Player()
        {
            PID = string.Empty;
            UID = string.Empty;
            User = new User();
            Role = string.Empty ;
            IsVirtual = false;
        }
    }

    public enum PLAYER_ROLE
    {
        DRAW=0,
        GUESS=1,
        NONE =2
    }

    public class PlayerUser
    {
        [JsonProperty(PropertyName = "uid")]
        public string UID { get; set; }
        
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "profileImgUrl")]
        public string profileImgUrl { get; set; }
    }
}
