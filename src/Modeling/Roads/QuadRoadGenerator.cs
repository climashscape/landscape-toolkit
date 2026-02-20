using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;
using LandscapeToolkit.Data;
using LandscapeToolkit.Data.Graph;

namespace LandscapeToolkit.Modeling.Roads
{
    /// <summary>
    /// Core logic for generating Quad Mesh road networks from centerlines.
    /// 核心逻辑：从中心线生成四边面路网
    /// </summary>
    public class QuadRoadGenerator
    {
        public List<Curve> Centerlines { get; set; }
        public List<double> RoadWidths { get; set; }
        public double DefaultWidth { get; set; } = 6.0;
        public double DefaultIntersectionRadius { get; set; } = 9.0;
        public RoadGraph GeneratedGraph { get; private set; }
        public List<Mesh> JunctionMeshes { get; private set; }
        public List<Mesh> StreetMeshes { get; private set; }

        public QuadRoadGenerator(List<Curve> curves, List<double> widths = null)
        {
            // Sanitize input: Remove null curves
            Centerlines = curves?.Where(c => c != null).ToList() ?? new List<Curve>();
            RoadWidths = new List<double>();

            if (widths != null && widths.Count > 0)
            {
                if (widths.Count == 1)
                {
                    // Single width applies to all
                    for (int i = 0; i < Centerlines.Count; i++) RoadWidths.Add(widths[0]);
                }
                else if (widths.Count == Centerlines.Count)
                {
                    // Copy to prevent external modification affecting this instance
                    RoadWidths = new List<double>(widths);
                }
                else
                {
                    // Mismatch: Cycle or pad with last
                    for (int i = 0; i < Centerlines.Count; i++)
                    {
                        if (i < widths.Count) RoadWidths.Add(widths[i]);
                        else RoadWidths.Add(widths[widths.Count - 1]);
                    }
                }
            }
            else
            {
                // Use default
                for(int i=0; i<Centerlines.Count; i++) RoadWidths.Add(DefaultWidth);
            }
        }

        /// <summary>
        /// Main execution method.
        /// 执行生成过程
        /// </summary>
        public Mesh Generate()
        {
            // Step 0: Pre-process (Flatten and Shatter)
            // 步骤0：预处理（压平并打断）
            var processedCurves = PreProcessCurves(Centerlines, RoadWidths, out List<double> processedWidths);

            // Step 1: Clean and Node Identification
            // 步骤1：清理曲线并识别节点（路口）
            var graph = BuildGraph(processedCurves, processedWidths);
            GeneratedGraph = graph;

            // Step 2: Generate Junction Meshes (3-way, 4-way, N-way)
            // 步骤2：生成路口网格
            JunctionMeshes = new List<Mesh>();
            foreach (var node in graph.Nodes)
            {
                JunctionMeshes.Add(CreateJunctionMesh(node));
            }

            // Step 3: Generate Street Segments (Quad Strips)
            // 步骤3：生成路段网格（四边面带）
            StreetMeshes = new List<Mesh>();
            foreach (var edge in graph.Edges)
            {
                StreetMeshes.Add(CreateStreetStrip(edge));
            }

            // Step 4: Combine and Weld
            // 步骤4：合并并焊接顶点
            Mesh finalMesh = new Mesh();
            finalMesh.Append(JunctionMeshes);
            finalMesh.Append(StreetMeshes);
            finalMesh.Vertices.CombineIdentical(true, true);
            finalMesh.Weld(3.14159); // Weld everything for smooth shading if needed
            finalMesh.UnifyNormals(); // Ensure consistent normal direction

            // Step 5: Optional Relaxation (Laplacian Smoothing)
            // 步骤5：可选的松弛平滑，使网格流动更自然
            finalMesh = RelaxMesh(finalMesh);

            return finalMesh;
        }

