using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace LandscapeToolkit.Modeling
{
    public class WallComponent : GH_Component
    {
        public WallComponent()
          : base("Landscape Wall", "Wall",
              "Creates a vertical wall from a curve with thickness and height (Class-A focus).",
              "Landscape", "Modeling")
        {
        }

        protected override System.Drawing.Bitmap Icon => Icons.Wall;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curve", "C", "Base curve for the wall", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "Height of the wall", GH_ParamAccess.item, 1.0);
            pManager.AddNumberParameter("Thickness", "T", "Thickness of the wall", GH_ParamAccess.item, 0.2);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Wall", "W", "Resulting wall geometry", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve curve = null;
            double height = 1.0;
            double thickness = 0.2;

            if (!DA.GetData(0, ref curve)) return;
            if (!DA.GetData(1, ref height)) return;
            if (!DA.GetData(2, ref thickness)) return;

            // Ensure curve is planar for simple extrusion, or handle 3D curves differently
            // For a high-standard landscape wall, we often want the top to follow the terrain or be level.
            // This is a basic implementation: Flat top, following curve path.

            if (curve.IsShort(Rhino.RhinoMath.ZeroTolerance)) return;

            // Offset Logic
            // Note: Curve.Offset can be tricky with complex curves. 
            // Ideally, we use Clipper or similar library for robust offsetting, 
            // but for now we stick to RhinoCommon.
            
            Plane plane = Plane.WorldXY;
            if (curve.TryGetPlane(out Plane curvePlane))
            {
                plane = curvePlane;
            }

            Curve[] offsets1 = curve.Offset(plane, thickness / 2.0, Rhino.RhinoMath.ZeroTolerance, CurveOffsetCornerStyle.Sharp);
            Curve[] offsets2 = curve.Offset(plane, -thickness / 2.0, Rhino.RhinoMath.ZeroTolerance, CurveOffsetCornerStyle.Sharp);

            if (offsets1 != null && offsets1.Length > 0 && offsets2 != null && offsets2.Length > 0)
            {
                // Join offsets and create a closed profile
                // This is a simplified approach assuming single resulting curves
                Curve c1 = offsets1[0];
                Curve c2 = offsets2[0];
                
                // Reverse one curve to form a loop
                c2.Reverse();

                List<Curve> profileCurves = new List<Curve> { c1, new LineCurve(c1.PointAtEnd, c2.PointAtStart), c2, new LineCurve(c2.PointAtEnd, c1.PointAtStart) };
                Curve[] joined = Curve.JoinCurves(profileCurves);

                if (joined != null && joined.Length > 0)
                {
                    Curve profile = joined[0];
                    Brep[] walls = Brep.CreateFromSweep(profile, new LineCurve(profile.PointAtStart, profile.PointAtStart + plane.ZAxis * height), true, Rhino.RhinoMath.ZeroTolerance);
                    
                    if (walls != null && walls.Length > 0)
                        DA.SetData(0, walls[0]);
                    else
                    {
                        // Fallback to extrusion
                        Extrusion ext = Extrusion.Create(profile, height, true);
                        if (ext != null) DA.SetData(0, ext.ToBrep());
                    }
                }
            }
        }



        public override Guid ComponentGuid => new Guid("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d");
    }
}
