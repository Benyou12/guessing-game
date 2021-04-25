using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using PolyPaint.AppContext;
using PolyPaint.Models;
using PolyPaint.Models.Game;
using PolyPaint.Models.Lobby;
using PolyPaint.Services;
using PolyPaint.Utils;

namespace PolyPaint.VueModels
{

    /// <summary>
    /// Sert d'intermédiaire entre la vue et le modèle.
    /// Expose des commandes et propriétés connectées au modèle aux des éléments de la vue peuvent se lier.
    /// Reçoit des avis de changement du modèle et envoie des avis de changements à la vue.
    /// </summary>
    class DrawingVueModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Editor editor = new Editor();

        // Ensemble d'attributs qui définissent l'apparence d'un trait.
        public DrawingAttributes DrawingAttributes { get; set; } = new DrawingAttributes();

        public Game CurrentGame { get { return GameService.Instance.CurrentGame; } }
        public Team TeamOne { get { return CurrentGame.TeamOne; } }
        public Team TeamTwo { get { return CurrentGame.TeamTwo; } }

        public string SelectedTool
        {
            get { return editor.SelectedTool; }
            set { ModifiedProperty(); }
        }

        public string SelectedDrawingShape
        {
            get { return editor.SelectedDrawingShape; }
            set { ModifiedProperty(); }
        }

        public string SelectedColor
        {
            get { return editor.SelectedColor; }
            set { editor.SelectedColor = value; }
        }

        public int LineSize
        {
            get { return editor.LineSize; }
            set { editor.LineSize = value; }
        }

        public StrokeCollection Lines { get; set; }
        public string WordToDraw
        {
            get
            {
                if (GameService.Instance.CurrentGame != null)
                {
                    return GameService.Instance.CurrentGame.CurrentRound.GameImage.Word;
                }

                return string.Empty;
            }
        }

        private DrawingService drawingService;


        // Commandes sur lesquels la vue pourra se connecter.
        public RelayCommand<string> SelectTool { get; set; }
        public RelayCommand<string> SelectDrawingShape { get; set; }
        public RelayCommand<object> Reset { get; set; }
        public RelayCommand<Point> MouseUp { get; set; }
        public RelayCommand<Point> MouseDown { get; set; }
        public RelayCommand<Point> MouseMove { get; set; }

        /// <summary>
        /// Constructeur de VueModele
        /// On récupère certaines données initiales du modèle et on construit les commandes
        /// sur lesquelles la vue se connectera.
        /// </summary>
        public DrawingVueModel()
        {
            // On écoute pour des changements sur le modèle. Lorsqu'il y en a, EditeurProprieteModifiee est appelée.
            editor.PropertyChanged += new PropertyChangedEventHandler(EditorModifiedProperty);

            // On initialise les attributs de dessin avec les valeurs de départ du modèle.
            this.DrawingAttributes = new DrawingAttributes();
            this.DrawingAttributes.Color = (Color)ColorConverter.ConvertFromString(editor.SelectedColor);
            AdjustTip();


            Lines = editor.lines;

            // Pour les commandes suivantes, il est toujours possible des les activer.
            // Donc, aucune vérification de type Peut"Action" à faire.
            SelectTool = new RelayCommand<string>(editor.SelectTool);
            SelectDrawingShape = new RelayCommand<string>(editor.SelectDrawingShape);

            Lines.StrokesChanged += Lines_StrokesChanged;

            MouseDown = new RelayCommand<Point>(Down);
            MouseUp = new RelayCommand<Point>(Up);
            MouseMove = new RelayCommand<Point>(Move);

            drawingService = DrawingServiceSingleton.Instance.DrawingService.IsCreatingGame ? DrawingServiceSingleton.Instance.DrawingService : new DrawingService();
            drawingService.Editor = editor;
        }

        private void Lines_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            if (SelectedTool == "crayon")
            {
                if (e.Added != null && e.Added.Count > 0)
                {
                    foreach (Stroke s in e.Added)
                    {
                        drawingService.AddStroke(s);
                    }
                }
            }
            else if (e.Removed != null && e.Removed.Count > 0)
            {
                drawingService.UpdateStrokeList(e.Removed, e.Added);
            }
        }

        public void Up(Point point)
        {
            drawingService.Up(point, SelectedTool);
        }

        public void Down(Point point)
        {
            drawingService.Down(point, SelectedTool);
        }

        public void Move(Point point)
        {
            drawingService.Move(point, SelectedTool);
        }

        /// <summary>
        /// Appelee lorsqu'une propriété de VueModele est modifiée.
        /// Un évènement indiquant qu'une propriété a été modifiée est alors émis à partir de VueModèle.
        /// L'évènement qui contient le nom de la propriété modifiée sera attrapé par la vue qui pourra
        /// alors mettre à jour les composants concernés.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée.</param>
        protected virtual void ModifiedProperty([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Traite les évènements de modifications de propriétés qui ont été lancés à partir
        /// du modèle.
        /// </summary>
        /// <param name="sender">L'émetteur de l'évènement (le modèle)</param>
        /// <param name="e">Les paramètres de l'évènement. PropertyName est celui qui nous intéresse. 
        /// Il indique quelle propriété a été modifiée dans le modèle.</param>
        private void EditorModifiedProperty(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedColor")
            {
                DrawingAttributes.Color = (Color)ColorConverter.ConvertFromString(editor.SelectedColor);
                SelectedTool = "crayon";
            }
            else if (e.PropertyName == "SelectedTool")
            {
                SelectedTool = editor.SelectedTool;
            }
            else if (e.PropertyName == "SelectedDrawingShape")
            {
                SelectedDrawingShape = editor.SelectedDrawingShape;
                AdjustTip();
            }
            else // e.PropertyName == "TailleTrait"
            {
                AdjustTip();
            }
        }

        /// <summary>
        /// C'est ici qu'est défini la forme de la pointe, mais aussi sa taille (TailleTrait).
        /// Pourquoi deux caractéristiques se retrouvent définies dans une même méthode? Parce que pour créer une pointe 
        /// horizontale ou verticale, on utilise une pointe carrée et on joue avec les tailles pour avoir l'effet désiré.
        /// </summary>
        private void AdjustTip()
        {
            DrawingAttributes.StylusTip = (editor.SelectedDrawingShape == "ronde") ? StylusTip.Ellipse : StylusTip.Rectangle;
            DrawingAttributes.Width = (editor.SelectedDrawingShape == "verticale") ? 1 : editor.LineSize;
            DrawingAttributes.Height = (editor.SelectedDrawingShape == "horizontale") ? 1 : editor.LineSize;
        }
    }
}
