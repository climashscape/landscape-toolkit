using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace LandscapeToolkit.Modeling.Surfaces
{
    public class MinimalSurfaceComponent : GH_Component
    {
        public MinimalSurfaceComponent()
          : base("Minimal Surface", "MinSurf",
              "Generates minimal surfaces from boundary curves using relaxation.",
              "Landscape", "Modeling")
        {
        }

        protected override System.Drawing.Bitmap Icon => Icons.MinimalSurface;

        public override Guid ComponentGuid => new Guid("2b3c4d5e-6f7a-8b9c-0d1e-2f3a4b5c6d7e");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Boundary", "B", "Closed boundary curve", GH_ParamAccess.item);
            pManager.AddPointParameter("Attractors", "A", "Internal attractor points", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddIntegerParameter("Iterations", "I", "Relaxation iterations", GH_ParamAccess.item, 50);
            pManager.AddNumberParameter("Resolution", "R", "Mesh resolution", GH_ParamAccess.item, 1.0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Generated minimal surface mesh", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve boundary = null;
            List<Point3d> attractors = new List<Point3d>();
            int iterations = 50;
            double resolution = 1.0;

            if (!DA.GetData(0, ref boundary)) return;
            DA.GetDataList(1, attractors);
            DA.GetData(2, ref iterations);
            DA.GetData(3, ref resolution);

            if (!boundary.IsClosed)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Boundary curve must be closed.");
                return;
            }

            if (!boundary.TryGetPolyline(out Polyline polyline))
            {
                // Try to convert to polyline with default tolerance
                var polyCurve = boundary.ToPolyline(0, 0, 0.1, 0, 0, 0, 0, 0, true);
                if (polyCurve == null || !polyCurve.TryGetPolyline(out polyline))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Could not convert boundary to polyline.");
                    return;
                }
            }

            var generator = new MinimalSurfaceGenerator
            {
                Boundary = polyline,
                InternalAttractors = attractors,
                MeshResolution = resolution,
                Iterations = iterations
            };

            Mesh result = generator.Generate();
            DA.SetData(0, result);
        }
    }
}
