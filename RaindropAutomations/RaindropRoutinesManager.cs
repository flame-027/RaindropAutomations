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
        private readonly RaindropOperationsService _raindropOperationsService;

        public RaindropRoutinesManager(YoutubeManager youtubeManager)
        {
            _youtubeManager = youtubeManager;
        }

        public void YoutubePlaylistToRaindrop(string playlistUrl, int saveCollectionId, int CompareScopeCollectionId)
        {
            var playlistVideoUrls = _youtubeManager.GetVideoUrlsFromPlaylistViaScrapping(playlistUrl).Select(x => x.UrlAndFirstPram);

            var targetCollection = new CollectionIdSaveModel { Id = saveCollectionId };

            var playlistUrlsAsBookmarks = playlistVideoUrls.Select
                (
                   x => new BookmarkSaveModel { Link = x, Collection = targetCollection, PleaseParse = new() }
                ).ToList();

            _raindropOperationsService.RemoveExistingBookmarksFromList(playlistUrlsAsBookmarks, CompareScopeCollectionId, HierarchyScopeOptions.DescendantsAndSelf, UrlOptions.UrlAndFirstParamOnly);
            
            _raindropApiService.CreateMultipleBookmarks(playlistUrlsAsBookmarks);
        }
    }

}
