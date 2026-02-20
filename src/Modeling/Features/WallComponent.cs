using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using LandscapeToolkit.Modeling.Features.Walls;

namespace LandscapeToolkit.Modeling
{
    public class WallComponent : GH_Component
    {
        public WallComponent()
          : base("Landscape Wall", "Wall",
              "Creates a vertical wall adapting to terrain.",
              "Landscape", "Modeling")
        {
        }

        protected override System.Drawing.Bitmap Icon => Icons.Wall;

        public override Guid ComponentGuid => new Guid("3c4d5e6f-7a8b-9c0d-1e2f-3a4b5c6d7e80");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Base curve for the wall", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "Height of the wall", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Thickness", "T", "Thickness of the wall", GH_ParamAccess.item, 0.2);
            pManager.AddMeshParameter("Terrain", "M", "Optional terrain mesh", GH_ParamAccess.item);
            pManager[3].Optional = true;
            pManager.AddBooleanParameter("LevelTop", "L", "Keep top level (flat)", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Wall", "W", "Resulting wall geometry", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            double height = 1.0;
            double thickness = 0.2;
            Mesh terrain = null;
            bool levelTop = false;

            if (!DA.GetData(0, ref curve)) return;
            if (!DA.GetData(1, ref height)) return;
            if (!DA.GetData(2, ref thickness)) return;
            DA.GetData(3, ref terrain);
            DA.GetData(4, ref levelTop);

            var generator = new WallGenerator(curve)
            {
                Height = height,
                Thickness = thickness,
                Terrain = terrain,
                LevelTop = levelTop
            };

            List<Brep> walls = generator.Generate();
            DA.SetDataList(0, walls);
        }
    }
}
