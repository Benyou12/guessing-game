using Microsoft.Win32;
using PolyPaint.AppContext;
using PolyPaint.Models.BuildGame;
using PolyPaint.Models.Game;
using PolyPaint.Server;
using PolyPaint.Utils;
using PolyPaint.Utils.CsPotrace;
using PolyPaint.Vues.BuildGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Ink;
using GameImage = PolyPaint.Models.BuildGame.GameImage;

namespace PolyPaint.VueModels.BuildGame
{
    class BuildGameViewModel : INotifyPropertyChanged
    {
        public RelayCommand<object> SaveGameImageCommand { get; set; }
        public RelayCommand<object> DownloadImageCommand { get; set; }
        public RelayCommand<object> OpenFileCommand { get; set; }
        public RelayCommand<object> AddHintCommand { get; set; }
        public RelayCommand<object> RemoveHintCommand { get; set; }
        public RelayCommand<object> PreviewCommand { get; set; }
        public RelayCommand<object> PreviousViewCommand { get; set; }
        public RelayCommand<object> NextViewCommand { get; set; }
        public RelayCommand<object> DrawViewCommand { get; set; }

        

        public delegate void NextViewEvent();
        public static NextViewEvent OnNextViewCommand = delegate { };
        public delegate void PreviousViewEvent();
        public static PreviousViewEvent OnPreviousViewCommand = delegate { };
        public delegate void GuesserCanvasWindowViewEvent();
        public static GuesserCanvasWindowViewEvent OnGuesserCanvasWindowView = delegate { };
        public delegate void ShowPreviewDrawingEvent();
        public static ShowPreviewDrawingEvent OnShowPreviewDrawing = delegate { };
        public delegate void GoogleSearchEvent(string SearchString, int startIndex);
        public static GoogleSearchEvent OnGoogleSearch = delegate { };
        public delegate void DrawViewEvent();
        public static DrawViewEvent OnDrawView = delegate { };

        public event PropertyChangedEventHandler PropertyChanged;

        public string CurrentHint { get; set; }
        public string CurrentWord { get; set; }
        public ObservableCollection<string> Hints { get; set; }

        private string Filepath { get; set; }
        private DRAWING_MODES _drawingMode = DRAWING_MODES.RANDOM;
        private PANORAMIC_DRAWING_DIRECTION _panoramicDrawingDirection = PANORAMIC_DRAWING_DIRECTION.FROM_LEFT_TO_RIGHT;
        private CENTER_DRAWING_DIRECTION _centerDrawingDirection = CENTER_DRAWING_DIRECTION.INSIDE_TO_OUTSIDE;
        private GAME_DIFFICULTY _gameDifficulty = GAME_DIFFICULTY.EASY;
        private LANG _wordLanguage = LANG.FR;

        private readonly string CurrentWordEmptyMessage = "Saisissez le mot ou l'impression qui sera deviné.";
        private readonly string HintListEmptyMessage = "Saisissez au moins un indice.";
        private readonly string ListOfStrokesEmptyMessage = "Vous devez faire un dessin ou choisir une image pour le mot ou l'expression.";
        private readonly string GameImageSavesMessage = "Game image saved.";

        public BuildGameViewModel()
        {
            SaveGameImageCommand = new RelayCommand<object>(SaveGameImage);
            DownloadImageCommand = new RelayCommand<object>(DownloadImage);
            OpenFileCommand = new RelayCommand<object>(OpenFile);
            AddHintCommand = new RelayCommand<object>(AddHint);
            RemoveHintCommand = new RelayCommand<object>(RemoveHint);
            PreviewCommand = new RelayCommand<object>(PreviewDraw);
            PreviousViewCommand = new RelayCommand<object>(ViewPreviousView);
            NextViewCommand = new RelayCommand<object>(ViewNextView);
            DrawViewCommand = new RelayCommand<object>(DrawCommand);

            CurrentHint = string.Empty;
            CurrentWord = string.Empty;
            Hints = new ObservableCollection<string>();

            WebSocket.Instance.ReceivedGameImage += new WebSocket.ReceivedGameImageEventHandler(ReceivedGameImage);

            GoogleSearchView.OnSelectSearchImage += OnSelectSearchImage;
        }

