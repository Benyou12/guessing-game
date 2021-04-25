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
    class RegisterViewModel : INotifyPropertyChanged
    {

        public RegisterRequestModel RegisterRequestModel {get;set;}
        public RelayCommand<object> RegistrationCommand { get; set; }
        public RelayCommand<object> ImportImageCommand { get; set; }

        private string userURI;
        private BitmapSource bitmapSource;
        private readonly string userRegisteredMessage = "L'utilisateur enregistré.";
        private readonly string registerMainMessage = "Veuillez saisir un courriel et un mot de passe.";
        private readonly string usernameInputMessageError = "Veuillez saisir une adresse courriel valide.";
        private readonly string nameInputMessageError = "Veuillez saisir un prénom et un nom.";

        public RegisterViewModel()
        {
            RegistrationCommand = new RelayCommand<object>(Register);
            ImportImageCommand = new RelayCommand<object>(ImportImage);
            RegisterRequestModel = new RegisterRequestModel();
            IsLoading = false;

        }

        private void ImportImage(object o)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";

            if (op.ShowDialog() == true)
            {
                var test = new BitmapImage(new System.Uri(op.FileName));
                RegisterRequestModel.LocalImageURI = op.FileName;
                RegisterRequestModel.PlayerImage = test;
            }
        }

        public BitmapSource BitmapImage
        {
            get { return bitmapSource; }
        }

        private void Register(object o)
        {
            RegisterRequestModel.Email = RegisterRequestModel.Email?.Trim();
            string password = (o as PasswordBox).Password;

            if (!ValidateInput(password))
                return;

            //TODO : Use the RegisterRequest in form to take directly the password as input
            RegisterRequestModel.Password = password;

            IsLoading = true;

            var requestResponse = Authentification.SignUp(this.RegisterRequestModel);

            if (requestResponse.Success){

                MainTransitions.TransitionPageControl.ShowPage(new Tutorial());
            }
            else
            {
                MessageBox.Show(requestResponse.Response);
            }
           
          
        }

        /// <summary>
        /// This method validate the user input
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        private bool ValidateInput(string pwd)
        {
            //todo : modify the method to take more than one parameter

            if (Guard.IsNullOrEmpty(RegisterRequestModel.Email) ||
                Guard.IsNullOrEmpty(pwd) ||
                Guard.IsNullOrEmpty(RegisterRequestModel.Username) ||
                Guard.IsNullOrEmpty(RegisterRequestModel.FirstName))
            {
                MessageBox.Show(this.registerMainMessage);

                return false;
            }

            if (Guard.IsNullOrEmpty(RegisterRequestModel.Email) || !Guard.IsValidEmail(RegisterRequestModel.Email))
            {
                MessageBox.Show(this.usernameInputMessageError);
                return false;
            }

            if (Guard.IsNullOrEmpty(RegisterRequestModel.FirstName) || Guard.IsNullOrEmpty(RegisterRequestModel.FirstName))
            {
                MessageBox.Show(this.nameInputMessageError);
                return false;
            }

            if (Guard.IsNullOrEmpty(RegisterRequestModel.LastName) || Guard.IsNullOrEmpty(RegisterRequestModel.LastName))
            {
                MessageBox.Show(this.nameInputMessageError);
                return false;
            }

            if (Guard.IsNullOrEmpty(RegisterRequestModel.FirstName) || Guard.IsNullOrEmpty(RegisterRequestModel.FirstName))
            {
                MessageBox.Show(this.nameInputMessageError);
                return false;
            }

            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool _isLoading { get; set; }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                NotifyPropertyChanged();
            }
        }
    }
}
