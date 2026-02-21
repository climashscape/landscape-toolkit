<div class="lang-en">

# Bio-Path Optimizer

**Category:** Landscape > Optimization
**Nickname:** BioPath

## 1. Introduction
Bio-Path Optimizer is a lightweight curve optimization tool designed to convert rigid hand-drawn polylines or orthogonal paths into natural, streamlined curves. It uses the **Laplacian Smoothing** algorithm to simulate the process of tension relaxation.

## 2. Core Algorithm
The algorithm achieves smoothing by iteratively moving vertices on the curve:
*   For each internal vertex $P_i$ on the curve, calculate the midpoint $P_{target}$ between its predecessor $P_{i-1}$ and successor $P_{i+1}$.
*   Move $P_i$ towards $P_{target}$ by a certain ratio (controlled by `Strength`).
*   Endpoints remain fixed to preserve the path's start and end locations.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Curves** | C | Curve (List) | The list of original curves to optimize. |
| **Iterations** | I | Integer | 10 | Number of iterations. More iterations result in smoother curves that tend towards straight lines. |
| **Strength** | S | Number | 0.5 | Smoothing strength (0.0 - 1.0). 1.0 means moving fully to the midpoint; 0.0 means no movement. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Optimized Curves** | OC | Curve (List) | The optimized smooth curves. |

## 5. Tips
*   Ideal for processing rough paths imported from GIS or quick sketches drawn by designers.
*   If the curves shrink too much, reduce `Iterations` or `Strength`.
*   This component is typically used as a pre-processing step for `Quad Road Network` to ensure the generated road network is fluid and aesthetically pleasing.


## 6. Related Documentation
*   [**Optimization Implementation**](../Core_Logic/Optimization_Implementation.md) - Detailed explanation of the underlying algorithm.


## 6. 相关文档 (Related Documentation)
*   [**Optimization Implementation**](../Core_Logic/Optimization_Implementation.md) - 底层算法的详细解释。


</div>

<div class="lang-zh">

# Bio-Path Optimizer (生物路径平滑器)

**Category:** Landscape > Optimization
**Nickname:** BioPath

## 1. 简介 (Introduction)
Bio-Path Optimizer 是一个轻量级的曲线优化工具，专门用于将生硬的手绘折线或直角路径转化为自然的流线型路径。它采用了 **Laplacian Smoothing (拉普拉斯平滑)** 算法，模拟张力松弛的过程。

## 2. 核心算法 (Core Algorithm)
算法通过迭代移动曲线上的顶点来实现平滑：
*   对于曲线上的每个内部顶点 $P_i$，计算其前一个点 $P_{i-1}$ 和后一个点 $P_{i+1}$ 的中点 $P_{target}$。
*   将 $P_i$ 向 $P_{target}$ 移动一定的比例（由 `Strength` 控制）。
*   端点保持固定，确保路径的起止点不变。

## 3. 输入参数 (Input Parameters)

| 参数名 | 缩写 | 类型 | 默认值 | 说明 |
| :--- | :--- | :--- | :--- | :--- |
| **Curves** | C | Curve (List) | 需要优化的原始曲线列表。 |
| **Iterations** | I | Integer | 10 | 迭代次数。次数越多，曲线越平滑，越趋向于直线。 |
| **Strength** | S | Number | 0.5 | 平滑强度 (0.0 - 1.0)。1.0 表示完全移动到中点，0.0 表示不移动。 |

## 4. 输出参数 (Output Parameters)

| 参数名 | 缩写 | 类型 | 说明 |
| :--- | :--- | :--- | :--- |
| **Optimized Curves** | OC | Curve (List) | 优化后的平滑曲线。 |

## 5. 使用建议 (Tips)
*   适用于处理从 GIS 导入的粗糙路径或设计师快速绘制的草图。
*   如果曲线变得过于收缩，请减少 `Iterations` 或 `Strength`。
*   该组件通常作为 `Quad Road Network` 的前置处理步骤，确保生成的路网流畅美观。


</div>
