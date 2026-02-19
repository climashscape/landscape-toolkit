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
