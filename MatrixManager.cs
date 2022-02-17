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
                        var record = new MatrixRecord()
                        {
                            HomeSector = reader.GetString(0).Replace("BE100_","s"),
                            WorkSector = reader.GetString(1).Replace("BE100_","s"),
                            Count = (int)reader.GetFloat(2)
                        };
                        records.Add(record);
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
    }
}