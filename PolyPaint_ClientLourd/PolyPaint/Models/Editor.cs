using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Ink;

namespace PolyPaint.Models
{
    /// <summary>
    /// Modélisation de l'éditeur.
    /// Contient ses différents états et propriétés ainsi que la logique
    /// qui régis son fonctionnement.
    /// </summary>
    class Editor : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public StrokeCollection lines = new StrokeCollection();
        private StrokeCollection removedLines = new StrokeCollection();

        // Outil actif dans l'éditeur
        private string selectedTool = "crayon";
        public string SelectedTool
        {
            get { return selectedTool; }
            set { selectedTool = value; OnPropertyChanged(); }
        }

        // Couleur des traits tracés par le crayon.
        private string selectedColor = "Black";
        public string SelectedColor
        {
            get { return selectedColor; }
            // Lorsqu'on sélectionne une couleur c'est généralement pour ensuite dessiner un trait.
            // C'est pourquoi lorsque la couleur est changée, l'outil est automatiquement changé pour le crayon.
            set
            {
                selectedColor = value;
                SelectedTool = "crayon";
                OnPropertyChanged();
            }
        }

        private string selectedDrawingShape = "ronde";
        public string SelectedDrawingShape
        {
            get { return selectedDrawingShape; }
            set
            {
                SelectedTool = "crayon";
                selectedDrawingShape = value;
                OnPropertyChanged();
            }
        }

        // Grosseur des traits tracés par le crayon.
        private int lineSize = 11;
        public int LineSize
        {
            get { return lineSize; }
            // Lorsqu'on sélectionne une taille de trait c'est généralement pour ensuite dessiner un trait.
            // C'est pourquoi lorsque la taille est changée, l'outil est automatiquement changé pour le crayon.
            set
            {
                lineSize = value;
                SelectedTool = "crayon";
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Appelee lorsqu'une propriété d'Editeur est modifiée.
        /// Un évènement indiquant qu'une propriété a été modifiée est alors émis à partir d'Editeur.
        /// L'évènement qui contient le nom de la propriété modifiée sera attrapé par VueModele qui pourra
        /// alors prendre action en conséquence.
        /// </summary>
        /// <param name="propertyName">Nom de la propriété modifiée.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SelectDrawingShape(string shape) => SelectedDrawingShape = shape;

        // L'outil actif devient celui passé en paramètre.
        public void SelectTool(string tool) => SelectedTool = tool;
        
    }
}