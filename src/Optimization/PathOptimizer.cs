using Rhino.Geometry;
using System.Collections.Generic;

namespace LandscapeToolkit.Optimization
{
    public static class PathOptimizer
    {
        /// <summary>
        /// Optimizes a curve using Laplacian smoothing (Relaxation) to simulate natural paths.
        /// </summary>
        public static Curve Optimize(Curve curve, int iterations, double strength)
        {
            if (curve == null || !curve.IsValid) return null;

            // Convert to Polyline for processing
            Polyline polyline;
            if (!curve.TryGetPolyline(out polyline))
            {
                // Resample if not a polyline
                // Use length-based division for uniform segments
                double len = curve.GetLength();
                int count = (int)(len / 2.0); // Every 2 units? Or fixed count?
                if (count < 10) count = 10;
                
                Point3d[] pts;
                curve.DivideByCount(count, true, out pts);
                if (pts == null) return curve;
                polyline = new Polyline(pts);
            }

            if (polyline.Count < 3) return curve;

            Point3d[] vertices = polyline.ToArray();
            Point3d[] newVertices = new Point3d[vertices.Length];
            
            // Pin endpoints
            newVertices[0] = vertices[0];
            newVertices[vertices.Length - 1] = vertices[vertices.Length - 1];

            // Iterative Relaxation
            for (int iter = 0; iter < iterations; iter++)
            {
                for (int i = 1; i < vertices.Length - 1; i++)
                {
                    Point3d prev = vertices[i - 1];
                    Point3d current = vertices[i];
                    Point3d next = vertices[i + 1];

                    // Laplacian vector: (Prev + Next) / 2 - Current
                    Point3d target = (prev + next) / 2.0;
                    Vector3d force = target - current;

                    // Apply force
                    newVertices[i] = current + force * strength;
                }

                // Update vertices for next iteration
                for (int i = 1; i < vertices.Length - 1; i++)
                {
                    vertices[i] = newVertices[i];
                }
            }

            return new PolylineCurve(vertices);
        }
    }
}
