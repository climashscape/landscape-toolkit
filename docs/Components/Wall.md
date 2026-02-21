<div class="lang-en">

# Landscape Wall

**Category:** Landscape > Modeling
**Nickname:** Wall

## 1. Introduction
The Wall component is used to rapidly generate landscape walls, retaining walls, or planter edges with thickness. Compared to a simple Extrude operation, it supports offset thickness along the curve normal and handles closed profiles to generate solid Breps.

## 2. Core Algorithm
1.  **Bidirectional Offset**: Offsets the base curve to both left and right sides by `Thickness / 2`.
2.  **Profile Closing**: Connects the endpoints of the offset curves to form a closed planar profile.
3.  **Sweep/Extrude**: Extrudes the closed profile along the Z-axis by the specified height to generate a solid.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Curve** | C | Curve | The base centerline of the wall. |
| **Height** | H | Number | 1.0 | Height of the wall. |
| **Thickness** | T | Number | 0.2 | Thickness of the wall. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Wall** | W | Brep | The generated wall solid. |

## 5. Tips
*   **Complex Curves**: For curves with extreme curvature or self-intersections, the offset operation may fail or produce artifacts. It is recommended to optimize curves first.
*   **Top Surface**: Currently generates flat-topped walls. Future versions will support top surfaces that follow terrain undulations.


</div>

<div class="lang-zh">

# Landscape Wall (景观挡墙)

**Category:** Landscape > Modeling
**Nickname:** Wall

## 1. 简介 (Introduction)
Wall 组件用于快速生成具有厚度的景观墙体、挡土墙或种植池边缘。相比简单的 Extrude 操作，它支持沿曲线法向偏移生成厚度，并处理闭合轮廓，生成实体 Brep。

## 2. 核心算法 (Core Algorithm)
1.  **双向偏移**: 将基础曲线向左右两侧各偏移 `Thickness / 2`。
2.  **轮廓闭合**: 连接偏移后的曲线端点，形成闭合的平面轮廓。
3.  **扫描/拉伸**: 将闭合轮廓沿 Z 轴拉伸指定高度，生成实体。

## 3. 输入参数 (Input Parameters)

| 参数名 | 缩写 | 类型 | 默认值 | 说明 |
| :--- | :--- | :--- | :--- | :--- |
| **Curve** | C | Curve | 墙体的基准中心线。 |
| **Height** | H | Number | 1.0 | 墙体高度。 |
| **Thickness** | T | Number | 0.2 | 墙体厚度。 |

## 4. 输出参数 (Output Parameters)

| 参数名 | 缩写 | 类型 | 说明 |
| :--- | :--- | :--- | :--- |
| **Wall** | W | Brep | 生成的墙体实体。 |

## 5. 使用建议 (Tips)
*   **复杂曲线**: 对于曲率极大或自交的曲线，偏移操作可能会失败或产生奇怪结果。建议先优化曲线。
*   **顶部处理**: 目前生成的是平顶墙。未来版本将支持顶面跟随地形起伏。


</div>
