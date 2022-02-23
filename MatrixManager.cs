using Microsoft.Data.Sqlite;

namespace Generator
{
    class MatrixRecord
    {
        public string HomeSector;

        public string WorkSector;

        public int Count;
        public int TotalBefore;
        public int Total;

    }

    class MatrixManager
    {
        Random rand = new Random();
        MatrixRecord[] recordsHeap;
        public async Task Load()
        {
            var records = new List<MatrixRecord>();
            using (var connection = new SqliteConnection("Data Source=Data/matrix.sqlite"))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM TU_GEO_LPW_MATRIX";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var homeRaw = reader.GetString(0);
                        var workRaw = reader.GetString(1);
                        if (!homeRaw.Contains("ZZZ") && !workRaw.Contains("ZZZ") && !homeRaw.Contains("FOR") && !workRaw.Contains("FOR"))
                        {
                            var record = new MatrixRecord()
                            {
                                HomeSector = "s" + homeRaw.Split("_").Last(),
                                WorkSector = "s" + workRaw.Split("_").Last(),
                                Count = (int)reader.GetFloat(2)
                            };
                            records.Add(record);
                        }
                    }
                }
            }
            recordsHeap = records.ToArray();
            CreateHeap(0);
        }

        private void CreateHeap(int idx)
        {
            var left = 2 * (idx+1) - 1;
            var right = 2 * (idx+1);

            recordsHeap[idx].Total = recordsHeap[idx].Count;

            if(left < recordsHeap.Length)
            {
                CreateHeap(left);
                recordsHeap[idx].TotalBefore = recordsHeap[left].Total;
                recordsHeap[idx].Total += recordsHeap[left].Total;
            }
            else
            {
                recordsHeap[idx].TotalBefore = 0;
            }
        
            if(right < recordsHeap.Length)
            {
                CreateHeap(right);
                recordsHeap[idx].Total += recordsHeap[right].Total;
            }
        }

        private MatrixRecord GetPersonaAtIndex(int index, int pos)
        {
            var left = 2 * (pos+1) - 1;
            var right = 2 * (pos+1);

            if(index < recordsHeap[pos].TotalBefore)
            {
                return GetPersonaAtIndex(index, left);
            }
            else if(index < recordsHeap[pos].TotalBefore + recordsHeap[pos].Count)
            {
                return recordsHeap[pos];
            }
            return GetPersonaAtIndex(index - (recordsHeap[pos].TotalBefore + recordsHeap[pos].Count), right);
        }

        public Persona GetRandomPersona()
        {
            var index = rand.Next(recordsHeap[0].Total);
            var result =  GetPersonaAtIndex(index,0);
            return new Persona() { WorkSector = result.WorkSector, HomeSector = result.HomeSector };
        }
    }
}