        private List<Curve> PreProcessCurves(List<Curve> inputCurves, List<double> inputWidths, out List<double> outWidths)
        {
            // Flatten all curves to XY plane to ensure intersections are found
            var flatCurves = new List<Curve>();
            var flatWidths = new List<double>();
            
            for(int i=0; i<inputCurves.Count; i++)
            {
                var c = inputCurves[i].DuplicateCurve();
                // Project to XY
                if (!c.IsPlanar(Rhino.RhinoMath.ZeroTolerance))
                {
                     c = Curve.ProjectToPlane(c, Plane.WorldXY);
                }
                else
                {
                    // Even if planar, force Z=0 for consistency
                    var poly = c.ToPolyline(Rhino.RhinoMath.ZeroTolerance, Rhino.RhinoMath.ZeroTolerance, 0.1, 10000);
                    if (poly != null)
                    {
                         var pl = poly.ToPolyline();
                         for(int k=0; k<pl.Count; k++) pl[k] = new Point3d(pl[k].X, pl[k].Y, 0);
                         c = pl.ToPolylineCurve();
                    }
                    else
                    {
                         c = Curve.ProjectToPlane(c, Plane.WorldXY);
                    }
                }
                
                if (c != null && c.IsValid)
                {
                    flatCurves.Add(c);
                    flatWidths.Add(inputWidths[i]);
                }
            }

            // Now shatter them at intersections
            // We use a simplified approach: Split all by all
            // Ideally we should use a graph builder, but here we just want segments.
            
            // To preserve widths, we need to map segments back to original width
            // A simple way is to use Curve.Split, and for each result segment, assign the original width.
            
            // However, Split doesn't handle self-intersections or complex networks well in one go.
            // Let's use Geometry.Intersect.Intersection.CurveCurve
            
            // Better strategy:
            // 1. Get all intersection parameters for each curve
            // 2. Split each curve
            // 3. Collect all segments
            
            Dictionary<int, List<double>> splitParams = new Dictionary<int, List<double>>();
            for (int i = 0; i < flatCurves.Count; i++) splitParams[i] = new List<double>();

            double tol = 0.01;

            for (int i = 0; i < flatCurves.Count; i++)
            {
                for (int j = i + 1; j < flatCurves.Count; j++)
                {
                    var events = Rhino.Geometry.Intersect.Intersection.CurveCurve(flatCurves[i], flatCurves[j], tol, tol);
                    if (events != null)
                    {
                        foreach (var e in events)
                        {
                             // Only split at points, not overlaps
                             if (e.IsPoint)
                             {
                                 splitParams[i].Add(e.ParameterA);
                                 splitParams[j].Add(e.ParameterB);
                             }
                        }
                    }
                }
            }
            
            var resultCurves = new List<Curve>();
            outWidths = new List<double>();
            
            for (int i = 0; i < flatCurves.Count; i++)
            {
                var c = flatCurves[i];
                var pars = splitParams[i].Distinct().ToList();
                pars.Sort();
                
                // Filter parameters near start/end
                var domain = c.Domain;
                pars.RemoveAll(p => Math.Abs(p - domain.Min) < tol || Math.Abs(p - domain.Max) < tol);
                
                var pieces = c.Split(pars);
                if (pieces != null)
                {
                    foreach (var p in pieces)
                    {
                        if (p.GetLength() > tol)
                        {
                            resultCurves.Add(p);
                            outWidths.Add(flatWidths[i]);
                        }
                    }
                }
                else
                {
                    if (c.GetLength() > tol)
                    {
                        resultCurves.Add(c);
                        outWidths.Add(flatWidths[i]);
                    }
                }
            }
            
            return resultCurves;
        }

        private RoadGraph BuildGraph(List<Curve> allSegments, List<double> allSegmentWidths)
        {
            RoadGraph graph = new RoadGraph();
            double tolerance = 0.01; 

            // 2. Build Nodes and Edges
            for (int i = 0; i < allSegments.Count; i++)
            {
                Curve seg = allSegments[i];
                double w = allSegmentWidths[i];
                
                Point3d start = seg.PointAtStart;
                Point3d end = seg.PointAtEnd;
                
                RoadNode startNode = GetOrCreateNode(graph, start, tolerance);
                RoadNode endNode = GetOrCreateNode(graph, end, tolerance);
                
                RoadEdge edge = new RoadEdge(seg, startNode, endNode)
                {
                    Type = new RoadType("Generic", w, DefaultIntersectionRadius, "Default")
                };
                
                graph.Edges.Add(edge);
                
                startNode.ConnectedEdges.Add(edge);
                endNode.ConnectedEdges.Add(edge);
            }

            return graph;
        }

        private RoadNode GetOrCreateNode(RoadGraph graph, Point3d pt, double tolerance)
        {
            // Simple linear search. For performance, use RTree.
            foreach (var node in graph.Nodes)
            {
                if (node.Position.DistanceTo(pt) < tolerance) return node;
            }
            var newNode = new RoadNode(pt);
            graph.Nodes.Add(newNode);
            return newNode;
        }

