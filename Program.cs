namespace Generator
{

    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Downloading required data files...");
            await DataHelper.DownloadData();
            Console.WriteLine("Loading geography definitions...");
            var geographyManager = new GeographyManager();
            await geographyManager.Load();
            Console.WriteLine("Done.");
        }
    }
}