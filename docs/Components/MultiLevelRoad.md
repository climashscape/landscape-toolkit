<div class="lang-en">

# Multi-Level Road

**Category:** Landscape > Modeling
**Nickname:** MultiRoad

## 1. Introduction
The Multi-Level Road Network component is an advanced version of the Quad Road generator. It allows for the creation of hierarchical road systems with up to three distinct levels (e.g., Main Roads, Secondary Roads, Trails). It handles priority junctions where higher-level roads maintain continuity, and lower-level roads connect with smooth bell-mouth aprons.

## 2. Core Features
*   **Priority Junctions**: Higher level roads (e.g., Level 1) cut through lower level roads, maintaining their flow.
*   **Layered Output**: Meshes for different levels are output separately, allowing for independent material assignment or post-processing.
*   **Bell-mouth Connections**: Smooth fillet connections are automatically generated where lower-level roads meet higher-level ones.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **L1 Curves** | L1_C | Curve (List) | - | Level 1 (Main) Road Centerlines. |
| **L1 Width** | L1_W | Number | 12.0 | Width for Level 1 Roads. |
| **L1 Radius** | L1_R | Number | 15.0 | Fillet Radius for Level 1 Intersections. |
| **L2 Curves** | L2_C | Curve (List) | - | Level 2 (Secondary) Road Centerlines (Optional). |
| **L2 Width** | L2_W | Number | 6.0 | Width for Level 2 Roads. |
| **L2 Radius** | L2_R | Number | 9.0 | Fillet Radius for Level 2 Intersections. |
| **L3 Curves** | L3_C | Curve (List) | - | Level 3 (Path) Road Centerlines (Optional). |
| **L3 Width** | L3_W | Number | 3.0 | Width for Level 3 Roads. |
| **L3 Radius** | L3_R | Number | 5.0 | Fillet Radius for Level 3 Intersections. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **All Meshes** | All | Mesh | Combined mesh of all levels. |
| **L1 Mesh** | L1 | Mesh | Mesh for Level 1 Roads only. |
| **L2 Mesh** | L2 | Mesh | Mesh for Level 2 Roads only. |
| **L3 Mesh** | L3 | Mesh | Mesh for Level 3 Roads only. |
| **Graph** | G | Generic | The underlying road graph structure. |

## 5. Tips
*   **Hierarchy**: Use L1 for boulevards, L2 for streets, and L3 for pedestrian paths.
*   **Independent Styling**: Since L1, L2, and L3 meshes are separate, you can easily bake them to different layers in Rhino.


</div>

<div class="lang-zh">

# Multi-Level Road (多级路网)

**Category:** Landscape > Modeling
**Nickname:** MultiRoad

## 1. 简介 (Introduction)
多级路网生成组件是 Quad Road 生成器的高级版本。它支持创建具有最多三个不同层级（例如主干道、次干道、小径）的分层道路系统。它处理优先路口，确保高等级道路保持连续，而低等级道路则通过平滑的喇叭口倒角连接。

## 2. 核心特性 (Core Features)
*   **优先路口 (Priority Junctions)**: 高等级道路（如 Level 1）切断低等级道路，保持自身的连续流线。
*   **分层输出 (Layered Output)**: 不同等级的网格单独输出，便于独立的材质赋值或后期处理。
*   **喇叭口连接 (Bell-mouth Connections)**: 在低等级道路与高等级道路交汇处自动生成平滑的倒角连接。

## 3. 输入参数 (Input Parameters)

| 参数名 | 缩写 | 类型 | 默认值 | 说明 |
| :--- | :--- | :--- | :--- | :--- |
| **L1 Curves** | L1_C | Curve (List) | - | 一级（主）道路中心线。 |
| **L1 Width** | L1_W | Number | 12.0 | 一级道路宽度。 |
| **L1 Radius** | L1_R | Number | 15.0 | 一级路口倒角半径。 |
| **L2 Curves** | L2_C | Curve (List) | - | 二级（次）道路中心线（可选）。 |
| **L2 Width** | L2_W | Number | 6.0 | 二级道路宽度。 |
| **L2 Radius** | L2_R | Number | 9.0 | 二级路口倒角半径。 |
| **L3 Curves** | L3_C | Curve (List) | - | 三级（小径）道路中心线（可选）。 |
| **L3 Width** | L3_W | Number | 3.0 | 三级道路宽度。 |
| **L3 Radius** | L3_R | Number | 5.0 | 三级路口倒角半径。 |

## 4. 输出参数 (Output Parameters)

| 参数名 | 缩写 | 类型 | 说明 |
| :--- | :--- | :--- | :--- |
| **All Meshes** | All | Mesh | 所有层级的合并网格。 |
| **L1 Mesh** | L1 | Mesh | 仅一级道路的网格。 |
| **L2 Mesh** | L2 | Mesh | 仅二级道路的网格。 |
| **L3 Mesh** | L3 | Mesh | 仅三级道路的网格。 |
| **Graph** | G | Generic | 底层路网图结构。 |

## 5. 使用建议 (Tips)
*   **层级规划**: 使用 L1 作为景观大道，L2 作为街道，L3 作为人行步道。
*   **独立样式**: 由于 L1、L2 和 L3 网格是分开的，您可以轻松地将它们烘焙到 Rhino 中的不同图层。


</div>
