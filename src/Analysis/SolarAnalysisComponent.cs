using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Drawing;
using System.Collections.Generic;

namespace LandscapeToolkit.Analysis
{
    public class SolarAnalysisComponent : GH_Component
    {
        public SolarAnalysisComponent()
          : base("Solar Exposure", "Solar",
              "Estimates solar exposure on a mesh using simple ray casting or normal orientation.",
              "Landscape", "Analysis")
        {
        }

        protected override Bitmap Icon => Icons.SolarAnalysis;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Input mesh to analyze", GH_ParamAccess.item);
            pManager.AddVectorParameter("SunDirection", "S", "Sun vector (light direction)", GH_ParamAccess.item, new Vector3d(-1, -1, -1));
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Colored Mesh", "CM", "Mesh colored by exposure", GH_ParamAccess.item);
            pManager.AddNumberParameter("Exposure", "E", "Exposure values per vertex (0-1)", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Mesh mesh = null;
            Vector3d sunDir = new Vector3d(-1, -1, -1);

            if (!DA.GetData(0, ref mesh)) return;
            DA.GetData(1, ref sunDir);

            if (mesh == null || !mesh.IsValid) return;

            Mesh coloredMesh = mesh.DuplicateMesh();
            coloredMesh.VertexColors.Clear();
            coloredMesh.Normals.ComputeNormals();

            // Normalize sun direction (pointing TO sun)
            Vector3d sunVector = -sunDir;
            sunVector.Unitize();

            List<double> exposures = new List<double>();

            for (int i = 0; i < coloredMesh.Vertices.Count; i++)
            {
                Vector3d normal = coloredMesh.Normals[i];
                
                // Dot product: N dot L
                // Value is 1.0 if facing sun directly, 0.0 if perpendicular, <0 if facing away
                double dot = normal * sunVector;
                double exposure = Math.Max(0.0, dot); // Clamp negative values to 0 (shadow side)

                // Self-shadowing raycast check could be added here for accuracy
                // For "Lite" version, we stick to N dot L
                
                exposures.Add(exposure);

                // Color map applied via GetColor
                coloredMesh.VertexColors.Add(GetColor(exposure));
            }

            DA.SetData(0, coloredMesh);
            DA.SetDataList(1, exposures);
        }

        private Color GetColor(double t)
        {
            // Simple thermal gradient
            // t is 0-1
            int r = 0, b = 0;
            int g;
            
            if (t < 0.5)
            {
                // Blue to Green
                double localT = t * 2;
                b = (int)(255 * (1 - localT));
                g = (int)(255 * localT);
            }
            else
            {
                // Green to Red
                double localT = (t - 0.5) * 2;
                g = (int)(255 * (1 - localT));
                r = (int)(255 * localT);
            }
            
            return Color.FromArgb(r, g, b);
        }


        public override Guid ComponentGuid => new Guid("3c4d5e6f-7a8b-9c0d-1e2f-3a4b5c6d7e8f");
    }
}
