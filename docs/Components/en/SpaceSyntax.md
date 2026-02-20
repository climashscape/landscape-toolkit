# Space Syntax Analysis

**Category:** Landscape > Analysis
**Nickname:** SpaceSyntax

## 1. Introduction
Space Syntax Analysis is a tool for analyzing the topological and geometric properties of road networks. It computes standard metrics like Integration and Choice to help understand accessibility and movement potential within the network.

## 2. Core Algorithm
The component follows standard Space Syntax logic:
1.  **Shatter**: Breaks all curves at intersections to create a segment map.
2.  **Natural Roads**: Merges collinear segments into continuous "strokes" (Axial Lines) based on continuity.
3.  **Graph Construction**: Builds an adjacency graph where nodes are natural roads and edges represent intersections.
4.  **Metric Computation**: Uses BFS/Shortest Path algorithms to calculate topological depth and other metrics.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Curves** | C | Curve (List) | - | The road network centerlines. |
| **Metric** | M | Integer | 0 | Analysis Metric: <br>0 = Integration (HH)<br>1 = Choice (Through-movement)<br>2 = Mean Depth<br>3 = Control |
| **Radius** | R | Number | 0.0 | Analysis Radius. 0 = Global (n), >0 = Local metric (e.g., 800m). |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Values** | V | Number (List) | The computed metric values for each segment. |
| **Segments** | S | Curve (List) | The analyzed segments (shattered at intersections). |
| **Colors** | C | Colour (List) | Visualization colors (Heatmap) corresponding to the values. |

## 5. Tips
*   **Integration (Global)**: Useful for identifying the "core" of the settlement or the most accessible streets.
*   **Choice**: Useful for predicting potential movement flows (where people might pass through).
*   **Graph Cleaning**: Ensure your input curves are reasonably clean, though the component handles intersections automatically.
