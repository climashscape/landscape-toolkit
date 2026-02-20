using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using LandscapeToolkit;

namespace LandscapeToolkit.Modeling.Roads
{
    public class RoadNetworkComponent : GH_Component
    {
        public RoadNetworkComponent()
          : base("Quad Road Network", "QuadRoad",
              "Generates a high-quality Quad Mesh road network from centerlines.\n" +
              "Process Flow:\n" +
              "1. Pre-process: Flatten curves to XY plane and shatter at intersections.\n" +
              "2. Build Graph: Identify nodes (junctions) and edges (streets).\n" +
              "3. Generate Junctions: Create 3-way, 4-way, or N-way intersection meshes with dynamic fillets.\n" +
              "4. Generate Streets: Create quad strip meshes connecting junctions.\n" +
              "5. Combine & Weld: Merge all meshes and weld vertices for smooth continuity.\n" +
              "6. Relax: Apply optional Laplacian smoothing for better flow.\n\n" +
              "处理流程：\n" +
              "1. 预处理：将曲线压平至XY平面并在交点处打断。\n" +
              "2. 构建图：识别节点（路口）和边（街道）。\n" +
              "3. 生成路口：生成带动态倒角的三岔、四岔或多岔路口网格。\n" +
              "4. 生成街道：创建连接路口的四边面带状网格。\n" +
              "5. 合并与焊接：合并所有网格并焊接顶点以保证连续性。\n" +
              "6. 松弛：应用可选的拉普拉斯平滑以优化网格流动。",
              "Landscape", "Modeling")
        {
        }

        protected override System.Drawing.Bitmap Icon => Icons.RoadNetwork;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Centerlines", "C", "Road centerlines (Curves/Polylines)", GH_ParamAccess.list);
            pManager.AddNumberParameter("Widths", "W", "Road widths (List matched to curves, or single value)", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("Fillet", "F", "Intersection fillet radius", GH_ParamAccess.item, 3.0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("QuadMesh", "M", "Resulting clean quad mesh topology", GH_ParamAccess.item);
            pManager.AddGenericParameter("Graph", "G", "Road network graph structure (for debugging)", GH_ParamAccess.item);
            pManager.AddMeshParameter("Junctions", "J", "Junction meshes", GH_ParamAccess.list);
            pManager.AddMeshParameter("Streets", "S", "Street segment meshes", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> curves = new List<Curve>();
            List<double> widths = new List<double>();
            double fillet = 3.0;

            if (!DA.GetDataList(0, curves)) return;
            DA.GetDataList(1, widths); // Optional input
            if (!DA.GetData(2, ref fillet)) return;

            // Pre-validation
            curves.RemoveAll(c => c == null || c.IsShort(RhinoMath.ZeroTolerance));
            if (curves.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No valid curves provided.");
                return;
            }

            // Initialize the Generator
            QuadRoadGenerator generator = new QuadRoadGenerator(curves, widths)
            {
                DefaultIntersectionRadius = fillet // Use fillet as radius base
            };

            try 
            {
                // Execute generation
                Mesh resultMesh = generator.Generate();
                
                if (resultMesh != null && resultMesh.IsValid)
                {
                    DA.SetData(0, resultMesh);
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Generated mesh was invalid or empty.");
                }

                // Output graph if available
                if (generator.GeneratedGraph != null)
                {
                    DA.SetData(1, generator.GeneratedGraph);
                }

                if (generator.JunctionMeshes != null)
                {
                    DA.SetDataList(2, generator.JunctionMeshes);
                }

                if (generator.StreetMeshes != null)
                {
                    DA.SetDataList(3, generator.StreetMeshes);
                }
            }
            catch (Exception ex)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Generation failed: " + ex.Message);
            }
        }



        public override Guid ComponentGuid => new Guid("12345678-1234-1234-1234-1234567890ab"); // Ensure unique GUID
    }
}
