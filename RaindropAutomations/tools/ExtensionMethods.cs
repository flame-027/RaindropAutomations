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
            var raindropManager = new RaindropManager(GlobalConfig.Config);

            var allBookmarksFromParent = new List<BookmarkFetchModel>();

            if(includeParentSettings == SelfInclusionOptions.ExcludeSelf)
            {
                var allDescendantCollectionsInParent = raindropManager.GetDescendantCollectionsById(parentCollectionId);
                var allDescendantBookmarksInParent = raindropManager.GetAllBookmarksFromMultipleCollections(allDescendantCollectionsInParent.AllIdsWithinForest);

                allBookmarksFromParent = allDescendantBookmarksInParent ?? new();
            }

            if(includeParentSettings == SelfInclusionOptions.IncludeSelf)
            {
                throw new NotImplementedException("Support is not added for IncludeSelf yet");
            }

            var bookmarksAsStrings = new List<string>();
            var nonMatchUrls = new List<BookmarkSaveModel>();

            if (MatchOptions != null && MatchOptions is UrlOptions confirmedType)
            {
                bookmarksAsStrings = allBookmarksFromParent.Select(x => x.Link.GetUrlType(confirmedType)).ToList();

               inputList.RemoveAll(x => bookmarksAsStrings.Contains(x.Link.GetUrlType(confirmedType)));
            }
            else
            {
                bookmarksAsStrings = allBookmarksFromParent.Select(x => x.Link).ToList();

                inputList.RemoveAll(x => bookmarksAsStrings.Contains(x.Link));
            }
            // the else statment could potentially be removed, if support for none was added to options enum 
        }
    }
}
