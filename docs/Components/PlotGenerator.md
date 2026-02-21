<div class="lang-en">

# Plot Generator

**Category:** Landscape > Modeling
**Nickname:** PlotGen

## 1. Introduction
Plot Generator is used to extract plots from road boundaries or any closed curves and convert them into regular quad meshes. It is a critical step after road network generation, used to fill green spaces, paved areas, or building lots between roads.

## 2. Core Algorithm
1.  **Region Finding**: Uses the `Curve.CreateBooleanRegions` algorithm to automatically identify all closed void regions from a set of disordered curves.
2.  **Quad Remesh**: Applies the **QuadRemesh** algorithm to each identified closed region to generate topologically uniform meshes. This ensures structural harmony between plot meshes and road network meshes (though not necessarily vertex-aligned).

## 3. Input Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Boundaries** | B | Curve (List) | A collection of curves defining plot boundaries. Typically, use edges from `Quad Road Network` or original design sketches. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **PlotMeshes** | M | Mesh (List) | List of generated quad meshes for each plot. |

## 5. Tips
*   **Integration with Roads**: Extract edges (`Mesh Edges`) from the output mesh of `Quad Road Network` and use them as input for this component to quickly generate plots that fit perfectly with the road network.
*   **Post-Processing**: Generated plot meshes can be further processed with the `Terrain` component for micro-terrain sculpting or populated with plants using the `Scatter` component.


</div>

<div class="lang-zh">

# Plot Generator (地块生成器)

**Category:** Landscape > Modeling
**Nickname:** PlotGen

## 1. 简介 (Introduction)
Plot Generator 用于从道路边界或任意闭合曲线中提取地块（Plot），并将其转化为规整的四边面网格。它是路网生成后的关键步骤，用于填充道路之间的绿地、铺装区域或建筑用地。

## 2. 核心算法 (Core Algorithm)
1.  **区域识别 (Region Finding)**: 使用 `Curve.CreateBooleanRegions` 算法，从一组杂乱的曲线中自动识别出所有闭合的空白区域。
2.  **四边面重构 (Quad Remesh)**: 对每个识别出的闭合区域应用 **QuadRemesh** 算法，生成拓扑均匀的网格。这确保了地块网格与路网网格在结构上的协调性（虽然不一定顶点对齐）。

## 3. 输入参数 (Input Parameters)

| 参数名 | 缩写 | 类型 | 说明 |
| :--- | :--- | :--- | :--- |
| **Boundaries** | B | Curve (List) | 定义地块边界的曲线集合。通常使用 `Quad Road Network` 的边缘或原始设计线稿。 |

## 4. 输出参数 (Output Parameters)

| 参数名 | 缩写 | 类型 | 说明 |
| :--- | :--- | :--- | :--- |
| **PlotMeshes** | M | Mesh (List) | 生成的每个地块的四边面网格列表。 |

## 5. 使用建议 (Tips)
*   **配合路网**: 将 `Quad Road Network` 的输出网格提取边缘 (`Mesh Edges`)，作为本组件的输入，可以快速生成与路网严丝合缝的地块。
*   **后期处理**: 生成的地块网格可以进一步用于 `Terrain` 组件进行微地形处理，或者通过 `Scatter` 组件布置植物。


</div>
