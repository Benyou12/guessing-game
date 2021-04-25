using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PolyPaint.Server;
using PolyPaint.Services;
using PolyPaint.Utils;
using PolyPaint.Utils.Localization;
using PolyPaint.VueModels.Account;

namespace PolyPaint.Vues.Account
{
    /// <summary>
    /// Logique d'interaction pour Tutorial.xaml
    /// </summary>
    /// 
    public partial class Tutorial : UserControl
    {

        //  The current Step in the Tutorial View
        private int Step;
        //  The Last Step in the Tutorial View
        private int MaxStep;

        public Tutorial()
        {
            DataContext = new TutorialViewModel();
            InitializeComponent();
            Step = 1;
            MaxStep = 7;
            ShowStep();
        }


        private void Next(object sender, MouseButtonEventArgs e)
        {
            if (Step < MaxStep)
                Step++;
            ShowStep();
        }

        private void Previous(object sender, MouseButtonEventArgs e)
        {
            if (Step>1)
            Step--;
            ShowStep();
        }
        private void ShowStep()
        {
            ((TutorialViewModel)DataContext).IsFirstStep = Step == 1;
            ((TutorialViewModel)DataContext).IsLastStep = Step == MaxStep;

            string Path = @"..\..\Resources\Tutorial\step";
            Path = Path + Step + ".png";

            ((TutorialViewModel)DataContext).Title = TitleText(Step);
            ((TutorialViewModel)DataContext).Description = DescriptionText(Step);
            ImageStep.Source = new BitmapImage(new Uri(Path, UriKind.Relative));
        }

        private string DescriptionText (int step)
        {
            string descrition;
            if (Global.language == "fr-Fr")
              switch (step)
              {
                 case 1:
                        descrition = "Vous pouvez rejoindre un groupe (moins de 4 joueurs) pour commencer la partie.";
                    break;
                 case 2:
                        descrition = "En attendant de compléter le nombre de joueurs (4 joueurs)  nécessaire pour commencer la partie, le bouton de “Commencer la partie” est désactivé. Une fois le nombre du groupe est 4, appuyez sur le bouton “Commencer la partie” pour avoir la division d’équipe proposée.";
                    break;
                 case 3:
                        descrition = "Appuyez  sur le bouton “Commencer la partie”, l’application divisera le groupe en deux équipes.";
                    break;
                 case 4:
                        descrition = "À votre tour, un membre de votre équipe est autorisé à dessiner le mot proposé en haut pour le dessiner en bas dans le canvas.";
                     break;
                 case 5:
                        descrition = "L'autre membre de votre équipe essaye de deviner le mot en envoyant la réponse.";
                     break;
                 case 6:
                        descrition = "Si la réponse est incorrect, l'équipe adversaire peut essayer de deviner à son tour.";
                     break;
                 case 7:
                        descrition = "L'équipe qui aura le meilleur score en devinant le maximum de mots gagnera.";
                     break;
                    default:
                        descrition = "Description " + step + "en Français";
                    break;
                }
              else
                switch (step)
                {
                    case 1:
                        descrition = "You can join a group (less than 4 players) to start the game.";
                        break;
                    case 2:
                        descrition = "While waiting to complete the number of players (4 players) needed to start the game, the “Start Game” button is disabled. Once the number of the group is 4, press the “Start Game” button to have the proposed team division.";
                        break;
                    case 3:
                        descrition = "Press the “Start Game” button, the app will divide the group into two teams.";
                        break;
                    case 4:
                        descrition = "In your turn, a member of your team is allowed to draw the proposed word at the top to draw it down in the canvas.";
                        break;
                    case 5:
                        descrition = "The other member of your team tries to guess the word by sending the answer.";
                        break;
                    case 6:
                        descrition = "If the answer is incorrect, the opposing team may try to guess in turn.";
                        break;
                    case 7:
                        descrition = "The team with the highest score by guessing the maximum number of words will win.";
                        break;
                    default:
                        descrition = "Description " + step + " in English";
                        break;
                }

            return descrition;
        }
        private string TitleText(int step)
        {
            string Title;
            if (Global.language == "fr-Fr")
                switch (step)
                {
                    case 1:
                        Title = "Rejoindre un groupe";
                        break;
                    case 2:
                        Title = "En attente de joueurs";
                        break;
                    case 3:
                        Title = "Début de la partie";
                        break;
                    case 4:
                        Title = "Votre équipe dessine";
                        break;
                    case 5:
                        Title = "Votre coéquipier devine le dessin";
                        break;
                    case 6:
                        Title = "Le droit de réplique";
                        break;
                    case 7:
                        Title = "Gagner la partie";
                        break;
                    default:
                        Title = "Gagner la partie";
                        break;
                }
            else
                switch (step)
                {
                    case 1:
                        Title = "Join a group";
                        break;
                    case 2:
                        Title = "Waiting for player(s)";
                        break;
                    case 3:
                        Title = "Start  the game";
                        break;
                    case 4:
                        Title = "Your team draws";
                        break;
                    case 5:
                        Title = "Your teammate guesses the drawing";
                        break;
                    case 6:
                        Title = "The right of reply";
                        break;
                    case 7:
                        Title = "Win the game";
                        break;
                    default:
                        Title = "";
                        break;
                }

            return Title;
        }

        public void GoToGame(object o, RoutedEventArgs e)
        {
            MainTransitions.TransitionPageControl.ShowPage(new Home());
        }
    }
}
