using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace LandscapeToolkit.Analysis
{
    public class HydrologyComponent : GH_Component
    {
        public HydrologyComponent()
          : base("Runoff Simulation", "Hydro",
              "Simulates water flow directions and accumulation on a mesh (Steepest Descent).",
              "Landscape", "Analysis")
        {
        }

        protected override System.Drawing.Bitmap Icon => Icons.Hydrology;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Input terrain mesh", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Raindrops", "N", "Number of raindrops to simulate", GH_ParamAccess.item, 500);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("FlowLines", "F", "Flow paths", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            int count = 500;

            if (!DA.GetData(0, ref mesh)) return;
            DA.GetData(1, ref count);

            if (mesh == null || !mesh.IsValid) return;

            // Ensure normals for gradient calculation
            mesh.Normals.ComputeNormals();

            List<Curve> flowLines = new List<Curve>();
            Random rnd = new Random();
            BoundingBox bbox = mesh.GetBoundingBox(true);

            for (int i = 0; i < count; i++)
            {
                // Random start point on XY plane projected to mesh
                double x = bbox.Min.X + rnd.NextDouble() * (bbox.Max.X - bbox.Min.X);
                double y = bbox.Min.Y + rnd.NextDouble() * (bbox.Max.Y - bbox.Min.Y);
                Point3d start = new Point3d(x, y, bbox.Max.Z + 1.0);
                
                // Project
                Point3d[] projected = Rhino.Geometry.Intersect.Intersection.ProjectPointsToMeshes(new Mesh[] { mesh }, new Point3d[] { start }, new Vector3d(0, 0, -1), 0.01);
                
                if (projected != null && projected.Length > 0)
                {
                    List<Point3d> path = TraceDrop(mesh, projected[0]);
                    if (path.Count > 1)
                    {
                        flowLines.Add(new PolylineCurve(path));
                    }
                }
            }

            DA.SetDataList(0, flowLines);
        }

        private List<Point3d> TraceDrop(Mesh mesh, Point3d start)
        {
            List<Point3d> path = new List<Point3d>();
            path.Add(start);
            Point3d current = start;
            
            for (int step = 0; step < 100; step++) // Max steps
            {
                // Find steepest descent
                // Get normal at current point
                MeshPoint mp = mesh.ClosestMeshPoint(current, 1.0);
                if (mp == null) break;
                
                Vector3d normal = mesh.NormalAt(mp);
                // Slope vector = Project gravity (0,0,-1) onto plane defined by normal
                // V_slope = g - (g dot n) * n
                Vector3d g = new Vector3d(0, 0, -1);
                Vector3d slope = g - (g * normal) * normal;
                
                if (slope.Length < 0.01) break; // Flat area or local minima
                
                slope.Unitize();
                current += slope * 1.0; // Step size
                
                // Clamp to mesh
                Point3d[] proj = Rhino.Geometry.Intersect.Intersection.ProjectPointsToMeshes(new Mesh[] { mesh }, new Point3d[] { current + new Vector3d(0,0,5) }, new Vector3d(0,0,-1), 0.1);
                if (proj != null && proj.Length > 0)
                {
                    current = proj[0];
                    path.Add(current);
                }
                else
                {
                    break; // Fell off mesh
                }
            }
            
            return path;
        }


        public override Guid ComponentGuid => new Guid("4d5e6f7a-8b9c-0d1e-2f3a-4b5c6d7e8f9a");
    }
}
