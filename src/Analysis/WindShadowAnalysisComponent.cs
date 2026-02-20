using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LandscapeToolkit.Analysis
{
    public class WindShadowAnalysisComponent : GH_Component
    {
        public WindShadowAnalysisComponent()
          : base("Wind Shadow Analysis", "WindShadow",
              "Simple wind shadow analysis based on raycasting (NOT a full CFD).",
              "Landscape", "Analysis")
        {
        }

        protected override System.Drawing.Bitmap Icon => Icons.WindShadowAnalysis;

        public override Guid ComponentGuid => new Guid("22334455-6677-8899-0011-aabbccddeeff");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Obstacles", "O", "Buildings, walls, or trees that block wind", GH_ParamAccess.list);
            pManager.AddVectorParameter("WindDirection", "D", "Wind direction vector", GH_ParamAccess.item, Vector3d.XAxis);
            pManager.AddPointParameter("TestPoints", "P", "Points to analyze", GH_ParamAccess.list);
            pManager.AddNumberParameter("WakeLength", "L", "Length of the wake behind obstacles (m)", GH_ParamAccess.item, 20.0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Exposure", "E", "Wind exposure factor (0=Blocked, 1=Exposed)", GH_ParamAccess.list);
            pManager.AddLineParameter("Rays", "R", "Visual rays for blocked points", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<GeometryBase> obstacles = new List<GeometryBase>();
            Vector3d windDir = Vector3d.XAxis;
            List<Point3d> testPoints = new List<Point3d>();
            double wakeLength = 20.0;

            if (!DA.GetDataList(0, obstacles)) obstacles = new List<GeometryBase>();
            DA.GetData(1, ref windDir);
            if (!DA.GetDataList(2, testPoints)) return;
            DA.GetData(3, ref wakeLength);

            if (windDir.IsZero)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Wind direction is zero.");
                return;
            }
            windDir.Unitize();
            Vector3d reverseWind = -windDir;

            // Prepare obstacles for raycasting
            // Convert everything to Meshes for faster intersection if possible, or use Brep/Mesh separately
            // Actually, Rhino.Geometry.Intersect.Intersection.RayShoot is good but requires a collection of geometry.
            // RayShoot takes IEnumerable<GeometryBase>.

            var exposure = new List<double>();
            var rays = new List<Line>();

            foreach (var pt in testPoints)
            {
                // Cast ray from point AGAINST wind direction to find upwind obstacles
                Ray3d ray = new Ray3d(pt, reverseWind);
                
                // Optimized RayShoot against all obstacles at once
                var hitPoints = Rhino.Geometry.Intersect.Intersection.RayShoot(ray, obstacles, 1);

                if (hitPoints != null && hitPoints.Length > 0)
                {
                    double dist = hitPoints[0].DistanceTo(pt);
                    
                    if (dist < wakeLength)
                    {
                        // Blocked
                        // Linear falloff: 0 at 0 dist, 1 at wakeLength.
                        double factor = dist / wakeLength;
                        exposure.Add(factor);
                        rays.Add(new Line(pt, hitPoints[0]));
                    }
                    else
                    {
                        // Exposed (obstacle too far)
                        exposure.Add(1.0);
                    }
                }
                else
                {
                    // Exposed (no obstacle hit)
                    exposure.Add(1.0);
                }
            }

            DA.SetDataList(0, exposure);
            DA.SetDataList(1, rays);
        }
    }
}
