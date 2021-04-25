using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Threading;
namespace PolyPaint.Vues.Account
{
    /// <summary>
    /// Logique d'interaction pour Confettis.xaml
    /// </summary>
    public partial class Confettis : Window
    {
        int Counter = 0;
        private DispatcherTimer Timer;
        public Confettis()
        {
            InitializeComponent();
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += Timer_Tick;
            Timer.Start();

            System.Media.SoundPlayer Victory = new System.Media.SoundPlayer(@"..\..\Resources\Sound\Victory.wav");
            Victory.Play();
        }
        void Timer_Tick(object sender , EventArgs e) 
        {
            Counter++;
            if (Counter == 10)
                this.Close();
        }
    }
}
