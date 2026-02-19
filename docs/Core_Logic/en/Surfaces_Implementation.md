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

### 1.3 Vertical & Slope Control
*   **Boundary Elevation**: Get elevation `Z` from road network edges.
*   **Internal Adjustment**:
    1.  **Point Attractor**: Place control points inside the plot to raise or lower their `Z` value to simulate mounds or depressions.
    2.  **Curve Attractor**: Use contours to constrain internal terrain.
    3.  **Slope Analysis Feedback**: Calculate slope in real-time; if it exceeds a set value (e.g., 25%), automatically adjust control point positions or heights.

---

## 2. Landscape Features Generation
Generate specific structures based on the terrain surface.

### 2.1 Steps Generation
*   **Input**: Path Curve + Terrain Surface.
*   **Algorithm**:
    1.  **Sample Points**: Sample points along the path at fixed intervals (e.g., 0.3m).
    2.  **Height Check**: Check the height difference $\Delta H$ between adjacent points.
    3.  **Tread & Riser Calculation**:
        *   If $\Delta H > H_{max}$ (e.g., 0.15m), steps need to be inserted.
        *   Calculate the number of steps and riser height using the formula $2R + T = 600 \sim 640mm$.
    4.  **Geometry Construction**: Generate step solids (Brep) or meshes (Mesh).

### 2.2 Retaining Wall Generation
*   **Input**: Boundary Curve + Terrain on both sides.
*   **Algorithm**:
    1.  **Offset**: Offset half the wall thickness to the high and low sides respectively.
    2.  **Loft**: Loft between the offset lines to generate the wall body.
    3.  **Top Cap**: Generate the coping.
    4.  **Foundation**: Automatically generate foundation depth based on wall height.

### 2.3 Scatter System
*   **Input**: Region or Curve.
*   **Algorithm**:
    1.  **Poisson Disk Sampling**: Blue noise sampling to ensure uniform and natural distribution of objects (no overlapping).
    2.  **Instance Placement**: Instantiate prefabricated models (e.g., trees, streetlights) at sampling points.
    3.  **Randomization**: Randomly rotate and scale instances to add natural variation.

---

## 3. Code Reference
Implemented in `src/Modeling/Surfaces/PlotGeneratorComponent.cs` and `src/Modeling/Features/StepsComponent.cs`:

```csharp
public class PlotGenerator 
{
    public Polyline Boundary { get; set; }
    
    public Mesh GenerateMinimalSurface(int uCount, int vCount) 
    {
        // 1. Create Initial Grid
        var mesh = CreateGridFromBoundary(Boundary, uCount, vCount);
        
        // 2. Relax Mesh (Physics Simulation)
        var kangaroo = new KangarooSolver();
        kangaroo.AddGoal(new SpringGoal(mesh.Edges));
        
        // CRITICAL: Lock Boundary Vertices to prevent SubD shrinkage
        kangaroo.AddGoal(new AnchorGoal(mesh.BoundaryVertices)); 
        
        kangaroo.Solve();
        
        return kangaroo.OutputMesh;
    }
}
```
