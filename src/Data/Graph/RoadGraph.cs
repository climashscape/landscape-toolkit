using Rhino.Geometry;
using System;
using System.Collections.Generic;
using LandscapeToolkit.Data;

namespace LandscapeToolkit.Data.Graph
{
    /// <summary>
    /// Represents the road network topology graph.
    /// </summary>
    public class RoadGraph
    {
        public List<RoadNode> Nodes { get; private set; } = new List<RoadNode>();
        public List<RoadEdge> Edges { get; private set; } = new List<RoadEdge>();

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

        public RoadNode() { } // Parameterless constructor
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
        public Curve Curve { get; set; } // Renamed from Centerline to match QuadRoadGenerator usage
        public RoadNode StartNode { get; set; }
        public RoadNode EndNode { get; set; }
        
        // Attributes
        public RoadType Type { get; set; }
        
        public RoadEdge() { } // Parameterless constructor

        public RoadEdge(Curve curve, RoadNode start, RoadNode end)
        {
            Curve = curve;
            StartNode = start;
            EndNode = end;
        }

        public Vector3d GetTangentAtNode(RoadNode node)
        {
            if (node == StartNode) return Curve.TangentAtStart;
            if (node == EndNode) return -Curve.TangentAtEnd;
            return Vector3d.Unset;
        }
    }
}
