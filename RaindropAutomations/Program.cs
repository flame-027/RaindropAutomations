using Microsoft.Extensions.Configuration;
using RaindropAutomations.Core.Options;
using RaindropAutomations.Models.Saving;
using RaindropAutomations.Services;
using RaindropAutomations.Tools;
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

            GlobalConfig.Config = config;

            var googleApiConfig = config.GetSection("GoogleApi");

            var applicationName = googleApiConfig.GetSection("ApplicationName").Value;
            var credentialsPath = googleApiConfig.GetSection("CredentialsPath").Value;
            var tokenPath = googleApiConfig.GetSection("TokenFilePath").Value;


            if (true)
            {
                var saveCollectionId = int.Parse(config.GetFromRaindropConfig("VideosSaveInboxId").Value);
                var checkParentCollectionId = int.Parse(config.GetFromRaindropConfig("VidoesCheckRootId").Value);

                var youtubeManager = new YoutubeManager(config);
                var raindropApiService = new RaindropApiWrapService(new RaindropRepository(config, new()));

                var playlistUrl = @"https://www.youtube.com/playlist?list=WL";

                YoutubePlaylistToRaindrop(youtubeManager, raindropApiService, playlistUrl, saveCollectionId, checkParentCollectionId);

                //raindropApiService.UpdateMultipleBookmarks(50386169, 50386170, [918116881]);
            }

        }


        private static void YoutubePlaylistToRaindrop(YoutubeManager youtubeManager, RaindropApiWrapService raindropApiService, string playlistUrl, int saveCollectionId, int checkParentCollectionId)
        {
            // for ViaApi version of GetVideos method
            // var youtubePlaylistName = "dump-wl";

            var videoUrls = youtubeManager.GetVideoUrlsFromPlaylistViaScrapping(playlistUrl).Select(x => x.UrlAndFirstPram);
            
            var targetCollection = new CollectionIdSaveModel { Id = saveCollectionId };

            var videosAsBookmarks = videoUrls.Select
                (
                   x => new BookmarkSaveModel { Link = x, Collection = targetCollection, PleaseParse = new() }
                ).ToList();

            videosAsBookmarks.RemoveMatchesFromDescendants(checkParentCollectionId, SelfInclusionOptions.IncludeSelf, UrlOptions.UrlAndFirstParamOnly);

            raindropApiService.CreateMultipleBookmarks(videosAsBookmarks);
        }

        private static void SaveYoutubeVideosInRaindropCollection()
        {

        }
    }
}