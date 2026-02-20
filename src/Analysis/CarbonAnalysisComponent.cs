using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using LandscapeToolkit;

namespace LandscapeToolkit.Analysis
{
    public class CarbonAnalysisComponent : GH_Component
    {
        public CarbonAnalysisComponent()
          : base("Carbon Analysis", "Carbon",
              "Estimates carbon sequestration based on landscape elements (Trees, Shrubs, Lawn).",
              "Landscape", "Analysis")
        {
        }

        protected override System.Drawing.Bitmap Icon => Icons.CarbonAnalysis;

        public override Guid ComponentGuid => new Guid("11223344-5566-7788-9900-aabbccddeeff");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("TreePoints", "T", "Points representing trees", GH_ParamAccess.list);
            pManager[0].Optional = true;
            pManager.AddGeometryParameter("GreenAreas", "A", "Surfaces/Meshes representing green areas (shrubs/lawn)", GH_ParamAccess.list);
            pManager[1].Optional = true;
            pManager.AddNumberParameter("TreeFactor", "TF", "Carbon sequestration per tree (kg/year)", GH_ParamAccess.item, 22.0); // Approx for mature tree
            pManager.AddNumberParameter("AreaFactor", "AF", "Carbon sequestration per m2 (kg/m2/year)", GH_ParamAccess.item, 1.5); // Approx for mixed vegetation
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("TotalCarbon", "C", "Total Carbon Sequestration (kg/year)", GH_ParamAccess.item);
            pManager.AddTextParameter("Report", "R", "Detailed breakdown report", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> trees = new List<Point3d>();
            List<GeometryBase> areas = new List<GeometryBase>();
            double treeFactor = 22.0;
            double areaFactor = 1.5;

            if (!DA.GetDataList(0, trees)) trees = new List<Point3d>();
            if (!DA.GetDataList(1, areas)) areas = new List<GeometryBase>();
            DA.GetData(2, ref treeFactor);
            DA.GetData(3, ref areaFactor);

            // Calculate Tree Carbon
            double treeCarbon = trees.Count * treeFactor;

            // Calculate Area Carbon
            double totalArea = 0;
            foreach (var geom in areas)
            {
                if (geom is Brep b) totalArea += b.GetArea();
                else if (geom is Mesh m)
                {
                    using (var mp = AreaMassProperties.Compute(m))
                    {
                        if (mp != null) totalArea += mp.Area;
                    }
                }
                else if (geom is Curve c && c.IsClosed)
                {
                     using (var amp = AreaMassProperties.Compute(c))
                     {
                         if (amp != null) totalArea += amp.Area;
                     }
                }
            }

            double areaCarbon = totalArea * areaFactor;
            double totalCarbon = treeCarbon + areaCarbon;

            DA.SetData(0, totalCarbon);

            string report = $"Carbon Sequestration Report:\n" +
                            $"--------------------------\n" +
                            $"Trees: {trees.Count} units * {treeFactor} kg = {treeCarbon:F2} kg/yr\n" +
                            $"Green Area: {totalArea:F2} m2 * {areaFactor} kg = {areaCarbon:F2} kg/yr\n" +
                            $"--------------------------\n" +
                            $"TOTAL: {totalCarbon:F2} kg/year";
            
            DA.SetData(1, report);
        }
    }
}
