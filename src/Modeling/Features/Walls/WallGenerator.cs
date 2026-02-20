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
            Polyline polyline;
            if (resampled == null || !resampled.TryGetPolyline(out polyline))
            {
                // Fallback to control points or end points
                if (!Path.TryGetPolyline(out polyline))
                {
                    polyline = new Polyline(new Point3d[] { Path.PointAtStart, Path.PointAtEnd });
                }
            }

            if (polyline.Count < 2) return new List<Brep>();

            // 2. Generate Offset Points (Left/Right) based on Centerline Tangents
            // This ensures vertex count match for Lofting
            
            Polyline leftBase = new Polyline();
            Polyline rightBase = new Polyline();
            Polyline leftTop = new Polyline();
            Polyline rightTop = new Polyline();

            double halfThick = Thickness / 2.0;
            double maxZ = double.MinValue;

            // Pre-calculate tangents for offset direction
            Vector3d[] tangents = new Vector3d[polyline.Count];
            for (int i = 0; i < polyline.Count; i++)
            {
                if (i == 0)
                {
                    tangents[i] = polyline[1] - polyline[0];
                }
                else if (i == polyline.Count - 1)
                {
                    tangents[i] = polyline[i] - polyline[i - 1];
                }
                else
                {
                    // Average of incoming and outgoing vectors for smooth corner
                    Vector3d v1 = polyline[i] - polyline[i - 1];
                    Vector3d v2 = polyline[i + 1] - polyline[i];
                    v1.Unitize();
                    v2.Unitize();
                    tangents[i] = v1 + v2;
                }
                tangents[i].Unitize();
            }

            // First pass: Calculate Base Z and Max Z (for LevelTop)
            List<double> baseZs = new List<double>();
            
            for(int i = 0; i < polyline.Count; i++)
            {
                Point3d centerPt = polyline[i];
                double z = centerPt.Z;

                // Terrain Projection
                if (Terrain != null)
                {
                    Point3d rayOrigin = new Point3d(centerPt.X, centerPt.Y, 10000);
                    Ray3d ray = new Ray3d(rayOrigin, -Vector3d.ZAxis);
                    double t = Rhino.Geometry.Intersect.Intersection.MeshRay(Terrain, ray);
                    
                    if (t > 0) 
                    {
                        z = ray.PointAt(t).Z;
                    }
                    else
                    {
                        // Try finding closest point if ray fails (e.g. edge case)
                        Point3d closest = Terrain.ClosestPoint(centerPt);
                        // Check if closest is reasonable (e.g. within some XY distance)
                        if (new Point3d(closest.X, closest.Y, 0).DistanceTo(new Point3d(centerPt.X, centerPt.Y, 0)) < 5.0)
                        {
                            z = closest.Z;
                        }
                    }
                }
                
                baseZs.Add(z);
                if (z > maxZ) maxZ = z;
            }

            // Second pass: Construct 4 corner points for each station
            for(int i = 0; i < polyline.Count; i++)
            {
                Point3d centerPt = polyline[i];
                double baseZ = baseZs[i];
                
                // Calculate Offset Vector
                Vector3d tangent = tangents[i];
                // Rotate 90 degrees around Z
                Vector3d normal = new Vector3d(-tangent.Y, tangent.X, 0);
                if (!normal.Unitize())
                {
                     // Fallback for vertical segments (tangent is vertical)
                     normal = Vector3d.XAxis;
                }
                
                Point3d ptLeft = centerPt + normal * halfThick;
                Point3d ptRight = centerPt - normal * halfThick;
                
                // Update Z
                ptLeft.Z = baseZ;
                ptRight.Z = baseZ;
                
                leftBase.Add(ptLeft);
                rightBase.Add(ptRight);
                
                // Calculate Top Z
                double topZ;
                if (LevelTop)
                {
                    topZ = maxZ + Height;
                }
                else
                {
                    topZ = baseZ + Height;
                }
                
                leftTop.Add(new Point3d(ptLeft.X, ptLeft.Y, topZ));
                rightTop.Add(new Point3d(ptRight.X, ptRight.Y, topZ));
            }

            // 3. Create Wall Brep
            // We have 4 perfectly matching polylines.
            // Create a closed Brep.

            List<Brep> result = new List<Brep>();
            
            // Convert to Curves
            Curve cLB = leftBase.ToPolylineCurve();
            Curve cRB = rightBase.ToPolylineCurve();
            Curve cLT = leftTop.ToPolylineCurve();
            Curve cRT = rightTop.ToPolylineCurve();

            // Create Lofts
            // Side 1: Left Face
            Brep[] sLeft = Brep.CreateFromLoft(new Curve[] { cLB, cLT }, Point3d.Unset, Point3d.Unset, LoftType.Straight, false);
            // Side 2: Right Face
            Brep[] sRight = Brep.CreateFromLoft(new Curve[] { cRB, cRT }, Point3d.Unset, Point3d.Unset, LoftType.Straight, false);
            // Top Face
            Brep[] sTop = Brep.CreateFromLoft(new Curve[] { cLT, cRT }, Point3d.Unset, Point3d.Unset, LoftType.Straight, false);
            // Bottom Face (optional if you want closed solid, usually yes for walls)
            Brep[] sBottom = Brep.CreateFromLoft(new Curve[] { cLB, cRB }, Point3d.Unset, Point3d.Unset, LoftType.Straight, false);

            if (sLeft != null && sRight != null && sTop != null && sBottom != null &&
                sLeft.Length > 0 && sRight.Length > 0 && sTop.Length > 0 && sBottom.Length > 0)
            {
                // Join all faces
                List<Brep> faces = new List<Brep>();
                faces.AddRange(sLeft);
                faces.AddRange(sRight);
                faces.AddRange(sTop);
                faces.AddRange(sBottom);
                
                // Cap ends (Start and End faces)
                // Since we have matching start/end points, we can create planar surfaces
                
                // Start Face
                Polyline startPoly = new Polyline(new Point3d[] { leftBase[0], rightBase[0], rightTop[0], leftTop[0], leftBase[0] });
                Brep startFace = Brep.CreatePlanarBreps(startPoly.ToPolylineCurve(), 0.01)?[0];
                if (startFace != null) faces.Add(startFace);

                // End Face
                int last = polyline.Count - 1;
                Polyline endPoly = new Polyline(new Point3d[] { leftBase[last], rightBase[last], rightTop[last], leftTop[last], leftBase[last] });
                Brep endFace = Brep.CreatePlanarBreps(endPoly.ToPolylineCurve(), 0.01)?[0];
                if (endFace != null) faces.Add(endFace);

                Brep[] joined = Brep.JoinBreps(faces, 0.01);
                if (joined != null && joined.Length > 0)
                {
                    result.Add(joined[0]);
                }
            }

            return result;
        }
    }
}