        private void ReceivedGameImage(GameImage gameImage)
        {
            if(!Guard.IsNullOrEmpty(gameImage.ID))
                MessageBox.Show(GameImageSavesMessage);
        }

        public string CurrentPathFile
        {
            get { return Filepath; }
            set { Filepath = value; OnPropertyChanged("CurrentPathFile"); }

        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DRAWING_MODES DrawingMode
        {
            get { return _drawingMode; }
            set
            {
                if (_drawingMode != value)
                {
                    _drawingMode = value;
                    OnPropertyChanged("DrawingMode");
                }
            }
        }

        public GAME_DIFFICULTY GameDifficulty
        {
            get { return _gameDifficulty; }
            set
            {
                if (_gameDifficulty != value)
                {
                    _gameDifficulty = value;
                    OnPropertyChanged("GameDifficulty");
                }
            }
        }

        public LANG WordLanguage
        {
            get { return _wordLanguage; }
            set
            {
                if (_wordLanguage != value)
                {
                    _wordLanguage = value;
                    OnPropertyChanged("WordLanguage");
                }
            }
        }

        public PANORAMIC_DRAWING_DIRECTION PanoramicDrawingDirection
        {
            get { return _panoramicDrawingDirection; }
            set
            {
                if (_panoramicDrawingDirection != value)
                {
                    _panoramicDrawingDirection = value;
                    OnPropertyChanged("PanoramicDrawingDirection");
                }
            }
        }

        public CENTER_DRAWING_DIRECTION CenterDrawingDirection
        {
            get { return _centerDrawingDirection; }
            set
            {
                if (_centerDrawingDirection != value)
                {
                    _centerDrawingDirection = value;
                    OnPropertyChanged("CenterDrawingDirection");
                }
            }
        }

        private void RemoveHint(object obj)
        {
            if (obj != null)
                if (obj is string hint)
                    Hints.Remove(hint.Trim());
        }

        private void AddHint(object obj)
        {
            if (!Guard.IsNullOrEmpty(CurrentHint))
                Hints.Add(CurrentHint.Trim());
        }

        private void OpenFile(object obj)
        {
            var filter = "Image Files (*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png";
            var openDlg = new OpenFileDialog { CheckFileExists = true, Filter = filter, Multiselect = false };
            if (openDlg.ShowDialog().GetValueOrDefault())
            {
                CurrentPathFile = openDlg.FileName;

                Image image = Image.FromFile(CurrentPathFile);
                var bitmap = (Bitmap)image;
                if (bitmap == null)
                    return;

                GuesserCanvasWindow(bitmap);
            }
        }

        private List<Models.Game.Path> ConvertBitmapToStrokes(Bitmap bitmap)
        {
            bool[,] Matrix;
            ArrayList ListOfCurveArray; ListOfCurveArray = new ArrayList();
            Matrix = Potrace.BitMapToBinary(bitmap, 150);
            Potrace.potrace_trace(Matrix, ListOfCurveArray);

            string selectedColor = "Black";
            int lineSize = 11;
            string shape = "rond";
            var listOfStrokes = new List<Models.Game.Path>();
            var canvasId = Guid.NewGuid().ToString();

            for (int i = 0; i < ListOfCurveArray.Count; i++)
            {
                ArrayList CurveArray = (ArrayList)ListOfCurveArray[i];
                for (int j = 0; j < CurveArray.Count; j++)
                {
                    Potrace.Curve[] Curves = (Potrace.Curve[])CurveArray[j];
                    var stroke = new Models.Game.Path(Guid.NewGuid().ToString(), canvasId, lineSize, selectedColor, false,shape);
                    for (int k = 0; k < Curves.Length; k++)
                    {
                        if (Curves[k].Kind == Potrace.CurveKind.Bezier)
                        {
                            stroke.Coordinates.Add(new Coordinate() { X = Curves[k].A.X, Y = Curves[k].A.Y });
                            stroke.Coordinates.Add(new Coordinate() { X = Curves[k].ControlPointA.X, Y = Curves[k].ControlPointA.Y });
                            stroke.Coordinates.Add(new Coordinate() { X = Curves[k].ControlPointB.X, Y = Curves[k].ControlPointB.Y });
                            stroke.Coordinates.Add(new Coordinate() { X = Curves[k].B.X, Y = Curves[k].B.Y });
                        }
                        else
                        {
                            stroke.Coordinates.Add(new Coordinate() { X = Curves[k].A.X, Y = Curves[k].A.Y });
                            stroke.Coordinates.Add(new Coordinate() { X = Curves[k].B.X, Y = Curves[k].B.Y });

                        }
                    }
                    listOfStrokes.Add(stroke);
                }
            }

            return listOfStrokes;
        }

        private void ViewNextView(object obj)
        {
            OnNextViewCommand.Invoke();
        }

        private void ViewPreviousView(object obj)
        {
            OnPreviousViewCommand.Invoke();
        }

        private void DownloadImage(object obj)
        {
            if (Guard.IsNullOrEmpty(CurrentWord))
                return;

            int startIndex = 1;
            OnGoogleSearch.Invoke(CurrentWord, startIndex);
        }

        private void SaveGameImage(object obj)
        {
            if (!CanSaveGameImage())
                return;

            SetDrawingMode();

            var gameImage = new GameImage();
            gameImage.Hints = new List<string>(Hints);
            gameImage.Word = CurrentWord;
            gameImage.Difficulty = _gameDifficulty;
            gameImage.DrawingMode = _drawingMode;
            gameImage.WordLanguage = _wordLanguage;
            gameImage.Canvas = new Canvas() {
                ID = DrawingServiceSingleton.Instance.DrawingService.ListOfStrokes[0].CanvasID,
                Strokes = DrawingServiceSingleton.Instance.DrawingService.ListOfStrokes,
                UserIds = new List<string>()
            };
            WebSocket.Instance.CreateGameImage(gameImage);
        }

        private void PreviewDraw(object obj)
        {
            if (DrawingServiceSingleton.Instance.DrawingService.ListOfStrokes.Count == 0)
            {
                DrawingServiceSingleton.Instance.DrawingService.PrepareVisualization();
                if (DrawingServiceSingleton.Instance.DrawingService.ListOfStrokes.Count == 0)
                    return;
            }

            if (DrawingServiceSingleton.Instance.DrawingService.Editor.lines.Count != 0)
            {
                DrawingServiceSingleton.Instance.DrawingService.PrepareVisualization();
            }



            SetDrawingMode();

            OnGuesserCanvasWindowView.Invoke();
            OnShowPreviewDrawing.Invoke();
        }

        private void DrawCommand(object obj)
        {
            OnDrawView.Invoke();
        }

        

        private bool CanSaveGameImage()
        {
            if (Guard.IsNullOrEmpty(CurrentWord))
            {
                MessageBox.Show(CurrentWordEmptyMessage);
                return false;
            }
            if (Hints.Count == 0)
            {
                MessageBox.Show(HintListEmptyMessage);
                return false;
            }
            if (DrawingServiceSingleton.Instance.DrawingService.ListOfStrokes.Count == 0)
            {
                MessageBox.Show(ListOfStrokesEmptyMessage);
                return false;
            }

            return true;
        }

        private void SetDrawingMode()
        {
            var listOfStrokes = new List<Models.Game.Path>();
            foreach (var stroke in DrawingServiceSingleton.Instance.DrawingService.ListOfStrokes)
                listOfStrokes.Add(stroke.DeepCopy());

            DrawingServiceSingleton.Instance.DrawingService.ListOfStrokes.Clear();

            if (_drawingMode == DRAWING_MODES.CENTERED)
                DrawingServiceSingleton.Instance.DrawingService.ListOfStrokes = StrokesSort(listOfStrokes, _centerDrawingDirection);
            else if (_drawingMode == DRAWING_MODES.PANORAMIC)
                DrawingServiceSingleton.Instance.DrawingService.ListOfStrokes = StrokesSort(listOfStrokes, _panoramicDrawingDirection);
            else
                DrawingServiceSingleton.Instance.DrawingService.ListOfStrokes = ShuffleList(listOfStrokes);
        }

        private Image DownloadImage(string fromUrl)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    using (Stream stream = webClient.OpenRead(fromUrl))
                    {
                        return Image.FromStream(stream);
                    }
                }
                catch (ExternalException)
                {
                    return null;
                }
            }
        }

        private void OnSelectSearchImage(string link)
        {
            if (link == null)
                return;

            var bitmap = new Bitmap(DownloadImage(link));
            GuesserCanvasWindow(bitmap);
        }

        private void GuesserCanvasWindow(Bitmap bitmap)
        {
            if (bitmap == null)
                return;

            DrawingServiceSingleton.Instance.DrawingService.ListOfStrokes.Clear();
            DrawingServiceSingleton.Instance.DrawingService.ListOfStrokes = ConvertBitmapToStrokes(bitmap);

            SetDrawingMode();

            OnGuesserCanvasWindowView.Invoke();
            OnShowPreviewDrawing.Invoke();
        }

        private int SortPredicatCenterDirection(List<Models.Game.Path> strokes, Models.Game.Path pathOne, Models.Game.Path pathTwo, CENTER_DRAWING_DIRECTION direction)
        {
            //find the center point of coordinates
            double totalX = 0, totalY = 0, countCoordinate = 0;
            foreach (var stroke in strokes)
            {
                foreach (var coodinate in stroke.Coordinates)
                {
                    totalX += coodinate.X;
                    totalY += coodinate.Y;
                    countCoordinate++;
                }
            }
            double centerX = totalX / countCoordinate;
            double centerY = totalY / countCoordinate;

            var originineOne = pathOne.Coordinates[0];
            var originineTwo = pathTwo.Coordinates[0];
            double distanceStrokeOne = Math.Sqrt(Math.Pow(originineOne.X - centerX, 2) + Math.Pow(originineOne.Y - centerY, 2));
            double distanceStrokeTwo = Math.Sqrt(Math.Pow(originineTwo.X - centerX, 2) + Math.Pow(originineTwo.Y - centerY, 2));
            switch (direction)
            {
                case CENTER_DRAWING_DIRECTION.INSIDE_TO_OUTSIDE:
                    return distanceStrokeOne.CompareTo(distanceStrokeTwo);
                case CENTER_DRAWING_DIRECTION.OUTSIDE_TO_INSIDE:
                    return distanceStrokeTwo.CompareTo(distanceStrokeOne);
                default: return 0;
            }
        }

        private List<Models.Game.Path> StrokesSort(List<Models.Game.Path> strokes, CENTER_DRAWING_DIRECTION op)
        {
            switch (op)
            {
                case CENTER_DRAWING_DIRECTION.INSIDE_TO_OUTSIDE:
                    {
                        strokes.Sort((strokeOne, strokeTwo) =>
                        {
                            return SortPredicatCenterDirection(strokes, strokeOne, strokeTwo, CENTER_DRAWING_DIRECTION.INSIDE_TO_OUTSIDE);
                        });
                        return strokes;
                    }
                case CENTER_DRAWING_DIRECTION.OUTSIDE_TO_INSIDE:
                    {
                        strokes.Sort((strokeOne, strokeTwo) =>
                        {
                            return SortPredicatCenterDirection(strokes, strokeOne, strokeTwo, CENTER_DRAWING_DIRECTION.OUTSIDE_TO_INSIDE);
                        });
                        return strokes;
                    }

                default: return null;
            }

        }

        private int SortPredicatForPanoramicDirection(Models.Game.Path pathOne, Models.Game.Path pathTwo, PANORAMIC_DRAWING_DIRECTION direction)
        {
            var originineOne = pathOne.Coordinates[0];
            var originineTwo = pathTwo.Coordinates[0];
            switch (direction)
            {
                case PANORAMIC_DRAWING_DIRECTION.FROM_LEFT_TO_RIGHT:
                    return originineOne.X.CompareTo(originineTwo.X);
                case PANORAMIC_DRAWING_DIRECTION.FROM_RIGHT_TO_LEFT:
                    return originineTwo.X.CompareTo(originineOne.X);
                case PANORAMIC_DRAWING_DIRECTION.FROM_TOP_TO_BOTTOM:
                    return originineOne.Y.CompareTo(originineTwo.Y);
                case PANORAMIC_DRAWING_DIRECTION.FROM_BOTTOM_TO_TOP:
                    return originineTwo.Y.CompareTo(originineOne.Y);
                default: return 0;
            }
        }

        private List<Models.Game.Path> StrokesSort(List<Models.Game.Path> strokes, PANORAMIC_DRAWING_DIRECTION op)
        {
            switch (op)
            {
                case PANORAMIC_DRAWING_DIRECTION.FROM_LEFT_TO_RIGHT:
                {
                        strokes.Sort((strokeOne, strokeTwo) =>
                        {
                            return SortPredicatForPanoramicDirection(strokeOne, strokeTwo, PANORAMIC_DRAWING_DIRECTION.FROM_LEFT_TO_RIGHT) ;
                        });
                        return strokes;
                }
                case PANORAMIC_DRAWING_DIRECTION.FROM_RIGHT_TO_LEFT:
                    {
                        strokes.Sort((strokeOne, strokeTwo) =>
                        {
                            return SortPredicatForPanoramicDirection(strokeOne, strokeTwo, PANORAMIC_DRAWING_DIRECTION.FROM_RIGHT_TO_LEFT);
                        });
                        return strokes;
                    }
                case PANORAMIC_DRAWING_DIRECTION.FROM_TOP_TO_BOTTOM:
                    {
                        strokes.Sort((strokeOne, strokeTwo) =>
                        {
                            return SortPredicatForPanoramicDirection(strokeOne, strokeTwo, PANORAMIC_DRAWING_DIRECTION.FROM_TOP_TO_BOTTOM);
                        });
                        return strokes;
                    }
                case PANORAMIC_DRAWING_DIRECTION.FROM_BOTTOM_TO_TOP:
                    {
                        strokes.Sort((strokeOne, strokeTwo) =>
                        {
                            return SortPredicatForPanoramicDirection(strokeOne, strokeTwo, PANORAMIC_DRAWING_DIRECTION.FROM_BOTTOM_TO_TOP);
                        });
                        return strokes;
                    }

                default: return null;
            }

        }

        private List<E> ShuffleList<E>(List<E> inputList)
        {
            List<E> randomList = new List<E>();

            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count);
                randomList.Add(inputList[randomIndex]);
                inputList.RemoveAt(randomIndex);
            }

            return randomList;
        }
    }

    public enum PANORAMIC_DRAWING_DIRECTION
    {
        FROM_LEFT_TO_RIGHT,
        FROM_RIGHT_TO_LEFT,
        FROM_TOP_TO_BOTTOM,
        FROM_BOTTOM_TO_TOP
    }

    public enum CENTER_DRAWING_DIRECTION
    {
        INSIDE_TO_OUTSIDE,
        OUTSIDE_TO_INSIDE
    }
}
