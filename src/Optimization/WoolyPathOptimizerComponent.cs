using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

namespace LandscapeToolkit.Optimization
{
    public class WoolyPathOptimizerComponent : GH_Component
    {
        private WoolyPathOptimizer _optimizer;
        private int _currentIteration = 0;
        private BoundingBox _bounds = BoundingBox.Unset;

        public WoolyPathOptimizerComponent()
          : base("Wooly Path Optimizer", "WoolyPath",
              "Bio-mimetic path optimization using Slime Mold / Physarum Polycephalum algorithm.",
              "Landscape", "Optimization")
        {
        }

        protected override System.Drawing.Bitmap Icon => Icons.WoolyPathOptimizer;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddRectangleParameter("Bounds", "B", "Simulation boundary", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Resolution", "Res", "Grid resolution (cells per side)", GH_ParamAccess.item, 100);
            pManager.AddIntegerParameter("AgentCount", "N", "Number of agents", GH_ParamAccess.item, 1000);
            pManager.AddNumberParameter("SensorDistance", "SD", "Sensor distance", GH_ParamAccess.item, 5.0);
            pManager.AddNumberParameter("SensorAngle", "SA", "Sensor angle (radians)", GH_ParamAccess.item, Math.PI / 4.0);
            pManager.AddNumberParameter("TurnAngle", "TA", "Turn angle (radians)", GH_ParamAccess.item, Math.PI / 4.0);
            pManager.AddBooleanParameter("Reset", "R", "Reset simulation", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Run", "Run", "Run simulation", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("TrailMap", "M", "Visualization of trail map", GH_ParamAccess.item);
            pManager.AddPointParameter("Agents", "A", "Agent positions", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Iteration", "I", "Current iteration", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Rectangle3d rect = Rectangle3d.Unset;
            int resolution = 100;
            int count = 1000;
            double sensorDist = 5.0;
            double sensorAngle = Math.PI / 4.0;
            double turnAngle = Math.PI / 4.0;
            bool reset = false;
            bool run = false;

            if (!DA.GetData(0, ref rect)) return;
            DA.GetData(1, ref resolution);
            DA.GetData(2, ref count);
            DA.GetData(3, ref sensorDist);
            DA.GetData(4, ref sensorAngle);
            DA.GetData(5, ref turnAngle);
            DA.GetData(6, ref reset);
            DA.GetData(7, ref run);

            BoundingBox currentBounds = rect.BoundingBox;

            if (reset || _optimizer == null || !_bounds.Equals(currentBounds))
            {
                _bounds = currentBounds;
                double cellSize = Math.Max(_bounds.Diagonal.X, _bounds.Diagonal.Y) / resolution;
                
                _optimizer = new WoolyPathOptimizer(_bounds, cellSize, count);
                _optimizer.SensorDistance = sensorDist;
                _optimizer.SensorAngle = sensorAngle;
                _optimizer.TurnAngle = turnAngle;
                _currentIteration = 0;
            }

            if (run && _optimizer != null)
            {
                _optimizer.Update();
                _currentIteration++;
            }

            if (_optimizer != null)
            {
                DA.SetData(0, _optimizer.GetTrailMesh());
                DA.SetDataList(1, _optimizer.GetAgentPositions());
                DA.SetData(2, _currentIteration);
            }
        }


        public override Guid ComponentGuid => new Guid("a1b2c3d4-e5f6-7890-1234-567890abcdef");
    }
}
