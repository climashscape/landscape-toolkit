<div class="lang-en">

# Minimal Surface

**Category:** Landscape > Modeling
**Nickname:** MinSurf

## 1. Introduction
The Minimal Surface component generates tensile membrane structures or "minimal surfaces" where the mean curvature is zero. This is ideal for designing fabric roofs, tents, or organic shade structures in landscape architecture.

## 2. Core Algorithm
1.  **Mesh Initialization**: Creates an initial coarse mesh from the input boundary curves.
2.  **Dynamic Relaxation**: Iteratively moves mesh vertices to minimize internal forces (simulating soap film physics).
3.  **Attractor Influence**: Pulls vertices towards internal attractor points to create high/low points (peaks/valleys) in the surface.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Boundary** | B | Curve | - | Closed boundary curve defining the edge of the surface. |
| **Attractors** | A | Point (List) | (Optional) | Internal points that pull the surface up or down (e.g., tent poles). |
| **Iterations** | I | Integer | 50 | Number of relaxation iterations. More iterations result in a smoother, more "relaxed" shape. |
| **Resolution** | R | Number | 1.0 | Target edge length for the mesh. Smaller values yield higher detail. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Mesh** | M | Mesh | The generated minimal surface mesh. |

## 5. Tips
*   **Closed Boundaries**: Ensure the input `Boundary` curve is closed.
*   **Anchor Points**: Attractor points work best when they are significantly above or below the average plane of the boundary curve to create dramatic curvature.


</div>

<div class="lang-zh">

# Minimal Surface (极小曲面)

**Category:** Landscape > Modeling
**Nickname:** MinSurf

## 1. 简介 (Introduction)
Minimal Surface 运算器用于生成张拉膜结构或“极小曲面”（平均曲率为零的曲面）。这非常适合设计景观建筑中的织物屋顶、帐篷或有机形态的遮阳结构。

## 2. 核心算法 (Core Algorithm)
1.  **网格初始化 (Mesh Initialization)**: 根据输入边界曲线创建初始粗糙网格。
2.  **动力学松弛 (Dynamic Relaxation)**: 迭代移动网格顶点以最小化内力（模拟肥皂膜物理特性）。
3.  **吸引子影响 (Attractor Influence)**: 将顶点拉向内部吸引点，在曲面中创造高点或低点（峰/谷）。

## 3. 输入参数 (Input Parameters)

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Boundary** | B | Curve | - | 定义曲面边缘的闭合边界曲线。 |
| **Attractors** | A | Point (List) | (可选) | 将曲面向上或向下拉动的内部点（例如帐篷支柱）。 |
| **Iterations** | I | Integer | 50 | 松弛迭代次数。次数越多，形状越平滑、越“松弛”。 |
| **Resolution** | R | Number | 1.0 | 网格的目标边长。数值越小，细节越高。 |

## 4. 输出参数 (Output Parameters)

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Mesh** | M | Mesh | 生成的极小曲面网格。 |

## 5. 使用技巧 (Tips)
*   **闭合边界**: 请确保输入的 `Boundary` 曲线是闭合的。
*   **锚点位置**: 当吸引点明显高于或低于边界曲线的平均平面时，效果最佳，可以创造出戏剧性的曲率。


</div>
