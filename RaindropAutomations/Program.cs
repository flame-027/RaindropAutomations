using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RaindropAutomations.Services;
using RaindropAutomations.Tools;
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
                  services.AddHttpClient<IRaindropRepository, RaindropRepository>((provider, client) =>
                  {
                      var config = provider.GetRequiredService<IConfiguration>();
                      var apiToken = config.GetFromRaindropConfig("ApiToken").Value ?? string.Empty;
                      var apiBaseUrl = config.GetFromRaindropConfig("ApiBaseUrl").Value ?? string.Empty;

                      client.BaseAddress = new Uri(apiBaseUrl);
                      client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiToken);
                      client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                  });
                  
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

            var playlistUrl = @"https://www.youtube.com/playlist?list=PLRqwX-V7Uu6Zu_uqEA6NqhLzKLACwU74X";

            var routineManager = host.Services.GetRequiredService<RaindropRoutinesManager>();

            routineManager.YoutubePlaylistToRaindrop(playlistUrl, saveCollectionId, checkParentCollectionId);

            //RaindropApiWrapService.UpdateMultipleBookmarks(50386169, 50386170, [918116881]);    
        }


    }
}