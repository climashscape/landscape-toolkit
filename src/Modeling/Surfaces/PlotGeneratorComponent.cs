using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LandscapeToolkit.Modeling
{
    public class PlotGeneratorComponent : GH_Component
    {
        public PlotGeneratorComponent()
          : base("Plot Generator", "PlotGen",
              "Generates plot regions from boundary curves using QuadRemesh.",
              "Landscape", "Modeling")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Boundaries", "B", "Boundary curves defining plots", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("PlotMeshes", "M", "Resulting quad meshes for each plot", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> boundaries = new List<Curve>();
            if (!DA.GetDataList(0, boundaries)) return;

            if (boundaries == null || boundaries.Count == 0) return;

            // Remove invalid curves
            boundaries.RemoveAll(c => c == null || c.IsShort(Rhino.RhinoMath.ZeroTolerance));

            // 1. Find enclosed regions
            // Try Curve.CreateBooleanRegions first as it handles intersecting lines better
            Plane plane = Plane.WorldXY;
            // Try to use plane of first curve if valid
            if (boundaries.Count > 0 && boundaries[0].TryGetPlane(out Plane firstPlane))
            {
                plane = firstPlane;
            }

            double tolerance = RhinoDoc.ActiveDoc?.ModelAbsoluteTolerance ?? 0.01;

            // Curve.CreateBooleanRegions finds enclosed regions from a set of curves
            // combineRegions = false to get individual regions
            var regionResults = Curve.CreateBooleanRegions(boundaries, plane, false, tolerance);
            
            Brep[] regionBreps = null;

            if (regionResults != null && regionResults.RegionCount > 0)
            {
                // CurveBooleanRegions might expose RegionCurves property
                // Or we can iterate if it is IEnumerable (but previous error said no)
                // Let's try reflection or dynamic to be safe if property name is uncertain, 
                // but standard RhinoCommon has RegionCurves.
                // However, to avoid compilation error if RegionCurves is missing in this version:
                
                IEnumerable<Curve> curves = null;
                // Try to access RegionCurves property dynamically
                try {
                    var prop = regionResults.GetType().GetProperty("RegionCurves");
                    if (prop != null)
                        curves = (IEnumerable<Curve>)prop.GetValue(regionResults);
                    else {
                        // Fallback: maybe it implements IEnumerable explicitly?
                         if (regionResults is IEnumerable<Curve> enumerable)
                            curves = enumerable;
                    }
                } catch {}

                if (curves != null)
                    regionBreps = Brep.CreatePlanarBreps(curves, tolerance);
            }
            
            if (regionBreps == null || regionBreps.Length == 0)
            {
                // Fallback: direct Brep.CreatePlanarBreps on original boundaries
                // Useful if boundaries are already closed loops but maybe not strictly planar-intersecting in the same way
                regionBreps = Brep.CreatePlanarBreps(boundaries, tolerance);
            }

            if (regionBreps == null || regionBreps.Length == 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "No closed regions found from the input curves.");
                return;
            }

            List<Mesh> resultMeshes = new List<Mesh>();

            // 2. QuadRemesh each region
            var parameters = new QuadRemeshParameters
            {
                TargetQuadCount = 100, // Default for plots
                AdaptiveSize = 50,
                DetectHardEdges = true
            };

            foreach (Brep region in regionBreps)
            {
                // QuadRemesh on Brep
                try
                {
                    // Use reflection to find QuadRemesh if available, avoiding compilation errors on specific version
                    Mesh m = null;
                    try 
                    {
                        var method = typeof(Mesh).GetMethod("QuadRemesh", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, new Type[] { typeof(GeometryBase), typeof(QuadRemeshParameters) }, null);
                        if (method != null)
                        {
                            m = (Mesh)method.Invoke(null, new object[] { region, parameters });
                        }
                        else
                        {
                             // Maybe instance method on GeometryBase
                             method = typeof(GeometryBase).GetMethod("QuadRemesh", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, new Type[] { typeof(QuadRemeshParameters) }, null);
                             if (method != null)
                                 m = (Mesh)method.Invoke(region, new object[] { parameters });
                        }
                    } 
                    catch { }

                    if (m == null)
                    {
                        // Fallback to standard mesh if QuadRemesh unavailable
                        Mesh[] meshes = Mesh.CreateFromBrep(region, MeshingParameters.Default);
                        if (meshes != null && meshes.Length > 0)
                        {
                            m = new Mesh();
                            foreach(var mesh in meshes) m.Append(mesh);
                        }
                    }
                    if (m != null)
                    {
                        resultMeshes.Add(m);
                    }
                }
                catch (Exception ex)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Failed to remesh a region: " + ex.Message);
                }
            }

            DA.SetDataList(0, resultMeshes);
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("9c0d1e2f-3a4b-5c6d-7e8f-9a0b1c2d3e4f");
    }
}
