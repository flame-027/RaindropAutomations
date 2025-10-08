using RaindropAutomations.Core.Options;
using RaindropAutomations.Models.Saving;
using RaindropAutomations.Services;
using RaindropAutomations.Tools;
using YoutubeAutomation;

namespace RaindropAutomations
{
    public class RaindropRoutinesManager
    {
        private readonly YoutubeManager _youtubeManager;
        private readonly RaindropApiWrapService _raindropApiService;

        public RaindropRoutinesManager(YoutubeManager youtubeManager) // , RaindropApiWrapService raindropApiService)
        {
            _youtubeManager = youtubeManager;
            //_raindropApiService = raindropApiService;
        }

        public void YoutubePlaylistToRaindrop(string playlistUrl, int saveCollectionId, int checkParentCollectionId)
        {
            // for ViaApi version of GetVideos method
            // var youtubePlaylistName = "dump-wl";

            var videoUrls = _youtubeManager.GetVideoUrlsFromPlaylistViaScrapping(playlistUrl).Select(x => x.UrlAndFirstPram);

            var targetCollection = new CollectionIdSaveModel { Id = saveCollectionId };

            var videosAsBookmarks = videoUrls.Select
                (
                   x => new BookmarkSaveModel { Link = x, Collection = targetCollection, PleaseParse = new() }
                ).ToList();

            videosAsBookmarks.RemoveMatchesFromDescendants(checkParentCollectionId, SelfInclusionOptions.IncludeSelf, UrlOptions.UrlAndFirstParamOnly);

            _raindropApiService.CreateMultipleBookmarks(videosAsBookmarks);
        }
    }

}
