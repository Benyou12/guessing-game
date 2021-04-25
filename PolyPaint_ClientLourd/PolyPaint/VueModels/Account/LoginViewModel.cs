using PolyPaint.AppContext;
using PolyPaint.Models;
using PolyPaint.Server;
using PolyPaint.Utils;
using PolyPaint.Vues;
using PolyPaint.Vues.Account;
using PolyPaint.Vues.Chat;
using PolyPaint.Vues.Lobby;
using System;
using PolyPaint.Vues.Account;
using System.Windows;
using System.Windows.Controls;
using PolyPaint.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Threading;

namespace PolyPaint.VueModels.Account
{
    class LoginViewModel : INotifyPropertyChanged
    {
        public RelayCommand<object> ConnectionCommand { get; set; }
        public LogInRequestModel LoginRequestModel { get; set; }

        public RelayCommand<string> LoginWithOtherAuthCommand { get; set; }

        private string loginMessage = "Veuillez saisir un courriel et un mot de passe.";
        private string emailInputErrorMessage = "Veuillez saisir une adresse courriel valide.";
        private string connexionSuccessfullMessage = "L'utilisateur connecte";
        private string connectionErrorMessage = "Connexion error";
        private string authentificationToken;
        private readonly string  pageUrl = "https://log3900.lbacreations.com/auth/{0}/?login_id={1}";


        public LoginViewModel()
        {
            ConnectionCommand = new RelayCommand<object>(Login);
            LoginWithOtherAuthCommand = new RelayCommand<string>(LoginWithOtherProvider);
            LoginRequestModel= new LogInRequestModel();
            authentificationToken = string.Empty;

            InitializeServices();
            WebSocket.Instance.ReceivedUserInformation += OnUserInformationReceived;
        }

        public void InitializeServices()
        {
            WebSocket.Instance.Initialize();
            MessagingService.Instance.Initialize();
            GameService.Instance.Initialize();
            IsLoading = false;
        }

        public void OnUserInformationReceived(OAuthLoginResponse loginResponse)
        {
            if (loginResponse.LoginID != authentificationToken)
                return;


            if (loginResponse.IsLoggedIn)
            {
                IsLoading = false;
                AppContextSingleton.Instance.AppContext.CurrentConnectedUser = loginResponse.User;
                GoToHome();
            }
            else
            {

                App.Current.Dispatcher.BeginInvoke((Action)delegate

                {
                    var registerView = new RegisterView();
                    var dataContext = registerView.DataContext as RegisterViewModel;

                    if (dataContext != null)
                    {
                        dataContext.RegisterRequestModel.FirstName = loginResponse.User.FirstName;
                        dataContext.RegisterRequestModel.LastName = loginResponse.User.LastName;
                        dataContext.RegisterRequestModel.Email = loginResponse.User.Email;
                        dataContext.RegisterRequestModel.ProfileImgUrl = loginResponse.User.ProfileImgURL;

                        MainTransitions.TransitionPageControl.ShowPage(registerView);
                    }
                });
            }
        }

        private void GoToHome()
        {
            App.Current.Dispatcher.BeginInvoke((Action)delegate

            {
                WebSocket.Instance.JoinRoom();
                MainTransitions.TransitionPageControl.ShowPage(new Home());
                WebSocket.Instance.LoginCompleted();
            });
        }

        private void Login(object o)
        {
            IsLoading = true;
            LoginRequestModel.Email = LoginRequestModel.Email.Trim();
            LoginRequestModel.Password = (o as PasswordBox).Password;

            if (!ValidateInput(LoginRequestModel.Password))
            {
                IsLoading = false;
                return;
            }


            Thread thread = new System.Threading.Thread(
                new System.Threading.ThreadStart(
                    delegate () {

                        var requestResult = Authentification.SignIn(LoginRequestModel);

                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                            new Action(() => {
                                // here we are back in the UI thread

                                if (requestResult.Success)
                                {
                                    IsLoading = false;
                                    GoToHome();
                                }
                                else
                                {
                                    IsLoading = false;
                                    MessageBox.Show(requestResult.Response);
                                }

                            }));
                    }
                )
            );

            thread.Start();
            
        }

        private void LoginWithOtherProvider(string provider)
        {
            var tempLoginId = GenerateRandomString();
           authentificationToken = tempLoginId;           

            System.Diagnostics.Process.Start(string.Format(pageUrl,provider,tempLoginId));
        }

        private string GenerateRandomString()
        {
            var guid = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(guid.ToByteArray());

            return GuidString;
        }

        private bool ValidateInput(string pwd)
        {

            if (Global.language == "en-US")
            {
                this.loginMessage = "Please enter an email and a password.";
                this.emailInputErrorMessage = "Please enter a valid email address.";
                this.connexionSuccessfullMessage = "The user is connected";
                this.connectionErrorMessage = "Connexion error";
            }
            else
            {
                this.loginMessage = "Please enter an email and a password.";
                this.emailInputErrorMessage = "Veuillez saisir une adresse courriel valide.";
                this.connexionSuccessfullMessage = "L'utilisateur connecte";
                this.connectionErrorMessage = "Erreur de connexion";
            }

        


            if (Guard.IsNullOrEmpty(LoginRequestModel.Email) || Guard.IsNullOrEmpty(pwd))
            {
                MessageBox.Show(this.loginMessage);
                return false;
            }

            if (Guard.IsNullOrEmpty(LoginRequestModel.Email) || !Guard.IsValidEmail(LoginRequestModel.Email))
            {
                MessageBox.Show(this.emailInputErrorMessage);
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
