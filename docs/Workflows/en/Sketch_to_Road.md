# Workflow: From Sketch to Road Network

This tutorial guides you through converting a rough hand-drawn sketch into a high-quality 3D road network using the Landscape Toolkit.

## 1. Preparation
*   **Input**: A set of 2D Curves. These can be hand-drawn lines in Rhino or sketches imported from CAD/GIS.
*   **Requirements**: Curves can be on the same plane or have elevation differences (the component handles this automatically).

## 2. Step-by-Step

### Step 1: Path Optimization
Use the **Bio-Path Optimizer** to smooth the original curves.
*   **Component**: `BioPath`
*   **Parameters**:
    *   `Iterations`: 10-20
    *   `Strength`: 0.5
*   **Goal**: Eliminate jitter in hand-drawn lines, making paths conform better to natural human walking trajectories.

### Step 2: Generate Network
Input the optimized curves into the **Quad Road Network** component.
*   **Component**: `QuadRoad`
*   **Parameters**:
    *   `Width`: Set road width (e.g., 6.0m).
    *   `Fillet`: Set junction fillet radius (e.g., 3.0m).
*   **Result**: Generates a road network mesh with pure quad topology.

### Step 3: Subdivision
Use the native Grasshopper `SubD from Mesh` component.
*   **Input**: `QuadMesh` output from `QuadRoad`.
*   **Result**: Obtain the final smooth SubD road surface model.

## 3. Pro Tips
*   **Complex Junctions**: If a junction is overly complex (e.g., a 5-way intersection), it is recommended to manually adjust curve endpoints to converge at a single point before input, or rely on the component's automatic tolerance merging function.
