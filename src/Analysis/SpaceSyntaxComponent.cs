using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace LandscapeToolkit.Analysis
{
    public class SpaceSyntaxComponent : GH_Component
    {
        public SpaceSyntaxComponent()
          : base("Space Syntax Analysis", "SpaceSyntax",
              "Analyze road networks using Space Syntax metrics (Integration, Choice, Depth).\n" +
              "Process Flow:\n" +
              "1. Shatter: Break curves at all intersections to create segment map.\n" +
              "2. Natural Roads: Merge collinear segments into continuous 'strokes' (Axial Lines).\n" +
              "3. Build Graph: Construct adjacency graph of natural roads.\n" +
              "4. Compute: Calculate metric (Integration/Choice) using BFS/Shortest Paths.\n" +
              "5. Visualize: Map values to heatmap colors.\n\n" +
              "处理流程：\n" +
              "1. 打断：在所有交点处打断曲线以创建线段图。\n" +
              "2. 自然道路：将共线线段合并为连续的“笔画”（轴线）。\n" +
              "3. 构建图：构建自然道路的邻接图。\n" +
              "4. 计算：使用广度优先搜索/最短路径计算指标（集成度/穿行度）。\n" +
              "5. 可视化：将数值映射为热力图颜色。",
              "Landscape", "Analysis")
        {
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Curves", "C", "Road network curves (centerlines)", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Metric", "M", "Analysis Metric: 0=Integration(HH), 1=Choice, 2=Mean Depth, 3=Control", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Radius", "R", "Analysis Radius (0 = Global, >0 = Local)", GH_ParamAccess.item, 0.0);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Values", "V", "Computed metric values for each segment", GH_ParamAccess.list);
            pManager.AddCurveParameter("Segments", "S", "Analyzed segments (shattered at intersections)", GH_ParamAccess.list);
            pManager.AddColourParameter("Colors", "C", "Visualization colors based on values", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> inputCurves = new List<Curve>();
            int metricType = 0;
            double radius = 0.0;

            if (!DA.GetDataList(0, inputCurves)) return;
            DA.GetData(1, ref metricType);
            DA.GetData(2, ref radius);

            // 1. Preprocess: Shatter curves at intersections to create a segment map
            List<Curve> rawSegments = ShatterCurves(inputCurves);
            if (rawSegments.Count == 0) return;

            // 1b. Merge collinear segments into "Natural Roads" (Strokes) for better topological analysis
            // This converts the Segment Map into something closer to an Axial Map
            List<Curve> segments = CreateNaturalRoads(rawSegments);

            // 2. Build Graph (Adjacency List)
            // Node i corresponds to segments[i]
            List<List<int>> adjacency = BuildAdjacencyGraph(segments);

            // 3. Compute Metrics
            double[] values = new double[segments.Count];

            switch (metricType)
            {
                case 0: // Integration (HH)
                    values = ComputeIntegration(adjacency, segments.Count, radius);
                    break;
                case 1: // Choice
                    values = ComputeChoice(adjacency, segments.Count, radius);
                    break;
                case 2: // Mean Depth
                    values = ComputeMeanDepth(adjacency, segments.Count, radius, out int[] _);
                    break;
                case 3: // Control
                    values = ComputeControl(adjacency, segments.Count);
                    break;
            }

            // 4. Color Mapping
            List<Color> colors = GenerateColors(values);

            DA.SetDataList(0, values);
            DA.SetDataList(1, segments);
            DA.SetDataList(2, colors);
        }

        private List<Curve> ShatterCurves(List<Curve> curves)
        {
            // Simple approach: intersect all curves and shatter them
            // For efficiency, we flatten to XY plane for intersection checks if 2D logic is desired,
            // but Space Syntax can be 3D. Assuming planar graph for road networks usually.
            // Let's stick to 3D intersection with tolerance.

            List<Curve> shattered = new List<Curve>();
            List<Curve> validCurves = curves.Where(c => c != null && c.IsValid).ToList();

            // Intersect all curves
            var intersections = new Dictionary<int, List<double>>();

            for (int i = 0; i < validCurves.Count; i++)
            {
                if (!intersections.ContainsKey(i)) intersections[i] = new List<double>();

                for (int j = i + 1; j < validCurves.Count; j++)
                {
                    var ev = Intersection.CurveCurve(validCurves[i], validCurves[j], 0.01, 0.01);
                    if (ev != null)
                    {
                        foreach (var e in ev)
                        {
                            if (e.IsPoint)
                            {
                                intersections[i].Add(e.ParameterA);
                                if (!intersections.ContainsKey(j)) intersections[j] = new List<double>();
                                intersections[j].Add(e.ParameterB);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < validCurves.Count; i++)
            {
                if (intersections.ContainsKey(i) && intersections[i].Count > 0)
                {
                    var tParams = intersections[i].Distinct().OrderBy(t => t).ToList();
                    // Filter params close to start/end
                    var validParams = tParams.Where(t => t > 1e-4 && t < validCurves[i].Domain.Max - 1e-4).ToList();

                    if (validParams.Count > 0)
                    {
                        var pieces = validCurves[i].Split(validParams);
                        shattered.AddRange(pieces);
                    }
                    else
                    {
                        shattered.Add(validCurves[i]);
                    }
                }
                else
                {
                    shattered.Add(validCurves[i]);
                }
            }

            // Remove very short segments
            return shattered.Where(c => c.GetLength() > 0.1).ToList();
        }

        private List<Curve> CreateNaturalRoads(List<Curve> segments)
        {
            int n = segments.Count;
            if (n == 0) return new List<Curve>();

            List<List<int>> adj = BuildAdjacencyGraph(segments);
            bool[] visited = new bool[n];
            List<Curve> naturalRoads = new List<Curve>();

            for (int i = 0; i < n; i++)
            {
                if (visited[i]) continue;

                LinkedList<int> chain = new LinkedList<int>();
                chain.AddFirst(i);
                visited[i] = true;

                // Extend Forward
                int current = i;
                bool currentRev = false; 

                while (true)
                {
                    Point3d tip = currentRev ? segments[current].PointAtStart : segments[current].PointAtEnd;
                    Vector3d dir = currentRev ? -segments[current].TangentAtStart : segments[current].TangentAtEnd;
                    
                    int bestNext = -1;
                    double bestDot = -1.0;
                    bool nextRev = false;

                    foreach (int neighbor in adj[current])
                    {
                        if (visited[neighbor]) continue;

                        Point3d nStart = segments[neighbor].PointAtStart;
                        Point3d nEnd = segments[neighbor].PointAtEnd;
                        
                        bool matchStart = tip.DistanceToSquared(nStart) < 0.0001;
                        bool matchEnd = tip.DistanceToSquared(nEnd) < 0.0001;

                        if (!matchStart && !matchEnd) continue;

                        Vector3d effectiveNDir = matchStart ? segments[neighbor].TangentAtStart : -segments[neighbor].TangentAtEnd;
                        
                        double dot = dir * effectiveNDir;
                        if (dot > bestDot && dot > 0.707) // 45 deg
                        {
                            bestDot = dot;
                            bestNext = neighbor;
                            nextRev = !matchStart; 
                        }
                    }

                    if (bestNext != -1)
                    {
                        visited[bestNext] = true;
                        chain.AddLast(bestNext);
                        current = bestNext;
                        currentRev = nextRev;
                    }
                    else
                    {
                        break;
                    }
                }

                // Extend Backward
                current = i;
                currentRev = false; 
                
                while (true)
                {
                    Point3d tip = currentRev ? segments[current].PointAtEnd : segments[current].PointAtStart;
                    Vector3d dir = currentRev ? segments[current].TangentAtEnd : -segments[current].TangentAtStart; 
                    
                    int bestNext = -1;
                    double bestDot = -1.0;
                    bool nextRev = false;

                    foreach (int neighbor in adj[current])
                    {
                        if (visited[neighbor]) continue;

                        Point3d nStart = segments[neighbor].PointAtStart;
                        Point3d nEnd = segments[neighbor].PointAtEnd;
                        
                        bool matchStart = tip.DistanceToSquared(nStart) < 0.0001;
                        bool matchEnd = tip.DistanceToSquared(nEnd) < 0.0001;

                        if (!matchStart && !matchEnd) continue;

                        Vector3d effectiveNDir = matchStart ? segments[neighbor].TangentAtStart : -segments[neighbor].TangentAtEnd;
                        
                        double dot = dir * effectiveNDir;
                        if (dot > bestDot && dot > 0.707)
                        {
                            bestDot = dot;
                            bestNext = neighbor;
                            nextRev = !matchStart;
                        }
                    }

                    if (bestNext != -1)
                    {
                        visited[bestNext] = true;
                        chain.AddFirst(bestNext);
                        current = bestNext;
                        currentRev = nextRev;
                    }
                    else
                    {
                        break;
                    }
                }
                
                List<Curve> chainCurves = new List<Curve>();
                foreach(int idx in chain) chainCurves.Add(segments[idx]);
                
                if (chainCurves.Count > 1)
                {
                    Curve[] joined = Curve.JoinCurves(chainCurves, 0.01, false);
                    if (joined != null) naturalRoads.AddRange(joined);
                }
                else
                {
                    naturalRoads.Add(segments[i]);
                }
            }

            return naturalRoads;
        }

        private List<List<int>> BuildAdjacencyGraph(List<Curve> segments)
        {
            int n = segments.Count;
            List<List<int>> adj = new List<List<int>>(n);
            for (int i = 0; i < n; i++) adj.Add(new List<int>());

            // Use RTree or bounding box check for speed
            // Since N might be small (<1000), O(N^2) endpoint check is okay.
            // For larger N, RTree is better. Let's use simple endpoint distance check.

            double tol = 0.01;
            double tolSq = tol * tol;

            // Optimization: Grid-based or just check endpoints
            // Endpoints: Start and End
            Point3d[] starts = new Point3d[n];
            Point3d[] ends = new Point3d[n];
            for(int i=0; i<n; i++)
            {
                starts[i] = segments[i].PointAtStart;
                ends[i] = segments[i].PointAtEnd;
            }

            Parallel.For(0, n, i =>
            {
                for (int j = i + 1; j < n; j++)
                {
                    bool connected = false;
                    if (starts[i].DistanceToSquared(starts[j]) < tolSq) connected = true;
                    else if (starts[i].DistanceToSquared(ends[j]) < tolSq) connected = true;
                    else if (ends[i].DistanceToSquared(starts[j]) < tolSq) connected = true;
                    else if (ends[i].DistanceToSquared(ends[j]) < tolSq) connected = true;

                    if (connected)
                    {
                        lock (adj)
                        {
                            adj[i].Add(j);
                            adj[j].Add(i);
                        }
                    }
                }
            });

            return adj;
        }

        private double[] ComputeMeanDepth(List<List<int>> adj, int n, double radius, out int[] nodeCounts)
        {
            double[] meanDepth = new double[n];
            int[] counts = new int[n];
            
            Parallel.For(0, n, i =>
            {
                var result = BFS_MeanDepth(adj, i, n, radius);
                meanDepth[i] = result.Item1;
                counts[i] = result.Item2;
            });

            nodeCounts = counts;
            return meanDepth;
        }

        private Tuple<double, int> BFS_MeanDepth(List<List<int>> adj, int startNode, int n, double radius)
        {
            Queue<int> q = new Queue<int>();
            q.Enqueue(startNode);
            
            Dictionary<int, int> dist = new Dictionary<int, int>();
            dist[startNode] = 0;

            long totalDepth = 0;
            int count = 0;

            while (q.Count > 0)
            {
                int u = q.Dequeue();
                int d = dist[u];

                if (radius > 0 && d > radius) continue;

                if (u != startNode)
                {
                    totalDepth += d;
                    count++;
                }

                foreach (int v in adj[u])
                {
                    if (!dist.ContainsKey(v))
                    {
                        dist[v] = d + 1;
                        q.Enqueue(v);
                    }
                }
            }

            if (count == 0) return new Tuple<double, int>(0, 0);
            return new Tuple<double, int>((double)totalDepth / count, count + 1); // +1 to include self in k
        }

        private double[] ComputeIntegration(List<List<int>> adj, int n, double radius)
        {
            int[] nodeCounts;
            double[] md = ComputeMeanDepth(adj, n, radius, out nodeCounts);
            double[] integration = new double[n];

            for (int i = 0; i < n; i++)
            {
                if (md[i] <= 1.0) 
                {
                    integration[i] = 0; 
                    continue;
                }

                double k = (double)nodeCounts[i];
                if (k < 3) 
                {
                    integration[i] = 0;
                    continue;
                }

                // RA calculation
                double ra = 2.0 * (md[i] - 1.0) / (k - 2.0);
                
                // D-value calculation for RRA
                // D_k = 2 * (k * (log2((k + 2) / 3) - 1) + 1) / ((k - 1) * (k - 2))
                double log2term = Math.Log((k + 2.0) / 3.0, 2.0);
                double d_k = 2.0 * (k * (log2term - 1.0) + 1.0) / ((k - 1.0) * (k - 2.0));

                double rra = ra / d_k;

                if (rra > 1e-9)
                    integration[i] = 1.0 / rra;
                else
                    integration[i] = 10.0; // Max cap
            }

            return integration;
        }

        private double[] ComputeChoice(List<List<int>> adj, int n, double radius)
        {
            double[] choice = new double[n];
            object lockObj = new object();

            Parallel.For(0, n, 
                () => new double[n], // Local state init
                (s, loop, localChoice) => // Body
                {
                    Stack<int> S = new Stack<int>();
                    List<List<int>> P = new List<List<int>>(n);
                    for(int k=0; k<n; k++) P.Add(new List<int>());
                    
                    double[] sigma = new double[n];
                    int[] d = new int[n];
                    for(int k=0; k<n; k++) { d[k] = -1; sigma[k] = 0; }

                    sigma[s] = 1;
                    d[s] = 0;
                    Queue<int> Q = new Queue<int>();
                    Q.Enqueue(s);

                    while (Q.Count > 0)
                    {
                        int v = Q.Dequeue();
                        S.Push(v);

                        if (radius > 0 && d[v] >= radius) continue;

                        foreach (int w in adj[v])
                        {
                            if (d[w] < 0)
                            {
                                Q.Enqueue(w);
                                d[w] = d[v] + 1;
                            }
                            if (d[w] == d[v] + 1)
                            {
                                sigma[w] += sigma[v];
                                P[w].Add(v);
                            }
                        }
                    }

                    double[] delta = new double[n];
                    while (S.Count > 0)
                    {
                        int w = S.Pop();
                        foreach (int v in P[w])
                        {
                            delta[v] += (sigma[v] / sigma[w]) * (1 + delta[w]);
                        }
                        if (w != s)
                        {
                            localChoice[w] += delta[w];
                        }
                    }
                    return localChoice;
                },
                (localChoice) => // Finalizer
                {
                    lock (lockObj)
                    {
                        for(int i=0; i<n; i++) choice[i] += localChoice[i];
                    }
                }
            );

            // Log choice to compress range
            for(int i=0; i<n; i++)
            {
                if (choice[i] > 0) choice[i] = Math.Log10(choice[i] + 1);
            }

            return choice;
        }

        private double[] ComputeControl(List<List<int>> adj, int n)
        {
            double[] control = new double[n];
            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                foreach (int neighbor in adj[i])
                {
                    int degree = adj[neighbor].Count;
                    if (degree > 0) sum += 1.0 / degree;
                }
                control[i] = sum;
            }
            return control;
        }

        private List<Color> GenerateColors(double[] values)
        {
            if (values.Length == 0) return new List<Color>();

            double min = values.Min();
            double max = values.Max();
            double range = max - min;

            List<Color> colors = new List<Color>();

            foreach (double v in values)
            {
                double t = (range > 1e-9) ? (v - min) / range : 0.5;
                // Heatmap: Blue (0) -> Green -> Red (1)
                colors.Add(GetHeatMapColor(t));
            }

            return colors;
        }

        private Color GetHeatMapColor(double value)
        {
            // Simple Blue-Cyan-Green-Yellow-Red gradient
            // 0.0 -> 0.25 -> 0.5 -> 0.75 -> 1.0
            byte r = 0, g = 0, b = 0;

            if (value < 0.25)
            {
                // Blue to Cyan
                b = 255;
                g = (byte)(value * 4 * 255);
            }
            else if (value < 0.5)
            {
                // Cyan to Green
                g = 255;
                b = (byte)((0.5 - value) * 4 * 255);
            }
            else if (value < 0.75)
            {
                // Green to Yellow
                g = 255;
                r = (byte)((value - 0.5) * 4 * 255);
            }
            else
            {
                // Yellow to Red
                r = 255;
                g = (byte)((1.0 - value) * 4 * 255);
            }

            return Color.FromArgb(r, g, b);
        }

        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("98765432-1234-5678-90ab-cdef12345678");
    }
}
