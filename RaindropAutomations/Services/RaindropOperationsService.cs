using RaindropAutomations.Core.Options;
using RaindropAutomations.Models;
using RaindropAutomations.Models.Fetching;
using RaindropAutomations.Models.Processing;
using RaindropAutomations.Models.Saving;
using RaindropAutomations.Tools;
using System.Security.Cryptography.X509Certificates;

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

            if (descendantsForest == null || descendantsForest.AllIds.Count <= 0)
                return new();

            var parentResponsePayload = _apiService.GetCollectionById(parentCollectionId);
            var parentModel = parentResponsePayload.Item;

            var parentNode = new RaindropCollectionTreeNode { Id = parentModel.Id, Children = descendantsForest.TopLevelNodes, Name = parentModel.Title };

            descendantsForest.TopLevelNodes = [parentNode];
            descendantsForest.AllIds.Insert(0, parentNode.Id);

            return descendantsForest;
        }


        public RaindropCollectionForest GetDescendantCollectionsById(long parentCollectionId)
        {
            var allChildrenOnAccount = _apiService.GetEveryChildCollectionOnAccount();

            var parentCollection = new RaindropCollectionTreeNode { Id = parentCollectionId };
            var payload = new RaindropCollectionForest();

            try { MatchChildrenAndSetToParentRecursively(allChildrenOnAccount, parentCollection, payload.AllIds); }

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


        /// <summary>
        /// Removes bookmarks that already exisiting within raindrop from a specific colection and / or its descendents.
        /// </summary>
        /// <param name="compareCollectionId">The ID of the root collection for the scope.</param>
        /// <param name="compareScopeSettings">Defines whether to the scope is self, descendents or descendents and self.</param>
        /// <param name="compareSettings">Optional settings to customize URL comparison logic - the logic the determines duplicates.</param>
        /// <param name="inputList">The list of bookmarks to filter by scope.</param>
        /// 
        /// THIS SUMMARY NEEDS UPDATING 
        /// THIS SUMMARY NEEDS UPDATING 
        /// THIS SUMMARY NEEDS UPDATING 
        ///
        public List<BookmarkSaveModel> RemoveExistingBookmarksFromList(List<BookmarkSaveModel> bookmarksToFilter,
                                                                         long compareCollectionId,
                                                                         HierarchyScopeOptions compareScopeSettings = HierarchyScopeOptions.SelfOnly,
                                                                         UrlOptions compareSettings = UrlOptions.RawUrl)
        {
            // GET ALL BOOKMARKS WITHIN COMPARE SCOPE

            var collections = GetAllCollectionsWithinScope(compareCollectionId, compareScopeSettings);

            var allscopedBookmarks = GetAllBookmarksFromMultipleCollections(collections.AllIds);

            if (allscopedBookmarks.Count < 1)
                return bookmarksToFilter;

            // CONVERT URL'S TO REFINED TYPES FOR COMPARE OR LEAVE RAW

            var scopedBookmarkUrls = allscopedBookmarks.Select(x => x.Link.GetUrlType(compareSettings)).ToList();

            // REMOVING ANY MATCHING BOOKMARKS FROM INPUT LIST & RETURN

            var result = bookmarksToFilter
                    .Where(x => !scopedBookmarkUrls.Contains(x.Link.GetUrlType(compareSettings)))
                    .ToList();

            return result;
        }


        private ICollectionScope GetAllCollectionsWithinScope(long scopeParentCollectionId, 
                                                                HierarchyScopeOptions compareScopeSettings)
        {
            switch (compareScopeSettings)
            {
                case HierarchyScopeOptions.SelfOnly:
                    var singleCollection = _apiService.GetCollectionById(scopeParentCollectionId);
                    return singleCollection;

                case HierarchyScopeOptions.DescendantsAndSelf:
                    var descendantAndSelfCollections = GetDescendantAndSelfCollectionsById(scopeParentCollectionId);
                    return descendantAndSelfCollections;

                case HierarchyScopeOptions.DescendantsOnly:
                    var descendantCollections = GetDescendantCollectionsById(scopeParentCollectionId);
                    return descendantCollections;

                default:
                    throw new ArgumentOutOfRangeException(nameof(compareScopeSettings),
                        $"Unexpected HierarchyScopeOptions value: {compareScopeSettings}");
            }

        }


    }
}
