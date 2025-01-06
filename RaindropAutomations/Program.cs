using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RaindropAutomations.Models.Saving;
using RaindropAutomations.Options;
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
            var host = Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((hostingContext, config) =>
               {
                   config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                   //config.AddJsonFile("appsettings.json");
                   config.AddUserSecrets<Program>();
               })
              .ConfigureServices(services =>
              {
                  services.AddSingleton<RaindropApiWrapService>();
                  services.AddSingleton<RaindropOperationsService>();
                  services.AddSingleton<RaindropRoutinesManager>();
                  //services.AddSingleton<IMyService, MyService>();
                  //services.AddTransient<IMyLogic, MyLogic>();
              }).Build();


            var config = host.Services.GetRequiredService<IConfiguration>();

            var saveCollectionId = int.Parse(config["RaindropConfig:VideosSaveInboxId"]);
            var checkParentCollectionId = int.Parse(config["RaindropConfig:VidoesCheckRootId"]);

            var playlistUrl = @"https://www.youtube.com/playlist?list=WL";

            var routineManager = host.Services.GetRequiredService<RaindropRoutinesManager>();

            routineManager.YoutubePlaylistToRaindrop(playlistUrl, saveCollectionId, checkParentCollectionId);

            //RaindropApiWrapService.UpdateMultipleBookmarks(50386169, 50386170, [918116881]);    
        }


    }
}