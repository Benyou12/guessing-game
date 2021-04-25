using PolyPaint.Models.Lobby;
using PolyPaint.VueModels.Chat.Dialogs;
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
using System.Windows.Shapes;

namespace PolyPaint.Vues.Chat.Dialogs
{
    /// <summary>
    /// Logique d'interaction pour AddUserChatDialog.xaml
    /// </summary>
    public partial class AddUserChatDialog : Window
    {
        public AddUserChatDialog()
        {
            DataContext = new AddUserChatDialogVueModel();

            SizeToContent = SizeToContent.WidthAndHeight;
            InitializeComponent();
        }
    }
}
