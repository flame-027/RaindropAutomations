using Microsoft.Extensions.Configuration;
using RaindropAutomations.models;
using RainDropAutomations.Youtube;
using System.Net.Mail;
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

            var youtubeManager = new YoutubeManager();
            var raindropCollectionId = 48328942;
            var youtubePlaylistName = "dump-wl";

            YoutubePlaylistToRaindrop(config, youtubeManager, youtubePlaylistName, raindropCollectionId);
        }

        private static void YoutubePlaylistToRaindrop(IConfiguration config, YoutubeManager youtubeManager, string playlistName, int raindropCollectionId)
        {
            var raindropManager = new RaindropManager(config);

            //var test = raindropManager.GetRaindropCollection(42221693);
            //var test = raindropManager.GetDescendantCollectionsById(42221693);
            var allNestedCollectionInDestinationCollection = raindropManager.GetDescendantCollectionsById(42221693);
            var allNestedBookmarksInDestinationCollection = raindropManager.GetAllBookmarksFromMultipleCollections(allNestedCollectionInDestinationCollection.AllIdsWithinTree);

            var allNestedBookmarksAsLinkStrings = allNestedBookmarksInDestinationCollection.Select(x => new YtVideoModel { rawVideoUrl = x.Link })
                                                                                        .Select(x => x.pureVideoUrl)
                                                                                        .ToList();

            var videoUrls = youtubeManager.GetVideoUrlsFromPlaylist(playlistName, new object());
            var nonDuplicateVideoUrls = videoUrls.Where(x => !allNestedBookmarksAsLinkStrings.Contains(x)).ToList();

            var debuggingTest = nonDuplicateVideoUrls.Count;
            
            var collection = new Collection { Id = raindropCollectionId };
            var youtubeBookmarks = videoUrls.Select
                (
                   x => new Bookmark { Link = x, Collection = collection, PleaseParse = new() }
                );

            var videoBookmarksInChuncks = youtubeBookmarks.Chunk(100).Select(x => x.ToList())?.ToList() ?? new();

            foreach (var bookmarksList in videoBookmarksInChuncks)
            {
                var bookmarksCollection = new BookmarksCreationPayload { Result = true, Bookmarks = bookmarksList };
                raindropManager.CreateMultipleBookmarks(bookmarksCollection);
            }
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