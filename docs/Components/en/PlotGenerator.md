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
