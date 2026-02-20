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
              "Optimizes curves using bio-inspired relaxation.\n" +
              "Process Flow:\n" +
              "1. Resample: Convert input curves to high-resolution polylines.\n" +
              "2. Pin: Fix start and end points (anchors).\n" +
              "3. Relax: Apply iterative Laplacian smoothing (average neighbor positions) to internal vertices.\n" +
              "4. Reconstruct: Convert smoothed polylines back to NURBS curves.\n\n" +
              "处理流程：\n" +
              "1. 重采样：将输入曲线转换为高分辨率多段线。\n" +
              "2. 锚定：固定起点和终点。\n" +
              "3. 松弛：对内部顶点应用迭代拉普拉斯平滑（取邻居平均值）。\n" +
              "4. 重建：将平滑后的多段线转换回NURBS曲线。",
              "Landscape", "Optimization")
        {
        }

        protected override System.Drawing.Bitmap Icon => Icons.PathOptimizer;

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


        public override Guid ComponentGuid => new Guid("1c2d3e4f-5a6b-7c8d-9e0f-1a2b3c4d5e6f");
    }
}
