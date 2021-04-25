using PolyPaint.VueModels.Chat;
using System.Windows;

namespace PolyPaint.Vues.Chat
{
    /// <summary>
    /// I am using this class as a dialog presenter for our application
    /// It will allow us to incorporate the dark mode easily and it is reusable
    /// </summary>
    public partial class ChatGenericDialogBoxVue : Window
    {
        public ChatGenericDialogBoxVue(string pannelInformation, bool showInput,string confirmBtnContent, string cancelBtnContent)
        {
            SizeToContent = SizeToContent.WidthAndHeight;
            DataContext = new CreateChannelViewModel(showInput);
            InitializeComponent();
            dialogInformationTxt.Text = pannelInformation;
            dialogInputTxt.Visibility = showInput ? Visibility.Visible : Visibility.Collapsed;

            cancelBtn.Content = cancelBtnContent;
            confirmBtn.Content = confirmBtnContent;
            Owner = App.Current.MainWindow;
        }
    }
}
