using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace LandscapeToolkit.Optimization
{
    public class PathOptimizerComponent : GH_Component
    {
        public PathOptimizerComponent()
          : base("Bio-Path Optimizer", "BioPath",
              "Optimizes curves using bio-inspired relaxation (Laplacian smoothing) to create natural paths.",
              "Landscape", "Optimization")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Input curves to optimize", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Iterations", "I", "Number of relaxation iterations", GH_ParamAccess.item, 10);
            pManager.AddNumberParameter("Strength", "S", "Relaxation strength (0.0 - 1.0)", GH_ParamAccess.item, 0.5);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Optimized Curves", "OC", "Smoothed curves", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> curves = new List<Curve>();
            int iterations = 10;
            double strength = 0.5;

            if (!DA.GetDataList(0, curves)) return;
            DA.GetData(1, ref iterations);
            DA.GetData(2, ref strength);

            List<Curve> optimized = new List<Curve>();
            foreach (var c in curves)
            {
                if (c != null)
                {
                    optimized.Add(PathOptimizer.Optimize(c, iterations, strength));
                }
            }

            DA.SetDataList(0, optimized);
        }

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("1c2d3e4f-5a6b-7c8d-9e0f-1a2b3c4d5e6f");
    }
}