        private Mesh CreateJunctionMesh(RoadNode node)
        {
            Mesh mesh = new Mesh();
            int edgeCount = node.ConnectedEdges.Count;

            // 1. Sort edges by angle
            var sortedEdges = SortEdgesByAngle(node);
            
            // 2. Calculate corner points
            List<Point3d> cornerPoints = new List<Point3d>();
            List<Point3d> connectionPoints = new List<Point3d>(); 

            // Determine safe radius for each edge to avoid overlapping or flipping
            // Calculate angles between edges
            List<double> angles = new List<double>();
            for(int i=0; i<edgeCount; i++)
            {
                int next = (i + 1) % edgeCount;
                Vector3d v1 = sortedEdges[i].GetTangentAtNode(node);
                Vector3d v2 = sortedEdges[next].GetTangentAtNode(node);
                double angle = Vector3d.VectorAngle(v1, v2);
                angles.Add(angle);
            }

            for (int i = 0; i < edgeCount; i++)
            {
                var edge = sortedEdges[i];
                double w = edge.Type.Width;
                double r = edge.Type.FilletRadius; 
                
                // Dynamic radius adjustment based on adjacent angles
                // Angle to previous
                int prevIdx = (i + edgeCount - 1) % edgeCount;
                double anglePrev = angles[prevIdx]; // Angle between prev and current
                double angleNext = angles[i];       // Angle between current and next
                
                double minAngle = Math.Min(anglePrev, angleNext);
                
                // Limit radius to prevent self-intersection in sharp corners
                // Simple heuristic: r should not be too large if angle is small
                // If angle is 90 deg, tan(45) = 1, dist = r.
                // If angle is 30 deg, tan(15) = 0.26, dist = r * 3.8.
                // Limit dist to a max value.
                
                double maxDist = 20.0; // Hard limit
                double edgeLen = edge.Curve.GetLength();
                maxDist = Math.Min(maxDist, edgeLen * 0.45); // Don't consume more than 45% of edge
                
                // Effective radius calculation
                // We want the connection point distance 'd'
                // d = r / tan(alpha/2) ?? No, this is for corner offset.
                // Actually we are placing a "cap" on the road end.
                // The distance from node center to the cap start is 'r'.
                // But we must ensure 'r' doesn't push the side corners too far out or in.
                
                // Let's stick to user 'r' but clamp it.
                if (r > maxDist) r = maxDist;
                
                // Also check if r is too small for the width? 
                // No, r is the distance along the centerline.
                
                Vector3d tangent = edge.GetTangentAtNode(node);
                Point3d centerOnEdge = node.Position + tangent * r;
                connectionPoints.Add(centerOnEdge);

                Vector3d perp = Vector3d.CrossProduct(tangent, Vector3d.ZAxis);
                if (perp.IsTiny(1e-6))
                {
                    perp = Vector3d.CrossProduct(tangent, Vector3d.XAxis);
                    if (perp.IsTiny(1e-6)) perp = Vector3d.CrossProduct(tangent, Vector3d.YAxis);
                }
                perp.Unitize();
                
                Point3d left = centerOnEdge + perp * (w * 0.5);
                Point3d right = centerOnEdge - perp * (w * 0.5);
                
                cornerPoints.Add(left);
                cornerPoints.Add(right);
            }

            // 3. Generate Topology based on degree
            if (edgeCount == 1) // Dead End
            {
                Point3d p1 = cornerPoints[0];
                Point3d p2 = cornerPoints[1];
                Vector3d dir = sortedEdges[0].GetTangentAtNode(node);
                double w = sortedEdges[0].Type.Width;
                double r = sortedEdges[0].Type.FilletRadius;
                
                mesh.Vertices.Add(p1);
                mesh.Vertices.Add(p2);
                mesh.Vertices.Add(node.Position); 
                
                // Create a rounded cap or just a box end
                Point3d pEnd = node.Position + dir * (r + w * 0.5); // Extend a bit
                
                // Quad 1: Connection
                mesh.Vertices.Add(pEnd - (p2 - node.Position)); 
                mesh.Vertices.Add(pEnd - (p1 - node.Position)); 
                
                // Simply 2 quads?
                // 0(L), 1(R), 2(Center)
                // Let's make it a simple quad cap
                mesh = new Mesh();
                mesh.Vertices.Add(cornerPoints[1]); // Right
                mesh.Vertices.Add(cornerPoints[0]); // Left
                mesh.Vertices.Add(node.Position - dir * (w*0.5) + (cornerPoints[0]-connectionPoints[0])); // Back Left
                mesh.Vertices.Add(node.Position - dir * (w*0.5) + (cornerPoints[1]-connectionPoints[0])); // Back Right
                mesh.Faces.AddFace(0, 1, 2, 3);
            }
            else if (edgeCount == 2) // Corner
            {
                mesh.Vertices.Add(cornerPoints[1]); // Edge1 Right
                mesh.Vertices.Add(cornerPoints[0]); // Edge1 Left
                mesh.Vertices.Add(cornerPoints[2]); // Edge2 Left
                mesh.Vertices.Add(cornerPoints[3]); // Edge2 Right
                mesh.Faces.AddFace(0, 1, 2, 3);
            }
            else if (edgeCount >= 3) // 3-Way, 4-Way, N-Way
            {
                // Generalized Fan/Star Topology with safety check
                mesh.Vertices.Add(node.Position); // 0
                List<int> centerIndices = new List<int>();
                List<int> leftIndices = new List<int>();
                List<int> rightIndices = new List<int>();
                List<int> valleyIndices = new List<int>();

                for (int i = 0; i < edgeCount; i++)
                {
                    mesh.Vertices.Add(connectionPoints[i]);
                    centerIndices.Add(mesh.Vertices.Count - 1);
                    mesh.Vertices.Add(cornerPoints[i * 2]);
                    leftIndices.Add(mesh.Vertices.Count - 1);
                    mesh.Vertices.Add(cornerPoints[i * 2 + 1]);
                    rightIndices.Add(mesh.Vertices.Count - 1);
                }
                
                // Valley points (between edges)
                for (int i = 0; i < edgeCount; i++)
                {
                    int next = (i + 1) % edgeCount;
                    Point3d pRight = cornerPoints[i * 2 + 1]; // Current edge Right
                    Point3d pNextLeft = cornerPoints[next * 2]; // Next edge Left
                    
                    // Improved Valley Point Calculation
                    // Instead of simple average, intersect the edge boundary lines
                    // Line 1: pRight - (pRight's Edge Vector)
                    // Line 2: pNextLeft - (pNextLeft's Edge Vector)
                    
                    Vector3d dir1 = -sortedEdges[i].GetTangentAtNode(node);
                    Vector3d dir2 = -sortedEdges[next].GetTangentAtNode(node);
                    
                    Point3d valley;
                    
                    // Intersect lines
                    if (!Rhino.Geometry.Intersect.Intersection.LineLine(new Line(pRight, dir1), new Line(pNextLeft, dir2), out double a, out double b, 0.01, false))
                    {
                        // Parallel or skew (should be parallel if width same and angle 0/180)
                        valley = (pRight + pNextLeft) * 0.5;
                        valley = (valley + node.Position) * 0.5; // Pull towards center to avoid huge extension
                    }
                    else
                    {
                         // Intersection found
                         Point3d intPt = new Line(pRight, dir1).PointAt(a);
                         
                         // Safety: Check if intersection is too far inward (past the node center)
                         // Project intPt onto bisector?
                         // Or just clamp distance from node
                         double dist = intPt.DistanceTo(node.Position);
                         if (dist > 30.0) // Heuristic clamp
                         {
                             valley = (pRight + pNextLeft) * 0.5;
                         }
                         else
                         {
                             valley = intPt;
                             // Pull valley slightly towards center to make quads nicer (Chamfer effect)
                             valley = valley * 0.8 + node.Position * 0.2; 
                         }
                    }

                    mesh.Vertices.Add(valley);
                    valleyIndices.Add(mesh.Vertices.Count - 1);
                }
                
                // Faces
                for (int i = 0; i < edgeCount; i++)
                {
                    int prev = (i + edgeCount - 1) % edgeCount;
                    // Left Quad
                    mesh.Faces.AddFace(0, centerIndices[i], leftIndices[i], valleyIndices[prev]);
                    // Right Quad
                    mesh.Faces.AddFace(0, valleyIndices[i], rightIndices[i], centerIndices[i]);
                }
            }
            
            return mesh;
        }

