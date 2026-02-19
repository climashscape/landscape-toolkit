# Landscape Terrain

**Category:** Landscape > Modeling
**Nickname:** Terrain

## 1. Introduction
The Terrain component is dedicated to generating high-quality landscape micro-terrain. Unlike traditional GIS terrain tools, this component focuses on design-scale earthwork modeling, supporting the generation of smooth, flowing, and well-topologized quad-mesh terrain (often called "Class-A" surfaces) from contours or scattered points.

## 2. Core Algorithm
1.  **Delaunay Triangulation**: First constructs a basic Triangulated Irregular Network (TIN) based on input points or contour sampling points.
2.  **Smoothing**: Applies Laplacian smoothing to the initial TIN to eliminate the common "stair-step effect" of contours.
3.  **Quad Remesh**: Reconstructs the smoothed model into a quad mesh, optimizing edge flow.
4.  **SubD Conversion**: (Optional) Outputs a SubD surface for a perfectly smooth final result.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Input** | G | Geometry (List) | Input contours (Curve) or elevation points (Point). |
| **Target Quad Count** | QC | Integer | 2000 | Target mesh face count. Higher values yield more detail. |
| **Smooth** | S | Boolean | True | Whether to smooth the initial terrain before remeshing. Recommended for stepped contours. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Quad Mesh** | QM | Mesh | The reconstructed quad mesh. |
| **SubD** | SD | SubD | The converted Subdivision Surface (high-precision smooth model). |

## 5. Tips
*   **Contour Repair**: Ensure input contours do not self-intersect or overlap, as this may cause the initial triangulation to fail.
*   **Micro-Terrain**: Excellent for creating undulating grass slopes and mounds. Local terrain features can be controlled by adjusting the density of input points.
