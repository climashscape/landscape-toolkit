using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace LandscapeToolkit.Modeling.Features.Scatter
{
    /// <summary>
    /// Distributes landscape elements (lights, benches, trees) based on rules.
    /// 散布系统：基于规则分布路灯、座椅、植物
    /// </summary>
    public class ScatterSystem
    {
        // 1. Inputs
        public Mesh TargetSurface { get; set; } // The terrain or plot
        public List<Curve> RoadEdges { get; set; } // Road boundaries
        
        // 2. Settings
        public double MinDistance { get; set; } = 5.0; // Poisson disk radius
        public ScatterType Type { get; set; } // Tree, Light, Bench
        
        public enum ScatterType { Tree, Light, Bench, GroundCover }

        /// <summary>
        /// Generate points and instances.
        /// </summary>
        public List<Point3d> GeneratePoints()
        {
            List<Point3d> points = new List<Point3d>();

            if (Type == ScatterType.Light)
            {
                // Lights follow road edges
                // 路灯：沿道路边缘等距分布
                points = DistributeAlongCurves(RoadEdges, 15.0);
            }
            else if (Type == ScatterType.Bench)
            {
                // Benches at nodes or specific intervals
                // 座椅：在节点或特定间隔分布
                points = DistributeAlongCurves(RoadEdges, 30.0);
            }
            else if (Type == ScatterType.Tree)
            {
                // Trees: Poisson Disk Sampling on Surface
                // 乔木：泊松盘采样，避免重叠
                points = PoissonDiskSampling(TargetSurface, MinDistance);
            }

            return points;
        }

        private List<Point3d> DistributeAlongCurves(List<Curve> curves, double spacing)
        {
            var result = new List<Point3d>();
            foreach (var crv in curves)
            {
                double[] params_ = crv.DivideByLength(spacing, true);
                if (params_ != null)
                {
                    foreach (double t in params_) result.Add(crv.PointAt(t));
                }
            }
            return result;
        }

        private List<Point3d> PoissonDiskSampling(Mesh mesh, double radius)
        {
            if (mesh == null || !mesh.IsValid) return new List<Point3d>();

            // 1. Get Bounding Box
            BoundingBox bbox = mesh.GetBoundingBox(true);
            double width = bbox.Max.X - bbox.Min.X;
            double height = bbox.Max.Y - bbox.Min.Y;

            // 2. Generate 2D Points
            List<Point2d> points2d = GeneratePoissonPoints2D(width, height, radius);

            // 3. Project to Mesh
            List<Point3d> result = new List<Point3d>();
            List<Point3d> probePoints = new List<Point3d>();

            foreach (var p in points2d)
            {
                probePoints.Add(new Point3d(bbox.Min.X + p.X, bbox.Min.Y + p.Y, bbox.Max.Z + 10.0));
            }

            // Batch project for performance
            Point3d[] projected = Rhino.Geometry.Intersect.Intersection.ProjectPointsToMeshes(
                new Mesh[] { mesh }, 
                probePoints, 
                new Vector3d(0, 0, -1), 
                0.01);

            if (projected != null)
            {
                result.AddRange(projected);
            }

            return result;
        }

        /// <summary>
        /// Standard 2D Poisson Disk Sampling (Bridson's algorithm).
        /// </summary>
        private List<Point2d> GeneratePoissonPoints2D(double width, double height, double radius)
        {
            double cellSize = radius / Math.Sqrt(2);
            int gridW = (int)Math.Ceiling(width / cellSize);
            int gridH = (int)Math.Ceiling(height / cellSize);

            int[,] grid = new int[gridW, gridH]; // Stores index of point
            for (int i = 0; i < gridW; i++)
                for (int j = 0; j < gridH; j++)
                    grid[i, j] = -1;

            List<Point2d> points = new List<Point2d>();
            List<Point2d> activeList = new List<Point2d>();

            // Initial point
            Random rnd = new Random();
            Point2d p0 = new Point2d(rnd.NextDouble() * width, rnd.NextDouble() * height);
            points.Add(p0);
            activeList.Add(p0);
            
            int gx = (int)(p0.X / cellSize);
            int gy = (int)(p0.Y / cellSize);
            if (gx >= 0 && gx < gridW && gy >= 0 && gy < gridH)
                grid[gx, gy] = 0;

            // Iteration
            int k = 30; // Max attempts
            
            while (activeList.Count > 0)
            {
                int idx = rnd.Next(activeList.Count);
                Point2d center = activeList[idx];
                bool found = false;

                for (int i = 0; i < k; i++)
                {
                    double angle = rnd.NextDouble() * 2 * Math.PI;
                    double dist = radius * (rnd.NextDouble() + 1); // r to 2r
                    
                    double newX = center.X + Math.Cos(angle) * dist;
                    double newY = center.Y + Math.Sin(angle) * dist;
                    Point2d candidate = new Point2d(newX, newY);

                    if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                    {
                        int cx = (int)(newX / cellSize);
                        int cy = (int)(newY / cellSize);
                        
                        if (cx >= 0 && cx < gridW && cy >= 0 && cy < gridH && grid[cx, cy] == -1)
                        {
                            // Check neighbors
                            bool tooClose = false;
                            for (int dx = -2; dx <= 2; dx++)
                            {
                                for (int dy = -2; dy <= 2; dy++)
                                {
                                    int nx = cx + dx;
                                    int ny = cy + dy;
                                    if (nx >= 0 && nx < gridW && ny >= 0 && ny < gridH && grid[nx, ny] != -1)
                                    {
                                        Point2d neighbor = points[grid[nx, ny]];
                                        if (candidate.DistanceTo(neighbor) < radius)
                                        {
                                            tooClose = true;
                                            break;
                                        }
                                    }
                                }
                                if (tooClose) break;
                            }

                            if (!tooClose)
                            {
                                points.Add(candidate);
                                activeList.Add(candidate);
                                grid[cx, cy] = points.Count - 1;
                                found = true;
                                break;
                            }
                        }
                    }
                }

                if (!found)
                {
                    activeList.RemoveAt(idx);
                }
            }

            return points;
        }
    }
}
