using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PolyPaint.Server
{
    public class RequestHandler
    {
        private static readonly HttpClient httpClient = new HttpClient(new HttpClientHandler());


        static RequestHandler()
        {
            Initialize();
        }

        /// <summary>
        /// This method initialize the httpClient information
        /// </summary>
        public static void Initialize()
        {
            httpClient.BaseAddress = new Uri("https://log3900.lbacreations.com");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// This method will contribute to add the post to the server
        /// </summary>
        /// <typeparam name="T"> class name </typeparam>
        /// <param name="pathURL">the url of the route </param>
        /// <param name="objectToPost"> the object to post, it should be declared in requestModel</param>
        /// <returns></returns>
        public async static Task<Models.ResponseModel> Post<T>(string pathURL, T objectToPost)
        {
            HttpResponseMessage response = await httpClient.PostAsync(
                        pathURL, new StringContent(JsonConvert.SerializeObject(objectToPost), Encoding.UTF8, "application/json"));
            try
            {

                string jsonContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {

                    return new Models.ResponseModel()
                    {
                        JSON = jsonContent,
                        Response = response.ReasonPhrase,
                        Success = true
                    };
                }
                else
                {
                    return new Models.ResponseModel()
                    {
                        JSON = null,
                        Response = jsonContent,
                        Success = false
                    };
                }

               
            } catch(HttpRequestException e)
            {
                return new Models.ResponseModel()
                {
                    JSON = null,
                    Response = e.Message,
                    Success = false
                };
            }
        }

        public async static Task<string> PostFile(string uri)
        {
            MultipartFormDataContent form = new MultipartFormDataContent();

            byte[] imb = File.ReadAllBytes(uri);

            form.Add(new ByteArrayContent(imb, 0, imb.Length), "file", "file.jpg");
            HttpResponseMessage response = await httpClient.PostAsync("/upload/", form);

            response.EnsureSuccessStatusCode();

            return  response.Content.ReadAsStringAsync().Result;
        }

        public static async Task<HttpResponseMessage> GoogleSearchImages(string searchText, int startIndex)
        {
            string txt = Uri.EscapeDataString(searchText);
            string url = $"https://www.googleapis.com/customsearch/v1?";
            url += "key=AIzaSyBY3qB5NyJKLfoTbnqA5pzPQbSqEisPUks";
            url += $"&q={txt}";
            url += "&num=10";
            url += $"&start={startIndex}";
            url += "&safe=active";
            url += "&cx=000553032175686555254:jzw3jdctfbd";//url += "&cx=000553032175686555254:1ej1zuvgygq";
            url += "&filter=1";
            url += "&searchType=image";
            url += "&fileType=png,jpg,bmp";
            url += "&imgSize=large";//medium
            url += "&imgType=clipart";//lineart,
            url += "&ImgColorType=mono,gray";
            url += "&imgDominantColor=gray";
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await SafeExecute(() => httpClient.GetAsync(url));
        }

        private static async Task<HttpResponseMessage> SafeExecute(Func<Task<HttpResponseMessage>> request)
        {
            HttpResponseMessage responseMsg;
            try
            {
                responseMsg = await request();
            }
            catch
            {
                responseMsg = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("{\"status\":\"error\",\"error\":\"Une erreur est survenue\",\"hints\":[{\"msg\":\"Une erreur est survenue\"}]}")
                };
            }

            return responseMsg;
        }

    }
}
