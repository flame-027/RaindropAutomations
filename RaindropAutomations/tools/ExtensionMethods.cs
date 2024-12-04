using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Configuration;
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
    }
}
