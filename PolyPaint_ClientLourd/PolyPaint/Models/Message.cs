using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PolyPaint.Models
{
    public class Message : INotifyPropertyChanged
    {
        public string MID { get; set;}
        public User User { get; set; }
        public long Timestamp { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;        
        public string Text
        {
            get { return text; }
            set
            {
                if (value != text)
                {
                    text = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private string text;

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Message()
        {
            MID = string.Empty;
            User = new User();
            Text = string.Empty;
            Timestamp = 0;
        }

        public Message DeepCopy()
        {
            Message other = (Message)this.MemberwiseClone();
            other.User = User;
            other.Timestamp = Timestamp;
            other.Text = String.Copy(Text);
            return other;
        }
    }
}
