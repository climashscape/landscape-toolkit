# 道路生成与四边面拓扑实现 (Road Network Implementation)

## 1. 核心算法思路 (Core Algorithm Logic)
从平面线稿（Centerlines）到三维四边面路网（Quad Mesh Road Network）的生成过程是整个工具的核心。我们需要解决的关键问题是：**如何保证拓扑结构全是四边形（Quads）以便于后续的 SubD 细分，同时处理任意角度的路口连接。**

### 1.1 输入处理 (Input Processing)
*   **输入**: Rhino Curve (Polyline or Nurbs Curve).
*   **预处理**:
    1.  **节点识别 (Node Identification)**: 检测曲线的所有交点 (Intersections)，将曲线打断为路段 (Edges)。
    2.  **图结构构建 (Graph Construction)**: 建立 `Graph` 数据结构，其中交点为 `Node`，路段为 `Edge`。
    3.  **度数分析 (Degree Analysis)**: 计算每个节点的连接路段数量 (Valence 3, 4, >4)。

### 1.2 偏移与轮廓生成 (Offset & Outline)
对于每一条路段 (Edge):
1.  根据道路等级 (Road Hierarchy) 设定宽度 `W`。
2.  向两侧偏移 `W/2`，得到两条边线 (Left/Right Border)。
3.  **关键步骤**: 在路口处，不直接倒角，而是计算相邻路段边线的交点，保留原始的“尖角”结构，留待后续处理。

### 1.3 路口处理策略 (Junction Strategy) - 核心难点
这是生成四边面网格最复杂的部分。我们需要根据路口的度数 (Valence) 采用不同的拓扑模板：

#### A. 三岔路口 (3-Way Junction)
*   **拓扑结构**: 类似于“Y”型。
*   **网格化**:
    1.  找到三条道路中心线的交点 `C`。
    2.  连接路口转角点，形成一个内部三角形。
    3.  **四边化技巧**: 将三角形中心添加一个顶点，连接至三边中点，将其分割为 **3个四边形**。
    4.  或者使用“旋转流”拓扑，使得车流方向的网格流线顺畅。

#### B. 十字路口 (4-Way Junction)
*   **拓扑结构**: 标准“+”型。
*   **网格化**:
    1.  直接连接四个转角点，形成一个大的四边形区域。
    2.  如果路口很大，可以进一步细分为 $2 \times 2$ 或 $3 \times 3$ 的网格。

#### C. 多岔路口 (>4-Way Junction)
*   **通用解法**:
    1.  生成一个多边形 (N-gon) 区域。
    2.  在中心添加一个点 `Center`。
    3.  连接 `Center` 到多边形的每个顶点，形成 $N$ 个三角形。
    4.  再次细分，将每个三角形转换为四边形（Catmull-Clark 细分思想）。

### 1.4 网格生成与平滑 (Meshing & Smoothing)
1.  **Strip Generation**: 沿着路段方向，连接左右边线的对应点，生成规则的四边形带 (Quad Strips)。
2.  **Weld**: 将路段网格与路口网格的顶点进行焊接 (Weld)。
3.  **Relaxation (松弛)**:
    *   使用拉普拉斯平滑 (Laplacian Smoothing) 或弹簧质点模型 (Spring System)。
    *   **约束**: 保持路边缘点不动（或仅在切向移动），只松弛内部点，确保路网流畅。

### 1.5 竖向拟合 (Vertical Fitting)
*   **投影**: 将生成的 2D 网格投影到地形 (Terrain) 上。
*   **平滑**: 道路不应完全贴合地形的微小起伏。
*   **算法**:
    1.  获取地形高程 `Z`。
    2.  对道路中心线进行“最小二乘法”拟合或 B-Spline 平滑，去除高频噪点。
    3.  重新计算网格点的 `Z` 值，确保道路纵坡 (Longitudinal Slope) 符合规范（例如 < 8%）。

---

## 2. 道路分级与图层管理 (Hierarchy & Layer Management)
针对用户提出的“不同分级的路不用材质的路的分类怎么衔接怎么输出为不同图层”，我们采用以下策略：

### 2.1 属性定义 (Attribute Definition)
每个 `RoadEdge` 对象携带以下属性：
*   **Level**: 道路等级 (e.g., 0=主干道, 1=次干道, 2=游步道)。
*   **Width**: 对应宽度。
*   **MaterialID**: 材质编号。

### 2.2 衔接处理 (Connection Logic)
当不同等级道路相交时（如主路与小径）：
1.  **优先权**: 高等级道路（主路）的网格结构保持完整。
2.  **打断**: 低等级道路在交界处被打断。
3.  **过渡网格 (Transition Mesh)**: 在接口处生成特殊的扇形或梯形过渡网格，确保从宽路到窄路的拓扑连续性，避免出现 T-Junctions (T型裂缝)。

### 2.3 图层输出 (Layer Output)
在生成最终 Rhino 几何体时：
*   根据 `Level` 或 `MaterialID` 自动将 Mesh Bake 到对应的图层 (e.g., "Roads::Primary", "Roads::Secondary")。
*   **User Data**: 将属性作为 User Text 附着在 Mesh 上，便于后续分析或 BIM 流程。

---

## 3. 仿生微调算法 (Bio-Mimetic / Wooly Path)
用户提到的“羊毛算法”或“粘菌算法”主要用于路径优化。

*   **原理**: 模拟羊毛线被拉扯时的自然弯曲，或粘菌寻找食物的最短路径。
*   **实现**:
    1.  **Agent-Based**: 释放大量智能体 (Agents) 在起点和终点之间行走。
    2.  **Trail Formation**: 智能体走过的地方留下“信息素” (Pheromone)。
    3.  **Path Extraction**: 提取信息素浓度最高的路径。
    4.  **Curve Optimization**: 对提取的路径进行平滑，使其既符合最短路径原则，又具有有机的弯曲美感。

---

## 4. 代码结构参考 (Code Reference)
在 `src/Modeling/Roads/RoadNetworkComponent.cs` 中，我们将实现主要的类：

```csharp
public class RoadNetwork 
{
    public List<Curve> Centerlines { get; set; }
    public double Width { get; set; }
    
    // 生成网格的方法
    public Mesh GenerateQuadMesh() 
    {
        // 1. Build Graph
        var graph = BuildGraph(Centerlines);
        
        // 2. Generate Junctions
        foreach(var node in graph.Nodes) {
            GenerateJunctionMesh(node);
        }
        
        // 3. Generate Streets
        foreach(var edge in graph.Edges) {
            GenerateStreetMesh(edge);
        }
        
        // 4. Weld & Relax
        return FinalizeMesh();
    }
}
```
