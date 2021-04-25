using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Models
{
    public class Conversation
    {
        [JsonProperty(PropertyName = "cid")]
        public string CID { get; set; }

        [JsonProperty(PropertyName = "uids")]
        public List<string> UIDS { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty(PropertyName = "updatedTimestamp")]
        public long UpdatedTimeStamp { get; set; }

        [JsonProperty(PropertyName = "convName")]
        public string ConvName { get; set; }

        [JsonProperty(PropertyName = "updateAction")]
        public ConversationUpdate UpdateAction { get; set; }

        [JsonProperty(PropertyName = "users")]
        public List<User> Users { get; set; }

        public string UsernamesString { get { return GetUsernames(); } }

        public Conversation()
        {
            CID = string.Empty;
            UIDS = new List<string>();
            Timestamp = 0;
            UpdatedTimeStamp = 0;
            ConvName = string.Empty;
            Users = new List<User>();
        }

        public override bool Equals(object obj)
        {
            var conversation = obj as Conversation;

            if (conversation == null)
            {
                return false;
            }

            return CID.Equals(conversation.CID);
        }
        public override int GetHashCode()
        {
            return CID.GetHashCode();
        }

        private string GetUsernames()
        {


            if (Users.Count > 1)
            {
                var count = Users.Count - 2;
                if (count == 0)
                {
                    return $"{Users[0].Username} and {Users[1].Username}";
                }
                else
                {
                    return $"{Users[0].Username}, {Users[1].Username} and {count} others";
                }

            }
            else if (Users.Count == 1)
            {
                return Users[0].Username;
            }

            return "";
 
        }


    }

    public class VirtualPlayerMessage
    {
        [JsonProperty(PropertyName = "fr")]
        public string FrenchMsg { get; set; }
        [JsonProperty(PropertyName = "en")]
        public string EnglishMsg { get; set; }
    }

    public enum ConversationUpdate
    {
        Default = 0,
        Created = 1,
        UserAdded = 2,
        UserRemoved = 3,
        Updated = 4,
        Deleted = 5
    }

    public class ConversationInviteResponse
    {
        [JsonProperty(PropertyName = "cid")]
        public string CID { get; set; }

        [JsonProperty(PropertyName = "conversation")]
        public Conversation Conversation { get; set; }

        [JsonProperty(PropertyName = "messages")]
        public List<Message> Messages { get; set; }

        [JsonProperty(PropertyName = "convName")]
        public string ConvName { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public string UID { get; set; }

        [JsonProperty(PropertyName = "user")]   
        public User User { get; set; }


    }
}
