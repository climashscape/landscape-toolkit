using Rhino.Geometry;
using System;

namespace LandscapeToolkit.Data
{
    /// <summary>
    /// Defines properties for a road type in the hierarchy.
    /// </summary>
    public class RoadType
    {
        public string Name { get; set; }
        public double Width { get; set; }
        public double FilletRadius { get; set; }
        public string LayerName { get; set; }

        public RoadType(string name, double width, double filletRadius, string layerName)
        {
            Name = name;
            Width = width;
            FilletRadius = filletRadius;
            LayerName = layerName;
        }

        public override string ToString()
        {
            return $"RoadType: {Name} (W={Width}, R={FilletRadius})";
        }
    }
}
