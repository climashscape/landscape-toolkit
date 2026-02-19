using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace LandscapeToolkit.Optimization
{
    /// <summary>
    /// Implements bio-mimetic path optimization (Wooly Path / Slime Mold).
    /// 仿生微调算法：羊毛算法/粘菌算法
    /// </summary>
    public class WoolyPathOptimizer
    {
        // Agent settings (智能体参数)
        public int AgentCount { get; set; } = 1000;
        public double SensorDistance { get; set; } = 5.0;
        public double SensorAngle { get; set; } = Math.PI / 4.0; // Angle of sensors relative to forward
        public double StepSize { get; set; } = 1.0;
        public double TurnAngle { get; set; } = Math.PI / 4.0; // Rotation speed
        public double DecayFactor { get; set; } = 0.9;
        public double DiffuseFactor { get; set; } = 1.0; // 0-1, higher means faster diffusion

        // Environment settings (环境参数)
        public double[,] TrailMap { get; private set; } // Pheromone map (信息素地图)
        public int Width { get; private set; }
        public int Height { get; private set; }
        public BoundingBox Bounds { get; private set; }
        public double CellSize { get; private set; }

        private List<Agent> Agents = new List<Agent>();
        private Random _random = new Random();

        public WoolyPathOptimizer(BoundingBox bounds, double resolution, int agentCount)
        {
            Bounds = bounds;
            CellSize = resolution;
            Width = (int)(bounds.Diagonal.X / resolution);
            Height = (int)(bounds.Diagonal.Y / resolution);
            
            // Ensure valid dimensions
            if (Width <= 0) Width = 1;
            if (Height <= 0) Height = 1;

            TrailMap = new double[Width, Height];
            AgentCount = agentCount;
            
            InitializeAgents();
        }

        private void InitializeAgents()
        {
            Agents.Clear();
            for (int i = 0; i < AgentCount; i++)
            {
                // Random position within bounds
                double x = Bounds.Min.X + _random.NextDouble() * (Bounds.Max.X - Bounds.Min.X);
                double y = Bounds.Min.Y + _random.NextDouble() * (Bounds.Max.Y - Bounds.Min.Y);
                // Random direction
                double angle = _random.NextDouble() * 2 * Math.PI;
                Vector3d dir = new Vector3d(Math.Cos(angle), Math.Sin(angle), 0);
                
                Agents.Add(new Agent { Position = new Point3d(x, y, 0), Direction = dir });
            }
        }

        /// <summary>
        /// Run simulation steps.
        /// 执行一步模拟
        /// </summary>
        public void Update()
        {
            // 1. Move Agents
            // 移动所有智能体
            foreach (var agent in Agents)
            {
                // Sense pheromones (Forward, Left, Right)
                // 感知前方、左前、右前的信息素浓度
                double sensorF = Sense(agent, 0);
                double sensorL = Sense(agent, SensorAngle);
                double sensorR = Sense(agent, -SensorAngle);
                
                // Steer towards strongest signal
                // 转向最强信号方向
                if (sensorF > sensorL && sensorF > sensorR) { /* Keep going */ }
                else if (sensorF < sensorL && sensorF < sensorR) { agent.Rotate((_random.NextDouble() - 0.5) * 2 * TurnAngle); }
                else if (sensorL > sensorR) { agent.Rotate(TurnAngle); }
                else if (sensorR > sensorL) { agent.Rotate(-TurnAngle); }
                
                // Update position
                agent.Move(StepSize);

                // Boundary check (Bounce)
                if (!Bounds.Contains(agent.Position))
                {
                    // Simple bounce: reverse direction and clamp
                    agent.Direction = -agent.Direction;
                    agent.Position = new Point3d(
                        Math.Max(Bounds.Min.X, Math.Min(Bounds.Max.X, agent.Position.X)),
                        Math.Max(Bounds.Min.Y, Math.Min(Bounds.Max.Y, agent.Position.Y)),
                        0);
                }
                
                // Deposit trail
                // 在当前位置留下足迹
                DepositTrail(agent.Position);
            }
            
            // 2. Diffuse and Decay Trail Map
            // 信息素扩散与衰减
            DiffuseTrailMap();
            DecayTrailMap();
        }
        
        /// <summary>
        /// Extract optimized curves from the trail map using a simplified tracing algorithm.
        /// 从信息素地图提取路径曲线
        /// </summary>
        public List<Curve> ExtractPaths(double threshold = 0.5)
        {
            List<Curve> paths = new List<Curve>();
            bool[,] visited = new bool[Width, Height];
            
            // Iterate through grid to find high intensity starting points
            for (int x = 1; x < Width - 1; x += 2) // Skip some pixels for speed
            {
                for (int y = 1; y < Height - 1; y += 2)
                {
                    if (visited[x, y]) continue;
                    
                    if (TrailMap[x, y] > threshold)
                    {
                        // Start tracing a path
                        List<Point3d> pts = TracePath(x, y, visited, threshold);
                        if (pts.Count > 5) // Minimum length filter
                        {
                            Polyline pl = new Polyline(pts);
                            if (pl.IsValid) paths.Add(pl.ToNurbsCurve());
                        }
                    }
                }
            }
            return paths;
        }

        private List<Point3d> TracePath(int startX, int startY, bool[,] visited, double threshold)
        {
            List<Point3d> path = new List<Point3d>();
            int cx = startX;
            int cy = startY;
            
            // Bidirectional trace? For now just one direction
            // Trace forward
            while (cx >= 0 && cx < Width && cy >= 0 && cy < Height)
            {
                if (visited[cx, cy]) break; // Already visited
                if (TrailMap[cx, cy] < threshold) break;
                
                visited[cx, cy] = true;
                path.Add(GridToWorld(cx, cy));
                
                // Find neighbor with highest value (Gradient ascent)
                double maxVal = -1.0;
                int nextX = -1;
                int nextY = -1;
                
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        int nx = cx + dx;
                        int ny = cy + dy;
                        
                        if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
                        {
                            // Check if visited to avoid loops, but also check value
                            if (!visited[nx, ny] && TrailMap[nx, ny] > maxVal)
                            {
                                maxVal = TrailMap[nx, ny];
                                nextX = nx;
                                nextY = ny;
                            }
                        }
                    }
                }
                
                if (nextX != -1)
                {
                    cx = nextX;
                    cy = nextY;
                }
                else
                {
                    break; // Dead end or local maximum
                }
            }
            return path;
        }

        public List<Point3d> GetAgentPositions()
        {
            List<Point3d> positions = new List<Point3d>();
            foreach (var agent in Agents)
            {
                positions.Add(agent.Position);
            }
            return positions;
        }

        public Mesh GetTrailMesh()
        {
            // Helper to visualize the trail map as a colored mesh
            Mesh mesh = new Mesh();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Point3d pt = GridToWorld(x, y);
                    mesh.Vertices.Add(pt);
                    double val = TrailMap[x, y];
                    int gray = (int)(Math.Min(1.0, val) * 255);
                    mesh.VertexColors.Add(System.Drawing.Color.FromArgb(gray, gray, gray));
                }
            }
            // Add faces... this is expensive for large grids. 
            // Better to use PointCloud or Bitmap for visualization.
            return mesh;
        }

        private double Sense(Agent agent, double angleOffset)
        {
            Vector3d sensorDir = agent.Direction;
            sensorDir.Rotate(angleOffset, Vector3d.ZAxis);
            Point3d sensorPos = agent.Position + sensorDir * SensorDistance;
            
            int gx, gy;
            WorldToGrid(sensorPos, out gx, out gy);
            
            if (gx >= 0 && gx < Width && gy >= 0 && gy < Height)
            {
                return TrailMap[gx, gy];
            }
            return 0.0;
        }

        private void DepositTrail(Point3d pos)
        {
            int gx, gy;
            WorldToGrid(pos, out gx, out gy);
            if (gx >= 0 && gx < Width && gy >= 0 && gy < Height)
            {
                TrailMap[gx, gy] = Math.Min(1.0, TrailMap[gx, gy] + 1.0); // Add pheromone, clamp at 1.0? Or let it accumulate?
                // Let's cap at 5.0 to prevent overflow but allow strong paths
                if (TrailMap[gx, gy] > 5.0) TrailMap[gx, gy] = 5.0;
            }
        }

        private void DiffuseTrailMap()
        {
            // 3x3 Box Blur
            double[,] newMap = new double[Width, Height];
            
            for (int x = 1; x < Width - 1; x++)
            {
                for (int y = 1; y < Height - 1; y++)
                {
                    double sum = 0;
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            sum += TrailMap[x + dx, y + dy];
                        }
                    }
                    double avg = sum / 9.0;
                    // Linear interpolation between original and average based on DiffuseFactor
                    newMap[x, y] = TrailMap[x, y] * (1.0 - DiffuseFactor) + avg * DiffuseFactor;
                }
            }
            TrailMap = newMap;
        }

        private void DecayTrailMap()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    TrailMap[x, y] *= DecayFactor;
                }
            }
        }

        private void WorldToGrid(Point3d pt, out int x, out int y)
        {
            x = (int)((pt.X - Bounds.Min.X) / CellSize);
            y = (int)((pt.Y - Bounds.Min.Y) / CellSize);
        }
        
        private Point3d GridToWorld(int x, int y)
        {
            return new Point3d(
                Bounds.Min.X + x * CellSize,
                Bounds.Min.Y + y * CellSize,
                0);
        }
    }

    public class Agent
    {
        public Point3d Position { get; set; }
        public Vector3d Direction { get; set; }
        
        public void Rotate(double angle)
        {
            Direction.Rotate(angle, Vector3d.ZAxis);
        }
        
        public void Move(double dist)
        {
            Position += Direction * dist;
        }
    }
}
