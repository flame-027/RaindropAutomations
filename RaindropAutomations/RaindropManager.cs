using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Extensions.Configuration;
using RaindropAutomations.models;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;
//using Newtonsoft.Json;

namespace RaindropAutomations
{
    public class RaindropManager
    {
        private readonly string _apiToken;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RaindropManager(IConfiguration config)
        {
            _configuration = config;

            _apiToken = config.GetSection("Raindrop")?.GetSection("ApiToken")?.Value ?? string.Empty;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);       
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void CreateSingleBookmark(Bookmark bookmark)
        {
            string bookmarkJson = Newtonsoft.Json.JsonConvert.SerializeObject(bookmark);
            HttpContent content = new StringContent(bookmarkJson, Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync("https://api.raindrop.io/rest/v1/raindrop", content).Result;
        }

        public void CreateMultipleBookmarks(BookmarksCreationPayload bookmarksCollection)
        {
            string bookmarkJson = Newtonsoft.Json.JsonConvert.SerializeObject(bookmarksCollection);
            HttpContent content = new StringContent(bookmarkJson, Encoding.UTF8, "application/json");

            var response = _httpClient.PostAsync("https://api.raindrop.io/rest/v1/raindrops", content).Result;
        }

        public RaindropSingleCollection GetRaindropCollectionById(int collectionId)
        {
            var response = _httpClient.GetAsync($"https://api.raindrop.io/rest/v1/collection/{collectionId}").Result;

            response.EnsureSuccessStatusCode();

            var jsonResult = response.Content.ReadAsStringAsync().Result; //?.Replace(@"$id", @"id");

            //string pattern = @"\$\b(id)\b";

            //jsonResult = Regex.Replace(jsonResult, pattern, "id");

            //var settings = new JsonSerializerSettings
            //{
            //    PreserveReferencesHandling = PreserveReferencesHandling.None,
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //    MetadataPropertyHandling = MetadataPropertyHandling.Ignore // Ignore $id as metadata
            //};

            //var resultModel = JsonConvert.DeserializeObject<RaindropSingleCollection>(jsonResult, settings);

            var resultModel = JsonSerializer.Deserialize<RaindropSingleCollection>(jsonResult);

            //var resultModel = JsonConvert

            return resultModel;

        }

        public RaindropCollections GetEveryChildCollectionOnAccount()
        {
            var response = _httpClient.GetAsync($"https://api.raindrop.io/rest/v1/collections/childrens").Result;

            response.EnsureSuccessStatusCode();

            var jsonResult = response.Content.ReadAsStringAsync().Result;

            var resultModel = JsonSerializer.Deserialize<RaindropCollections>(jsonResult);

            if (resultModel != null) 
                 resultModel.Items = resultModel.Items.Where(x => x.Parent != null).ToList();

            return resultModel;
        }

        public RaindropTree GetDescendantCollectionsById(int parentCollectionId)
        {
            var allChildrenOnAccount = GetEveryChildCollectionOnAccount();

            //var firstGenerationList = allChildrenOnAccount.Items.Where(x => x.Parent.Id == collectionId)?.Select(x => new RaindropTreeNode { Id = x.Id, Name = x.Title }).ToList();

            //foreach (var child in firstGenerationList)
            //{
            //    GetMatchedChildren(child, allChildrenOnAccount);
            //}

            var parentCollection = new RaindropTreeNode { Id = parentCollectionId };
            var payload = new RaindropTree();

            MatchChildrenAndSetToParentRecursively(allChildrenOnAccount, parentCollection, payload.AllIdsWithinTree);

            var descendants = parentCollection.Children;
            payload.TopNodes.AddRange(descendants);

            return payload;
        }

        private static void MatchChildrenAndSetToParentRecursively(RaindropCollections allChildrenOnAccount, RaindropTreeNode parent, List<long> masterIdList = null)
        {
            var allPossibleChildren = allChildrenOnAccount.Items;
            var children = allPossibleChildren.Where(x => x.Parent.Id == parent.Id).Select(x => new RaindropTreeNode { Id = x.Id, Name = x.Title }).ToList();

            parent.Children = children;

            foreach(var child in children)
            {
                masterIdList?.Add(child.Id);
                MatchChildrenAndSetToParentRecursively(allChildrenOnAccount, child, masterIdList);
            }
        }

        public List<RaindropItem> GetAllBookmarksFromMultipleCollections(List<long> collectionIds)
        {
            var allBookmarks = new List<RaindropItem>();
            var doneCollectionsCount = 0;

            foreach (var id in collectionIds)
            {
                var currentPageIndex = 0;
                var maxPerPage = 50;
                var hasMorePages = false;

                do
                {
                    var url = $"https://api.raindrop.io/rest/v1/raindrops/{id}?perpage={maxPerPage}&page={currentPageIndex}";

                    HttpResponseMessage response = null;

                    while (true)
                    {
                        try
                        {
                            response = _httpClient.GetAsync(url).Result;

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
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            throw;
                        }
                    }

                    var pageResponseJson = response.Content.ReadAsStringAsync().Result;
                    var pageResponse = JsonSerializer.Deserialize<RaindropPageResponse>(pageResponseJson);

                    if (pageResponse.Items != null)
                    {
                        allBookmarks.AddRange(pageResponse.Items);
                        hasMorePages = pageResponse.Items.Count == maxPerPage;
                    }
                    else
                    {
                        hasMorePages = false;
                    }

                    currentPageIndex++;

                } while (hasMorePages);

                doneCollectionsCount++;
            }

            return allBookmarks;
        }

    }

}
