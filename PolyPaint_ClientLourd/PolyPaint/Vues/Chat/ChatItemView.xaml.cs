using PolyPaint.Models;
using System.Windows.Controls;

namespace PolyPaint.Vues.Chat
{
    /// <summary>
    /// Logique d'interaction pour ChatItemView.xaml
    /// </summary>
    public partial class ChatItemView : UserControl
    {
        public Message Message { get; set; }
        public string UserName { get; set; }
        public string PostionValue { get; set; }
        public string BackGroundColor { get; set; }
        public string ForeGroundColor { get; set; }
        public ChatItemView()
        {
            //DataContext = this;
            //Message = message.DeepCopy();
            Message = new Message();
            PostionValue = "Right";
            BackGroundColor = "#2796ED";
            ForeGroundColor = "#FFF";

            if (Message.User.UID == AppContext.AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID){
                PostionValue = "Right";
                BackGroundColor = "#2796ED";
                ForeGroundColor = "#FFF";
            }
            else
            {
                PostionValue = "Left";
                BackGroundColor = "#F1F1F1";
                ForeGroundColor = "#333";
            }

            InitializeComponent();
            PrintChatView();
        }

        public void PrintChatView()
        {
            var horizontalAlignement = Message.User.UID == AppContext.AppContextSingleton.Instance.AppContext.CurrentConnectedUser.UID ?
                                        System.Windows.HorizontalAlignment.Left : System.Windows.HorizontalAlignment.Right;


            txtTimeStamp.HorizontalAlignment = horizontalAlignement ;
            txtMessageContext.HorizontalAlignment = horizontalAlignement;
            stackPannel.HorizontalAlignment = horizontalAlignement;
            txtUsername.HorizontalAlignment = horizontalAlignement;
            grid.HorizontalAlignment = horizontalAlignement;
        }
    }
}
