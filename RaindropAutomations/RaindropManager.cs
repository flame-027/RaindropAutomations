using Newtonsoft.Json;
using RaindropAutomations.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RaindropAutomations
{
    public class RaindropManager
    {
        private readonly string _apiToken;
        private readonly HttpClient _httpClient;

        public RaindropManager()
        {
            _apiToken = "";

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);       
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void CreateSingleBookmark(Bookmark bookmark)
        {
            string bookmarkJson = JsonConvert.SerializeObject(bookmark);
            HttpContent content = new StringContent(bookmarkJson, Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync("https://api.raindrop.io/rest/v1/raindrop", content).Result;
        }

        public void CreateMultipleBookmarks(BookmarksCollection bookmarksCollection)
        {
            string bookmarkJson = JsonConvert.SerializeObject(bookmarksCollection);
            HttpContent content = new StringContent(bookmarkJson, Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync("https://api.raindrop.io/rest/v1/raindrops", content).Result;
        }

    }

}
