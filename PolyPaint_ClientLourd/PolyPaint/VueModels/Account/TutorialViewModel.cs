using PolyPaint.Utils;
using PolyPaint.Vues;
using PolyPaint.Vues.Account;
using System.Windows;
using System.Windows.Controls;
using PolyPaint.Server;
using PolyPaint.Models;
using PolyPaint.AppContext;
using PolyPaint.Vues.Chat;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System;
using System.Runtime.CompilerServices;

namespace PolyPaint.VueModels.Account
{
    class TutorialViewModel : INotifyPropertyChanged
    {

        public TutorialViewModel()
        {
            IsFirstStep = true;
            IsLastStep = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isFirstStep { get; set; }

        public bool IsFirstStep
        {
            get { return _isFirstStep; }
            set
            {
                _isFirstStep = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isLastStep { get; set; }

        public bool IsLastStep
        {
            get { return _isLastStep; }
            set
            {
                _isLastStep = value;
                NotifyPropertyChanged();
            }
        }

        private string _title { get; set; }

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                NotifyPropertyChanged();
            }
        }

        private string _description { get; set; }

        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                NotifyPropertyChanged();
            }
        }

    }
}
