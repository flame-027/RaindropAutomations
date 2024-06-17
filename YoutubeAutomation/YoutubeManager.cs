namespace YoutubeAutomation
{
    using Google.Apis;
    using Google.Apis.Auth.OAuth2;

    public class YoutubeManager
    {
        public HttpClient ApiHttpClient { get; set; }

        public YoutubeManager()
        {
            ApiHttpClient = new HttpClient();
            //ApiHttpClient.BaseAddress = 
        }

        private void Test()
        {
            var test = new GoogleWebAuthorizationBroker();
            test.
        }
    }
}