# Hydrology Analysis

**Category:** Landscape > Analysis
**Nickname:** Hydro

## 1. Introduction
Hydrology Analysis is a water flow simulation tool based on the Steepest Descent method on meshes. By generating random "raindrops" on the terrain and simulating their flow path along gravity, it helps designers quickly identify catchment areas, drainage channel locations, and potential ponding risks.

## 2. Core Algorithm
1.  **Random Drop**: Generates starting points randomly above the terrain and projects them onto the mesh surface.
2.  **Steepest Descent**:
    *   Finds the local steepest descent direction at the current point (projection of gravity onto the tangent plane defined by the surface normal).
    *   $\vec{Slope} = \vec{g} - (\vec{g} \cdot \vec{n}) \vec{n}$, where $\vec{g} = (0, 0, -1)$.
3.  **Path Tracing**: Iteratively moves along the descent direction until a local minimum or terrain edge is reached.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Mesh** | M | Mesh | The terrain mesh to analyze. Ensure the mesh has good topology and normals. |
| **Raindrops** | N | Integer | 500 | Number of simulated raindrops. More drops result in a more realistic catchment distribution. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **FlowLines** | F | Curve (List) | Generated water flow path curves. Areas with dense lines indicate catchment lines (valleys). |

## 5. Tips
*   **Mesh Quality**: The input Mesh does not need to be extremely detailed, but it must be a single-layer manifold mesh. Holes or overlapping faces will interrupt the simulation.
*   **Catchment Extraction**: By observing the clustering density of FlowLines, you can visually trace the main drainage channels of the site.
*   **Ponding Detection**: Areas where path endpoints cluster are typically local depressions, indicating potential ponding risks that may require drainage design.
