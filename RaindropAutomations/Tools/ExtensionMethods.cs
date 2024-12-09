using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Configuration;
using RaindropAutomations.Models.Fetching;
using RaindropAutomations.Models.Saving;
using RaindropAutomations.Options;
using RainDropAutomations.Youtube.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaindropAutomations.Tools
{
    public static class ExtensionMethods
    {
        public static IConfigurationSection GetRaindropConfig(this IConfiguration config)
        {
            var raindropConfig = config.GetSection("Raindrop");

            return raindropConfig;
        }

        public static IConfigurationSection GetFromRaindropConfig(this IConfiguration config, string sectionToGet) 
        {
            var sectionResult = config.GetRaindropConfig().GetSection(sectionToGet);

            return sectionResult;
        }

        public static string GetUrlType(this string input, UrlOptions urlType)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            if(urlType == UrlOptions.PureUrl)
            {
                int index = input.IndexOf('?');
                return index >= 0 ? input.Substring(0, index).Trim() : input.Trim();
            }

            if (urlType == UrlOptions.UrlAndFirstParamOnly)
            {
                int index = input.IndexOf('&');
                return index >= 0 ? input.Substring(0, index).Trim() : input.Trim();
            }

            else
                throw new NotImplementedException("Support has not been added for this url option");

            //return input.Split('&')[0].Trim();
            // easy to read but apparently has unnecessary overhead
        }


        public static void RemoveMatchesFromDescendants(this List<BookmarkSaveModel> inputList, long parentCollectionId, SelfInclusionOptions includeParentSettings, UrlOptions? MatchOptions = null)
        {
            // FINDING MATCHES
            var raindropManager = new RaindropManager(GlobalConfig.Config);

            var allMatchingBookmarks = new List<BookmarkFetchModel>();

            if(includeParentSettings == SelfInclusionOptions.ExcludeSelf)
            {
                var descendantCollections = raindropManager.GetDescendantCollectionsById(parentCollectionId);
                var descendantBookmarks = raindropManager.GetBookmarksFromMultipleCollections(descendantCollections.AllIdsWithinForest);

                allMatchingBookmarks = descendantBookmarks ?? [];
            }

            if(includeParentSettings == SelfInclusionOptions.IncludeSelf)
            {
                var parentAndDescendantCollections = raindropManager.GetDescendantAndSelfCollectionsById(parentCollectionId);
                var allBookmarksInParentAndDescendants = raindropManager.GetBookmarksFromMultipleCollections(parentAndDescendantCollections.AllIdsWithinForest);

                allMatchingBookmarks = allBookmarksInParentAndDescendants ?? [];
            }

            // could stop repeated code here by having conditional however I currently like how easy this is to read
            // with seperate concerns and specific variable names. May change down the line depending on hows this method evolves.

            // REMOVING MATCHES
            if (MatchOptions != null && MatchOptions is UrlOptions confirmedType)
            {
                var refinedMatchingUrls = allMatchingBookmarks.Select(x => x.Link
                                                        .GetUrlType(confirmedType))
                                                        .ToList();

               inputList.RemoveAll(x => refinedMatchingUrls.Contains(x.Link
                                                         .GetUrlType(confirmedType)));
            }
            else
            {
                var rawMatchingUrls = allMatchingBookmarks.Select(x => x.Link)
                                                    .ToList();

                inputList.RemoveAll(x => rawMatchingUrls.Contains(x.Link));
            }
            // the else statment could potentially be removed, if support for none was added to options enum 
        }
    }
}
