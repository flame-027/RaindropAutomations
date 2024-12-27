using Microsoft.Extensions.Configuration;
using RaindropAutomations.Models.Fetching;
using RaindropAutomations.Models.Saving;
using RaindropAutomations.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RaindropAutomations
{
    public class RaindropRepository : IRaindropRepository
    {
        private readonly string _apiToken;
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public RaindropRepository(IConfiguration config)
        {
            _config = config;

            _apiToken = config.GetFromRaindropConfig("ApiToken").Value ?? string.Empty;
            _apiBaseUrl = config.GetFromRaindropConfig("ApiBaseUrl").Value ?? string.Empty;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _config = config;
        }


        public SingleCollectionPayload GetRaindropCollectionById(long collectionId)
        {
            var response = _httpClient.GetAsync($"{_apiBaseUrl}/{collectionId}").Result;

            response.EnsureSuccessStatusCode();

            var jsonResult = response.Content.ReadAsStringAsync().Result;

            var resultModel = JsonSerializer.Deserialize<SingleCollectionPayload>(jsonResult);

            return resultModel;

        }

        public BookmarksQueryResponse GetCollectionBookmarksById(long collectionId, int maxPerPage, int pageIndex)
        {
            var requestUrl = $"{_apiBaseUrl}/raindrops/{collectionId}?perpage={maxPerPage}&page={pageIndex}";

            while (true)
            {
                try
                {
                    var response = _httpClient.GetAsync(requestUrl).Result;

                    if (response.StatusCode == (HttpStatusCode)429)
                    {
                        var retryAfter = response.Headers.Contains("Retry-After")
                                                            ? int.Parse(response.Headers.GetValues("Retry-After").First())
                                                            : 5;

                        Console.WriteLine($"Rate limit hit. Retrying after {retryAfter} seconds...");

                        Thread.Sleep(retryAfter * 1000);
                        continue;
                    }

                    response.EnsureSuccessStatusCode();

                    var pageResponseJson = response.Content.ReadAsStringAsync().Result;
                    var pageResponseModel = JsonSerializer.Deserialize<BookmarksQueryResponse>(pageResponseJson);

                    return pageResponseModel;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }
        }


        public void CreateSingleBookmark(BookmarkSaveModel bookmark)
        {
            string bookmarkJson = Newtonsoft.Json.JsonConvert.SerializeObject(bookmark);
            HttpContent content = new StringContent(bookmarkJson, Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync($"{_apiBaseUrl}/raindrop", content).Result;
        }


        public void CreateMultipleBookmarks(List<BookmarkSaveModel> bookmarks)
        {
            var BookmarkChuncks = bookmarks.Chunk(100).Select(x => x.ToList())?.ToList() ?? new();

            foreach (var bookmarkChunk in BookmarkChuncks)
            {
                var bookmarksPayload = new MultiBookmarkSavePayload { Result = true, Bookmarks = bookmarkChunk };

                string bookmarkPayloadJson = Newtonsoft.Json.JsonConvert.SerializeObject(bookmarksPayload);
                HttpContent content = new StringContent(bookmarkPayloadJson, Encoding.UTF8, "application/json");

                var response = _httpClient.PostAsync($"{_apiBaseUrl}/raindrops", content).Result;
                //TODO: need to create proper error / rate handling?
            }
        }


        public MultiCollectionPayload GetEveryChildCollectionOnAccount()
        {
            var response = _httpClient.GetAsync($"{_apiBaseUrl}/collections/childrens").Result;

            response.EnsureSuccessStatusCode();

            var jsonResult = response.Content.ReadAsStringAsync().Result;

            var resultModel = JsonSerializer.Deserialize<MultiCollectionPayload>(jsonResult);

            if (resultModel != null)
                resultModel.Items = resultModel.Items.Where(x => x.Parent != null).ToList();

            return resultModel;
        }


        public void UpdateMultipleBookmarks(long sourceCollectionId, long destinationCollectionId, List<long> specifiedBookmarkIds = null)
        {
            var requestUrl = $"{_apiBaseUrl}/raindrops/{sourceCollectionId}";

            var model = new MultiBookmarkModifyModel
            {
                Collection = new CollectionIdSaveModel { Id = destinationCollectionId },
                Ids = specifiedBookmarkIds
            };

            string requestJson = JsonSerializer.Serialize(model);
            HttpContent content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = _httpClient.PutAsync(requestUrl, content).Result;
        }
    }
}
