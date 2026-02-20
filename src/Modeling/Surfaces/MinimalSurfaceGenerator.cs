using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace LandscapeToolkit.Modeling.Surfaces
{
    /// <summary>
    /// Generates minimal surfaces or smooth terrain patches from boundaries.
    /// 生成最小曲面或光顺地形
    /// </summary>
    public class MinimalSurfaceGenerator
    {
        public Polyline Boundary { get; set; }
        public List<Point3d> InternalAttractors { get; set; } // Points to pull terrain up/down
        public double MeshResolution { get; set; } = 1.0;

        public int Iterations { get; set; } = 50;

        /// <summary>
        /// Generate a relaxed mesh surface.
        /// 生成松弛后的网格曲面
        /// </summary>
        public Mesh Generate()
        {
            // Step 1: Create Initial Mesh
            // 步骤1：基于边界生成初始三角或四边面网格
            Mesh mesh = CreateInitialMesh(Boundary);

            // Step 2: Set Constraints
            // 步骤2：设置约束条件（固定边界顶点）
            var fixedVertices = IdentifyBoundaryVertices(mesh);

            // Step 3: Iterative Relaxation (Physics Simulation)
            // 步骤3：迭代松弛（物理模拟）
            for (int i = 0; i < Iterations; i++)
            {
                mesh = RelaxStep(mesh, fixedVertices, InternalAttractors);
            }

            // Step 4: Subdivide (Optional for higher quality)
            // 步骤4：细分（可选）
            mesh.Vertices.CombineIdentical(true, true);
            // mesh = CatmullClarkSubdivide(mesh);

            return mesh;
        }

        private Mesh CreateInitialMesh(Polyline boundary)
        {
            // Handle 3D boundary by projecting to XY first
            Polyline projected = new Polyline(boundary);
            for(int i=0; i<projected.Count; i++)
            {
                projected[i] = new Point3d(projected[i].X, projected[i].Y, 0);
            }

            Curve c = projected.ToPolylineCurve();
            Mesh mesh = Mesh.CreateFromPlanarBoundary(c, MeshingParameters.Default, 0.01);
            
            if (mesh == null) return new Mesh();

            // Lift boundary vertices to original 3D boundary
            // We need to match mesh boundary vertices to the input polyline
            // This is tricky if vertex order changes.
            // A robust way: For each naked edge vertex, find closest point on original boundary
            
            Curve originalCrv = boundary.ToPolylineCurve();
            bool[] naked = mesh.GetNakedEdgePointStatus();
            
            for(int i=0; i<mesh.Vertices.Count; i++)
            {
                if (naked[i])
                {
                    Point3d v = mesh.Vertices[i]; // This is on XY plane (mostly)
                    // Find closest point on original 3D curve (ignoring Z for search if possible, but v has Z=0)
                    // Actually, CreateFromPlanarBoundary creates mesh on the plane of the curve.
                    // Since we projected to XY, mesh is on XY.
                    
                    if (originalCrv.ClosestPoint(v, out double t))
                    {
                        Point3d newPos = originalCrv.PointAt(t);
                        mesh.Vertices[i] = (Point3f)newPos;
                    }
                }
                else
                {
                    // Internal vertices: Set Z to average of boundary Z to start
                    // Or just leave at 0 and let relaxation handle it?
                    // Better to start at average Z.
                    double avgZ = boundary.BoundingBox.Center.Z;
                    Point3d v = mesh.Vertices[i];
                    mesh.Vertices[i] = new Point3f((float)v.X, (float)v.Y, (float)avgZ);
                }
            }

            return mesh;
        }

        private bool[] IdentifyBoundaryVertices(Mesh mesh)
        {
            return mesh.GetNakedEdgePointStatus();
        }

        private Mesh RelaxStep(Mesh mesh, bool[] fixedMask, List<Point3d> attractors)
        {
            Mesh newMesh = mesh.DuplicateMesh();
            // Loop through all vertices
            for (int i = 0; i < mesh.Vertices.Count; i++)
            {
                if (fixedMask[i]) continue; // Skip boundary

                // Calculate average position of neighbors
                Point3d avgPos = GetAverageNeighborPos(mesh, i);
                Point3d currentPos = mesh.Vertices[i];

                // Move towards average (Laplacian Smoothing)
                Point3d newPos = (currentPos + avgPos) * 0.5;

                // Apply attractor force (e.g. pull up/down)
                if (attractors != null) {
                    foreach(var att in attractors) {
                        double dist = newPos.DistanceTo(att);
                        if (dist < 10.0) {
                            // Simple gaussian influence
                            // Pull towards attractor Z? Or just pull towards attractor point?
                            // Minimal surface usually means minimizing area.
                            // Attractors act as constraints.
                            // Let's pull towards attractor with a falloff
                            Vector3d pull = att - newPos;
                            double strength = 0.1 * Math.Exp(-dist * dist / 20.0);
                            newPos += pull * strength;
                        }
                    }
                }

                newMesh.Vertices[i] = (Point3f)newPos;
            }
            return newMesh;
        }

        private Point3d GetAverageNeighborPos(Mesh mesh, int vertexIndex)
        {
            // Use TopologyVertices to find connected vertices
            int topoIndex = mesh.TopologyVertices.TopologyVertexIndex(vertexIndex);
            int[] connectedTopoIndices = mesh.TopologyVertices.ConnectedTopologyVertices(topoIndex);
            
            Point3d sum = Point3d.Origin;
            foreach (int idx in connectedTopoIndices)
            {
                sum += mesh.TopologyVertices[idx];
            }
            
            if (connectedTopoIndices.Length > 0)
                return sum / connectedTopoIndices.Length;
                
            return mesh.Vertices[vertexIndex];
        }
    }
}
