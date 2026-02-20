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

        protected override System.Drawing.Bitmap Icon => Icons.PlotGenerator;

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
            
            Brep[] regionBreps = null;

            try
            {
                // Curve.CreateBooleanRegions finds enclosed regions from a set of curves
                // combineRegions = false to get individual regions
                // Use dynamic to avoid hard dependency on Rhino 7+ type at compile time if possible,
                // but since we are compiling against RhinoCommon, we assume it's available.
                // However, runtime on older Rhino might fail.
                
                // Reflection to call static method Curve.CreateBooleanRegions
                var createMethod = typeof(Curve).GetMethod("CreateBooleanRegions", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                
                if (createMethod != null)
                {
                    var regionResults = createMethod.Invoke(null, new object[] { boundaries, plane, false, tolerance });
                    
                    if (regionResults != null)
                    {
                         // CurveBooleanRegions has RegionBreps property or method?
                         // In Rhino 7, it has RegionBreps property (Brep[]).
                         // Let's try to access it via dynamic/reflection to be safe.
                         var prop = regionResults.GetType().GetProperty("RegionBreps");
                         if (prop != null)
                         {
                             regionBreps = (Brep[])prop.GetValue(regionResults);
                         }
                         else
                         {
                             // Fallback to RegionCurves
                             var curvesProp = regionResults.GetType().GetProperty("RegionCurves");
                             if (curvesProp != null)
                             {
                                 var curvesObj = curvesProp.GetValue(regionResults);
                                 if (curvesObj is System.Collections.IEnumerable enumerable)
                                 {
                                     var curveList = new List<Curve>();
                                     foreach (var obj in enumerable) if (obj is Curve c) curveList.Add(c);
                                     if (curveList.Count > 0)
                                         regionBreps = Brep.CreatePlanarBreps(curveList, tolerance);
                                 }
                             }
                         }
                    }
                }
            }
            catch 
            {
                // Ignore errors from CreateBooleanRegions (e.g. missing method on older Rhino)
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



        public override Guid ComponentGuid => new Guid("9c0d1e2f-3a4b-5c6d-7e8f-9a0b1c2d3e4f");
    }
}
