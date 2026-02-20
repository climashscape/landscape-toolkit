using System.Collections.Generic;
using Rhino.Geometry;

namespace LandscapeToolkit.Modeling.Features.Walls
{
    public class WallGenerator
    {
        public Curve Path { get; set; }
        public double Height { get; set; } = 1.0;
        public double Thickness { get; set; } = 0.2;
        public Mesh Terrain { get; set; }
        public bool LevelTop { get; set; } = false;

        public WallGenerator(Curve path)
        {
            Path = path;
        }

        public List<Brep> Generate()
        {
            if (Path == null || Path.GetLength() < 0.01) return new List<Brep>();

            // 1. Resample Path for terrain adaptation
            // Divide path into segments to follow terrain closely
            double segmentLength = 1.0; // 1 meter segments
            
            // Resample
            PolylineCurve resampled = Path.ToPolyline(0, 0, 0, 0, 0, 0.1, 0, segmentLength, true);
            if (resampled == null || !resampled.TryGetPolyline(out Polyline polyline))
            {
                // Fallback to control points or end points
                if (!Path.TryGetPolyline(out polyline))
                {
                    polyline = new Polyline(new Point3d[] { Path.PointAtStart, Path.PointAtEnd });
                }
            }

            // 2. Adjust Z to Terrain
            Polyline baseLine = new Polyline(polyline);
            Polyline topLine = new Polyline(polyline);

            double maxZ = double.MinValue;
            double minZ = double.MaxValue;

            for(int i=0; i<baseLine.Count; i++)
            {
                Point3d pt = baseLine[i];
                double z = pt.Z;

                if (Terrain != null)
                {
                    Point3d rayOrigin = new Point3d(pt.X, pt.Y, 10000);
                    Ray3d ray = new Ray3d(rayOrigin, -Vector3d.ZAxis);
                    double t = Rhino.Geometry.Intersect.Intersection.MeshRay(Terrain, ray);
                    if (t > 0) z = ray.PointAt(t).Z;
                }
                
                baseLine[i] = new Point3d(pt.X, pt.Y, z);
                if (z > maxZ) maxZ = z;
                if (z < minZ) minZ = z;
            }

            // 3. Set Top Z
            for(int i=0; i<topLine.Count; i++)
            {
                Point3d basePt = baseLine[i];
                double topZ;
                
                if (LevelTop)
                {
                    // If LevelTop, we want the wall to be at least Height tall at the highest point?
                    // Or constant elevation? Usually constant elevation.
                    // Let's set topZ to maxZ + Height
                    topZ = maxZ + Height;
                }
                else
                {
                    topZ = basePt.Z + Height;
                }
                
                topLine[i] = new Point3d(basePt.X, basePt.Y, topZ);
            }

            // 4. Create Wall Brep
            // Construct solid from offsets to ensure thickness centered on path
            
            // Project to XY for consistent offsetting
            Polyline xyPoly = new Polyline(polyline);
            for(int i=0; i<xyPoly.Count; i++) xyPoly[i] = new Point3d(xyPoly[i].X, xyPoly[i].Y, 0);
            Curve xyCurve = xyPoly.ToPolylineCurve();
            
            double halfThick = Thickness / 2.0;
            
            Curve[] leftOffsets = xyCurve.Offset(Plane.WorldXY, halfThick, 0.01, CurveOffsetCornerStyle.Sharp);
            Curve[] rightOffsets = xyCurve.Offset(Plane.WorldXY, -halfThick, 0.01, CurveOffsetCornerStyle.Sharp);
            
            List<Brep> result = new List<Brep>();
            
            if (leftOffsets != null && leftOffsets.Length > 0 && rightOffsets != null && rightOffsets.Length > 0)
            {
                // Assume simple path for now - take first offset
                Curve leftCrv = leftOffsets[0];
                Curve rightCrv = rightOffsets[0];
                
                // Resample offsets to match original point count/distribution if possible, 
                // but Offset changes structure. 
                // Better to just sample Z for the new curves.
                
                // Helper to apply Z
                Curve ApplyZ(Curve inputCrv, bool isTop)
                {
                    if (!inputCrv.TryGetPolyline(out Polyline p)) 
                    {
                        // Convert if needed
                         p = inputCrv.ToPolyline(0,0,0,0,0,0.1,0,1.0,true).ToPolyline();
                    }
                    
                    Polyline newP = new Polyline(p);
                    for(int i=0; i<newP.Count; i++)
                    {
                        Point3d pt = newP[i];
                        double z = 0;
                        
                        // Get Base Z
                        if (Terrain != null)
                        {
                            Point3d rayOrigin = new Point3d(pt.X, pt.Y, 10000);
                            Ray3d ray = new Ray3d(rayOrigin, -Vector3d.ZAxis);
                            double t = Rhino.Geometry.Intersect.Intersection.MeshRay(Terrain, ray);
                            if (t > 0) z = ray.PointAt(t).Z;
                            else z = Path.PointAtStart.Z; // Fallback
                        }
                        else
                        {
                            // Use closest point on original 3D path to get Z?
                            // Or just flat?
                            // Let's use original path Z
                            if (Path.ClosestPoint(pt, out double t))
                            {
                                z = Path.PointAt(t).Z;
                            }
                        }
                        
                        if (isTop)
                        {
                            if (LevelTop)
                                z = maxZ + Height; // Use maxZ calculated earlier? Re-calc might be safer but maxZ is from center
                            else
                                z += Height;
                        }
                        
                        newP[i] = new Point3d(pt.X, pt.Y, z);
                    }
                    return newP.ToPolylineCurve();
                }
                
                Curve leftBase = ApplyZ(leftCrv, false);
                Curve leftTop = ApplyZ(leftCrv, true);
                Curve rightBase = ApplyZ(rightCrv, false);
                Curve rightTop = ApplyZ(rightCrv, true);
                
                // Create surfaces
                Brep[] loftsLeft = Brep.CreateFromLoft(new Curve[]{leftBase, leftTop}, Point3d.Unset, Point3d.Unset, LoftType.Normal, false);
                Brep[] loftsRight = Brep.CreateFromLoft(new Curve[]{rightBase, rightTop}, Point3d.Unset, Point3d.Unset, LoftType.Normal, false);
                Brep[] loftsTop = Brep.CreateFromLoft(new Curve[]{leftTop, rightTop}, Point3d.Unset, Point3d.Unset, LoftType.Normal, false);
                Brep[] loftsBottom = Brep.CreateFromLoft(new Curve[]{leftBase, rightBase}, Point3d.Unset, Point3d.Unset, LoftType.Normal, false);

                if (loftsLeft != null && loftsLeft.Length > 0 && 
                    loftsRight != null && loftsRight.Length > 0 &&
                    loftsTop != null && loftsTop.Length > 0 &&
                    loftsBottom != null && loftsBottom.Length > 0)
                {
                    Brep sLeft = loftsLeft[0];
                    Brep sRight = loftsRight[0];
                    Brep sTop = loftsTop[0];
                    Brep sBottom = loftsBottom[0];
                    
                    // Join
                    Brep[] joined = Brep.JoinBreps(new Brep[]{sLeft, sRight, sTop, sBottom}, 0.01);
                    if (joined != null && joined.Length > 0)
                    {
                        Brep wall = joined[0];
                        wall = wall.CapPlanarHoles(0.01); // Cap ends
                        result.Add(wall);
                    }
                }
            }
            else
            {
                // Fallback: simple extrusion of center line (surface)
                Curve baseCrv = baseLine.ToPolylineCurve();
                Curve topCrv = topLine.ToPolylineCurve();
                Brep[] lofts = Brep.CreateFromLoft(new Curve[] { baseCrv, topCrv }, Point3d.Unset, Point3d.Unset, LoftType.Normal, false);
                if (lofts != null) result.AddRange(lofts);
            }

            return result;
        }
    }
}
