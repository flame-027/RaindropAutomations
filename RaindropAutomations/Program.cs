using Microsoft.Extensions.Configuration;
using RaindropAutomations.Models.Saving;
using RaindropAutomations.Services;
using RaindropAutomations.Tools;
using RainDropAutomations.Youtube.Models;
using System.Reflection.Metadata.Ecma335;
using YoutubeAutomation;

namespace RaindropAutomations
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                         //.AddJsonFile("appsettings.json")
                        .AddUserSecrets<Program>()
                        .Build();

            GlobalConfig.Config = config;

            var saveCollectionId = int.Parse(config.GetFromRaindropConfig("VideosSaveInboxId").Value);
            var checkParentCollectionId = int.Parse(config.GetFromRaindropConfig("VidoesCheckRootId").Value);

            var playlistUrl = @"https://www.youtube.com/playlist?list=WL";

            var routineManager = new RaindropRoutinesManager(new YoutubeManager(config), new RaindropApiWrapService(new RaindropRepository(config, new HttpClient())));

            routineManager.YoutubePlaylistToRaindrop(playlistUrl, saveCollectionId, checkParentCollectionId);

            //raindropApiService.UpdateMultipleBookmarks(50386169, 50386170, [918116881]);    
        }


    }
}