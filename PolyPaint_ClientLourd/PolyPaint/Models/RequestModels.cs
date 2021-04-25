using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PolyPaint.Models
{
    /// <summary>
    /// This class represent the model that will be send to the server for login as JSON
    /// </summary>
    public class LogInRequestModel
    {
        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }

        public LogInRequestModel()
        {
            Email = string.Empty;
            Password = string.Empty;
        }
    }

    public class OAuthLoginResponse
    {
        [JsonProperty(PropertyName = "login_id")]
        public string LoginID { get; set; }

        [JsonProperty(PropertyName = "isLogin")]
        public bool IsLoggedIn { get; set; }

        [JsonProperty(PropertyName = "user")]
        public User User { get; set; }

    }

    public class LogOutRequestModel
    {
        [JsonProperty(PropertyName = "uid")]
        public string UID { get; set; }

        public LogOutRequestModel()
        {
            UID = string.Empty;
        }
    }

    /// <summary>
    /// This class represent the nodel that will be send to the server for Register
    /// </summary>
    public class RegisterRequestModel : INotifyPropertyChanged
    {
        private string email;
        [JsonProperty(PropertyName = "email")]
        public string Email
        {
            get
            {
                return email;
            }
            set
            {

                if (email == value)
                    return;
                email = value;
                NotifyPropertyChanged();
            }

        }

        [JsonProperty(PropertyName = "password")]
        public string Password { get; set; }


        private string username;
        [JsonProperty(PropertyName = "username")]
        public string Username { get
            {
                return username;
            }
            set
            {
                if (username == value)
                    return;

                username = value;
                NotifyPropertyChanged();
            }
        }

        private string firstName;
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName
        {
            get
            {
                return firstName;
            }

            set
            {
                if (firstName == value)
                    return;

                firstName = value;
                NotifyPropertyChanged();
            }
        }


        private string lastName;
        [JsonProperty(PropertyName = "lastName")]
        public string LastName
        {
            get
            {
                return lastName;
            }
            set
            {
                if (lastName == value)
                    return;

                lastName = value;
                NotifyPropertyChanged();
            }
        }

        private string profileImgUrl;
        [JsonProperty(PropertyName = "profileImgUrl")]
        public string ProfileImgUrl
        {
            get
            {
                return profileImgUrl;
            }
            set
            {
                if (profileImgUrl == value)
                    return;

                profileImgUrl = value;
                if (!string.IsNullOrEmpty(ProfileImgUrl))
                    PlayerImage = new BitmapImage(new Uri(ProfileImgUrl));
                NotifyPropertyChanged();

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [JsonIgnore]
        private BitmapSource playerImage;

        [JsonIgnore]
        public BitmapSource PlayerImage
        {
            get { return playerImage; }
            set
            {
                if (value != playerImage)
                {
                    playerImage = value;
                    NotifyPropertyChanged();
                }
            }
        }

        [JsonIgnore]
        public string LocalImageURI { get; set; }

        public RegisterRequestModel()
        {
            this.Email = string.Empty;
            this.Password = string.Empty;
            this.Username = string.Empty;
            this.LastName = string.Empty;
            this.FirstName = string.Empty;
            this.ProfileImgUrl = string.Empty;
            playerImage = new BitmapImage(new Uri("pack://application:,,,/Resources/default_profile_picture.png"));
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    /// <summary>
    /// This class will be used to store the response from the server. 
    /// </summary>
    public class ResponseModel
    {
        public string JSON { get; set; }
        public string Response { get; set; }
        public bool Success { get; set; }

    }
}
