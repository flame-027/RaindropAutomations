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


    public class YoutubeManager
    {
        //public HttpClient ApiHttpClient { get; set; }
        private readonly UserCredential _userCredential;

        public YoutubeManager()
        {
            //ApiHttpClient = new HttpClient();
            //ApiHttpClient.BaseAddress = 

            var scopeList = new List<string>()
            {
                YouTubeService.Scope.Youtube
            };

            _userCredential = GetUserCredentials(scopeList);
            _userCredential.RefreshToken();
        }

        public void Main()
        {
            Test2();

            //var httpClient = new HttpClient();
            //var test = GetElibilityToken(httpClient);

        }

        private void Test2()
        {
            _userCredential.RefreshToken();

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = _userCredential,
                ApplicationName = "MyAutomations",
            });

            var selectedPlaylist = GetMyPlaylists(youtubeService).First(y => y.Snippet.Title == "dump-wl");  //.Select(x => x.Snippet)

            var playlistVideos = GetVideosFromPlaylist(youtubeService, selectedPlaylist.Id);

            //var playlistVideosAsIds = playlistVideos.Select(y => y.Id).ToList();
            var playlistVideosAsIds = playlistVideos.Select(x => $"https://www.youtube.com/watch?v={x.Snippet.ResourceId.VideoId}").ToList();
        }


        private static List<Playlist> GetMyPlaylists(YouTubeService service)
        {
            var getPlaylistsRequest = service.Playlists.List("snippet");

            getPlaylistsRequest.Mine = true;
            getPlaylistsRequest.MaxResults = 50;

            var currentPlaylistPage = getPlaylistsRequest.Execute();

            var allPlaylists = currentPlaylistPage.Items.ToList();
          
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

            var allVideos = currentVideoPage.Items.ToList();

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

            var allPagesDetails = currentDetailsPage.Items.ToList();
         
            while (currentDetailsPage.NextPageToken != null)
            {
                detailsRequest.AccessToken = currentDetailsPage.NextPageToken;
                var newDetailsPage = detailsRequest.Execute();

                allPagesDetails.AddRange(newDetailsPage.Items);
                currentDetailsPage = newDetailsPage;
            }

            return allPagesDetails;
        }


        private static UserCredential GetUserCredentials(List<string> scopeList)
        {
            using var clientSecretsStream = new FileStream("C:\\users\\h\\downloads\\google-desktop.json", FileMode.Open, FileAccess.Read);
            var credPath = "other_token.json";

            var credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                          GoogleClientSecrets.FromStream(clientSecretsStream).Secrets,
                          scopeList,
                          "h",
                          CancellationToken.None,
                          new FileDataStore(credPath, true)).Result;

            return credentials;
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