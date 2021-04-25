using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Models.Game
{
    public class GameImage
    {
        [JsonProperty(PropertyName = "_id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "hints")]
        public List<string> Hints { get; set; }

        [JsonProperty(PropertyName = "word")]
        public string Word { get; set; }

        [JsonProperty(PropertyName = "difficulty")]
        public int Difficulty { get; set; }

        [JsonProperty(PropertyName = "svg_link", Required = Required.AllowNull)]
        public string SvgLink {get;set;}

        [JsonProperty(PropertyName = "canvas")]
        public Canvas Canvas { get; set; }

        public GameImage()
        {
            ID = string.Empty;
            Hints = new List<string>();
            Word = string.Empty;
            SvgLink = string.Empty;
            Canvas = new Canvas();
        }

    }
}
