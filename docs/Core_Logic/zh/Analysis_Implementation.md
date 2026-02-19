# 环境分析与优化实现 (Analysis & Optimization Implementation)

## 1. 核心分析模块 (Core Analysis Modules)
在“设计-分析-优化”闭环中，分析工具不仅要给出结果，还要提供**实时反馈** (Real-Time Feedback) 供设计师调整模型。

### 1.1 物理环境模拟 (Physics Simulation)
*   **微气候 (Microclimate)**:
    *   **风环境 (CFD)**: 接入 OpenFOAM 或 Butterfly 接口，模拟场地风速与风压，评估通风廊道。
    *   **热舒适度 (UTCI)**: 结合 Ladybug Tools，计算室外热舒适度指数，优化树木遮阴布局。
    *   **光环境**: 计算全年日照时数 (Solar Hours) 与辐射量 (Radiation)，指导种植与活动场地布局。

### 1.2 坡度与土方 (Slope & Earthwork)
*   **坡度分析**:
    *   **算法**: 计算 Mesh 面法线与 Z 轴夹角。
    *   **分级显示**: 平地 (<2%)、缓坡 (2-8%)、中坡 (8-25%)、陡坡 (>25%)。
*   **土方平衡 (Cut & Fill)**:
    *   **算法**: 采样对比设计地形与原始地形的高程差。
    *   **目标**: 追求 $V_{fill} \approx V_{cut}$，减少外运土方成本。

### 1.3 水文与生态 (Hydrology & Ecology)
*   **径流分析**:
    *   **算法**: 基于 D8 或 D∞ 算法模拟雨水在地表的流动路径 (Flow Path)。
    *   **汇水区 (Catchment)**: 识别低洼积水区，指导雨水花园 (Rain Garden) 选址。
*   **碳汇估算 (Carbon Sequestration)**:
    *   **数据库**: 内置常见乔木、灌木的固碳因子库。
    *   **计算**: $Total\_Carbon = \sum (Plant\_Age \times Sequestration\_Rate)$。

### 1.4 GIS 与大数据集成 (GIS Integration) - *Future*
*   **数据源**: 接入 OpenStreetMap (OSM) 或本地 Shapefile (.shp)。
*   **功能**:
    *   自动生成周边建筑白模。
    *   基于地形数据 (DEM) 自动生成原始地形 Mesh。
    *   利用城市路网数据作为生成的起始中心线。

---

## 2. 优化算法 (Optimization Algorithms)
基于分析结果自动调整设计参数。

### 2.1 路径优化 (Path Optimization)
*   **目标**: 寻找连接起点 A 和终点 B 的路径，使得总坡度最小且路径最短。
*   **算法**: **A* (A-Star)** 或 **Dijkstra**。
    *   **Cost Function**: $Cost = Length + w \times Slope$。
    *   如果在陡坡区域，代价极大，算法会自动绕行。

### 2.2 多目标遗传算法 (Multi-Objective Genetic Algorithm)
*   **应用场景**: 自动寻找最佳的路网密度与地形起伏组合。
*   **变量**: 道路间距、地形控制点高度。
*   **目标函数**:
    1.  Minimize Earthwork (土方最小)。
    2.  Maximize Solar Access (日照最大)。
    3.  Minimize Path Length (路径最短)。
*   **工具**: 集成 `Galapagos` 或 `Octopus` (Grasshopper 插件) 的逻辑。

---

## 3. 代码结构参考 (Code Reference)
在 `src/Analysis/SlopeAnalysisComponent.cs` 和 `src/Integration/GISConnector.cs` (Future) 中：

```csharp
public class SlopeAnalysis 
{
    public Mesh Terrain { get; set; }
    
    public List<double> ComputeSlope() 
    {
        var slopes = new List<double>();
        foreach (var normal in Terrain.FaceNormals) {
            double angle = Vector3d.VectorAngle(normal, Vector3d.ZAxis);
            double slope = Math.Tan(angle);
            slopes.Add(slope);
        }
        return slopes;
    }
}
```
