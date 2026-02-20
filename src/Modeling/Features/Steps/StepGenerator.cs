using Rhino.Geometry;
using System.Collections.Generic;

namespace LandscapeToolkit.Modeling.Features.Steps
{
    /// <summary>
    /// Generates parametric landscape steps.
    /// 核心逻辑：台阶生成器
    /// </summary>
    public class StepGenerator
    {
        public Curve Path { get; set; }
        public double Width { get; set; } = 2.0;
        public double Tread { get; set; } = 0.3;
        public double Riser { get; set; } = 0.15;
        public bool AdaptToTerrain { get; set; } = false;
        public Mesh Terrain { get; set; }

        public StepGenerator(Curve path)
        {
            Path = path;
        }

        public List<Brep> Generate()
        {
            if (Path == null || Path.GetLength() < Tread) return new List<Brep>();

            // 1. Prepare Path (XY Projection)
            // 路径投影到平面，确保踏步水平距离恒定
            Curve xyPath = Curve.ProjectToPlane(Path, Plane.WorldXY);
            if (xyPath == null) return new List<Brep>();

            double length = xyPath.GetLength();
            int count = (int)(length / Tread);
            
            var steps = new List<Brep>();

            for (int i = 0; i < count; i++)
            {
                // 2. Calculate Position
                double dist = i * Tread + (Tread / 2.0); // Center of tread
                if (!xyPath.LengthParameter(dist, out double t)) continue;

                Point3d center = xyPath.PointAt(t);
                Vector3d tangent = xyPath.TangentAt(t);
                Vector3d normal = Vector3d.CrossProduct(tangent, Vector3d.ZAxis); // Width direction
                
                // 3. Determine Height (Z)
                double currentZ;
                if (AdaptToTerrain && Terrain != null)
                {
                    // Project center to terrain to find Z
                    Point3d rayOrigin = new Point3d(center.X, center.Y, 10000);
                    Ray3d ray = new Ray3d(rayOrigin, -Vector3d.ZAxis);
                    double tHit = Rhino.Geometry.Intersect.Intersection.MeshRay(Terrain, ray);
                    
                    if (tHit > 0)
                    {
                        currentZ = ray.PointAt(tHit).Z;
                    }
                    else
                    {
                        // Fallback to path Z if terrain miss
                        currentZ = Path.PointAt(t).Z;
                    }
                }
                else
                {
                    // Follow Path Z
                    // 如果不贴合地形，则跟随路径的 Z 坐标
                    // 这样可以支持有坡度的路径，而不仅仅是固定台阶高度
                    currentZ = Path.PointAt(t).Z;
                }

                // 4. Construct Geometry
                Plane plane = new Plane(new Point3d(center.X, center.Y, currentZ), tangent, normal);
                
                // Tread Rectangle: X=Tangent (Tread), Y=Normal (Width)
                Interval xDomain = new Interval(-Tread / 2.0, Tread / 2.0);
                Interval yDomain = new Interval(-Width / 2.0, Width / 2.0);
                Rectangle3d rect = new Rectangle3d(plane, xDomain, yDomain);
                
                // Create Solid Block (Extrude Down)
                // Downward extrusion ensures solid footing
                Curve profile = rect.ToNurbsCurve();
                Extrusion ext = Extrusion.Create(profile, -Riser, true); 
                
                if (ext != null)
                {
                    steps.Add(ext.ToBrep());
                }
            }

            return steps;
        }
    }
}
