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

            //var test = $"{AppDomain.CurrentDomain.BaseDirectory}".ParentOfDoubleSlashPath();
           
            var scopeList = new List<string>()
            {
                YouTubeService.Scope.Youtube
            };

            _userToken = GetUserToken(scopeList);
            _userToken.RefreshToken();
        }

        public void Main()
        {
            //GetVideoUrlsFromPlaylist();

            //var httpClient = new HttpClient();
            //var test = GetElibilityToken(httpClient);

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
            using var clientSecretsStream = new FileStream(_credentialPath, FileMode.Open, FileAccess.Read);

            var userToken = GoogleWebAuthorizationBroker.AuthorizeAsync(
                          GoogleClientSecrets.FromStream(clientSecretsStream).Secrets,
                          scopeList,
                          "h",
                          CancellationToken.None,
                          new FileDataStore(_tokenPath, true)).Result;

            return userToken;
        }


       

        //    private static Token GetElibilityToken(HttpClient client)
        //{
        //    string baseAddress = @"https://accounts.google.com/o/oauth2/auth";

        //    string grant_type = "client_credentials";
        //    string client_id = "REMOVED_CREDS";
        //    string client_secret = "REMOVED_CREDS";

        //    var responseType  = "code";
        //    var scope = @"https://www.googleapis.com/auth/youtube";
        //    var redirectUrl = @"http://localhost:8080";

        //    var form = new Dictionary<string, string>
        //            {
        //                //{"grant_type", grant_type},
        //                {"client_id", client_id},
        //                //{"client_secret", client_secret},
        //                {"response_type", responseType },
        //                {"scope", scope},
        //                {"acess-type", "offline"},
        //                {"redirect_uri", redirectUrl}          
        //            };

        //    var tokenResponse = client.PostAsync(baseAddress, new FormUrlEncodedContent(form)).Result;
        //    var jsonContent =  tokenResponse.Content.ReadAsStringAsync().Result;
        //    var tok = JsonConvert.DeserializeObject<Token>(jsonContent);
        //    return tok;
        //}


        //internal class Token
        //{
        //    [JsonProperty("access_token")]
        //    public string AccessToken { get; set; }

        //    [JsonProperty("token_type")]
        //    public string TokenType { get; set; }

        //    [JsonProperty("expires_in")]
        //    public int ExpiresIn { get; set; }

        //    [JsonProperty("refresh_token")]
        //    public string RefreshToken { get; set; }
        //}
    }
}