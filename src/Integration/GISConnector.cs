using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace LandscapeToolkit.Integration
{
    /// <summary>
    /// Connects to external GIS data sources (OSM, Shapefiles).
    /// 集成 GIS 与大数据接口（未来扩展）
    /// </summary>
    public class GISConnector
    {
        // Future placeholders
        // 1. Load Shapefile
        public List<GeometryBase> LoadShapefile(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException(nameof(path));
            // TODO: Use GDAL or SharpMap to parse .shp
            throw new NotImplementedException("Shapefile loading is not yet implemented.");
        }

        // 2. Fetch OSM Data
        public List<Curve> FetchOSM(double lat, double lon, double radius)
        {
            if (radius <= 0) throw new ArgumentOutOfRangeException(nameof(radius));
            if (lat < -90 || lat > 90) throw new ArgumentOutOfRangeException(nameof(lat), "Latitude must be between -90 and 90.");
            if (lon < -180 || lon > 180) throw new ArgumentOutOfRangeException(nameof(lon), "Longitude must be between -180 and 180.");
            
            // TODO: Query Overpass API
            throw new NotImplementedException("OSM fetching is not yet implemented.");
        }

        // 3. Generate Terrain from DEM
        public Mesh CreateTerrainFromDEM(string tiffPath)
        {
            if (string.IsNullOrEmpty(tiffPath)) throw new ArgumentNullException(nameof(tiffPath));
            // TODO: Convert GeoTIFF to Mesh
            throw new NotImplementedException("DEM processing is not yet implemented.");
        }
    }
}
