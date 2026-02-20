# Scatter System

**Category:** Landscape > Modeling
**Nickname:** Scatter

## 1. Introduction
The Scatter component provides a rule-based system for distributing landscape elements such as trees, street lights, and benches. Instead of pure randomization, it respects constraints like minimum distance, road edges, and surface boundaries.

## 2. Core Algorithm
1.  **Surface Sampling**: Generates candidate points on the target surface.
2.  **Distance Culling**: Removes points that are too close to each other based on the `MinDistance` parameter (Poisson-disk sampling logic).
3.  **Edge Alignment**: (Optional) Aligns elements like street lights along specified road edges with regular spacing.
4.  **Type Filtering**: Applies specific rules based on the element type (e.g., benches face the path, trees avoid road centers).

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **TargetSurface** | S | Mesh | - | The terrain or plot mesh where elements will be placed. |
| **RoadEdges** | E | Curve (List) | (Optional) | Curves defining road boundaries for aligning lights or benches. |
| **Type** | T | Integer | 0 | Element type: 0 = Tree, 1 = Light, 2 = Bench. |
| **MinDistance** | D | Number | 5.0 | Minimum distance (radius) between elements to prevent overcrowding. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Points** | P | Point (List) | The generated insertion points for the landscape elements. |

## 5. Tips
*   **Context Aware**: Use `RoadEdges` when placing street lights to ensure they follow the road network.
*   **Density Control**: Adjust `MinDistance` to control the overall density of the scattered elements (e.g., larger distance for big trees, smaller for shrubs).