        private List<RoadEdge> SortEdgesByAngle(RoadNode node)
        {
            var edges = new List<RoadEdge>(node.ConnectedEdges);
            edges.Sort((a, b) =>
            {
                Vector3d dirA = a.GetTangentAtNode(node);
                Vector3d dirB = b.GetTangentAtNode(node);
                double angleA = Math.Atan2(dirA.Y, dirA.X);
                double angleB = Math.Atan2(dirB.Y, dirB.X);
                return angleA.CompareTo(angleB);
            });
            return edges;
        }

        private Mesh CreateStreetStrip(RoadEdge edge)
        {
            double w = edge.Type.Width;
            double r = edge.Type.FilletRadius;

            Curve c = edge.Curve;
            if (c == null) return new Mesh();
            
            double len = c.GetLength();
            double startDist = r;
            double endDist = len - r;

            if (endDist <= startDist) return new Mesh();

            // Trim curve to valid length
            // We need parameters for start and end distance
            c.LengthParameter(startDist, out double tStart);
            c.LengthParameter(endDist, out double tEnd);

            Curve trimmed = c.Trim(new Interval(tStart, tEnd));
            
            if (trimmed == null) return new Mesh();

            double trimmedLen = trimmed.GetLength();
            int segCount = (int)(trimmedLen / w); // Try to keep quads square
            if (segCount < 1) segCount = 1;
            
            double[] params_ = trimmed.DivideByCount(segCount, true);
            if (params_ == null) return new Mesh();

            Mesh mesh = new Mesh();
            
            for (int i = 0; i < params_.Length; i++)
            {
                double t = params_[i];
                Point3d pt = trimmed.PointAt(t);
                Vector3d tan = trimmed.TangentAt(t);
                Vector3d perp = Vector3d.CrossProduct(tan, Vector3d.ZAxis);
                if (perp.IsTiny(1e-6))
                {
                    perp = Vector3d.CrossProduct(tan, Vector3d.XAxis);
                    if (perp.IsTiny(1e-6)) perp = Vector3d.CrossProduct(tan, Vector3d.YAxis);
                }
                perp.Unitize();

                Point3d left = pt + perp * (w * 0.5);
                Point3d right = pt - perp * (w * 0.5);

                mesh.Vertices.Add(left);
                mesh.Vertices.Add(right);
            }

            for (int i = 0; i < segCount; i++)
            {
                int baseIdx = i * 2;
                // Add quad face (0, 1, 3, 2) order
                mesh.Faces.AddFace(baseIdx, baseIdx + 1, baseIdx + 3, baseIdx + 2);
            }

            return mesh;
        }

