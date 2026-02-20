# Minimal Surface

**Category:** Landscape > Modeling
**Nickname:** MinSurf

## 1. Introduction
The Minimal Surface component generates tensile membrane structures or "minimal surfaces" where the mean curvature is zero. This is ideal for designing fabric roofs, tents, or organic shade structures in landscape architecture.

## 2. Core Algorithm
1.  **Mesh Initialization**: Creates an initial coarse mesh from the input boundary curves.
2.  **Dynamic Relaxation**: Iteratively moves mesh vertices to minimize internal forces (simulating soap film physics).
3.  **Attractor Influence**: Pulls vertices towards internal attractor points to create high/low points (peaks/valleys) in the surface.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Boundary** | B | Curve | - | Closed boundary curve defining the edge of the surface. |
| **Attractors** | A | Point (List) | (Optional) | Internal points that pull the surface up or down (e.g., tent poles). |
| **Iterations** | I | Integer | 50 | Number of relaxation iterations. More iterations result in a smoother, more "relaxed" shape. |
| **Resolution** | R | Number | 1.0 | Target edge length for the mesh. Smaller values yield higher detail. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Mesh** | M | Mesh | The generated minimal surface mesh. |

## 5. Tips
*   **Closed Boundaries**: Ensure the input `Boundary` curve is closed.
*   **Anchor Points**: Attractor points work best when they are significantly above or below the average plane of the boundary curve to create dramatic curvature.
