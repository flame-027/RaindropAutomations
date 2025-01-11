using Microsoft.Extensions.Configuration;
using RaindropAutomations.Core.Options;
using RaindropAutomations.Models.Fetching;
using RaindropAutomations.Models.Saving;
using RaindropAutomations.Services;

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

        public static string GetUrlType(this string input, UrlOptions? urlType)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            if(urlType == null)
                return string.Empty;

            if (urlType == UrlOptions.PureUrl)
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
        

    }
}
