using System.Collections.Generic;
using Rhino.Geometry;

namespace LandscapeToolkit.Modeling.Features.Boardwalks
{
    /// <summary>
    /// Generates boardwalk structures along a path.
    /// 生成栈道结构（面板、立柱、栏杆）
    /// </summary>
    public class BoardwalkGenerator
    {
        public Curve Path { get; set; }
        public double Width { get; set; } = 2.0;
        public double HeightAboveTerrain { get; set; } = 0.5;
        public double SupportSpacing { get; set; } = 3.0;
        public double SupportRadius { get; set; } = 0.1;
        public bool HasRailings { get; set; } = true;
        public Mesh Terrain { get; set; }

        public BoardwalkGenerator(Curve path, Mesh terrain)
        {
            Path = path;
            Terrain = terrain;
        }

        public void Generate(out Brep deck, out List<Brep> supports, out List<Brep> railings)
        {
            deck = null;
            supports = new List<Brep>();
            railings = new List<Brep>();

            if (Path == null || !Path.IsValid) return;

            // 1. Generate Deck (Sweep)
            // Create a rectangle profile perpendicular to start of curve
            double tStart = Path.Domain.Min;
            Point3d startPt = Path.PointAt(tStart);
            Vector3d startTangent = Path.TangentAt(tStart);
            
            // Construct a horizontal frame
            Vector3d startRight = Vector3d.CrossProduct(startTangent, Vector3d.ZAxis);
            if (startRight.Length < 1e-6) startRight = Vector3d.XAxis; // Handle vertical path
            startRight.Unitize();
            
            Vector3d startUp = Vector3d.CrossProduct(startRight, startTangent);
            startUp.Unitize();
            
            Plane startPlane = new Plane(startPt, startRight, startUp);

            if (startPlane.IsValid)
            {
                // Deck profile: Rectangle centered on path
                // X is Width (Horizontal), Y is Thickness (Vertical)
                double deckThickness = 0.15;
                Rectangle3d rect = new Rectangle3d(startPlane, new Interval(-Width / 2, Width / 2), new Interval(-deckThickness, 0));
                Curve profile = rect.ToNurbsCurve();

                // Sweep along path
                SweepOneRail sweep = new SweepOneRail
                {
                    AngleToleranceRadians = 0.01,
                    ClosedSweep = false,
                    SweepTolerance = 0.01
                };
                
                var breps = sweep.PerformSweep(Path, profile);
                if (breps != null && breps.Length > 0)
                {
                    deck = breps[0];
                    // Cap planar holes if needed, though sweep usually returns open or closed depending on input
                    deck = deck.CapPlanarHoles(0.01);
                }
            }

            // 2. Generate Supports (Piles)
            if (Terrain != null && Terrain.IsValid)
            {
                double terrainMaxZ = Terrain.GetBoundingBox(true).Max.Z;
                double[] tParams = Path.DivideByLength(SupportSpacing, true);
                if (tParams != null)
                {
                    foreach (double t in tParams)
                    {
                        Point3d ptOnCurve = Path.PointAt(t);
                        
                        // Find ground Z
                        // Ray cast down from high up
                        Point3d rayStart = new Point3d(ptOnCurve.X, ptOnCurve.Y, terrainMaxZ + 10.0);
                        Point3d[] hits = Rhino.Geometry.Intersect.Intersection.ProjectPointsToMeshes(
                            new Mesh[] { Terrain },
                            new Point3d[] { rayStart },
                            new Vector3d(0, 0, -1),
                            0.01);

                        if (hits != null && hits.Length > 0)
                        {
                            Point3d groundPt = hits[0];
                            // Create cylinder from ground to curve - thickness
                            double topZ = ptOnCurve.Z - 0.15; // Below deck
                            double bottomZ = groundPt.Z;

                            if (topZ > bottomZ)
                            {
                                Point3d basePt = new Point3d(ptOnCurve.X, ptOnCurve.Y, bottomZ);
                                Point3d topPt = new Point3d(ptOnCurve.X, ptOnCurve.Y, topZ);
                                Line axis = new Line(basePt, topPt);
                                Cylinder cyl = new Cylinder(new Circle(new Plane(basePt, Vector3d.ZAxis), SupportRadius), axis.Length);
                                supports.Add(cyl.ToBrep(true, true));
                            }
                        }
                    }
                }
            }

            // 3. Generate Railings (Sweep along offset curves)
            if (HasRailings)
            {
                // Offset curve left and right
                // Using Offset on Surface logic if possible, or just simpler offset
                // For 3D curves, offsetting is tricky. 
                // Simple approach: Railing posts + handrail curve
                
                // Let's sweep a small circle along the edges of the deck surface if possible.
                // Or create offset curves.
                
                // Simplified: Railing posts at supports, and top rail.
                // Not implementing full railing sweep for now to keep it simple and robust.
                // Just add posts for now.
                
                double railHeight = 1.1;
                double railRadius = 0.04;
                
                double[] tParams = Path.DivideByLength(SupportSpacing, true);
                if (tParams != null)
                {
                    foreach (double t in tParams)
                    {
                        Vector3d tangent = Path.TangentAt(t);
                        Vector3d right = Vector3d.CrossProduct(tangent, Vector3d.ZAxis);
                        if (right.Length < 0.01) right = Vector3d.XAxis; // Vertical path case
                        right.Unitize();
                        
                        Point3d pt = Path.PointAt(t);
                        Point3d lPos = pt - right * (Width / 2 - 0.1);
                        Point3d rPos = pt + right * (Width / 2 - 0.1);
                        
                        // Left Post
                        Cylinder lCyl = new Cylinder(new Circle(new Plane(lPos, Vector3d.ZAxis), railRadius), railHeight);
                        railings.Add(lCyl.ToBrep(true, true));
                        
                        // Right Post
                        Cylinder rCyl = new Cylinder(new Circle(new Plane(rPos, Vector3d.ZAxis), railRadius), railHeight);
                        railings.Add(rCyl.ToBrep(true, true));
                    }
                }
                
                // Handrails (Top horizontal bars)
                // We can generate curves connecting the tops of posts
                // But for now, let's just leave posts or try to sweep along offset curve
            }
        }
    }
}
