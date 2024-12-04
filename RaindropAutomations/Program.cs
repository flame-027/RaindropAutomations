using Microsoft.Extensions.Configuration;
using RaindropAutomations.Models.Saving;
using RaindropAutomations.Tools;
using RainDropAutomations.Youtube.Models;
using YoutubeAutomation;

namespace RaindropAutomations
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                         //.AddJsonFile("appsettings.json")
                        .AddUserSecrets<Program>()
                        .Build();

            var googleApiConfig = config.GetSection("GoogleApi");

            var applicationName = googleApiConfig.GetSection("ApplicationName").Value;
            var credentialsPath = googleApiConfig.GetSection("CredentialsPath").Value;
            var tokenPath = googleApiConfig.GetSection("TokenFilePath").Value;

            var saveCollectionId = int.Parse(config.GetFromRaindropConfig("VideosSaveInboxId").Value);
            var checkParentCollectionId = int.Parse(config.GetFromRaindropConfig("VidoesCheckRootId").Value);


            var youtubeManager = new YoutubeManager(applicationName, credentialsPath, tokenPath);

            var playlistUrl = @"https://www.youtube.com/playlist?list=WL";

            YoutubePlaylistToRaindrop(config, youtubeManager, playlistUrl, saveCollectionId, checkParentCollectionId);
        }

        private static void YoutubePlaylistToRaindrop(IConfiguration config, YoutubeManager youtubeManager, string playlistUrl, int saveCollectionId, int checkParentCollectionId)
        {
            // for ViaApi version of method
            // var youtubePlaylistName = "dump-wl";

            var chromuimDataDirectory = @config.GetSection("Playwright")
                                                .GetSection("Chromium")
                                                .GetSection("DataDirectory").Value;

            var raindropManager = new RaindropManager(config);

            //var test = raindropManager.GetRaindropCollection(42221693);
            //var test = raindropManager.GetDescendantCollectionsById(42221693);

            var allNestedCollectionInDestinationCollection = raindropManager.GetDescendantCollectionsById(42221693);
            var allNestedBookmarksInDestinationCollection = raindropManager.GetAllBookmarksFromMultipleCollections(allNestedCollectionInDestinationCollection.AllIdsWithinTree);

            var allNestedBookmarksAsLinkStrings = allNestedBookmarksInDestinationCollection.Select(x => new YtVideoUrlModel { rawCapturedVideoUrl = x.Link })
                                                                                        .Select(x => x.pureVideoUrl)
                                                                                        .ToList();

            var videoUrls = youtubeManager.GetVideoUrlsFromPlaylistViaScrapping(playlistUrl, chromuimDataDirectory).Select(x => x.pureVideoUrl);

            var nonDuplicateVideoUrls = videoUrls.Where(x => !allNestedBookmarksAsLinkStrings.Contains(x)).ToList();

            var debuggingTest = nonDuplicateVideoUrls.Count;
            
            var targetCollection = new CollectionIdSaveModel { Id = saveCollectionId };

            var youtubeBookmarks = nonDuplicateVideoUrls.Select
                (
                   x => new BookmarkSaveModel { Link = x, Collection = targetCollection, PleaseParse = new() }
                ).ToList();

            raindropManager.CreateMultipleBookmarks(youtubeBookmarks);
        }

        
    }
}

//var bookmarks = new List<Bookmark>();

//var bookmark1 = new Bookmark()
//{
//    Link = @"https://www.youtube.com/watch?v=j5q9t4hXZz4",
//    Collection = new Collection {Id = 43166517},
//    PleaseParse = new(),
//};

//var bookmark2 = new Bookmark()
//{
//    Link = @"https://www.youtube.com/watch?v=5rSU21PXTGE",
//    Collection = new Collection {Id = 43166517},
//    PleaseParse = new(),
//};

//bookmarks.Add(bookmark1);
//bookmarks.Add(bookmark2);