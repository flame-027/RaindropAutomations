using RaindropAutomations.Models.Fetching;
using RaindropAutomations.Models.Processing;
using RaindropAutomations.Tools;

namespace RaindropAutomations.Services
{
    public class RaindropOperationsService
    {
        public readonly RaindropApiWrapService _apiService;

        public RaindropOperationsService(RaindropApiWrapService apiService)
        {
            _apiService = apiService;
        }

        public RaindropCollectionForest GetDescendantAndSelfCollectionsById(long parentCollectionId)
        {
            var descendantsForest = GetDescendantCollectionsById(parentCollectionId);

            if (descendantsForest == null || descendantsForest.AllIdsWithinForest.Count <= 0)
                return new();

            var parentResponsePayload = _apiService.GetCollectionById(parentCollectionId);
            var parentModel = parentResponsePayload.Item;

            var parentNode = new RaindropCollectionTreeNode { Id = parentModel.Id, Children = descendantsForest.TopLevelNodes, Name = parentModel.Title };

            descendantsForest.TopLevelNodes = [parentNode];
            descendantsForest.AllIdsWithinForest.Insert(0, parentNode.Id);

            return descendantsForest;
        }


        public RaindropCollectionForest GetDescendantCollectionsById(long parentCollectionId)
        {
            var allChildrenOnAccount = _apiService.GetEveryChildCollectionOnAccount();

            var parentCollection = new RaindropCollectionTreeNode { Id = parentCollectionId };
            var payload = new RaindropCollectionForest();

            try { MatchChildrenAndSetToParentRecursively(allChildrenOnAccount, parentCollection, payload.AllIdsWithinForest); }

            catch (Exception) { throw; }

            var descendants = parentCollection.Children;
            payload.TopLevelNodes.AddRange(descendants);

            return payload;
        }


        private static void MatchChildrenAndSetToParentRecursively(MultiCollectionPayload allPossibleChildrenPayload, RaindropCollectionTreeNode currentParent, List<long> masterIdList)
        {
            ExceptionHandler.ThrowIfAnyNull(nameof(MatchChildrenAndSetToParentRecursively),
                (allPossibleChildrenPayload, nameof(allPossibleChildrenPayload)),
                (currentParent, nameof(currentParent)),
                (masterIdList, nameof(masterIdList))
                );

            var allPossibleChildren = allPossibleChildrenPayload.Items;
            var children = allPossibleChildren.Where(x => x.Parent.Id == currentParent.Id).Select(x => new RaindropCollectionTreeNode { Id = x.Id, Name = x.Title }).ToList();

            currentParent.Children = children;

            foreach (var child in children)
            {
                masterIdList?.Add(child.Id);
                MatchChildrenAndSetToParentRecursively(allPossibleChildrenPayload, child, masterIdList);
            }
        }


        public List<BookmarkFetchModel> GetAllBookmarksFromMultipleCollections(List<long> collectionIds)
        {
            var allBookmarks = new List<BookmarkFetchModel>();
            var doneCollectionsCount = 0;

            foreach (var id in collectionIds)
            {
                var collectionBookmarks = GetBookmarksFromSingleCollection(id);
                allBookmarks.AddRange(collectionBookmarks);

                doneCollectionsCount++;
            }

            return allBookmarks;
        }


        public List<BookmarkFetchModel> GetBookmarksFromSingleCollection(long collectionId)
        {
            var maxPerPage = 50;
            var currentPageIndex = 0;
            bool hasMorePages;

            var allBookmarks = new List<BookmarkFetchModel>();

            do
            {
                var bookmarksPage = _apiService.GetCollectionBookmarksById(collectionId, maxPerPage, currentPageIndex);

                if (bookmarksPage.Items != null && bookmarksPage.Items.Count > 0)
                {
                    allBookmarks.AddRange(bookmarksPage.Items);
                    hasMorePages = bookmarksPage.Items.Count == maxPerPage;
                }
                else
                    hasMorePages = false;

                currentPageIndex++;

            } while (hasMorePages);

            return allBookmarks;
        }

    }
}
