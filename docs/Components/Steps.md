<div class="lang-en">

# Landscape Steps

**Category:** Landscape > Modeling
**Nickname:** Steps

## 1. Introduction
The Steps component is used to rapidly generate landscape steps along a specified path. It is not just a simple geometric array but a logic-driven parametric generator that automatically calculates the number of steps based on path length, tread depth, and riser height, generating solid models.

## 2. Core Algorithm
1.  **Path Analysis**: Calculates the length of the input path.
2.  **Parameter Calculation**: Calculates the number of steps based on the set `Tread` depth: $Count = Length / Tread$.
3.  **Section Generation**: Generates rectangular sections for the steps at each division point along the path.
4.  **Solid Construction**: Combines sections into closed Brep solids.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Path** | C | Curve | The center path for the steps (planar or 3D curve). |
| **Width** | W | Number | 2.0 | Total width of the steps. |
| **Tread** | T | Number | 0.3 | Tread depth (Run). Typically 0.3m - 0.45m. |
| **Riser** | R | Number | 0.15 | Riser height (Rise). Typically 0.12m - 0.15m. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Steps** | S | Brep (List) | List of generated step solids. |

## 5. Tips
*   **Slope Integration**: The generated steps are "horizontal". If the path has a slope, the steps will automatically stack along the Z-axis.
*   **Standard Compliance**: When designing, keep in mind the ergonomic formula $2R + T \approx 600-650mm$.


</div>

<div class="lang-zh">

# Landscape Steps (景观台阶)

**Category:** Landscape > Modeling
**Nickname:** Steps

## 1. 简介 (Introduction)
Steps 组件用于沿指定路径快速生成景观台阶。它不仅是简单的几何阵列，而是包含逻辑的参数化生成器，能够根据路径长度、踏步宽度和踢面高度自动计算阶数，生成实体模型。

## 2. 核心算法 (Core Algorithm)
1.  **路径分析**: 计算输入路径的长度。
2.  **参数计算**: 根据设定的 `Tread` (踏面深) 计算台阶数量：$Count = Length / Tread$。
3.  **截面生成**: 在路径的每个分割点生成台阶的矩形截面。
4.  **实体构建**: 将截面组合成闭合的 Brep 实体。

## 3. 输入参数 (Input Parameters)

| 参数名 | 缩写 | 类型 | 默认值 | 说明 |
| :--- | :--- | :--- | :--- | :--- |
| **Path** | C | Curve | 台阶的中心路径（平面或三维曲线）。 |
| **Width** | W | Number | 2.0 | 台阶的总宽度。 |
| **Tread** | T | Number | 0.3 | 踏面深度 (Run)。通常为 0.3m - 0.45m。 |
| **Riser** | R | Number | 0.15 | 踢面高度 (Rise)。通常为 0.12m - 0.15m。 |

## 4. 输出参数 (Output Parameters)

| 参数名 | 缩写 | 类型 | 说明 |
| :--- | :--- | :--- | :--- |
| **Steps** | S | Brep (List) | 生成的台阶实体列表。 |

## 5. 使用建议 (Tips)
*   **坡道结合**: 该组件生成的台阶是“水平”的。如果路径有坡度，台阶会自动沿 Z 轴堆叠。
*   **规范检查**: 设计时请注意 $2R + T \approx 600-650mm$ 的人体工学公式。


</div>
