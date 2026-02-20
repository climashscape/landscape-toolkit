using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace LandscapeToolkit.Optimization
{
    public class WoolyPathOptimizerComponent : GH_Component
    {
        private WoolyPathOptimizer _optimizer;
        private int _currentIteration = 0;
        private BoundingBox _bounds = BoundingBox.Unset;

        public WoolyPathOptimizerComponent()
          : base("Wooly Path Optimizer", "WoolyPath",
              "Bio-mimetic path optimization using Slime Mold (Physarum) algorithm.\n" +
              "Process Flow:\n" +
              "1. Initialize: Create agent population and trail map grid within bounds.\n" +
              "2. Sense: Agents sense trail pheromones ahead (Left/Front/Right).\n" +
              "3. Move: Agents rotate towards strongest signal and move forward.\n" +
              "4. Deposit: Agents deposit trail pheromones at new position.\n" +
              "5. Diffuse & Decay: Pheromones diffuse to neighbors and decay over time.\n" +
              "6. Visualize: Output trail map as mesh and agent positions.\n\n" +
              "处理流程：\n" +
              "1. 初始化：在边界内创建代理种群和轨迹网格。\n" +
              "2. 感知：代理感知前方的轨迹信息素（左/前/右）。\n" +
              "3. 移动：代理转向信号最强的方向并前进。\n" +
              "4. 沉积：代理在新位置留下轨迹信息素。\n" +
              "5. 扩散与衰减：信息素向邻居扩散并随时间衰减。\n" +
              "6. 可视化：输出轨迹图网格和代理位置。",
              "Landscape", "Optimization")
        {
        }

        protected override System.Drawing.Bitmap Icon => Icons.WoolyPathOptimizer;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
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

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
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
                
                _optimizer = new WoolyPathOptimizer(_bounds, cellSize, count)
                {
                    SensorDistance = sensorDist,
                    SensorAngle = sensorAngle,
                    TurnAngle = turnAngle
                };
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


        public override Guid ComponentGuid => new Guid("e229914d-0d41-4ffb-b2ef-d82d4d6795c9");
    }
}
