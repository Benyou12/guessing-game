using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Models.Game
{

    public struct Coordinate
    {
        [JsonProperty(PropertyName = "x")]
        public double X { get; set; }
        [JsonProperty(PropertyName = "y")]
        public double Y { get; set; }
    }
    


    public class Path
    {
        [JsonProperty(PropertyName = "_id")]
        public string ID { get; set; }
        [JsonProperty(PropertyName = "canvas_id")]
        public string CanvasID { get; set; }
        [JsonProperty(PropertyName = "size")]
        public int Size { get; set; }
        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }
        [JsonProperty(PropertyName = "coordinates")]
        public  List<Coordinate> Coordinates { get; set; }
        [JsonProperty(PropertyName = "toDelete")]
        public bool ToDelete { get; set; } = false;
        [JsonProperty(PropertyName = "cap")]
        public Cap DrawingShape { get; set; }

        public Path()
        {
            Coordinates = new List<Coordinate>();
        }

       public Path(string id, string canvasID, int size, string color, bool toDelete, string drawingShape)
        {
            ID = id;
            CanvasID = canvasID;
            Size = size;
            Color = color;
            Coordinates = new List<Coordinate>();
            ToDelete = toDelete;
            DrawingShape = drawingShape == "ronde" ? Cap.Round : Cap.Square;
        }

        public Path DeepCopy()
        {
            var drawingShape = DrawingShapeToString();
            var copy = new Path(ID, CanvasID, Size, Color, ToDelete, drawingShape);
            copy.Coordinates = new List<Coordinate>(Coordinates);

            return copy;
        }

        private string DrawingShapeToString()
        {
            return DrawingShape == Cap.Round ? "ronde" : "carree";
        }
    }

   

    public class Canvas
    {
        [JsonProperty(PropertyName = "_id", Required=Required.AllowNull)]
        public string ID { get; set; }
        [JsonProperty(PropertyName = "strokes", Required = Required.AllowNull)]
        public List<Path> Strokes { get; set; }
        [JsonProperty(PropertyName = "uids", Required = Required.AllowNull)]
        public List<string> UserIds  { get; set; }

        public Canvas()
        {
            ID = string.Empty;
            Strokes = new List<Path>();
            UserIds = new List<string>();
        }

        
    }
    public enum Cap
    {
        Square,
        Round
    }

}
