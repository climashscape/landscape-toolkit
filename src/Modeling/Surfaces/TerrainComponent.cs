using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LandscapeToolkit.Modeling
{
    public class TerrainComponent : GH_Component
    {
        public TerrainComponent()
          : base("Landscape Terrain", "Terrain",
              "Creates a high-quality Quad Mesh terrain from input points or curves using QuadRemesh.",
              "Landscape", "Modeling")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Input", "G", "Input Points or Curves (Contours)", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Target Quad Count", "QC", "Target number of quads for the remeshed terrain", GH_ParamAccess.item, 2000);
            pManager.AddBooleanParameter("Smooth", "S", "Apply smoothing to the initial mesh before remeshing", GH_ParamAccess.item, true);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Quad Mesh", "QM", "Resulting Quad Mesh terrain", GH_ParamAccess.item);
            pManager.AddGeometryParameter("SubD", "SD", "Resulting SubD surface (Class-A quality)", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GeometryBase> inputGeo = new List<GeometryBase>();
            int targetCount = 2000;
            bool smooth = true;

            if (!DA.GetDataList(0, inputGeo)) return;
            DA.GetData(1, ref targetCount);
            DA.GetData(2, ref smooth);

            // 1. Collect points from input
            List<Point3d> points = new List<Point3d>();
            foreach (var geo in inputGeo)
            {
                if (geo is Point pt)
                {
                    points.Add(pt.Location);
                }
                else if (geo is Curve crv)
                {
                    // Divide curve to get points
                    double len = crv.GetLength();
                    if (len > 0)
                    {
                        // Adaptive division based on length, e.g., every 1 unit
                        // Or just simple division for now
                        Point3d[] pts;
                        crv.DivideByCount(Math.Max(10, (int)len), true, out pts);
                        if (pts != null) points.AddRange(pts);
                    }
                }
            }

            if (points.Count < 3)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Insufficient points to create terrain.");
                return;
            }

            // 2. Create Delaunay Mesh (Initial Triangulated Mesh)
            // Using Rhino.Geometry.Mesh.CreateFromTessellation is one way, 
            // or simpler: Node2List and Mesh.CreateFromDelaunay (if available in this context, usually needs Grasshopper.Kernel.Geometry.Delaunay)
            // Let's use Rhino's standard patch or simple Delaunay if accessible.
            // Actually, for simplicity and robustness in RhinoCommon, we can use Mesh.CreatePatch or just project to XY plane for Delaunay.
            
            // A robust way for terrain is to use the Grasshopper Delaunay solver or Rhino's.
            // Let's try to use a bounding box based Delaunay.
            
            var nodes = new Grasshopper.Kernel.Geometry.Node2List();
            foreach (var p in points)
            {
                nodes.Append(new Grasshopper.Kernel.Geometry.Node2(p.X, p.Y));
            }
            
            var faces = Grasshopper.Kernel.Geometry.Delaunay.Solver.Solve_Faces(nodes, 0.1);
            
            Mesh initialMesh = new Mesh();
            foreach (var p in points) initialMesh.Vertices.Add(p);
            
            foreach (var f in faces)
            {
                initialMesh.Faces.AddFace(f.A, f.B, f.C);
            }
            
            initialMesh.Normals.ComputeNormals();
            initialMesh.Compact();

            if (smooth)
            {
                // Simple smoothing (3 iterations)
                for (int i = 0; i < 3; i++)
                {
                    initialMesh.Smooth(0.5, true, true, true, true, SmoothingCoordinateSystem.Object, Plane.Unset);
                }
            }

            // 3. Quad Remesh
            // Requires Rhino 7+
            var parameters = new QuadRemeshParameters
            {
                TargetQuadCount = targetCount,
                AdaptiveSize = 50, // Allow some size variation
                DetectHardEdges = false // For terrain, usually we want soft edges
            };

            Mesh quadMesh = initialMesh.QuadRemesh(parameters);

            if (quadMesh != null)
            {
                DA.SetData(0, quadMesh);

                // 4. Convert to SubD for "Class-A" look
                // SubDCreaseMethod.Smooth is standard for organic terrain
                SubD subD = SubD.CreateFromMesh(quadMesh); 
                DA.SetData(1, subD);
            }
            else
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Quad Remesh failed.");
                DA.SetData(0, initialMesh); // Fallback
            }
        }

        protected override System.Drawing.Bitmap Icon => null; // TODO: Add icon

        public override Guid ComponentGuid => new Guid("2b3c4d5e-6f7a-8b9c-0d1e-2f3a4b5c6d7e");
    }
}
