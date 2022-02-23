namespace Generator
{
    class Program
    {
        const int MAX_COUNT = 10000000;
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
            Console.WriteLine("Generating "+ MAX_COUNT + " persona schedules...");
            var personaManager = new PersonaManager();
            await personaManager.Generate(MAX_COUNT, matrixManager, geographyManager);
            Console.WriteLine("Done.");
        }
    }
}