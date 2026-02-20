using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using LandscapeToolkit.Modeling.Features.Scatter;

namespace LandscapeToolkit.Modeling
{
    public class ScatterComponent : GH_Component
    {
        public ScatterComponent()
          : base("Scatter Elements", "Scatter",
              "Distributes landscape elements (Trees, Lights, Benches) based on rules.",
              "Landscape", "Modeling")
        {
        }

        protected override System.Drawing.Bitmap Icon => Icons.Scatter;

        public override Guid ComponentGuid => new Guid("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("TargetSurface", "S", "Target terrain or plot mesh", GH_ParamAccess.item);
            pManager.AddCurveParameter("RoadEdges", "E", "Road edges for lights/benches (Optional)", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Type", "T", "0=Tree, 1=Light, 2=Bench", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("MinDistance", "D", "Minimum distance (Radius)", GH_ParamAccess.item, 5.0);
            
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "P", "Generated points", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            List<Curve> edges = new List<Curve>();
            int typeVal = 0;
            double dist = 5.0;

            if (!DA.GetData(0, ref mesh)) return;
            DA.GetDataList(1, edges);
            DA.GetData(2, ref typeVal);
            DA.GetData(3, ref dist);

            var system = new ScatterSystem
            {
                TargetSurface = mesh,
                RoadEdges = edges,
                MinDistance = dist,
                Type = (ScatterSystem.ScatterType)typeVal
            };

            List<Point3d> points = system.GeneratePoints();

            DA.SetDataList(0, points);
        }
    }
}
