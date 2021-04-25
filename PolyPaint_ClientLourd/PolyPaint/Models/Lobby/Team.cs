using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PolyPaint.Models.Lobby
{
    public class Team : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [JsonProperty(PropertyName ="_id")]
        public string ID { get; set; }

        private Player playerOne;
        [JsonProperty(PropertyName = "playerOne")]
        public Player PlayerOne
        {
            get { return playerOne; }
            set
            {
                if (playerOne == value)
                    return;

                 playerOne = value;
                 NotifyPropertyChanged();

               
            }
        }

        private Player playerTwo;
        [JsonProperty(PropertyName = "playerTwo")]
        public Player PlayerTwo
        {
            get { return playerTwo; }
            set
            {
                if (playerTwo == value)
                    return;

                 playerTwo = value;
                 NotifyPropertyChanged();
                
            }
        }

        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; }

        //private static readonly Player DummyPlayer = new Player() { User =  new User() { FirstName = "Still", LastName = "waiting" } };
        public Team()
        {
            Score = 0;
        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
