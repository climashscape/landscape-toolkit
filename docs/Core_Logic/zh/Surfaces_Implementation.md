# 地形与围合曲面实现 (Surfaces & Terrain Implementation)

## 1. 围合地块生成 (Enclosed Plot Generation)
在生成完路网 (Road Network) 之后，路网之间的区域自然形成了封闭的多边形，即“地块” (Plot)。

### 1.1 自动识别 (Automatic Detection)
*   **输入**: 路网边缘线 (Road Boundaries) 构成的闭合环 (Loop)。
*   **算法**:
    1.  **Extract Naked Edges**: 获取路网 Mesh 的裸露边缘。
    2.  **Sort & Join**: 将这些边缘按照连通性排序并连接成闭合多段线 (Polyline Loop)。
    3.  **Validation**: 检查多段线是否闭合、平面投影面积是否大于阈值。

### 1.2 最小曲面与 SubD 边界问题 (Minimal Surface & Boundary Matching)
用户提出了一个关键技术难点：“SubD封面不可控边界与道路维和边界重合怎么解决”。SubD (细分曲面) 算法在细分时通常会导致边缘向内收缩，从而与道路边缘产生缝隙。

#### 解决方案 (Solution)
我们采用以下混合策略来确保 **G0 连续 (位置重合)** 甚至 **G1 连续 (切线连续)**：

1.  **策略 A: 边界顶点锁定 (Boundary Vertex Locking)**
    *   在进行 Kangaroo 或自定义 Relaxation 时，将位于边界的顶点设为 **Anchor (锚点)**，其强度 (Strength) 设为无穷大。
    *   这保证了网格在松弛过程中，边缘点绝对不动。

2.  **策略 B: SubD Crease (折痕)**
    *   在转换为 SubD 之前，将边界边缘标记为 **Crease (权重=无穷大)**。
    *   Rhino 的 SubD 引擎会保持 Crease 边缘不发生收缩，严格贴合原始网格边缘。

3.  **策略 C: 边界高密度化 (Boundary Densification)**
    *   在路网与地形的交界处，生成一圈高密度的“过渡带” (Transition Strip)。
    *   外侧顶点焊接在道路上，内侧顶点参与地形松弛。

### 1.3 极小曲面算法 (Minimal Surface Algorithm - v1.1.0 新增)
`MinimalSurface` 组件实现了一个基于物理的松弛求解器：
*   **初始化 (Initialization)**: 使用约束 Delaunay 三角剖分或 Quad 网格映射在边界内生成粗糙网格。
*   **动力学松弛 (Dynamic Relaxation)**: 顶点被视为由弹簧连接的粒子。系统迭代最小化总能量（模拟肥皂膜表面张力）直至达到平衡。
*   **平均曲率流 (Mean Curvature Flow)**: 该算法有效地使每个内部点的平均曲率最小化 ($H = 0$)。

### 1.4 竖向与坡度控制 (Vertical & Slope Control)
*   **边界高程**: 从路网边缘获取高程 `Z`。
*   **内部调整**:
    1.  **Point Attractor**: 在地块内部放置控制点，提升或降低其 `Z` 值以模拟土丘或洼地。
    2.  **Curve Attractor**: 使用等高线约束内部地形。
    3.  **Slope Analysis Feedback**: 实时计算坡度，若超过设定值（如 25%），自动调整控制点位置或高度。
