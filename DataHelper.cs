using System.IO.Compression;
using System.Net;

using SharpCompress.Readers;

namespace Generator
{
    static class DataHelper
    {
        public static async Task DownloadData()
        {
            Directory.CreateDirectory("Data");
            if (!File.Exists("Data/sectors.shp"))
            {
                if (!File.Exists("Data/sectors.shp.zip"))
                {
                    var client = new WebClient();
                    await client.DownloadFileTaskAsync("https://statbel.fgov.be/sites/default/files/files/opendata/Statistische%20sectoren/sh_statbel_statistical_sectors_31370_lastversion.shp.zip", "Data/sectors.shp.zip");
                }
                if (Directory.Exists("Data/Unzip"))
                {
                    Directory.Delete("Data/Unzip", true);
                }
                Directory.CreateDirectory("Data/Unzip/");
                ZipFile.ExtractToDirectory("Data/sectors.shp.zip", "Data/Unzip/");
                File.Delete("Data/sectors.shp.zip");

                var shpPath = Directory.GetFiles("Data/Unzip", "*.shp", SearchOption.AllDirectories).First();
                File.Move(shpPath, "Data/sectors.shp");
                var dbfPath = Directory.GetFiles("Data/Unzip", "*.dbf", SearchOption.AllDirectories).First();
                File.Move(dbfPath, "Data/sectors.dbf");
                Directory.Delete("Data/Unzip", true);
            }
            if (!File.Exists("Data/matrix.sqlite"))
            {
                if (!File.Exists("Data/matrix.sqlite.tar.gz"))
                {
                    var client = new WebClient();
                    await client.DownloadFileTaskAsync("https://statbel.fgov.be/sites/default/files/files/opendata/census%202011Matrix%20woon-%20werkverkeer%20per%20stat%20sec/TU_GEO_LPW_SECTOR.sqlite.tar.gz", "Data/matrix.sqlite.tar.gz");
                }
                
                using (Stream stream = File.OpenRead("Data/matrix.sqlite.tar.gz"))
                using (var reader = ReaderFactory.Open(stream))
                {
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            reader.WriteEntryToFile("Data/matrix.sqlite");
                        }
                    }
                }
                File.Delete("Data/matrix.sqlite.tar.gz");
            }
        }
    }
}