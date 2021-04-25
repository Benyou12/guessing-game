using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Models.BuildGame
{
    class SearchImageResult
    {
        [JsonProperty("items")]
        public List<SearchItem> Items { get; set; }
    }

    class SearchItem
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

    }
}
