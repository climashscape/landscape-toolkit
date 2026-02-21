<div class="lang-en">

# Space Syntax Analysis

**Category:** Landscape > Analysis
**Nickname:** SpaceSyntax

## 1. Introduction
Space Syntax Analysis is a tool for analyzing the topological and geometric properties of road networks. It computes standard metrics like Integration and Choice to help understand accessibility and movement potential within the network.

## 2. Core Algorithm
The component follows standard Space Syntax logic:
1.  **Shatter**: Breaks all curves at intersections to create a segment map.
2.  **Natural Roads**: Merges collinear segments into continuous "strokes" (Axial Lines) based on continuity.
3.  **Graph Construction**: Builds an adjacency graph where nodes are natural roads and edges represent intersections.
4.  **Metric Computation**: Uses BFS/Shortest Path algorithms to calculate topological depth and other metrics.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Curves** | C | Curve (List) | - | The road network centerlines. |
| **Metric** | M | Integer | 0 | Analysis Metric: <br>0 = Integration (HH)<br>1 = Choice (Through-movement)<br>2 = Mean Depth<br>3 = Control |
| **Radius** | R | Number | 0.0 | Analysis Radius. 0 = Global (n), >0 = Local metric (e.g., 800m). |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Values** | V | Number (List) | The computed metric values for each segment. |
| **Segments** | S | Curve (List) | The analyzed segments (shattered at intersections). |
| **Colors** | C | Colour (List) | Visualization colors (Heatmap) corresponding to the values. |

## 5. Tips
*   **Integration (Global)**: Useful for identifying the "core" of the settlement or the most accessible streets.
*   **Choice**: Useful for predicting potential movement flows (where people might pass through).
*   **Graph Cleaning**: Ensure your input curves are reasonably clean, though the component handles intersections automatically.


</div>

<div class="lang-zh">

# 空间句法分析 (Space Syntax Analysis)

**Category:** Landscape > Analysis
**Nickname:** SpaceSyntax

## 1. 简介 (Introduction)
空间句法分析组件用于分析路网的拓扑和几何属性。它计算集成度 (Integration) 和穿行度 (Choice) 等标准指标，帮助设计师理解网络中的可达性和潜在人流。

## 2. 核心算法 (Core Algorithm)
该组件遵循标准的空间句法逻辑：
1.  **打断 (Shatter)**: 在所有交点处打断曲线，创建线段图。
2.  **自然道路 (Natural Roads)**: 基于连续性原则，将共线线段合并为连续的“笔画”（轴线）。
3.  **图构建 (Graph Construction)**: 构建邻接图，其中节点代表自然道路，边代表交叉口。
4.  **指标计算 (Metric Computation)**: 使用广度优先搜索 (BFS) 或最短路径算法计算拓扑深度和其他指标。

## 3. 输入参数 (Input Parameters)

| 参数名 | 缩写 | 类型 | 默认值 | 说明 |
| :--- | :--- | :--- | :--- | :--- |
| **Curves** | C | Curve (List) | - | 道路网络中心线。 |
| **Metric** | M | Integer | 0 | 分析指标选择：<br>0 = 集成度 (Integration HH)<br>1 = 穿行度 (Choice)<br>2 = 平均深度 (Mean Depth)<br>3 = 控制值 (Control) |
| **Radius** | R | Number | 0.0 | 分析半径。0 = 全局 (Global n), >0 = 局部指标 (如 800m)。 |

## 4. 输出参数 (Output Parameters)

| 参数名 | 缩写 | 类型 | 说明 |
| :--- | :--- | :--- | :--- |
| **Values** | V | Number (List) | 每个线段计算出的指标数值。 |
| **Segments** | S | Curve (List) | 分析后的线段（在交点处被打断）。 |
| **Colors** | C | Colour (List) | 对应数值的可视化颜色（热力图）。 |

## 5. 使用建议 (Tips)
*   **集成度 (Integration)**: 用于识别聚落的“核心”或最便捷的街道。
*   **穿行度 (Choice)**: 用于预测潜在的通过性人流（人们可能经过的地方）。
*   **数据清洗**: 确保输入曲线尽量干净，尽管组件会自动处理交点。


</div>
