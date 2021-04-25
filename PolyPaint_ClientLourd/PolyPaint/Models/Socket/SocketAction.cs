using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Models.Socket
{
    public class SocketAction
    {
        [Newtonsoft.Json.JsonProperty(PropertyName = "route")]
        public string Route { get; set; }

        [Newtonsoft.Json.JsonProperty(PropertyName = "payload")]
        public object Payload { get; set; }

    }
}
