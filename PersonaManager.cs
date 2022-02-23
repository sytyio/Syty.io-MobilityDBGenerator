namespace Generator
{
    class PersonaManager
    {
        private Random rand = new Random();
        const int batchSize = 100;
        public PersonaManager()
        {

        }

        public async Task Generate(int count, MatrixManager matrixManager, GeographyManager geographyManager)
        {
            using (StreamWriter sql = new StreamWriter("Data/generator.sql"))
            {
                foreach(var line in File.ReadAllLines("header.sql"))
                {
                    sql.WriteLine(line);
                }
                for (int i = 0; i < count; i++)
                {
                    if (i % batchSize == 0)
                    {
                        Console.WriteLine(".");
                        sql.WriteLine("INSERT INTO persona (home_sector, work_sector, home_location, work_location, gender, age) VALUES ");
                    }
                    var persona = matrixManager.GetRandomPersona();
                    persona.HomeLocation = geographyManager.GetLocation(persona.HomeSector);
                    persona.WorkLocation = geographyManager.GetLocation(persona.WorkSector);
                    if (persona.HomeLocation == null)
                    {
                        i--;
                    }
                    else if (persona.WorkLocation == null)
                    {
                        i--;
                    }
                    else
                    {
                        sql.Write("('" + persona.HomeSector + "', '" + persona.WorkSector + "', '" + persona.HomeLocation.ToText() + "', '" + persona.WorkLocation.ToText() + "', '" + (rand.Next(2) > 0 ? "Male" : "Female") + "', " + ((int)(rand.NextDouble()*rand.NextDouble()*80+18)) + ")");
                        if (i + 1 == count || (i + 1) % batchSize == 0)
                        {
                            sql.WriteLine(";");
                        }
                        else
                        {
                            sql.WriteLine(",");
                        }
                    }

                }
            }
        }

    }
}