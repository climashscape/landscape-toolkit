using Rhino.Geometry;

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
            // Use ToPolyline with strict tolerance to keep shape
            var polyCurve = curve.ToPolyline(0, 0, 0.1, 0, 0, 0, 0, 0, true);
            if (polyCurve == null || !polyCurve.TryGetPolyline(out Polyline polyline))
            {
                // Resample if conversion fails (e.g. single line segment)
                if (curve.IsLinear(0.01))
                {
                    // Linear curve: just divide it
                     polyline = new Polyline(new Point3d[] { curve.PointAtStart, curve.PointAtEnd });
                     // Subdivide for flexibility
                     for(int k=0; k<3; k++) polyline.Insert(1, (polyline[0]+polyline[1])*0.5);
                }
                else
                {
                    double[] t = curve.DivideByCount(20, true);
                    if (t == null) return curve;
                    Point3d[] pts = new Point3d[t.Length];
                    for(int i=0; i<t.Length; i++) pts[i] = curve.PointAt(t[i]);
                    polyline = new Polyline(pts);
                }
            }

            if (polyline.Count < 3) 
            {
                 // Insert points if too few
                 while(polyline.Count < 10)
                 {
                     Polyline newPl = new Polyline();
                     newPl.Add(polyline[0]);
                     for(int i=0; i<polyline.Count-1; i++)
                     {
                         newPl.Add((polyline[i] + polyline[i+1])*0.5);
                         newPl.Add(polyline[i+1]);
                     }
                     polyline = newPl;
                 }
            }

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
