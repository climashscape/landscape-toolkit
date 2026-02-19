using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace LandscapeToolkit.Analysis
{
    public class SlopeAnalysisComponent : GH_Component
    {
        public SlopeAnalysisComponent()
          : base("Slope Analysis", "Slope",
              "Analyzes the slope of a mesh and colors vertices based on angle relative to Z-axis.",
              "Landscape", "Analysis")
        {
        }

        protected override Bitmap Icon => Icons.SlopeAnalysis;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Input mesh to analyze", GH_ParamAccess.item);
            pManager.AddIntervalParameter("Range", "R", "Slope range (degrees) for color mapping (e.g., 0 to 45)", GH_ParamAccess.item, new Interval(0, 45));
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Colored Mesh", "CM", "Mesh colored by slope", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            Interval range = new Interval(0, 45);

            if (!DA.GetData(0, ref mesh)) return;
            if (!DA.GetData(1, ref range)) return;

            Mesh coloredMesh = mesh.DuplicateMesh();
            coloredMesh.VertexColors.Clear();

            // Ensure normals are computed
            coloredMesh.FaceNormals.ComputeFaceNormals();
            coloredMesh.Normals.ComputeNormals();
            
            if (coloredMesh.Normals.Count != coloredMesh.Vertices.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Mesh normals could not be computed correctly.");
                return;
            }

            // Loop through vertices for smooth gradient visualization
            // Angle is between Normal and Z-Axis.
            
            for (int i = 0; i < coloredMesh.Vertices.Count; i++)
            {
                Vector3d normal = coloredMesh.Normals[i];
                
                // VectorAngle returns 0 to Pi. 
                // Z-Axis is (0,0,1). Flat ground normal is (0,0,1), angle is 0.
                // Vertical wall normal is (x,y,0), angle is 90.
                double angleRad = Vector3d.VectorAngle(normal, Vector3d.ZAxis);
                double angleDeg = Rhino.RhinoMath.ToDegrees(angleRad);
                
                // Map angle to color
                Color color = GetColor(angleDeg, range.Min, range.Max);
                coloredMesh.VertexColors.Add(color);
            }

            DA.SetData(0, coloredMesh);
        }

        private Color GetColor(double value, double min, double max)
        {
            if (max <= min) max = min + 0.001; // Avoid divide by zero

            double t = (value - min) / (max - min);
            t = Math.Max(0, Math.Min(1, t)); // Clamp 0-1
            
            // Simple gradient: Green (flat/safe) -> Yellow -> Red (steep/danger)
            // 0.0 (Green) -> 0.5 (Yellow) -> 1.0 (Red)
            
            int r, g, b;
            
            if (t < 0.5)
            {
                // Green (0,255,0) to Yellow (255,255,0)
                r = (int)(255 * (t * 2));
                g = 255;
                b = 0;
            }
            else
            {
                // Yellow (255,255,0) to Red (255,0,0)
                r = 255;
                g = (int)(255 * (1 - (t - 0.5) * 2));
                b = 0;
            }
            
            return Color.FromArgb(r, g, b);
        }



        public override Guid ComponentGuid => new Guid("9e8d7c6b-5a4f-3e2d-1c0b-a9b8c7d6e5f4");
    }
}
