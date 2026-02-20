using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using LandscapeToolkit;

namespace LandscapeToolkit.Modeling.Roads
{
    public class MultiLevelRoadComponent : GH_Component
    {
        public MultiLevelRoadComponent()
          : base("Quad Road Network (Multi-Level)", "MultiRoad",
              "Generates a hierarchical Quad Mesh road network supporting up to 3 levels.\n" +
              "Features:\n" +
              "- Priority Junctions: Higher level roads (e.g., L1) remain continuous.\n" +
              "- Disconnected Levels: Meshes for different levels are separate (unwelded) for independent styling.\n" +
              "- Bell-mouth Aprons: Lower level roads connect to higher levels with smooth fillets.\n\n" +
              "多级路网生成器（支持3级）：\n" +
              "- 优先路口：高等级道路（如L1）保持连续。\n" +
              "- 分层输出：不同等级的Mesh相互独立（不焊接），便于分层赋予材质。\n" +
              "- 喇叭口倒角：低等级道路通过平滑倒角连接到高等级道路。",
              "Landscape", "Modeling")
        {
        }

        protected override System.Drawing.Bitmap Icon => Icons.RoadNetwork;

        public override Guid ComponentGuid => new Guid("D7295638-4D21-4E8B-9123-56789ABCDEF0");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            // Level 1 Inputs
            pManager.AddCurveParameter("L1 Curves", "L1_C", "Level 1 (Main) Road Centerlines", GH_ParamAccess.list);
            pManager.AddNumberParameter("L1 Width", "L1_W", "Width for Level 1 Roads", GH_ParamAccess.item, 12.0);
            pManager.AddNumberParameter("L1 Radius", "L1_R", "Fillet Radius for Level 1 Intersections", GH_ParamAccess.item, 15.0);

            // Level 2 Inputs (Optional)
            pManager.AddCurveParameter("L2 Curves", "L2_C", "Level 2 (Secondary) Road Centerlines", GH_ParamAccess.list);
            pManager[3].Optional = true;
            pManager.AddNumberParameter("L2 Width", "L2_W", "Width for Level 2 Roads", GH_ParamAccess.item, 6.0);
            pManager.AddNumberParameter("L2 Radius", "L2_R", "Fillet Radius for Level 2 Intersections", GH_ParamAccess.item, 9.0);

            // Level 3 Inputs (Optional)
            pManager.AddCurveParameter("L3 Curves", "L3_C", "Level 3 (Path) Road Centerlines", GH_ParamAccess.list);
            pManager[6].Optional = true;
            pManager.AddNumberParameter("L3 Width", "L3_W", "Width for Level 3 Roads", GH_ParamAccess.item, 3.0);
            pManager.AddNumberParameter("L3 Radius", "L3_R", "Fillet Radius for Level 3 Intersections", GH_ParamAccess.item, 5.0);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("All Meshes", "All", "Combined mesh of all levels", GH_ParamAccess.item);
            pManager.AddMeshParameter("L1 Mesh", "L1", "Mesh for Level 1 Roads", GH_ParamAccess.item);
            pManager.AddMeshParameter("L2 Mesh", "L2", "Mesh for Level 2 Roads", GH_ParamAccess.item);
            pManager.AddMeshParameter("L3 Mesh", "L3", "Mesh for Level 3 Roads", GH_ParamAccess.item);
            pManager.AddGenericParameter("Graph", "G", "Road Graph Structure", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Gather Inputs
            List<Curve> l1Curves = new List<Curve>();
            double l1Width = 12.0;
            double l1Radius = 15.0;

            if (!DA.GetDataList(0, l1Curves)) return;
            DA.GetData(1, ref l1Width);
            DA.GetData(2, ref l1Radius);

            List<Curve> l2Curves = new List<Curve>();
            double l2Width = 6.0;
            double l2Radius = 9.0;
            DA.GetDataList(3, l2Curves);
            DA.GetData(4, ref l2Width);
            DA.GetData(5, ref l2Radius);

            List<Curve> l3Curves = new List<Curve>();
            double l3Width = 3.0;
            double l3Radius = 5.0;
            DA.GetDataList(6, l3Curves);
            DA.GetData(7, ref l3Width);
            DA.GetData(8, ref l3Radius);

            // Validate
            l1Curves.RemoveAll(c => c == null || c.IsShort(RhinoMath.ZeroTolerance));
            l2Curves.RemoveAll(c => c == null || c.IsShort(RhinoMath.ZeroTolerance));
            l3Curves.RemoveAll(c => c == null || c.IsShort(RhinoMath.ZeroTolerance));

            if (l1Curves.Count == 0 && l2Curves.Count == 0 && l3Curves.Count == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No valid curves provided.");
                return;
            }

            // Initialize Generator
            QuadRoadGenerator generator = new QuadRoadGenerator();
            
            // Add Roads
            if (l1Curves.Count > 0) generator.AddRoads(l1Curves, 1, l1Width, l1Radius);
            if (l2Curves.Count > 0) generator.AddRoads(l2Curves, 2, l2Width, l2Radius);
            if (l3Curves.Count > 0) generator.AddRoads(l3Curves, 3, l3Width, l3Radius);

            try
            {
                // Generate
                Mesh resultMesh = generator.Generate();

                // Set Outputs
                if (resultMesh != null && resultMesh.IsValid)
                {
                    DA.SetData(0, resultMesh);
                }

                if (generator.MeshesByLevel.ContainsKey(1))
                {
                    Mesh m1 = new Mesh();
                    m1.Append(generator.MeshesByLevel[1]);
                    m1.Vertices.CombineIdentical(true, true);
                    m1.Weld(3.14159);
                    m1.UnifyNormals();
                    m1.Compact();
                    DA.SetData(1, m1);
                }

                if (generator.MeshesByLevel.ContainsKey(2))
                {
                    Mesh m2 = new Mesh();
                    m2.Append(generator.MeshesByLevel[2]);
                    m2.Vertices.CombineIdentical(true, true);
                    m2.Weld(3.14159);
                    m2.UnifyNormals();
                    m2.Compact();
                    DA.SetData(2, m2);
                }

                if (generator.MeshesByLevel.ContainsKey(3))
                {
                    Mesh m3 = new Mesh();
                    m3.Append(generator.MeshesByLevel[3]);
                    m3.Vertices.CombineIdentical(true, true);
                    m3.Weld(3.14159);
                    m3.UnifyNormals();
                    m3.Compact();
                    DA.SetData(3, m3);
                }

                if (generator.GeneratedGraph != null)
                {
                    DA.SetData(4, generator.GeneratedGraph);
                }
            }
            catch (Exception ex)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Generation failed: " + ex.Message);
            }
        }
    }
}