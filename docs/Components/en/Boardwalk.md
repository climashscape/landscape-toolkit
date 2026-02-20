# Boardwalk Generator

**Category:** Landscape > Modeling
**Nickname:** Boardwalk

## 1. Introduction
The Boardwalk component generates raised wooden boardwalks, complete with support structures and railings, directly from a path curve. It is designed for creating pedestrian paths over water, wetlands, or uneven terrain.

## 2. Core Algorithm
1.  **Path Framing**: Extracts perpendicular frames along the input curve to define the boardwalk's cross-sections.
2.  **Deck Generation**: Extrudes the cross-sections along the path to create the main walking surface.
3.  **Support Placement**: Raycasts downwards from the path at regular intervals to find intersection points with the terrain mesh, generating vertical support columns.
4.  **Railing System**: Offsets the path edges vertically to create handrails and posts.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Path** | P | Curve | - | Centerline curve of the boardwalk. |
| **Terrain** | T | Mesh | (Optional) | Terrain mesh used to determine support column heights. If omitted, supports are not generated. |
| **Width** | W | Number | 2.0 | Total width of the boardwalk deck. |
| **SupportSpacing** | S | Number | 3.0 | Distance between support columns along the path. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Deck** | D | Brep | The main walking surface of the boardwalk. |
| **Supports** | S | Brep (List) | Vertical support columns extending to the terrain. |
| **Railings** | R | Brep (List) | Side railings including posts and handrails. |

## 5. Tips
*   **Terrain Alignment**: Ensure the input path is elevated above the terrain mesh so supports can be generated downwards.
*   **Curvature**: Avoid extremely sharp turns in the input path, as they may cause self-intersection in the deck geometry.
