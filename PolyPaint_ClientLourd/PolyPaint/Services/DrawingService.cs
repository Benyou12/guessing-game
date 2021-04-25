using PolyPaint.Models;
using PolyPaint.Server;
using PolyPaint.Services;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;

namespace PolyPaint.AppContext
{
    class DrawingService
    {
        public StrokeCollection Lines { get; set; }
        private readonly Dictionary<string, Stroke> _strokeById;
        private readonly Dictionary<Stroke, Models.Game.Path> _strokeByStroke;
        private Models.Game.Path CurrentPath { get; set; }
        public List<Models.Game.Path> ListOfStrokes;
        private bool _isPressingDown;
        public Editor Editor { get; set; }
        public bool IsCreatingGame { get; set; }
        private readonly Timer _timer;

        public DrawingService()
        {
            _timer = new Timer();
            _timer.Elapsed += SendCoordinateAfterTime;
            _timer.Interval = 3;
            _timer.Enabled = false;
            _strokeById = new Dictionary<string, Stroke>();
            IsCreatingGame = false;
            _strokeByStroke = new Dictionary<Stroke, Models.Game.Path>();

            CurrentPath = new Models.Game.Path();
            Editor = new Editor();
            Lines = Editor.lines;
            ListOfStrokes = new List<Models.Game.Path>();
        }

        public void LinesToListOfStrokes()
        {
            var result = new List<Models.Game.Path>();

            foreach (var stroke in Editor.lines)
            {
                var tempStroke = new Models.Game.Path(Guid.NewGuid().ToString(), GameService.Instance.CurrentGame.CurrentRound.Canvas.ID,
                                                  Editor.LineSize, Editor.SelectedColor, false, Editor.SelectedDrawingShape)
                {
                    Coordinates = new List<Models.Game.Coordinate>()
                };

                foreach (var point in stroke.StylusPoints)
                {
                    tempStroke.Coordinates.Add(new Models.Game.Coordinate() { X = point.X, Y = point.Y });
                }

                result.Add(tempStroke);
            }

            ListOfStrokes = result;
        }

        /// <summary>
        /// This method will prepare the list for a required visualization of the strokes
        /// </summary>
        public void PrepareVisualization()
        {
            LinesToListOfStrokes();
            _strokeById.Clear();
        }

        public void Up(Point point, string selectedTool)
        {
            if (selectedTool != "crayon")
                return;

            _isPressingDown = false;
            _timer.Enabled = false;

            CurrentPath.Coordinates.Add(new Models.Game.Coordinate() { X = point.X, Y = point.Y });

            if (!IsCreatingGame)
            {
                WebSocket.Instance.CreateStroke(CurrentPath);
            }
        }

        public void Down(Point point, string selectedTool)
        {

            if (selectedTool != "crayon")
                return;

            CurrentPath = new Models.Game.Path();

            _isPressingDown = true;
            CurrentPath = new Models.Game.Path(Guid.NewGuid().ToString(), GameService.Instance.CurrentGame.CurrentRound.Canvas.ID,
                                                    Editor.LineSize, Editor.SelectedColor, false, Editor.SelectedDrawingShape)
            {
                Coordinates = new List<Models.Game.Coordinate>()
            {
                new Models.Game.Coordinate()
                {
                    X = point.X,
                    Y= point.Y
                }
            }
            };

            if (!IsCreatingGame)
            {
                _timer.Enabled = true;
            }
        }

        public void Move(Point point, string selectedTool)
        {
            if (!_isPressingDown || selectedTool != "crayon")
                return;

            CurrentPath.Coordinates.Add(new Models.Game.Coordinate() { X = point.X, Y = point.Y });
        }

        private void SendCoordinateAfterTime(object sender, ElapsedEventArgs e)
        {
            var cloneStroke = CurrentPath.DeepCopy();

            if (string.IsNullOrEmpty(cloneStroke.ID))
                return;

            WebSocket.Instance.CreateStroke(cloneStroke);

            CurrentPath.Coordinates.Clear();
        }

        public void Redraw(Models.Game.Path path)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                if (path.ToDelete)
                {
                    var str = _strokeById[path.ID];
                    Lines.Remove(str);
                    _strokeById.Remove(path.ID);

                    return;
                }

                if (path.Coordinates.Count == 0)
                    return;

                var points = new StylusPointCollection();

                foreach (var coordinate in path.Coordinates)
                    points.Add(new StylusPoint(coordinate.X, coordinate.Y));


                if (!_strokeById.ContainsKey(path.ID))
                {
                    _strokeById.Add(path.ID, new Stroke(points));
                    _strokeById[path.ID].DrawingAttributes.Width = path.Size;
                    _strokeById[path.ID].DrawingAttributes.StylusTip = path.DrawingShape == Models.Game.Cap.Round ? StylusTip.Ellipse : StylusTip.Rectangle;
                    _strokeById[path.ID].DrawingAttributes.Height = path.Size;
                    _strokeById[path.ID].DrawingAttributes.Color = (Color)ColorConverter.ConvertFromString(path.Color);
                }
                else
                {
                    _strokeById[path.ID].StylusPoints.Add(points);
                }

                var tempStroke = _strokeById[path.ID];
                if (!Lines.Contains(tempStroke))
                    Lines.Add(tempStroke);

            });

        }

        public void AddStroke(Stroke stroke)
        {
            if (stroke == null)
                return;

            _strokeByStroke.Add(stroke, CurrentPath);
        }

        public void UpdateStrokeList(StrokeCollection oldStrokes, StrokeCollection newStrokes)
        {
            AddNewStroke(newStrokes);
            RemoveOldStroke(oldStrokes);
        }

        private void RemoveOldStroke(StrokeCollection oldStrokes)
        {
            if (IsCreatingGame)
            {
                return;
            }

            foreach (var stroke in oldStrokes)
            {
                var strokeBeRemoved = _strokeByStroke[stroke];
                strokeBeRemoved.ToDelete = true;
                WebSocket.Instance.CreateStroke(strokeBeRemoved);
            }
        }

        private void AddNewStroke(StrokeCollection newStroke)
        {
            if (IsCreatingGame)
            {
                return;
            }

            foreach (var stroke in newStroke)
            {
                var tempStroke = new Models.Game.Path(Guid.NewGuid().ToString(),
                    GameService.Instance.CurrentGame.CurrentRound.Canvas.ID,
                    Editor.LineSize, Editor.SelectedColor, false, Editor.SelectedDrawingShape)
                {
                    Coordinates = new List<Models.Game.Coordinate>()
                };

                foreach (var point in stroke.StylusPoints)
                {
                    tempStroke.Coordinates.Add(new Models.Game.Coordinate() { X = point.X, Y = point.Y });
                }

                WebSocket.Instance.CreateStroke(tempStroke);
                _strokeByStroke.Add(stroke, tempStroke);
            }
        }


    }
}
