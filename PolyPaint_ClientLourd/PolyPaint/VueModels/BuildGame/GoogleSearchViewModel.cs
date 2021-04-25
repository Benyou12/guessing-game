using Newtonsoft.Json;
using PolyPaint.Models.BuildGame;
using PolyPaint.Server;
using PolyPaint.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PolyPaint.VueModels.BuildGame
{
    class GoogleSearchViewModel
    {
        public RelayCommand<object> NextImagePageCommand { get; set; }
        public RelayCommand<object> PreviousImagePageCommand { get; set; }

        private readonly string GoogleCustomImageSearchMessage = "Un problème est survenu en communiquant avec le serveur.\nVeuillez réessayer";
        public ObservableCollection<SearchItem> ImagesResults { get; set; }

        public delegate void GoogleSearchViewEvent();
        public static GoogleSearchViewEvent OnGoogleSearchView = delegate { };
        private int StartIndex { get; set; } = 0;
        private string SearchString { get; set; } = string.Empty;

        public GoogleSearchViewModel()
        {
            ImagesResults = new ObservableCollection<SearchItem>();
            NextImagePageCommand = new RelayCommand<object>(NextImagePage);
            PreviousImagePageCommand = new RelayCommand<object>(PreviousImagePage);

            BuildGameViewModel.OnGoogleSearch += GoToSearchImages;
        }

        private void PreviousImagePage(object obj)
        {
            StartIndex = StartIndex == 1 ? 91 : StartIndex -= 10;
            GoToSearchImages(SearchString, StartIndex);
        }

        private void NextImagePage(object obj)
        {
            StartIndex = StartIndex == 91 ? 1 : StartIndex += 10;
            GoToSearchImages(SearchString, StartIndex);
        }

        private void GoToSearchImages(string searchString, int startIndex)
        {
            if (Guard.IsNullOrEmpty(searchString))
                return;

            StartIndex = startIndex;
            SearchString = searchString;
            ImagesResults.Clear();
            HttpResponseMessage response = Task.Run(() => { return RequestHandler.GoogleSearchImages(searchString, startIndex).Result; }).Result;

            if (response.IsSuccessStatusCode)
            {
                SearchImageResult results = JsonConvert.DeserializeObject<SearchImageResult>(response.Content.ReadAsStringAsync().Result);
                List<SearchItem> items = results.Items ?? new List<SearchItem>();
                foreach (SearchItem res in items)
                {
                    ImagesResults.Add(res);
                }
                OnGoogleSearchView.Invoke();
            }
            else
            {
                MessageBox.Show(GoogleCustomImageSearchMessage);
            }
        }
    }
}
