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
        /// <summary>
        /// Hierarchy level of the road. Lower value means higher priority (e.g., 1 = Main, 2 = Secondary).
        /// </summary>
        public int Level { get; set; } = 1;

        public RoadType(string name, double width, double filletRadius, string layerName, int level = 1)
        {
            Name = name;
            Width = width;
            FilletRadius = filletRadius;
            LayerName = layerName;
            Level = level;
        }

        public override string ToString()
        {
            return $"RoadType: {Name} (W={Width}, R={FilletRadius})";
        }
    }
}
