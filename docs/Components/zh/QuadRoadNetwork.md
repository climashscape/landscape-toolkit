# Quad Road Network (四边面路网生成器)

**Category:** Landscape > Modeling
**Nickname:** QuadRoad

## 1. 简介 (Introduction)
Quad Road Network 是本工具箱的核心组件之一，用于将二维中心线网络转化为高质量的 **全四边面 (All-Quad)** 网格模型。生成的路网拓扑结构清晰，非常适合后续的 **SubD (细分曲面)** 工作流，能够实现完美的平滑效果，避免传统布尔运算产生的碎面和破面。

## 2. 核心算法 (Core Algorithm)
该组件内置了专门针对景观路网的拓扑生成逻辑：
1.  **图构建 (Graph Building)**: 自动打断相交曲线，构建节点 (Node) 和边 (Edge) 的拓扑关系。
2.  **路口处理 (Junction Meshing)**:
    *   **3-Way (三岔路)**: 生成 Y 型拓扑补丁。
    *   **4-Way (十字路)**: 生成 Grid 型拓扑补丁。
    *   **Dead End (断头路)**: 自动生成方形封口。
    *   **N-Way**: 自适应扇形拓扑。
3.  **路段生成 (Strip Generation)**: 在路口之间生成四边面带状网格，并根据路宽自动细分。
4.  **焊接 (Welding)**: 自动合并所有顶点，输出单一的流形网格 (Manifold Mesh)。

## 3. 输入参数 (Input Parameters)

| 参数名 | 缩写 | 类型 | 默认值 | 说明 |
| :--- | :--- | :--- | :--- | :--- |
| **Centerlines** | C | Curve (List) | 道路中心线网络。支持相交、重叠的曲线（组件会自动处理）。 |
| **Widths** | W | Number (List) | 6.0 | 道路宽度列表。如果是单一数值，则统一应用；如果是列表，需与曲线数量一一对应，支持不同宽度的道路自动连接。 |
| **Fillet** | F | Number | 3.0 | 路口倒角半径（即路口中心到路段起点的距离）。 |

## 4. 输出参数 (Output Parameters)

| 参数名 | 缩写 | 类型 | 说明 |
| :--- | :--- | :--- | :--- |
| **QuadMesh** | M | Mesh | 生成的路网网格。全四边面结构，可直接转换为 SubD。 |
| **Graph** | G | Generic | 内部生成的图结构数据（用于调试或高级分析）。 |
| **Junctions** | J | Mesh (List) | 单独的路口网格列表（便于赋予不同材质）。 |
| **Streets** | S | Mesh (List) | 单独的路段网格列表（便于赋予不同材质）。 |

## 5. 使用建议 (Tips)
*   **SubD 转换**: 生成的 Mesh 建议在 Grasshopper 中连接 `SubD from Mesh` 运算器，以获得最终的光滑路面效果。
*   **曲线质量**: 虽然组件能处理相交，但输入曲线最好预先进行过 `Bio-Path Optimizer` 优化，避免极度锐角（< 30°）的交叉，否则可能导致拓扑重叠。
*   **路网分级**: 利用 `Widths` 参数可以一次性生成主路（如 10m）和次路（如 4m）混合的路网，路口会自动适配不同宽度。
