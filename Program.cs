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
            Console.WriteLine("Loading work-home movement matrix...");
            var matrixManager = new MatrixManager();
            await matrixManager.Load();
            Console.WriteLine("Done.");
        }
    }
}