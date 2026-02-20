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
        // New structure to hold input data
        private class RoadInputData
        {
            public List<Curve> Curves { get; set; }
            public double Width { get; set; }
            public double Radius { get; set; }
            public int Level { get; set; }
        }

        private readonly List<RoadInputData> _inputs = new List<RoadInputData>();
        
        // Backward compatibility properties
        public List<Curve> Centerlines { get; set; }
        public List<double> RoadWidths { get; set; }
        public double DefaultWidth { get; set; } = 6.0;
        public double DefaultIntersectionRadius { get; set; } = 9.0;
        
        public RoadGraph GeneratedGraph { get; private set; }
        
        // Output Meshes by Level
        public Dictionary<int, List<Mesh>> MeshesByLevel { get; private set; }
        public List<Mesh> JunctionMeshes { get; private set; } // Legacy aggregate
        public List<Mesh> StreetMeshes { get; private set; } // Legacy aggregate

        // Store calculated radii for each edge end to ensure gapless connections
        private Dictionary<RoadEdge, double> _edgeStartRadii;
        private Dictionary<RoadEdge, double> _edgeEndRadii;

        public QuadRoadGenerator()
        {
            // Empty constructor for advanced usage
        }

        public QuadRoadGenerator(List<Curve> curves, List<double> widths = null)
        {
            // Legacy constructor support
            AddRoads(curves, 1, DefaultWidth, DefaultIntersectionRadius);
            
            // Handle custom widths if provided (override default width)
            if (widths != null && widths.Count > 0 && _inputs.Count > 0)
            {
                 // This is tricky because the legacy logic supported per-curve width.
                 // We will handle this in PreProcess by injecting custom logic if needed.
                 // For now, let's just store them and use them if available.
                 Centerlines = curves;
                 RoadWidths = widths;
            }
        }

        public void AddRoads(List<Curve> curves, int level, double width, double radius)
        {
            if (curves == null) return;
            _inputs.Add(new RoadInputData 
            { 
                Curves = curves.Where(c => c != null).ToList(), 
                Level = level, 
                Width = width, 
                Radius = radius 
            });
        }

        /// <summary>
        /// Main execution method.
        /// 执行生成过程
        /// </summary>
        public Mesh Generate()
        {
            _edgeStartRadii = new Dictionary<RoadEdge, double>();
            _edgeEndRadii = new Dictionary<RoadEdge, double>();
            MeshesByLevel = new Dictionary<int, List<Mesh>>();
            JunctionMeshes = new List<Mesh>();
            StreetMeshes = new List<Mesh>();

            // Step 0: Pre-process (Flatten and Shatter)
            // 步骤0：预处理（压平并打断）
            var processedData = PreProcessCurvesMulti();

            // Step 1: Clean and Node Identification
            // 步骤1：清理曲线并识别节点（路口）
            var graph = BuildGraph(processedData);
            GeneratedGraph = graph;

            // Step 2: Generate Junction Meshes
            // 步骤2：生成路口网格
            foreach (var node in graph.Nodes)
            {
                var result = CreateJunctionMeshes(node);
                foreach(var item in result)
                {
                    int level = item.Item2;
                    if (!MeshesByLevel.ContainsKey(level)) MeshesByLevel[level] = new List<Mesh>();
                    MeshesByLevel[level].Add(item.Item1);
                    JunctionMeshes.Add(item.Item1);
                }
            }

            // Step 3: Generate Street Segments (Quad Strips)
            // 步骤3：生成路段网格（四边面带）
            foreach (var edge in graph.Edges)
            {
                Mesh m = CreateStreetStrip(edge);
                int level = edge.Type.Level;
                if (!MeshesByLevel.ContainsKey(level)) MeshesByLevel[level] = new List<Mesh>();
                MeshesByLevel[level].Add(m);
                StreetMeshes.Add(m);
            }

            // Step 4: Combine and Weld
            // 步骤4：合并并焊接顶点 (Per Level? Or All?)
            // User requested "断开" (disconnected) for different levels.
            // So we should weld per level.
            
            Mesh finalMesh = new Mesh();
            
            foreach(var level in MeshesByLevel.Keys)
            {
                List<Mesh> levelMeshes = MeshesByLevel[level];
                Mesh combined = new Mesh();
                combined.Append(levelMeshes);
                combined.Vertices.CombineIdentical(true, true);
                combined.Weld(3.14159);
                combined.UnifyNormals();
                
                // Relax per level? Or global?
                // If we relax per level, they might separate more.
                // Let's relax per level for now.
                combined = RelaxMesh(combined);
                
                finalMesh.Append(combined);
            }

            return finalMesh;
        }

        private List<(Curve curve, int level, double width, double radius)> PreProcessCurvesMulti()
        {
            // Flatten all inputs into a single list but keep track of properties
            var flatCurves = new List<Curve>();
            var properties = new List<(int level, double width, double radius)>();
            
            // If legacy inputs are used (Centerlines/RoadWidths are set but _inputs is empty or just default)
            if (RoadWidths != null && RoadWidths.Count > 0 && (_inputs.Count == 0 || (_inputs.Count == 1 && _inputs[0].Curves == Centerlines)))
            {
                // Legacy mode handling
                for(int i=0; i<Centerlines.Count; i++)
                {
                    double w = (i < RoadWidths.Count) ? RoadWidths[i] : RoadWidths.Last();
                    // Project to XY
                    Curve c = ProjectToXY(Centerlines[i]);
                    if (c != null)
                    {
                        flatCurves.Add(c);
                        properties.Add((1, w, DefaultIntersectionRadius));
                    }
                }
            }
            else
            {
                // Multi-input mode
                foreach(var input in _inputs)
                {
                    foreach(var cRaw in input.Curves)
                    {
                        Curve c = ProjectToXY(cRaw);
                        if (c != null)
                        {
                            flatCurves.Add(c);
                            properties.Add((input.Level, input.Width, input.Radius));
                        }
                    }
                }
            }

            // Intersection and Shattering
            Dictionary<int, List<double>> splitParams = new Dictionary<int, List<double>>();
            for (int i = 0; i < flatCurves.Count; i++) splitParams[i] = new List<double>();

            double tol = 0.01;

            // Find Intersections (including endpoints touching other curves)
            for (int i = 0; i < flatCurves.Count; i++)
            {
                for (int j = i + 1; j < flatCurves.Count; j++)
                {
                    Curve cA = flatCurves[i];
                    Curve cB = flatCurves[j];

                    // 1. Standard Intersection
                    var events = Rhino.Geometry.Intersect.Intersection.CurveCurve(cA, cB, tol, tol);
                    if (events != null)
                    {
                        foreach (var e in events)
                        {
                             if (e.IsPoint)
                             {
                                 splitParams[i].Add(e.ParameterA);
                                 splitParams[j].Add(e.ParameterB);
                             }
                        }
                    }

                    // 2. Check Endpoints of A on B
                    CheckEndpointOnCurve(cA.PointAtStart, cB, splitParams[j], tol);
                    CheckEndpointOnCurve(cA.PointAtEnd, cB, splitParams[j], tol);

                    // 3. Check Endpoints of B on A
                    CheckEndpointOnCurve(cB.PointAtStart, cA, splitParams[i], tol);
                    CheckEndpointOnCurve(cB.PointAtEnd, cA, splitParams[i], tol);
                }
            }
            
            var result = new List<(Curve curve, int level, double width, double radius)>();
            
            for (int i = 0; i < flatCurves.Count; i++)
            {
                var curve = flatCurves[i];
                var (level, width, radius) = properties[i];
                var pars = splitParams[i].Distinct().ToList();
                pars.Sort();
                
                var domain = curve.Domain;
                pars.RemoveAll(p => Math.Abs(p - domain.Min) < tol || Math.Abs(p - domain.Max) < tol);
                
                var pieces = curve.Split(pars);
                if (pieces != null)
                {
                    foreach (var p in pieces)
                    {
                        if (p.GetLength() > tol)
                        {
                            result.Add((p, level, width, radius));
                        }
                    }
                }
                else
                {
                    if (curve.GetLength() > tol)
                    {
                        result.Add((curve, level, width, radius));
                    }
                }
            }
            
            return result;
        }

        private void CheckEndpointOnCurve(Point3d pt, Curve c, List<double> splitParams, double tol)
        {
            if (c.ClosestPoint(pt, out double t))
            {
                if (pt.DistanceTo(c.PointAt(t)) < tol)
                {
                    splitParams.Add(t);
                }
            }
        }

        private Curve ProjectToXY(Curve c)
        {
            if (c == null) return null;
            var dup = c.DuplicateCurve();
            if (!dup.IsPlanar(Rhino.RhinoMath.ZeroTolerance))
            {
                 dup = Curve.ProjectToPlane(dup, Plane.WorldXY);
            }
            else
            {
                var poly = dup.ToPolyline(Rhino.RhinoMath.ZeroTolerance, Rhino.RhinoMath.ZeroTolerance, 0.1, 10000);
                if (poly != null)
                {
                     var pl = poly.ToPolyline();
                     for(int k=0; k<pl.Count; k++) pl[k] = new Point3d(pl[k].X, pl[k].Y, 0);
                     dup = pl.ToPolylineCurve();
                }
                else
                {
                     dup = Curve.ProjectToPlane(dup, Plane.WorldXY);
                }
            }
            return (dup != null && dup.IsValid) ? dup : null;
        }

        private RoadGraph BuildGraph(List<(Curve curve, int level, double width, double radius)> processedData)
        {
            RoadGraph graph = new RoadGraph();
            double tolerance = 0.01; 

            // 2. Build Nodes and Edges
            for (int i = 0; i < processedData.Count; i++)
            {
                var (curve, level, width, radius) = processedData[i];
                
                Point3d start = curve.PointAtStart;
                Point3d end = curve.PointAtEnd;
                
                RoadNode startNode = GetOrCreateNode(graph, start, tolerance);
                RoadNode endNode = GetOrCreateNode(graph, end, tolerance);
                
                RoadEdge edge = new RoadEdge(curve, startNode, endNode)
                {
                    Type = new RoadType($"L{level}", width, radius, "Default", level)
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

        private List<(Mesh, int)> CreateJunctionMeshes(RoadNode node)
        {
            List<(Mesh, int)> result = new List<(Mesh, int)>();
            int edgeCount = node.ConnectedEdges.Count;

            // 1. Sort edges by angle
            var sortedEdges = SortEdgesByAngle(node);
            
            if (sortedEdges.Count == 0) return result;

            // 2. Identify hierarchy
            // Group edges by level
            var edgesByLevel = sortedEdges.GroupBy(e => e.Type.Level).OrderBy(g => g.Key).ToList();
            
            if (edgesByLevel.Count == 0) return result;

            int minLevel = edgesByLevel[0].Key; // Highest priority (smallest number)
            
            // If all edges are same level, use standard logic
            if (edgesByLevel.Count == 1)
            {
                Mesh m = CreateJunctionMeshStandard(node, sortedEdges);
                if (m != null && m.Faces.Count > 0)
                    result.Add((m, minLevel));
                return result;
            }
            
            // If mixed levels, check for Priority Intersection pattern (Main Road Through)
            // Pattern: 2 edges of minLevel forming a straight-ish line (Main Road)
            var mainEdges = edgesByLevel[0].ToList();
            
            if (mainEdges.Count == 2)
            {
                return CreatePriorityJunctionMeshes(node, sortedEdges, mainEdges);
            }
            
            // Fallback: treat as standard junction with mixed widths (which is already supported)
            Mesh mStandard = CreateJunctionMeshStandard(node, sortedEdges);
            if (mStandard != null && mStandard.Faces.Count > 0)
                result.Add((mStandard, minLevel));
                
            return result;
        }

        private List<(Mesh, int)> CreatePriorityJunctionMeshes(RoadNode node, List<RoadEdge> sortedEdges, List<RoadEdge> mainEdges)
        {
            List<(Mesh, int)> result = new List<(Mesh, int)>();
            
            // 1. Generate Main Road Mesh (Standard Junction for Main Pair)
            RoadEdge m1 = mainEdges[0];
            RoadEdge m2 = mainEdges[1];
            int mainLevel = m1.Type.Level;
            
            List<RoadEdge> mainPair = new List<RoadEdge> { m1, m2 };
            
            // Use Standard Junction for the main pair to ensure continuity
            Mesh mainMesh = CreateJunctionMeshStandard(node, mainPair);
            if (mainMesh != null)
            {
                result.Add((mainMesh, mainLevel));
            }
            
            // 2. Generate Side Road Meshes (Aprons)
            var sideEdges = sortedEdges.Except(mainEdges).ToList();
            
            foreach(var edge in sideEdges)
            {
                Mesh apron = CreateSideRoadApron(node, edge, m1, m2);
                if (apron != null)
                {
                    result.Add((apron, edge.Type.Level));
                }
            }
            
            return result;
        }

        private Mesh CreateSideRoadApron(RoadNode node, RoadEdge sideEdge, RoadEdge m1, RoadEdge m2)
        {
            // 1. Setup Side Road Geometry
            Vector3d sideTan = sideEdge.GetTangentAtNode(node);
            if (node == sideEdge.EndNode) sideTan = -sideTan; // Points AWAY from node center
            
            double sideW = sideEdge.Type.Width;
            double filletR = sideEdge.Type.FilletRadius;

            // 2. Setup Main Road Geometry (Flow Direction)
            Vector3d t1 = m1.GetTangentAtNode(node);
            if (node == m1.EndNode) t1 = -t1;
            Vector3d t2 = m2.GetTangentAtNode(node);
            if (node == m2.EndNode) t2 = -t2;

            // Main Road Direction (Average flow)
            Vector3d mainDir = t1 - t2; 
            if (mainDir.IsTiny(1e-6)) mainDir = t1; // Fallback
            mainDir.Unitize();
            
            Vector3d mainPerp = Vector3d.CrossProduct(mainDir, Vector3d.ZAxis);
            double mainW = Math.Max(m1.Type.Width, m2.Type.Width); // Use max width for safety

            // 3. Define Main Road Side Lines
            Line sideLineL = new Line(node.Position + mainPerp * mainW * 0.5, mainDir);
            Line sideLineR = new Line(node.Position - mainPerp * mainW * 0.5, mainDir);

            // 4. Determine which side to attach to
            // sideTan points AWAY from node. Incoming direction is -sideTan.
            double sideDot = (-sideTan) * mainPerp;
            Line targetLine = (sideDot > 0) ? sideLineL : sideLineR;

            // 5. Calculate Intersection and Setback
            Line sideCenterLine = new Line(node.Position, sideTan);
            Rhino.Geometry.Intersect.Intersection.LineLine(sideCenterLine, targetLine, out double tSide, out _, 0.01, false);
            Point3d pInt = sideCenterLine.PointAt(tSide);

            double distToInt = pInt.DistanceTo(node.Position);
            
            // Setback radius for Street Strip
            // Reserve space for fillet
            double setRadius = distToInt + filletR;
            
            // Clamp radius
            double edgeLen = sideEdge.Curve.GetLength();
            if (setRadius > edgeLen * 0.45) setRadius = edgeLen * 0.45;
            
            // Store radius for CreateStreetStrip
            SetEdgeRadius(sideEdge, node, setRadius);

            // 6. Generate Fillet Mesh (The Bell/Trumpet)
            
            // Connection Point on Side Edge (where strip ends)
            double tParam;
            if (node == sideEdge.StartNode) sideEdge.Curve.LengthParameter(setRadius, out tParam);
            else sideEdge.Curve.LengthParameter(edgeLen - setRadius, out tParam);
            
            Point3d pConn = sideEdge.Curve.PointAt(tParam);
            Vector3d pConnTan = sideEdge.Curve.TangentAt(tParam);
            if (node == sideEdge.EndNode) pConnTan = -pConnTan; // Outward tangent
            
            // Perpendicular at connection point
            Vector3d pPerp = Vector3d.CrossProduct(pConnTan, Vector3d.ZAxis);
            if (pPerp.IsTiny(1e-6)) pPerp = Vector3d.CrossProduct(pConnTan, Vector3d.XAxis);
            pPerp.Unitize();

            Point3d pLeft = pConn + pPerp * (sideW * 0.5);
            Point3d pRight = pConn - pPerp * (sideW * 0.5);

            // Generate Fillets
            // Left Fillet: pLeft -> TargetLine
            List<Point3d> leftPts = GenerateFilletPoints(pLeft, pConnTan, targetLine, 5);
            
            // Right Fillet: pRight -> TargetLine
            List<Point3d> rightPts = GenerateFilletPoints(pRight, pConnTan, targetLine, 5);

            // Build Mesh from Left and Right fillet curves
            Mesh filletMesh = new Mesh();
            int segs = Math.Min(leftPts.Count, rightPts.Count);
            
            for (int k = 0; k < segs - 1; k++)
            {
                Point3d v0 = leftPts[k];
                Point3d v1 = rightPts[k];
                Point3d v2 = rightPts[k+1];
                Point3d v3 = leftPts[k+1];
                
                filletMesh.Vertices.Add(v0);
                filletMesh.Vertices.Add(v1);
                filletMesh.Vertices.Add(v2);
                filletMesh.Vertices.Add(v3);
                
                filletMesh.Faces.AddFace(
                    filletMesh.Vertices.Count - 4,
                    filletMesh.Vertices.Count - 3,
                    filletMesh.Vertices.Count - 2,
                    filletMesh.Vertices.Count - 1
                );
            }
            
            return filletMesh;
        }

        private List<Point3d> GenerateFilletPoints(Point3d start, Vector3d startTan, Line targetLine, int count)
        {
            List<Point3d> pts = new List<Point3d> { start };
            
            // Intersect Line(start, startTan) with targetLine
            Line l1 = new Line(start, startTan);
            if (!Rhino.Geometry.Intersect.Intersection.LineLine(l1, targetLine, out double a, out double b, 0.01, false))
            {
                // Parallel? Just project?
                pts.Add(targetLine.ClosestPoint(start, false));
                return pts;
            }
            
            Point3d control = l1.PointAt(a);
            Point3d end = targetLine.PointAt(b);
            
            // Check direction consistency
            Vector3d toControl = control - start;
            if (toControl * startTan < 0) 
            {
                end = targetLine.ClosestPoint(start, false);
                control = (start + end) * 0.5;
            }

            // Generate points
            for(int k=1; k<=count; k++)
            {
                double t = (double)k / (count + 1);
                double u = 1 - t;
                Point3d pt = (u * u * start) + (2 * u * t * control) + (t * t * end);
                pts.Add(pt);
            }
            
            pts.Add(end);
            return pts;
        }

        private void SetEdgeRadius(RoadEdge edge, RoadNode node, double r)
        {
             if (edge.StartNode == node) _edgeStartRadii[edge] = r;
             else if (edge.EndNode == node) _edgeEndRadii[edge] = r;
        }

        // Renamed existing CreateJunctionMesh to CreateJunctionMeshStandard
        private Mesh CreateJunctionMeshStandard(RoadNode node, List<RoadEdge> sortedEdges = null)
        {
            Mesh mesh = new Mesh();
            if (sortedEdges == null) sortedEdges = SortEdgesByAngle(node);
            
            int edgeCount = sortedEdges.Count;
            List<Point3d> cornerPoints = new List<Point3d>();
            List<Point3d> connectionPoints = new List<Point3d>(); 
            List<Vector3d> edgeTangents = new List<Vector3d>();

            // 1. Handle Dead Ends (Degree 1)
            // Just let the road strip extend to the node center.
            if (edgeCount == 1) 
            {
                SetEdgeRadius(sortedEdges[0], node, 0.0);
                return null;
            }

            // Determine safe radius for each edge to avoid overlapping or flipping

            for (int i = 0; i < edgeCount; i++)
            {
                var edge = sortedEdges[i];
                double w = edge.Type.Width;
                double r = edge.Type.FilletRadius; 
                
                // Dynamic radius adjustment based on adjacent angles
                
                double maxDist = 20.0; // Hard limit
                double edgeLen = edge.Curve.GetLength();
                maxDist = Math.Min(maxDist, edgeLen * 0.45); // Don't consume more than 45% of edge
                
                if (r > maxDist) r = maxDist;
                
                // Store the calculated radius for CreateStreetStrip to use
                SetEdgeRadius(edge, node, r);

                // Use Curve Length Parameter to find exact point on curve, avoiding gaps
                double tParam;
                if (node == edge.StartNode) 
                    edge.Curve.LengthParameter(r, out tParam);
                else 
                    edge.Curve.LengthParameter(edge.Curve.GetLength() - r, out tParam);

                Point3d centerOnEdge = edge.Curve.PointAt(tParam);
                Vector3d tangent = edge.Curve.TangentAt(tParam);
                
                if (node == edge.EndNode) tangent = -tangent;

                connectionPoints.Add(centerOnEdge);
                edgeTangents.Add(tangent);

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
            if (edgeCount >= 2) // Corner, T-Junction, Cross, N-Way
            {
                GenerateFanTopology(mesh, node.Position, connectionPoints, cornerPoints, sortedEdges, edgeTangents);
            }
            
            return mesh;
        }




        private void GenerateFanTopology(Mesh mesh, Point3d center, List<Point3d> connectionPoints, List<Point3d> cornerPoints, List<RoadEdge> edges, List<Vector3d> tangents)
        {
            int edgeCount = edges.Count;
            mesh.Vertices.Add(center); // 0
            int centerIdx = 0;

            for (int i = 0; i < edgeCount; i++)
            {
                int next = (i + 1) % edgeCount;
                
                Point3d pConn = connectionPoints[i];
                Point3d pLeft = cornerPoints[i * 2]; 
                
                Point3d pNextRight = cornerPoints[next * 2 + 1];
                Point3d pNextConn = connectionPoints[next];
                
                Vector3d dir1 = tangents[i]; // Points OUT along Edge i
                Vector3d dir2 = tangents[next]; // Points OUT along Edge next
                
                List<Point3d> filletPoints = new List<Point3d>();
                
                Line l1 = new Line(pLeft, dir1);
                Line l2 = new Line(pNextRight, -dir2);
                
                bool intersectionFound = Rhino.Geometry.Intersect.Intersection.LineLine(l1, l2, out double a, out _, 0.01, false);
                Point3d controlPt;
                
                if (intersectionFound)
                {
                    controlPt = l1.PointAt(a);
                    if (controlPt.DistanceTo(center) > 100.0) // Heuristic
                    {
                         controlPt = (pLeft + pNextRight) * 0.5;
                    }
                }
                else
                {
                    controlPt = (pLeft + pNextRight) * 0.5;
                }

                int filletCount = 3; 
                for(int k=1; k<=filletCount; k++)
                {
                    double t = (double)k / (filletCount + 1);
                    double u = 1 - t;
                    Point3d pt = (u * u * pLeft) + (2 * u * t * controlPt) + (t * t * pNextRight);
                    filletPoints.Add(pt);
                }
                
                List<int> sectorIndices = new List<int>();
                mesh.Vertices.Add(pConn); sectorIndices.Add(mesh.Vertices.Count - 1);
                mesh.Vertices.Add(pLeft); sectorIndices.Add(mesh.Vertices.Count - 1);
                foreach(var fp in filletPoints) { mesh.Vertices.Add(fp); sectorIndices.Add(mesh.Vertices.Count - 1); }
                mesh.Vertices.Add(pNextRight); sectorIndices.Add(mesh.Vertices.Count - 1);
                mesh.Vertices.Add(pNextConn); sectorIndices.Add(mesh.Vertices.Count - 1);
                
                mesh.Faces.AddFace(centerIdx, sectorIndices[0], sectorIndices[1], sectorIndices[2]);
                mesh.Faces.AddFace(centerIdx, sectorIndices[2], sectorIndices[3], sectorIndices[4]);
                mesh.Faces.AddFace(centerIdx, sectorIndices[4], sectorIndices[5], sectorIndices[6]);
            }
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
            
            // Retrieve calculated radii
            double rStart = _edgeStartRadii.ContainsKey(edge) ? _edgeStartRadii[edge] : edge.Type.FilletRadius;
            double rEnd = _edgeEndRadii.ContainsKey(edge) ? _edgeEndRadii[edge] : edge.Type.FilletRadius;

            Curve c = edge.Curve;
            if (c == null) return new Mesh();
            
            double len = c.GetLength();
            double startDist = rStart;
            double endDist = len - rEnd;

            if (endDist <= startDist) return new Mesh();

            // Trim curve to valid length
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