using Microsoft.Extensions.Configuration;
using RaindropAutomations.Models.Fetching;
using RaindropAutomations.Models.Processing;
using RaindropAutomations.Models.Saving;
using RaindropAutomations.Tools;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
//using Newtonsoft.Json;

namespace RaindropAutomations
{
    public class RaindropManager
    {
        private readonly string _apiToken;
        private readonly string _apiBaseUrl;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public RaindropManager(IConfiguration config)
        {
            _config = config;

            _apiToken = config.GetFromRaindropConfig("ApiToken").Value ?? string.Empty;
            _apiBaseUrl = config.GetFromRaindropConfig("ApiBaseUrl").Value ?? string.Empty;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);       
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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

        public SingleCollectionPayload GetRaindropCollectionById(long collectionId)
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

            var resultModel = JsonSerializer.Deserialize<SingleCollectionPayload>(jsonResult);

            //var resultModel = JsonConvert

            return resultModel;

        }

        public MultiCollectionPayload GetEveryChildCollectionOnAccount()
        {
            var response = _httpClient.GetAsync($"https://api.raindrop.io/rest/v1/collections/childrens").Result;

            response.EnsureSuccessStatusCode();

            var jsonResult = response.Content.ReadAsStringAsync().Result;

            var resultModel = JsonSerializer.Deserialize<MultiCollectionPayload>(jsonResult);

            if (resultModel != null) 
                 resultModel.Items = resultModel.Items.Where(x => x.Parent != null).ToList();

            return resultModel;
        }

        public RaindropCollectionTree GetDescendantCollectionsById(long parentCollectionId)
        public RaindropCollectionForest GetDescendantCollectionsById(long parentCollectionId)
        {
            var allChildrenOnAccount = GetEveryChildCollectionOnAccount();

            //var firstGenerationList = allChildrenOnAccount.Items.Where(x => x.Parent.Id == collectionId)?.Select(x => new RaindropTreeNode { Id = x.Id, Name = x.Title }).ToList();

            //foreach (var child in firstGenerationList)
            //{
            //    GetMatchedChildren(child, allChildrenOnAccount);
            //}

            var parentCollection = new RaindropCollectionTreeNode { Id = parentCollectionId };
            var payload = new RaindropCollectionForest();

            MatchChildrenAndSetToParentRecursively(allChildrenOnAccount, parentCollection, payload.AllIdsWithinForest);

            var descendants = parentCollection.Children;
            payload.TopLevelNodes.AddRange(descendants);

            return payload;
        }

        private static void MatchChildrenAndSetToParentRecursively(MultiCollectionPayload allPossibleChildrenPayload, RaindropCollectionTreeNode currentParent, List<long> masterIdList)
        {
            ExceptionHandler.ThrowIfAnyNull(nameof(MatchChildrenAndSetToParentRecursively),
                (allPossibleChildrenPayload, nameof(allPossibleChildrenPayload)),
                (currentParent, nameof(currentParent)),
                (masterIdList, nameof(masterIdList))
                );

            var allPossibleChildren = allPossibleChildrenPayload.Items;
            var children = allPossibleChildren.Where(x => x.Parent.Id == currentParent.Id).Select(x => new RaindropCollectionTreeNode { Id = x.Id, Name = x.Title }).ToList();

            currentParent.Children = children;

            foreach(var child in children)
            {
                masterIdList?.Add(child.Id);
                MatchChildrenAndSetToParentRecursively(allPossibleChildrenPayload, child, masterIdList);
            }
        }

        public List<BookmarkFetchModel> GetAllBookmarksFromMultipleCollections(List<long> collectionIds)
        {
            var allBookmarks = new List<BookmarkFetchModel>();
            var doneCollectionsCount = 0;
            var maxPerPage = 50;


            foreach (var id in collectionIds)
            {
                var currentPageIndex = 0;
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
                    var pageResponse = JsonSerializer.Deserialize<BookmarksQueryResponse>(pageResponseJson);

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
