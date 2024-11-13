namespace YoutubeAutomation
{
    using Newtonsoft.Json;
    using System.Text.Json;
    using System.Collections.Generic;
    using System.Net.Http;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Util.Store;
    using Google.Apis.YouTube.v3;
    using Google.Apis.Services;
    using Google.Apis.YouTube.v3.Data;
    using YoutubeAutomation.Tools;
    using System.IO;
    using Microsoft.Playwright;
    using RainDropAutomations.Youtube;

    public class YoutubeManager
    {
        private readonly UserCredential _userToken;
        private readonly string _applicationName;
        private readonly string _credentialPath;
        private readonly string _tokenPath;

        public YoutubeManager()
        {
            _applicationName = "MyAutomations";
            _credentialPath = "C:\\users\\h\\downloads\\google-desktop.json";
            _tokenPath = "Token";

            var scopeList = new List<string>()
            {
                YouTubeService.Scope.Youtube
            };

            _userToken = GetUserToken(scopeList);
            _userToken?.RefreshToken();
        }

        public List<string> GetVideoUrlsFromPlaylist(string playlistName)
        {
            _userToken.RefreshToken();

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _userToken,
                ApplicationName = _applicationName
            });

            var playlistVideosAsUrls = new List<string>();
            var allPlaylists = GetMyPlaylists(youtubeService);

            if (allPlaylists.Count > 0)
            {
                var selectedPlaylist = allPlaylists.FirstOrDefault(y => y.Snippet.Title == playlistName);

                if (selectedPlaylist == null)
                    throw new InvalidOperationException("Did not find a playlist with that name");

                var playlistVideos = GetVideosFromPlaylist(youtubeService, selectedPlaylist.Id);
                playlistVideosAsUrls = playlistVideos.Select(x => $"https://www.youtube.com/watch?v={x.Snippet.ResourceId.VideoId}").ToList();
            }
            else
            {
                throw new InvalidOperationException("Did not get any playlists back from Youtube");
            } 

            return playlistVideosAsUrls;
        }


        public List<string> GetVideoUrlsFromPlaylist(string playlistName, object test)
        {
            var userDataDir = "D:\\Files (Users)\\Projects\\Programming\\RaindropAutomation-Stuff\\Playwright-Browser-Data";

            var playwright = Playwright.CreateAsync().Result;
            var browser = playwright.Chromium.LaunchPersistentContextAsync(userDataDir, new BrowserTypeLaunchPersistentContextOptions
            {
                Headless = false, //
                Args = ["--start-maximized", /*"--disable-blink-features=AutomationControlled"*/],
                //ChromiumSandbox = false
                //IgnoreDefaultArgs = new[] { "--enable-automation" }
            }).Result;

            var page = browser.Pages.FirstOrDefault() ??  browser.NewPageAsync().Result;

            page.GotoAsync("https://www.youtube.com/playlist?list=WL");

            page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

            Thread.Sleep(1500);

            // JavaScript code to scroll to the bottom
            string scrollScript = @"
            (async () => {
                const scrollToBottom = async () => {
                    let scrollHeight = document.documentElement.scrollHeight;
                    let clientHeight = document.documentElement.clientHeight;
                    let scrollPosition = 0;

                    // Loop to scroll until bottom is reached
                    while (scrollPosition + clientHeight < scrollHeight) {
                        window.scrollBy(0, clientHeight);
                        scrollPosition = document.documentElement.scrollTop;

                        // Wait for more content to load
                        await new Promise(resolve => setTimeout(resolve, 300));

                        // Update heights in case of dynamic content
                        scrollHeight = document.documentElement.scrollHeight;
                    }
                };

                await scrollToBottom();
            })();
        ";

            var test2 = page.EvaluateAsync(scrollScript).Result;

            page.WaitForTimeoutAsync(3000);



            var elements =  page.QuerySelectorAllAsync("#video-title").Result;
            var videoLinks = new List<YtVideoModel>();

            foreach (var element in elements)
            {
                var href = element.EvaluateAsync<string>("el => el.href").Result;

                if (href != null)
                {
                    var video = new YtVideoModel { rawVideoUrl = href };
                    videoLinks.Add(video);
                }
            }

            var result = videoLinks.Select(x => x.pureVideoUrl).ToList();

            return result;
        }


        private static List<Playlist> GetMyPlaylists(YouTubeService service)
        {
            var getPlaylistsRequest = service.Playlists.List("snippet");

            getPlaylistsRequest.Mine = true;
            getPlaylistsRequest.MaxResults = 50;

            var currentPlaylistPage = getPlaylistsRequest.Execute();

            var allPlaylists = currentPlaylistPage?.Items?.ToList();

            if(allPlaylists == null)
                    return new List<Playlist>();
          
            while (currentPlaylistPage.NextPageToken != null)
            {
                getPlaylistsRequest.PageToken = currentPlaylistPage.NextPageToken;
                var nextPlaylistPage = getPlaylistsRequest.Execute();

                allPlaylists.AddRange(nextPlaylistPage.Items);
                currentPlaylistPage = nextPlaylistPage;
            }

            return allPlaylists;
        }


        private static List<PlaylistItem> GetVideosFromPlaylist(YouTubeService service, string playlistId)
        {
            var getVideosRequest = service.PlaylistItems.List("snippet");
            getVideosRequest.PlaylistId = playlistId;
            getVideosRequest.MaxResults = 50;

            var currentVideoPage = getVideosRequest.Execute();

            var allVideos = currentVideoPage?.Items?.ToList();

            if (allVideos == null)
                return new List<PlaylistItem>();

            while (currentVideoPage.NextPageToken != null)
            {
                getVideosRequest.PageToken = currentVideoPage.NextPageToken;
                var nextVideoPage = getVideosRequest.Execute();

                allVideos.AddRange(nextVideoPage.Items);
                currentVideoPage = nextVideoPage;
            }

            return allVideos;
        }


        private static List<Video> GetDetailsForAllVideos(YouTubeService service, List<string> videoIds)
        {
            var detailsRequest = service.Videos.List("snippet");

            detailsRequest.Id = videoIds;
            detailsRequest.MaxResults = 50;

            var currentDetailsPage = detailsRequest.Execute();

            var allPagesDetails = currentDetailsPage?.Items?.ToList();

            if (allPagesDetails == null)
                return new List<Video>();

            while (currentDetailsPage.NextPageToken != null)
            {
                detailsRequest.AccessToken = currentDetailsPage.NextPageToken;
                var newDetailsPage = detailsRequest.Execute();

                allPagesDetails.AddRange(newDetailsPage.Items);
                currentDetailsPage = newDetailsPage;
            }

            return allPagesDetails;
        }


        private UserCredential GetUserToken(List<string> scopeList)
        {

            Stream clientSecretsStream = null;

            try
            {
                clientSecretsStream = new FileStream(_credentialPath, FileMode.Open, FileAccess.Read);
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {_credentialPath}. Exception: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error accessing client secrets file: {ex.Message}");
                return null;
            }

            var userToken = GoogleWebAuthorizationBroker.AuthorizeAsync(
                              GoogleClientSecrets.FromStream(clientSecretsStream).Secrets,
                              scopeList,
                              "h",
                              CancellationToken.None,
                              new FileDataStore(_tokenPath, true)).Result;

            clientSecretsStream.Dispose();

            return userToken;
        }

    }
}