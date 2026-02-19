# Solar Analysis

**Category:** Landscape > Analysis
**Nickname:** Solar

## 1. Introduction
Solar Analysis is a fast solar exposure estimation tool based on mesh normals. By calculating the angle (dot product) between the terrain surface normal and sunlight rays, it generates intuitive heatmaps of sunlit and shaded areas. Suitable for preliminary site sunlight assessment and aspect analysis.

## 2. Core Algorithm
1.  **Normal Calculation**: Calculates the normal vector for each mesh vertex.
2.  **Dot Product**: Computes the dot product of the normal vector $\vec{N}$ and the reverse sunlight vector $\vec{L}$: $Exposure = \max(0, \vec{N} \cdot \vec{L})$.
    *   A result of 1.0 indicates perpendicular sunlight (strongest).
    *   A result of 0.0 indicates grazing light or shadow (weakest).
3.  **Color Mapping**: Maps exposure values to a heatmap gradient from blue (shadow) to red (sunlit).

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Mesh** | M | Mesh | The terrain or building mesh to analyze. |
| **SunDirection** | S | Vector | (-1, -1, -1) | The sunlight direction vector. Typically obtained from Rhino's Sun panel or Ladybug. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Colored Mesh** | CM | Mesh | The analysis result mesh with vertex colors. |
| **Exposure** | E | Number (List) | Solar exposure value for each vertex (0.0 - 1.0). |

## 5. Tips
*   **Rapid Analysis**: This is a geometric estimation tool and does not include shadow occlusion calculations (i.e., does not account for building self-shading or mutual shading). It is extremely fast and suitable for early-stage terrain aspect analysis.
*   **Accurate Simulation**: For precise hourly sunlight hours or radiation values, it is recommended to use Ladybug Tools.
