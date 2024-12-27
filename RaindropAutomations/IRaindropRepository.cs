using RaindropAutomations.Models.Fetching;
using RaindropAutomations.Models.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaindropAutomations
{
    public interface IRaindropRepository
    {
        SingleCollectionPayload GetCollectionById(long collectionId);

        BookmarksQueryResponse GetCollectionBookmarksById(long collectionId, int maxPerPage, int pageIndex);

        MultiCollectionPayload GetEveryChildCollectionOnAccount();

        void CreateSingleBookmark(BookmarkSaveModel bookmark);

        void CreateMultipleBookmarks(List<BookmarkSaveModel> bookmarks);

        void UpdateMultipleBookmarks(long sourceCollectionId, long destinationCollectionId, List<long> specifiedBookmarkIds = null);
    }
}
