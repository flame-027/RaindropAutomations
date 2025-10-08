using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
//using RaindropAutomations.Options;
using RaindropAutomations.Services;
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
                  //services.AddHttpClient();
                  //services.AddHttpClient<RaindropRepository>();
                  
                  services.AddSingleton<IRaindropRepository, RaindropRepository>(); 
                  services.AddSingleton<RaindropApiWrapService>();
                  services.AddSingleton<RaindropOperationsService>();
                  services.AddSingleton<RaindropRoutinesManager>();
                  services.AddSingleton<YoutubeManager>();
                  
                  //services.AddSingleton<IMyService, MyService>();
                  //services.AddTransient<IMyLogic, MyLogic>();
              }).Build();


            var config = host.Services.GetRequiredService<IConfiguration>();
            GlobalConfig.Config = config;

            var saveCollectionId = int.Parse(config["Raindrop:VideosSaveInboxId"]);
            var checkParentCollectionId = int.Parse(config["Raindrop:VideosCheckRootId"]);

            var playlistUrl = @"https://www.youtube.com/playlist?list=WL";

            var routineManager = host.Services.GetRequiredService<RaindropRoutinesManager>();

            routineManager.YoutubePlaylistToRaindrop(playlistUrl, saveCollectionId, checkParentCollectionId);

            //RaindropApiWrapService.UpdateMultipleBookmarks(50386169, 50386170, [918116881]);    
        }


    }
}