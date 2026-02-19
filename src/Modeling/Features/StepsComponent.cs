using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using LandscapeToolkit.Modeling.Features.Steps; // Import the Logic namespace

namespace LandscapeToolkit.Modeling
{
    public class StepsComponent : GH_Component
    {
        public StepsComponent()
          : base("Landscape Steps", "Steps",
              "Generates parametric steps along a path using the StepGenerator logic.",
              "Landscape", "Modeling")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Path", "C", "Centerline path for the steps", GH_ParamAccess.item);
            pManager.AddNumberParameter("Width", "W", "Width of the steps", GH_ParamAccess.item, 2.0);
            pManager.AddNumberParameter("Tread", "T", "Tread length (Run)", GH_ParamAccess.item, 0.3);
            pManager.AddNumberParameter("Riser", "R", "Riser height (Rise)", GH_ParamAccess.item, 0.15);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Steps", "S", "Generated step geometry", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve path = null;
            double width = 2.0;
            double tread = 0.3;
            double riser = 0.15;

            if (!DA.GetData(0, ref path)) return;
            if (!DA.GetData(1, ref width)) return;
            if (!DA.GetData(2, ref tread)) return;
            if (!DA.GetData(3, ref riser)) return;

            // Use the Logic Class
            // 调用逻辑类，实现 UI 与 逻辑分离
            var generator = new StepGenerator(path)
            {
                Width = width,
                Tread = tread,
                Riser = riser
            };

            try 
            {
                List<Brep> steps = generator.Generate();
                DA.SetDataList(0, steps);
            }
            catch (Exception ex)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Generation failed: " + ex.Message);
            }
        }

        protected override System.Drawing.Bitmap Icon => null; // TODO: Add Icon

        public override Guid ComponentGuid => new Guid("87654321-4321-4321-4321-210987654321"); // Ensure unique GUID
    }
}
