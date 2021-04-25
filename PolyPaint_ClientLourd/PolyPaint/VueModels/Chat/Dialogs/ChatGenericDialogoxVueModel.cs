using PolyPaint.AppContext;
using PolyPaint.Server;
using PolyPaint.Services;
using PolyPaint.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace PolyPaint.VueModels.Chat
{
    class CreateChannelViewModel
    {
        public RelayCommand<object> CreateChannelCommand { get; set; }
        public RelayCommand<object> CancelCreateChannelCommand { get; set; }
        public string ChannelName { get; set; }
        private bool hasInput;

        public delegate void TaskFinished();
        public static  TaskFinished TaskFinishedEvent = delegate { };

        public CreateChannelViewModel(bool hasInput)
        {
            CreateChannelCommand = new RelayCommand<object>(CreateChannel);
            CancelCreateChannelCommand = new RelayCommand<object>(CancelCreateChannel);

            this.hasInput = hasInput;
        }

        private void CreateChannel(object o)
        {

            if (hasInput && string.IsNullOrEmpty(ChannelName))
                return;

            if(hasInput && !string.IsNullOrEmpty(ChannelName))
            {
                MessagingService.Instance.CreateConversation(ChannelName);
            }

            CloseDialogWithResult(o as Window, DialogResult.Yes,true);
        }

        private void CancelCreateChannel(object o)
        {
            CloseDialogWithResult(o as Window, DialogResult.Cancel,false);
        }

        public void CloseDialogWithResult(Window dialog, DialogResult result,bool isSucces)
        {
            if (dialog != null)
                dialog.DialogResult = isSucces;
        }
    }
}
