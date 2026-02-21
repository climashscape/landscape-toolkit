<div class="lang-en">

# Slope Analysis

**Category:** Landscape > Analysis
**Nickname:** Slope

## 1. Introduction
Slope Analysis is a real-time terrain analysis tool used to visualize slope variations on a mesh surface. It uses color gradients to intuitively display terrain steepness, helping designers assess site drainage, accessibility, and earthwork stability.

## 2. Core Algorithm
1.  **Normal Calculation**: Calculates the normal vector for each mesh vertex.
2.  **Angle Calculation**: Computes the angle between the normal vector and the World Z-axis (0,0,1).
    *   Flat ground (Normal Up): Angle = 0°
    *   Vertical wall (Normal Horizontal): Angle = 90°
3.  **Color Mapping**: Maps the calculated angle values to a specified color gradient (Green -> Yellow -> Red).

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Mesh** | M | Mesh | The terrain mesh to analyze. |
| **Range** | R | Interval | 0 To 45 | The slope range (degrees) for color mapping. Values below min display as start color (Green), above max as end color (Red). |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Colored Mesh** | CM | Mesh | The analysis mesh with vertex colors. Viewable directly in Rhino viewport (Rendered or Shaded mode required). |

## 5. Tips
*   **Common Thresholds**:
    *   **0-5% (~0-3°)**: Suitable for plazas and activity areas.
    *   **5-8% (~3-5°)**: Maximum limit for accessible ramps.
    *   **< 25% (~14°)**: Limit for lawn mowing.
    *   **< 33% (~18°)**: Natural angle of repose, no retaining wall needed.
*   **Visualization**: It is recommended to connect a `Legend` component in Grasshopper to display the color key.


</div>

<div class="lang-zh">

# Slope Analysis (坡度分析)

**Category:** Landscape > Analysis
**Nickname:** Slope

## 1. 简介 (Introduction)
Slope Analysis 是一个实时的地形分析工具，用于可视化网格表面的坡度变化。它通过颜色梯度直观地展示地形的陡峭程度，帮助设计师判断场地排水、可达性以及土方稳定性。

## 2. 核心算法 (Core Algorithm)
1.  **法线计算**: 计算网格每个顶点的法向量 (Normal Vector)。
2.  **角度计算**: 计算法向量与世界 Z 轴 (0,0,1) 之间的夹角。
    *   平地 (法线向上): 夹角 = 0°
    *   垂直墙面 (法线水平): 夹角 = 90°
3.  **颜色映射**: 将计算出的角度值映射到指定的颜色渐变（绿 -> 黄 -> 红）。

## 3. 输入参数 (Input Parameters)

| 参数名 | 缩写 | 类型 | 默认值 | 说明 |
| :--- | :--- | :--- | :--- | :--- |
| **Mesh** | M | Mesh | 需要分析的地形网格。 |
| **Range** | R | Interval | 0 To 45 | 颜色映射的坡度范围（度）。小于最小值的显示为起始色（绿），大于最大值的显示为终止色（红）。 |

## 4. 输出参数 (Output Parameters)

| 参数名 | 缩写 | 类型 | 说明 |
| :--- | :--- | :--- | :--- |
| **Colored Mesh** | CM | Mesh | 带有顶点颜色的分析网格。可在 Rhino 视口中直接查看（需开启“渲染”或“着色”模式）。 |

## 5. 使用建议 (Tips)
*   **常用阈值**:
    *   **0-5% (约0-3°)**: 适宜广场、活动场地。
    *   **5-8% (约3-5°)**: 极限无障碍坡道。
    *   **< 25% (约14°)**: 草坪修剪极限。
    *   **< 33% (约18°)**: 自然安息角，无需挡土墙。
*   **可视化**: 建议在 Grasshopper 中连接 `Legend` 组件以显示图例。


</div>
