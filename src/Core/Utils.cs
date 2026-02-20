using Rhino.Geometry;

namespace LandscapeToolkit.Core
{
    public static class Utils
    {
        /// <summary>
        /// Converts a curve to a high-quality NURBS curve if it isn't already.
        /// </summary>
        public static Curve EnsureHighQualityCurve(Curve curve)
        {
            if (curve == null) return null;
            
            // Example: Rebuild curve to ensure smooth continuity (C2)
            // This is a placeholder for more advanced "Class-A" logic
            if (curve.Degree < 3)
            {
                return curve.Rebuild(curve.PointAtStart.DistanceTo(curve.PointAtEnd) > 0 ? 10 : 20, 3, true);
            }
            return curve;
        }

        /// <summary>
        /// Helper to validate if a mesh is suitable for analysis (e.g. valid, has normals).
        /// </summary>
        public static bool ValidateMeshForAnalysis(Mesh mesh, out string message)
        {
            message = string.Empty;
            if (mesh == null)
            {
                message = "Mesh is null.";
                return false;
            }
            if (!mesh.IsValid)
            {
                message = "Mesh is invalid.";
                return false;
            }
            return true;
        }
    }
}
