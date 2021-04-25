using PolyPaint.VueModels.Chat;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PolyPaint.Utils;
using System.Windows.Media;
using PolyPaint.Server;
using MaterialDesignThemes.Wpf;

namespace PolyPaint.Vues.Chat
{
    /// <summary>
    /// Logique d'interaction pour ChatView.xaml
    /// </summary>
    public partial class ChatView : UserControl
    {
        private Boolean AutoScroll = true;

        public ChatView()
        {
            DataContext = new ChatViewModel();
            InitializeComponent();
            Mode(Global.mode);
        }


        private void ScrollViewer_ScrollChanged(Object sender, ScrollChangedEventArgs e)
        {
            // User scroll event : set or unset auto-scroll mode
            var scrollViewer = sender as ScrollViewer;
            if (e.ExtentHeightChange == 0)
            {   // Content unchanged : user scroll event
                if (scrollViewer.VerticalOffset == scrollViewer.ScrollableHeight)
                {   // Scroll bar is in bottom
                    // Set auto-scroll mode
                    AutoScroll = true;
                }
                else
                {   // Scroll bar isn't in bottom
                    // Unset auto-scroll mode
                    AutoScroll = true;
                }
            }

            // Content scroll event : auto-scroll eventually
            if (AutoScroll && e.ExtentHeightChange != 0)
            {   // Content changed and auto-scroll mode set
                // Autoscroll
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
            }
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            if (txtBox.Text == "Saisie un message...")
                txtBox.Text = string.Empty;
        }

        private void Mode(string mode)
        {
            if (mode == "Light")
            {
                // White
                //Page.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                
                //Rectangle.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
               
                // Black
                //ChannelName.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
 
            }
            if (mode == "Dark")
            {
                // Black - Gray
                //Page.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
              
                // Rectangle.Background = new SolidColorBrush(Color.FromRgb(52, 52, 61));
               
               // White
               //ChannelName.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
 
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((ChatViewModel)this.DataContext).GoToConversationsCommand.Execute(sender);
        }

        private void RegisterRedirectLink_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            RegisterRedirectLink.Visibility = Visibility.Collapsed;
            ((ChatViewModel)this.DataContext).ShowUserHistoryCommand.Execute(sender);
        }

        private void DetachChat(object sender, RoutedEventArgs e)
        {
            WebSocket.Instance.DetachChat();
        }
    }
}
