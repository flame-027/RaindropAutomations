using Microsoft.Extensions.Configuration;
using RaindropAutomations.Models.Fetching;
using RaindropAutomations.Models.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RaindropAutomations.Services
{
    public class RaindropApiWrapService
    {
        private readonly IRaindropRepository _repo;

        public RaindropApiWrapService(IRaindropRepository repository)
        {
            _repo = repository;
        }

        public SingleCollectionPayload GetCollectionById(long collectionId)
        {
            var resultModel = _repo.GetCollectionById(collectionId);

            return resultModel;
        }


        public BookmarksQueryResponse GetCollectionBookmarksById(long collectionId, int maxPerPage, int pageIndex)
        {
            var resulModel = _repo.GetCollectionBookmarksById(collectionId, maxPerPage, pageIndex);

            return resulModel;
        }


        public void CreateSingleBookmark(BookmarkSaveModel bookmark)
        {
            _repo.CreateSingleBookmark(bookmark);
        }


        public void CreateMultipleBookmarks(List<BookmarkSaveModel> bookmarks)
        {
             _repo.CreateMultipleBookmarks(bookmarks);
        }


        public MultiCollectionPayload GetEveryChildCollectionOnAccount()
        {
            var resultModel = _repo.GetEveryChildCollectionOnAccount();

            return resultModel;
        }


        public void UpdateMultipleBookmarks(long sourceCollectionId, long destinationCollectionId, List<long> specifiedBookmarkIds = null)
        {
            _repo.UpdateMultipleBookmarks(sourceCollectionId, destinationCollectionId, specifiedBookmarkIds);
        }

    }
}
