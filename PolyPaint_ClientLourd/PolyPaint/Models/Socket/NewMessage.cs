using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Models.Socket
{
    public class NewMessage
    {

        [Newtonsoft.Json.JsonProperty(PropertyName = "cid")]
        public string CID { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "message")]
        public Message Message { get; set; }

    }
}
