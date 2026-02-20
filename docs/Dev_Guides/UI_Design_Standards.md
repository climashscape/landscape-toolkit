# UI Design & Development Standards Manual

This manual outlines the standards for User Interface (UI) design and development within the **Landscape Toolkit**. It ensures consistency across all Grasshopper components and provides a predictable experience for users.

## 1. Visual Identity (Iconography)

All icons are programmatically generated in `src/Icons.cs` to ensure high resolution and style consistency.

### 1.1 Design Philosophy
The UI follows a **"Tile"** design language:
*   **Background**: A rounded rectangle representing the component's category.
*   **Foreground**: A high-contrast, simplified vector icon representing the tool's function.

### 1.2 Color Coding
We use a strict color-coding system to differentiate component categories:

| Category | Color (RGB/Name) | Hex | Usage |
| :--- | :--- | :--- | :--- |
| **Modeling** | `ForestGreen` (34, 139, 34) | `#228B22` | Geometry generation (Roads, Terrain, Walls). |
| **Analysis** | `OrangeRed` (255, 69, 0) | `#FF4500` | Environmental analysis (Slope, Solar). |
| **Hydrology** | `DodgerBlue` (30, 144, 255) | `#1E90FF` | Water-related analysis. |
| **Optimization** | `Purple` (128, 0, 128) | `#800080` | Algorithms (Path finding, smoothing). |
| **Utility** | `Gray` (128, 128, 128) | `#808080` | Helper tools (Pickers, Converters). |

### 1.3 Icon Style
*   **Size**: 24x24 pixels (Standard GH icon size), scalable to 128x128 for Web.
*   **Background Shape**: Full bleed Rounded Rectangle (4px radius).
*   **Visual Effects**:
    *   **3D Gradient**: Linear Gradient from Top-Left (Base Color + 40 brightness) to Bottom-Right (Base Color - 40 brightness).
    *   **Border**: 1px Inset Border (Dark Color) for definition.
*   **Foreground**:
    *   **Scale**: Scaled by 0.8x and centered to fit within the background.
    *   **Color**: Use **White** (`#FFFFFF`) or high-contrast colors for primary elements.
    *   Avoid complex gradients.
    *   Lines should be at least 1.5px thick (at 24px scale) for readability.

### 1.4 Implementation
To add a new icon:
1.  Open `src/Icons.cs`.
2.  Define the drawing logic in a static `Action<Graphics>`.
3.  Use the `WithBackground` helper to apply the category color.
    ```csharp
    public static Bitmap MyComponent => CreateIcon(
        WithBackground(CategoryColors.Modeling, DrawMyComponent)
    );
    ```

## 2. Component Design (UX)

### 2.1 Naming Conventions
*   **Name**: Full descriptive name (e.g., "Quad Road Network").
*   **Nickname**: Short, camelCase name without spaces (e.g., "QuadRoad").
*   **Category**: "Landscape".
*   **Sub-category**: "Modeling", "Analysis", "Optimization", "Workflow".

### 2.2 Input Parameters
*   **Order**:
    1.  **Main Geometry** (Curves, Points, Breps).
    2.  **Key Parameters** (Width, Height, Count).
    3.  **Options/Settings** (Booleans, Modes).
*   **Defaults**: Always provide sensible default values.

### 2.3 Output Parameters
*   **Order**:
    1.  **Main Result** (Mesh, Curve).
    2.  **Secondary Geometry** (Graph, Junctions).
    3.  **Data/Metrics** (Area, Count, Report).

## 3. Web Documentation
The web documentation (`docs/index.html`) mirrors the plugin UI:
*   Uses the same generated icons (exported via `tools/IconExporter.cs`).
*   Uses the same color scheme for headers and accents.

---
*Maintained by Landscape Toolkit Dev Team*
