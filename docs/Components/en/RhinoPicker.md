# Rhino Picker

**Rhino Picker** component allows users to filter and select geometry directly from the Rhino document within Grasshopper by Layer, Name, or Type.

## Inputs
*   **Layer (L)**: List of layer names (Optional).
*   **Name (N)**: Object name filter (Supports `*` wildcard).
*   **Type (T)**: Object type (Curve, Surface, Brep, Mesh, Point, Text, etc., Default: All).
*   **Refresh (R)**: Boolean to force refresh the selection.

## Outputs
*   **Geometry (G)**: List of selected geometry.
*   **Count (C)**: Number of objects selected.

## Usage Example
1.  Input "Roads" to the **Layer** port to get all road curves.
2.  Input "Building_*" to the **Name** port to get all objects starting with "Building_".
