# Landscape Wall

**Category:** Landscape > Modeling
**Nickname:** Wall

## 1. Introduction
The Wall component is used to rapidly generate landscape walls, retaining walls, or planter edges with thickness. Compared to a simple Extrude operation, it supports offset thickness along the curve normal and handles closed profiles to generate solid Breps.

## 2. Core Algorithm
1.  **Bidirectional Offset**: Offsets the base curve to both left and right sides by `Thickness / 2`.
2.  **Profile Closing**: Connects the endpoints of the offset curves to form a closed planar profile.
3.  **Sweep/Extrude**: Extrudes the closed profile along the Z-axis by the specified height to generate a solid.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Curve** | C | Curve | The base centerline of the wall. |
| **Height** | H | Number | 1.0 | Height of the wall. |
| **Thickness** | T | Number | 0.2 | Thickness of the wall. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Wall** | W | Brep | The generated wall solid. |

## 5. Tips
*   **Complex Curves**: For curves with extreme curvature or self-intersections, the offset operation may fail or produce artifacts. It is recommended to optimize curves first.
*   **Top Surface**: Currently generates flat-topped walls. Future versions will support top surfaces that follow terrain undulations.
