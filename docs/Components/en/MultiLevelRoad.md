# Multi-Level Road Network

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
