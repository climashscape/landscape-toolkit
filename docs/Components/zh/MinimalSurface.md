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
