using Microsoft.Extensions.Configuration;
using RaindropAutomations.models;
using System.Net.Mail;

namespace RaindropAutomations
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
             .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            // .AddJsonFile("appsettings.json")
             .AddUserSecrets<Program>()
             .Build();

            var manager = new RaindropManager(config);
            //manager.CreateSingleBookmark();

            var bookmarks = new List<Bookmark>();

            var bookmark1 = new Bookmark()
            {
                Link = @"https://www.youtube.com/watch?v=j5q9t4hXZz4",
                Collection = new Collection {Id = 43166517},
                PleaseParse = new(),
            };

            var bookmark2 = new Bookmark()
            {
                Link = @"https://www.youtube.com/watch?v=5rSU21PXTGE",
                Collection = new Collection {Id = 43166517},
                PleaseParse = new(),
            };

            bookmarks.Add(bookmark1);
            bookmarks.Add(bookmark2);

            var bookmarksCollection = new BookmarksCollection {Result = true, Bookmarks = bookmarks};

            manager.CreateMultipleBookmarks(bookmarksCollection);
        }
    }
}
