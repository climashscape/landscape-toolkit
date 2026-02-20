using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using LandscapeToolkit.Modeling.Features.Boardwalks;

namespace LandscapeToolkit.Modeling.Features
{
    public class BoardwalkComponent : GH_Component
    {
        public BoardwalkComponent()
          : base("Boardwalk", "Boardwalk",
              "Creates a raised boardwalk with supports and railings from a path curve.",
              "Landscape", "Modeling")
        {
        }

        protected override System.Drawing.Bitmap Icon => Icons.Boardwalk;

        public override Guid ComponentGuid => new Guid("4b5c6d7e-8f9a-0b1c-2d3e-4f5a6b7c8d9e");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Path", "P", "Centerline of the boardwalk", GH_ParamAccess.item);
            pManager.AddMeshParameter("Terrain", "T", "Terrain mesh for supports", GH_ParamAccess.item);
            pManager.AddNumberParameter("Width", "W", "Width of the boardwalk", GH_ParamAccess.item, 2.0);
            pManager.AddNumberParameter("SupportSpacing", "S", "Distance between supports", GH_ParamAccess.item, 3.0);
            
            pManager[1].Optional = true; // Terrain optional (if null, no supports)
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Deck", "D", "Boardwalk deck surface", GH_ParamAccess.item);
            pManager.AddBrepParameter("Supports", "S", "Support columns", GH_ParamAccess.list);
            pManager.AddBrepParameter("Railings", "R", "Railing posts and handrails", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve path = null;
            Mesh terrain = null;
            double width = 2.0;
            double spacing = 3.0;

            if (!DA.GetData(0, ref path)) return;
            DA.GetData(1, ref terrain);
            DA.GetData(2, ref width);
            DA.GetData(3, ref spacing);

            var generator = new BoardwalkGenerator(path, terrain)
            {
                Width = width,
                SupportSpacing = spacing,
                HasRailings = true
            };

            generator.Generate(out Brep deck, out List<Brep> supports, out List<Brep> railings);

            DA.SetData(0, deck);
            DA.SetDataList(1, supports);
            DA.SetDataList(2, railings);
        }
    }
}
