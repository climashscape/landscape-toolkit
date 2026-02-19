using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace LandscapeToolkit.Data.Graph
{
    /// <summary>
    /// Represents the road network topology graph.
    /// </summary>
    public class RoadGraph
    {
        public List<RoadNode> Nodes { get; private set; } = new List<RoadNode>();
        public List<RoadEdge> Edges { get; private set; } = new List<RoadEdge>();

        /// <summary>
        /// Rebuilds the graph from a set of curves.
        /// </summary>
        public void BuildFromCurves(IEnumerable<Curve> curves, double tolerance)
        {
            // TODO: Implement curve shattering and graph construction logic
            // 1. Shatter curves at intersections
            // 2. Create unique Nodes for endpoints
            // 3. Create Edges linking Nodes
        }
        
        public void AddNode(RoadNode node) => Nodes.Add(node);
        public void AddEdge(RoadEdge edge) => Edges.Add(edge);
    }

    /// <summary>
    /// A node in the road graph (intersection or endpoint).
    /// </summary>
    public class RoadNode
    {
        public int ID { get; set; }
        public Point3d Position { get; set; }
        public List<RoadEdge> ConnectedEdges { get; set; } = new List<RoadEdge>();
        
        public int Valence => ConnectedEdges.Count;

        public RoadNode(Point3d pos)
        {
            Position = pos;
        }
    }

    /// <summary>
    /// An edge in the road graph (a road segment).
    /// </summary>
    public class RoadEdge
    {
        public int ID { get; set; }
        public Curve Centerline { get; set; }
        public RoadNode StartNode { get; set; }
        public RoadNode EndNode { get; set; }
        
        // Attributes
        public Models.RoadType Type { get; set; }
        
        public RoadEdge(Curve curve, RoadNode start, RoadNode end)
        {
            Centerline = curve;
            StartNode = start;
            EndNode = end;
        }
    }
}
