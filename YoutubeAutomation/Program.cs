namespace YoutubeAutomation
{
    public class Program : YoutubeManager
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");

            var youtubeManager = new YoutubeManager();
            youtubeManager.Main();
        }
    }
}
