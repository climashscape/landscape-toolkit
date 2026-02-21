<div class="lang-en">

# Scatter Elements

**Category:** Landscape > Modeling
**Nickname:** Scatter

## 1. Introduction
The Scatter component provides a rule-based system for distributing landscape elements such as trees, street lights, and benches. Instead of pure randomization, it respects constraints like minimum distance, road edges, and surface boundaries.

## 2. Core Algorithm
1.  **Surface Sampling**: Generates candidate points on the target surface.
2.  **Distance Culling**: Removes points that are too close to each other based on the `MinDistance` parameter (Poisson-disk sampling logic).
3.  **Edge Alignment**: (Optional) Aligns elements like street lights along specified road edges with regular spacing.
4.  **Type Filtering**: Applies specific rules based on the element type (e.g., benches face the path, trees avoid road centers).

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **TargetSurface** | S | Mesh | - | The terrain or plot mesh where elements will be placed. |
| **RoadEdges** | E | Curve (List) | (Optional) | Curves defining road boundaries for aligning lights or benches. |
| **Type** | T | Integer | 0 | Element type: 0 = Tree, 1 = Light, 2 = Bench. |
| **MinDistance** | D | Number | 5.0 | Minimum distance (radius) between elements to prevent overcrowding. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Points** | P | Point (List) | The generated insertion points for the landscape elements. |

## 5. Tips
*   **Context Aware**: Use `RoadEdges` when placing street lights to ensure they follow the road network.
*   **Density Control**: Adjust `MinDistance` to control the overall density of the scattered elements (e.g., larger distance for big trees, smaller for shrubs).


</div>

<div class="lang-zh">

# Scatter Elements (配景散布)

**Category:** Landscape > Modeling
**Nickname:** Scatter

## 1. 简介 (Introduction)
Scatter 运算器提供了一套基于规则的景观元素散布系统，用于布置乔木、路灯和座椅等配景。不同于纯随机分布，它遵循最小间距、道路边缘对齐和表面边界等约束条件。

## 2. 核心算法 (Core Algorithm)
1.  **表面采样 (Surface Sampling)**: 在目标表面上生成候选点。
2.  **距离剔除 (Distance Culling)**: 基于 `MinDistance` 参数移除过于靠近的点（泊松盘采样逻辑）。
3.  **边缘对齐 (Edge Alignment)**: (可选) 沿指定的道路边缘按规律间隔排列元素（如路灯）。
4.  **类型过滤 (Type Filtering)**: 根据元素类型应用特定规则（例如：座椅面向路径，乔木避开道路中心）。

## 3. 输入参数 (Input Parameters)

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **TargetSurface** | S | Mesh | - | 放置元素的地形或地块网格。 |
| **RoadEdges** | E | Curve (List) | (可选) | 用于对齐路灯或座椅的道路边界曲线。 |
| **Type** | T | Integer | 0 | 元素类型：0 = 乔木 (Tree), 1 = 路灯 (Light), 2 = 座椅 (Bench)。 |
| **MinDistance** | D | Number | 5.0 | 元素之间的最小间距（半径），用于防止过度拥挤。 |

## 4. 输出参数 (Output Parameters)

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Points** | P | Point (List) | 生成的景观元素插入点。 |

## 5. 使用技巧 (Tips)
*   **环境感知**: 布置路灯时请连接 `RoadEdges`，以确保路灯沿路网排列。
*   **密度控制**: 调整 `MinDistance` 可控制散布元素的整体密度（例如：大树使用较大间距，灌木使用较小间距）。


</div>
