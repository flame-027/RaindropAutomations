using Microsoft.Extensions.Configuration;
using RaindropAutomations.models;
using RaindropAutomations.tools;
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
            var raindropCollectionId = Convert.ToInt32(config.GetFromRaindropConfig("WatchLaterOutputCollectionId").Value);
            var youtubePlaylistName = "dump-wl";

            YoutubePlaylistToRaindrop(config, youtubeManager, youtubePlaylistName, raindropCollectionId);
        }

        private static void YoutubePlaylistToRaindrop(IConfiguration config, YoutubeManager youtubeManager, string playlistName, int raindropCollectionId)
        {
            var chromuimDataDirectory = @config.GetSection("Playwright")
                                                .GetSection("Chromium")
                                                .GetSection("DataDirectory").Value;

            var playlistUrl = @"https://www.youtube.com/playlist?list=WL";

            var videoUrls = youtubeManager.GetVideoUrlsFromPlaylistViaScrapping(playlistUrl, chromuimDataDirectory);

            var raindropManager = new RaindropManager(config);

            var collection = new Collection { Id = raindropCollectionId };
            var youtubeBookmarks = videoUrls.Select
                (
                   x => new Bookmark { Link = x.pureVideoUrl, Collection = collection, PleaseParse = new() }
                );

            var videoBookmarksInChuncks = youtubeBookmarks.Chunk(100).Select(x => x.ToList())?.ToList() ?? new();

            foreach (var bookmarksList in videoBookmarksInChuncks)
            {
                var bookmarksCollection = new BookmarksCollection { Result = true, Bookmarks = bookmarksList };
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