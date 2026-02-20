# Quad Road Network

**Category:** Landscape > Modeling
**Nickname:** QuadRoad

## 1. Introduction
Quad Road Network is one of the core components of this toolkit, designed to convert 2D centerline networks into high-quality **All-Quad** mesh models. The generated road network has clean topology, making it ideal for **SubD (Subdivision Surface)** workflows, achieving perfectly smooth surfaces while avoiding the fragmented faces and naked edges common in traditional boolean operations.

## 2. Core Algorithm
This component features specialized topology generation logic for landscape road networks:
1.  **Graph Building**: Automatically shatters intersecting curves and builds a topological relationship of Nodes and Edges.
2.  **Junction Meshing**:
    *   **3-Way**: Generates a Y-topology patch.
    *   **4-Way**: Generates a Grid-topology patch.
    *   **Dead End**: Automatically generates a square cap.
    *   **N-Way**: Adaptive fan topology.
3.  **Strip Generation**: Generates quad strip meshes between junctions, automatically subdivided based on road width.
4.  **Welding**: Automatically merges all vertices, outputting a single Manifold Mesh.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Centerlines** | C | Curve (List) | The road centerline network. Supports intersecting and overlapping curves (handled automatically). |
| **Widths** | W | Number (List) | 6.0 | List of road widths. If a single value, it applies globally; if a list, it must correspond to the curve count, supporting automatic connection of roads with different widths. |
| **Fillet** | F | Number | 3.0 | Junction fillet radius (distance from junction center to strip start). |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **QuadMesh** | M | Mesh | The generated road network mesh. All-quad structure, ready for SubD conversion. |
| **Graph** | G | Generic | Internal graph structure data (for debugging or advanced analysis). |
| **Junctions** | J | Mesh (List) | Separate list of junction meshes (useful for assigning different materials). |
| **Streets** | S | Mesh (List) | Separate list of street meshes (useful for assigning different materials). |

## 5. Tips
*   **SubD Conversion**: It is recommended to connect the generated Mesh to the `SubD from Mesh` component in Grasshopper to achieve the final smooth road surface effect.
*   **Curve Quality**: Although the component handles intersections, input curves are best optimized with `Bio-Path Optimizer` beforehand to avoid extremely acute intersections (< 30°), which may cause topology overlap.
*   **Road Hierarchy**: Use the `Widths` parameter to generate a mixed network of main roads (e.g., 10m) and secondary roads (e.g., 4m) in one go; junctions will automatically adapt to different widths.
