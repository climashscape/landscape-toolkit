# Road Generation & Quad Mesh Topology (Road Network Implementation)

## 1. Core Algorithm Logic
The process of generating a 3D Quad Mesh Road Network from 2D Centerlines is the core of this toolkit. The key challenge is: **How to ensure the topology consists entirely of Quads for subsequent SubD subdivision, while handling road junctions at arbitrary angles.**

### 1.1 Input Processing
*   **Input**: Rhino Curve (Polyline or Nurbs Curve).
*   **Preprocessing**:
    1.  **Node Identification**: Detect all intersections of the curves and shatter them into segments (Edges).
    2.  **Graph Construction**: Build a `Graph` data structure where intersections are `Nodes` and segments are `Edges`.
    3.  **Degree Analysis**: Calculate the number of connected segments for each node (Valence 3, 4, >4).

### 1.2 Offset & Outline Generation
For each road segment (Edge):
1.  Set width `W` based on **Road Hierarchy**.
2.  Offset to both sides by `W/2` to get two border lines (Left/Right Border).
3.  **Critical Step**: At junctions, do not simply fillet. Instead, calculate the intersection points of adjacent segment borders, preserving the original "sharp corner" structure for later processing.

### 1.3 Junction Strategy - The Core Challenge
This is the most complex part of generating a quad mesh. We apply different topological templates based on the junction's Valence:

#### A. 3-Way Junction
*   **Topology**: Similar to a "Y" shape.
*   **Meshing**:
    1.  Find the intersection point `C` of the three road centerlines.
    2.  Connect the junction corner points to form an inner triangle.
    3.  **Quad Technique**: Add a vertex at the center of the triangle and connect it to the midpoints of the three sides, splitting it into **3 Quads**.
    4.  Alternatively, use "Rotational Flow" topology to smooth traffic flow direction mesh lines.

#### B. 4-Way Junction
*   **Topology**: Standard "+" shape.
*   **Meshing**:
    1.  Directly connect the four corner points to form a large quad region.
    2.  If the junction is large, it can be further subdivided into a $2 \times 2$ or $3 \times 3$ grid.

#### C. Multi-Way Junction (>4-Way)
*   **General Solution**:
    1.  Generate an N-sided polygon (N-gon) region.
    2.  Add a `Center` point.
    3.  Connect `Center` to each vertex of the polygon, forming $N$ triangles.
    4.  Subdivide again to convert each triangle into quads (Catmull-Clark subdivision concept).

### 1.4 Meshing & Smoothing
1.  **Strip Generation**: Along the direction of the road segment, connect corresponding points on the left and right borders to generate regular **Quad Strips**.
2.  **Weld**: Weld the vertices of the segment meshes and junction meshes.
3.  **Relaxation**:
    *   Use Laplacian Smoothing or a Spring System.
    *   **Constraint**: Keep road edge points fixed (or allow only tangential movement) and relax only internal points to ensure network fluidity.

### 1.5 Vertical Fitting
*   **Projection**: Project the generated 2D mesh onto the **Terrain**.
*   **Smoothing**: Roads should not perfectly follow every micro-undulation of the terrain.
*   **Algorithm**:
    1.  Get terrain elevation `Z`.
    2.  Apply "Least Squares" fitting or B-Spline smoothing to road centerlines to remove high-frequency noise.
    3.  Recalculate `Z` values for mesh points to ensure longitudinal slope complies with regulations (e.g., < 8%).

---

## 2. Hierarchy & Layer Management
Addressing the user requirement: "How to connect and output different hierarchy roads without materials to different layers", we adopt the following strategy:

### 2.1 Attribute Definition
Each `RoadEdge` object carries the following attributes:
*   **Level**: Road hierarchy level (e.g., 0=Primary, 1=Secondary, 2=Trail).
*   **Width**: Corresponding width.
*   **MaterialID**: Material identifier.

### 2.2 Connection Logic
When roads of different levels intersect (e.g., Main Road vs. Path):
1.  **Priority**: The mesh structure of the higher-level road (Primary) remains intact.
2.  **Break**: The lower-level road is broken at the junction.
3.  **Transition Mesh**: Generate a special fan-shaped or trapezoidal transition mesh at the interface to ensure topological continuity from wide to narrow roads, avoiding T-Junctions (cracks).

### 2.3 Layer Output
When generating final Rhino geometry:
*   Automatically **Bake** the Mesh to the corresponding layer based on `Level` or `MaterialID` (e.g., "Roads::Primary", "Roads::Secondary").
*   **User Data**: Attach attributes as User Text to the Mesh for subsequent analysis or BIM workflows.

---

## 3. Bio-Mimetic / Wooly Path Algorithm
The "Wooly Algorithm" or "Slime Mold Algorithm" mentioned by the user is primarily used for path optimization.

*   **Principle**: Simulates the natural curvature of wool threads when pulled, or the shortest path finding of slime mold foraging.
*   **Implementation**:
    1.  **Agent-Based**: Release a large number of **Agents** walking between start and end points.
    2.  **Trail Formation**: Agents leave "Pheromones" where they walk.
    3.  **Path Extraction**: Extract paths with the highest pheromone concentration.
    4.  **Curve Optimization**: Smooth the extracted paths so they conform to shortest path principles while possessing organic curvature aesthetics.

---

## 4. Code Reference
In `src/Modeling/Roads/RoadNetworkComponent.cs`, we implement the main class:

```csharp
public class RoadNetwork 
{
    public List<Curve> Centerlines { get; set; }
    public double Width { get; set; }
    
    // Method to generate mesh
    public Mesh GenerateQuadMesh() 
    {
        // 1. Build Graph
        var graph = BuildGraph(Centerlines);
        
        // 2. Generate Junctions
        foreach(var node in graph.Nodes) {
            GenerateJunctionMesh(node);
        }
        
        // 3. Generate Streets
        foreach(var edge in graph.Edges) {
            GenerateStreetMesh(edge);
        }
        
        // 4. Weld & Relax
        return FinalizeMesh();
    }
}
```
