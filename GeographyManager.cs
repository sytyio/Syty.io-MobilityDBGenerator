using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;


namespace Generator
{
    class GeographyManager
    {
        Dictionary<string, Geometry> sectors = new Dictionary<string, Geometry>();

        public GeographyManager()
        {
            NtsGeometryServices.Instance = new NtsGeometryServices(
                NetTopologySuite.Geometries.Implementation.CoordinateArraySequenceFactory.Instance,
                new NetTopologySuite.Geometries.PrecisionModel(1000d),
                4326,
                NetTopologySuite.Geometries.GeometryOverlay.NG,
                new NetTopologySuite.Geometries.CoordinateEqualityComparer());
        }

        public async Task Load()
        {
            var factory = new GeometryFactory();
            var shapeFileDataReader = new ShapefileDataReader("Data/sectors.shp", factory);
            
            while (shapeFileDataReader.Read())
            {
                var geometry = shapeFileDataReader.Geometry;
                var key = shapeFileDataReader.GetValue(1).ToString();
                Console.WriteLine(key);
                sectors.Add("s"+key, geometry);
            }

            shapeFileDataReader.Close();
            shapeFileDataReader.Dispose();
        }
    }
}