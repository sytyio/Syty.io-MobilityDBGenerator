using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.Shape.Random;
using NetTopologySuite.Geometries.Utilities;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using ProjNet;

namespace Generator
{

    class GeographyManager
    {
        Dictionary<string, RandomPointsBuilder> sectors = new Dictionary<string, RandomPointsBuilder>();
        
        MTF transformer;

        public GeographyManager()
        {
            NtsGeometryServices.Instance = new NtsGeometryServices(
                NetTopologySuite.Geometries.Implementation.CoordinateArraySequenceFactory.Instance,
                new NetTopologySuite.Geometries.PrecisionModel(1000d),
                4326,
                NetTopologySuite.Geometries.GeometryOverlay.NG,
                new NetTopologySuite.Geometries.CoordinateEqualityComparer());

            var css = new CoordinateSystemServices(new CoordinateSystemFactory(),
                new CoordinateTransformationFactory());
            var lambert = "PROJCS[\"Belge 1972 / Belgian Lambert 72\",GEOGCS[\"Belge 1972\",DATUM[\"Reseau_National_Belge_1972\",SPHEROID[\"International 1924\",6378388,297,AUTHORITY[\"EPSG\",\"7022\"]],TOWGS84[-106.8686,52.2978,-103.7239,0.3366,-0.457,1.8422,-1.2747],AUTHORITY[\"EPSG\",\"6313\"]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"9122\"]],AUTHORITY[\"EPSG\",\"4313\"]],PROJECTION[\"Lambert_Conformal_Conic_2SP\"],PARAMETER[\"standard_parallel_1\",51.16666723333333],PARAMETER[\"standard_parallel_2\",49.8333339],PARAMETER[\"latitude_of_origin\",90],PARAMETER[\"central_meridian\",4.367486666666666],PARAMETER[\"false_easting\",150000.013],PARAMETER[\"false_northing\",5400088.438],UNIT[\"metre\",1,AUTHORITY[\"EPSG\",\"9001\"]],AXIS[\"X\",EAST],AXIS[\"Y\",NORTH],AUTHORITY[\"EPSG\",\"31370\"]]";

            transformer = new MTF(new CoordinateTransformationFactory().CreateFromCoordinateSystems(new CoordinateSystemFactory().CreateFromWkt(lambert),css.GetCoordinateSystem(4326)).MathTransform);
        }

        public async Task Load()
        {
            var factory = new GeometryFactory();
            var shapeFileDataReader = new ShapefileDataReader("Data/sectors.shp", factory);

            while (shapeFileDataReader.Read())
            {
                var geometry = shapeFileDataReader.Geometry;
                TransformPolygon(geometry);
                var key = shapeFileDataReader.GetValue(1).ToString();
                var builder = new RandomPointsBuilder() { NumPoints = 1 };
                builder.SetExtent(geometry);
                sectors.Add("s" + key, builder);
            }

            shapeFileDataReader.Close();
            shapeFileDataReader.Dispose();
        }

        public Point GetLocation(string sector)
        {
            if (sectors.ContainsKey(sector))
            {
                var result = ((MultiPoint)sectors[sector].GetGeometry()).Cast<Point>().First();
                //TransformPoint(result);
                return result;
            }            
            return null;
        }

        public void TransformPoint(Point geom)
        {
            geom.Apply(transformer);
        }
        public void TransformPolygon(Geometry geom)
        {
            geom.Apply(transformer);
        }
    }

    sealed class MTF : NetTopologySuite.Geometries.ICoordinateSequenceFilter
    {
        private readonly MathTransform _mathTransform;

        public MTF(MathTransform mathTransform) => _mathTransform = mathTransform;

        public bool Done => false;
        public bool GeometryChanged => true;
        public void Filter(NetTopologySuite.Geometries.CoordinateSequence seq, int i)
        {
            double x = seq.GetX(i);
            double y = seq.GetY(i);
            double z = seq.GetZ(i);
            _mathTransform.Transform(ref x, ref y, ref z);
            seq.SetX(i, x);
            seq.SetY(i, y);
            seq.SetZ(i, z);
        }
    }
}