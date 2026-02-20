using Grasshopper.Kernel;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LandscapeToolkit.Integration
{
    public class RhinoPickerComponent : GH_Component
    {
        public RhinoPickerComponent()
          : base("Rhino Picker", "Picker",
              "Selects Rhino objects by Layer, Name, or Type.",
              "Landscape", "Integration")
        {
        }

        protected override System.Drawing.Bitmap Icon => Icons.RhinoPicker;

        public override Guid ComponentGuid => new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef");

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Layer", "L", "Layer name filter (optional)", GH_ParamAccess.list);
            pManager[0].Optional = true;
            pManager.AddTextParameter("Name", "N", "Object name filter (optional, supports * wildcards)", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager.AddTextParameter("Type", "T", "Object type filter (Curve, Brep, Mesh, Point, Text, etc.)", GH_ParamAccess.item, "All");
            pManager[2].Optional = true;
            pManager.AddBooleanParameter("Refresh", "R", "Refresh selection", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "G", "Selected geometry", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Count", "C", "Number of selected objects", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<string> layers = new List<string>();
            string nameFilter = null;
            string typeFilter = "All";
            bool refresh = false;

            if (!DA.GetDataList(0, layers)) layers = new List<string>();
            DA.GetData(1, ref nameFilter);
            DA.GetData(2, ref typeFilter);
            DA.GetData(3, ref refresh);

            // Always run if inputs change, but refresh button forces update
            // In GH, components run on input change anyway.

            var doc = RhinoDoc.ActiveDoc;
            if (doc == null) return;

            var settings = new ObjectEnumeratorSettings();
            settings.NormalObjects = true;
            settings.LockedObjects = false;
            settings.HiddenObjects = false;
            settings.ReferenceObjects = true; // Support worksession files? Maybe optional.

            ObjectType typeMask = ObjectType.AnyObject;
            if (!string.IsNullOrEmpty(typeFilter) && !typeFilter.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                if (Enum.TryParse(typeFilter, true, out ObjectType parsedType))
                {
                    typeMask = parsedType;
                }
                else
                {
                    // Map simple names to ObjectType
                    switch (typeFilter.ToLower())
                    {
                        case "curve": typeMask = ObjectType.Curve; break;
                        case "surface": typeMask = ObjectType.Surface; break;
                        case "brep": typeMask = ObjectType.Brep; break;
                        case "mesh": typeMask = ObjectType.Mesh; break;
                        case "point": typeMask = ObjectType.Point; break;
                        case "text": typeMask = ObjectType.Annotation; break; // Text is Annotation
                        default: 
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Unknown type filter: {typeFilter}. Using All.");
                            break;
                    }
                }
            }
            settings.ObjectTypeFilter = typeMask;

            var objects = doc.Objects.GetObjectList(settings);
            var result = new List<GeometryBase>();

            foreach (var obj in objects)
            {
                // Layer Filter
                if (layers.Count > 0)
                {
                    var layerIndex = obj.Attributes.LayerIndex;
                    var layer = doc.Layers[layerIndex];
                    // Check full path or name
                    bool layerMatch = false;
                    foreach (var l in layers)
                    {
                        if (string.Equals(layer.Name, l, StringComparison.OrdinalIgnoreCase) || 
                            string.Equals(layer.FullPath, l, StringComparison.OrdinalIgnoreCase))
                        {
                            layerMatch = true;
                            break;
                        }
                    }
                    if (!layerMatch) continue;
                }

                // Name Filter
                if (!string.IsNullOrEmpty(nameFilter))
                {
                    if (string.IsNullOrEmpty(obj.Attributes.Name)) continue;
                    // Simple wildcard match
                    if (!LikelyMatch(obj.Attributes.Name, nameFilter)) continue;
                }

                result.Add(obj.Geometry);
            }

            DA.SetDataList(0, result);
            DA.SetData(1, result.Count);
        }

        private bool LikelyMatch(string input, string pattern)
        {
            if (pattern == "*") return true;
            if (pattern.StartsWith("*") && pattern.EndsWith("*")) 
                return input.Contains(pattern.Trim('*'));
            if (pattern.StartsWith("*")) 
                return input.EndsWith(pattern.TrimStart('*'));
            if (pattern.EndsWith("*")) 
                return input.StartsWith(pattern.TrimEnd('*'));
            return string.Equals(input, pattern, StringComparison.OrdinalIgnoreCase);
        }
    }
}
