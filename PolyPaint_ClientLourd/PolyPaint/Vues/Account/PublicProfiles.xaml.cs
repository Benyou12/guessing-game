using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PolyPaint.Utils;
using System.Windows.Media;
using PolyPaint.Server;
using PolyPaint.VueModels.Account;

namespace PolyPaint.Vues.Account
{
    /// <summary>
    /// Logique d'interaction pour PublicProfiles.xaml
    /// </summary>
    public partial class PublicProfiles : UserControl
    {
        public PublicProfiles()
        {
            DataContext = new PublicProfilesViewModel();
            InitializeComponent();
        }
    }
}
