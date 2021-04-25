using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Models.Socket
{
    public class JoinRoomRequest
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "uid")]
        public string UID { get; set; }

    }
}
