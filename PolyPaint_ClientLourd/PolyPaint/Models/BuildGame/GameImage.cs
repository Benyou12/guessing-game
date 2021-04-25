using Newtonsoft.Json;
using PolyPaint.Models.Game;
using System.Collections.Generic;

namespace PolyPaint.Models.BuildGame
{
    public class GameImage
    {
        [JsonProperty(PropertyName = "_id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "word")]
        public string Word { get; set; }

        [JsonProperty(PropertyName = "lang")]
        public LANG WordLanguage { get; set; }

        [JsonProperty(PropertyName = "hints")]
        public List<string> Hints { get; set; }

        [JsonProperty(PropertyName = "difficulty")]
        public GAME_DIFFICULTY Difficulty { get; set; }

        [JsonProperty(PropertyName = "svg_link")]
        public string SvgLink { get; set; }

        [JsonProperty(PropertyName = "canvas")]
        public Canvas Canvas { get; set; }

        [JsonProperty(PropertyName = "drawing_mode")]
        public DRAWING_MODES DrawingMode { get; set; }

        public GameImage()
        {
            ID = string.Empty;
            Word = string.Empty;
            Hints = new List<string>();
            Difficulty = GAME_DIFFICULTY.EASY;
            SvgLink = string.Empty;
            Canvas = new Canvas();
            DrawingMode = DRAWING_MODES.RANDOM;
            WordLanguage = LANG.FR;
        }
    }

    public enum DRAWING_MODES
    {
        RANDOM = 0,
        CENTERED = 1,
        PANORAMIC = 2
    }

    public enum GAME_DIFFICULTY
    {
        EASY = 0,
        MEDIUM = 1,
        HARD = 2
    }

    public enum LANG
    {
        FR = 0,
        EN = 1
    }
}
