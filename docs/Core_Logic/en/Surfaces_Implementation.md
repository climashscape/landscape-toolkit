# Surfaces & Terrain Implementation

## 1. Enclosed Plot Generation
After generating the Road Network, the areas between the roads naturally form closed polygons, i.e., "Plots".

### 1.1 Automatic Detection
*   **Input**: A closed Loop formed by Road Boundaries.
*   **Algorithm**:
    1.  **Extract Naked Edges**: Get the naked edges of the road network Mesh.
    2.  **Sort & Join**: Sort these edges by connectivity and join them into a closed Polyline Loop.
    3.  **Validation**: Check if the polyline is closed and if the planar projected area is greater than a threshold.

### 1.2 Minimal Surface & Boundary Matching
The user raised a key technical challenge: "How to solve the overlapping of SubD uncontrollable boundaries with road boundaries." SubD (Subdivision Surface) algorithms typically cause edges to shrink during subdivision, creating gaps with road edges.

#### Solution
We adopt the following hybrid strategy to ensure **G0 Continuity (Position)** or even **G1 Continuity (Tangent)**:

1.  **Strategy A: Boundary Vertex Locking**
    *   During Kangaroo or custom Relaxation, set vertices located on the boundary as **Anchors**, with infinite Strength.
    *   This guarantees that edge points absolutely do not move during relaxation.

2.  **Strategy B: SubD Crease**
    *   Before converting to SubD, mark boundary edges as **Crease (Weight = Infinity)**.
    *   Rhino's SubD engine maintains Crease edges without shrinking, strictly adhering to the original mesh edges.

3.  **Strategy C: Boundary Densification**
    *   Generate a high-density "Transition Strip" at the interface between the road network and terrain.
    *   Outer vertices are welded to the road, and inner vertices participate in terrain relaxation.

### 1.3 Minimal Surface Algorithm (New in v1.1.0)
The `MinimalSurface` component implements a physics-based relaxation solver:
*   **Initialization**: A coarse mesh is generated within the boundary using constrained Delaunay triangulation or Quad grid mapping.
*   **Dynamic Relaxation**: Vertices are treated as particles connected by springs. The system iteratively minimizes the total energy (simulating soap film surface tension) until equilibrium is reached.
*   **Mean Curvature Flow**: The algorithm effectively minimizes Mean Curvature ($H = 0$) at every internal point.

### 1.4 Vertical & Slope Control
*   **Boundary Elevation**: Get elevation `Z` from road network edges.
*   **Internal Adjustment**:
    1.  **Point Attractor**: Place control points inside the plot to raise or lower their `Z` value to simulate mounds or depressions.
    2.  **Curve Attractor**: Use contours to constrain internal terrain.
    3.  **Slope Analysis Feedback**: Calculate slope in real-time; if it exceeds a set value (e.g., 25%), automatically adjust control point positions or heights.