        private Mesh RelaxMesh(Mesh input)
        {
            if (input == null || input.Vertices.Count == 0) return input;

            // Laplacian smoothing logic
            // 1. Identify boundary vertices (naked edges) to pin them
            bool[] isBoundary = input.GetNakedEdgePointStatus();
            if (isBoundary == null) return input;
            
            // 2. Build adjacency list
            var topo = input.TopologyEdges;
            int vertexCount = input.Vertices.Count;
            Point3d[] newVertices = new Point3d[vertexCount];
            
            // Initialize newVertices with current positions
            for(int i=0; i<vertexCount; i++) newVertices[i] = input.Vertices[i];

            // 3. Iterations
            int iterations = 3;
            double strength = 0.5;

            for (int iter = 0; iter < iterations; iter++)
            {
                // Calculate new positions
                Point3d[] currentIterVertices = new Point3d[vertexCount];
                
                for (int i = 0; i < vertexCount; i++)
                {
                    if (isBoundary[i])
                    {
                        currentIterVertices[i] = newVertices[i];
                        continue;
                    }

                    int topoIndex = input.TopologyVertices.TopologyVertexIndex(i);
                    int[] connectedEdges = input.TopologyVertices.ConnectedEdges(topoIndex);
                    
                    Point3d sum = Point3d.Origin;
                    int count = 0;

                    foreach (int edgeIndex in connectedEdges)
                    {
                        var pair = topo.GetTopologyVertices(edgeIndex);
                        int otherTopoIndex = (pair.I == topoIndex) ? pair.J : pair.I;
                        
                        // Get point from topology vertex
                        // Note: TopologyVertices stores the representative point for welded vertices
                        Point3d neighborPt = input.TopologyVertices[otherTopoIndex];
                        sum += neighborPt;
                        count++;
                    }

                    if (count > 0)
                    {
                        Point3d avg = sum / count;
                        Point3d current = newVertices[i];
                        Vector3d move = avg - current;
                        currentIterVertices[i] = current + move * strength;
                    }
                    else
                    {
                        currentIterVertices[i] = newVertices[i];
                    }
                }
                
                // Update array for next iteration
                newVertices = currentIterVertices;
            }

            // Update mesh vertices finally
            for (int i = 0; i < vertexCount; i++)
            {
                input.Vertices[i] = new Point3f((float)newVertices[i].X, (float)newVertices[i].Y, (float)newVertices[i].Z);
            }
            
            input.Normals.ComputeNormals();
            input.Compact();
            return input;
        }
    }
}
