using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace LandscapeToolkit.Modeling.Roads
{
    public class RoadNetworkComponent : GH_Component
    {
        public RoadNetworkComponent()
          : base("Quad Road Network", "QuadRoad",
              "Generates a high-quality Quad Mesh road network from centerlines, suitable for SubD workflows.",
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
