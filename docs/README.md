# Landscape Toolkit - Project Documentation

[**中文文档 (Chinese Documentation)**](README_CN.md)

**Current Version: 1.2.2**

> **New in v1.2.2**: Comprehensive documentation update, version synchronization, and content consistency checks.

## 1. Project Vision
This project aims to provide a set of efficient modeling and analysis tools for landscape designers based on Rhino/Grasshopper. The core philosophy is to generate high-quality **3D Quad Mesh Road Networks**, terrain, and landscape structures in real-time from **2D Sketches** through parametric algorithms, integrating multi-dimensional environmental analysis to achieve a "Design-Analysis-Optimization" closed-loop workflow.

## 2. Core Modules

### 2.1 Road Network Generation
*   **Input**: 2D Road Centerlines (Curve).
*   **Core Algorithms**:
    *   **Quad Topology**: Abandoning traditional Trimmed Surfaces in favor of pure mesh modeling ensures lightweight models that are easy to subdivide (SubD).
    *   **Real-time Derivation**: Drag centerlines to update road width, fillets, and junction connections in real-time.
    *   **Hierarchy Management**: Supports material and construction transitions for different road levels (Primary, Secondary, Trails).
    *   **Multi-Level Logic**: Dedicated component for L1/L2/L3 priority networks with bell-mouth connections.
    *   **Bio-Mimetic Tuning (Wooly Path)**: Introduces Wooly/Slime Mold algorithms to naturally smooth rigid straight lines.

### 2.2 Surfaces & Terrain
*   **Enclosure Generation**: Automatically identifies closed areas enclosed by the road network to generate Plots.
*   **Minimal Surface**: Generates smooth surfaces based on boundary lines, supporting SubD subdivision to ensure perfect transitions with road edges.
*   **Vertical Design**: Supports real-time adjustment of terrain undulation via control points or slope parameters.

### 2.3 Landscape Features
*   **Parametric Components**:
    *   **Steps**: Automatically generates steps adapting to slope, balancing tread width and riser height calculations.
    *   **Retaining Walls**: Automatically generated based on elevation differences.
    *   **Boardwalks**: Generates elevated structures along lines.
    *   **Scatter System**: Automatically places streetlights, benches, and street trees along road edges.

### 2.4 Environmental Analysis
*   **Multi-Objective Optimization**:
    *   **Slope/Earthwork**: Calculates cut and fill volumes to optimize site grading.
    *   **Space Syntax**: Analyzes network integration and choice to evaluate accessibility.
    *   **Hydrology**: Catchment area identification, runoff simulation.
    *   **Microclimate**: Wind environment (simplified CFD), Thermal Comfort (UTCI), Solar Hours.
    *   **Carbon Sequestration**: Estimates ecological benefits based on biomass.

## 3. Technical Architecture

### 3.1 File Structure
```
src/
├── Core/               # Geometry Kernels
├── Data/               # Data Models
├── Modeling/           # Modeling Modules
│   ├── Roads/          # Road Algorithms (RoadNetwork, Junctions)
│   ├── Surfaces/       # Terrain & Plot Generation (Terrain, PlotGenerator)
│   └── Features/       # Feature Generation (Steps, Walls, Scatter)
├── Analysis/           # Analysis Modules (Slope, Solar, Hydro, Carbon)
├── Optimization/       # Optimization Algorithms (PathFinder, Multi-Objective)
└── UI/                 # User Interface Logic
```

### 3.2 Key Algorithm Implementation
*   **Road Network**: Uses `Half-Edge` data structure to manage mesh topology, ensuring shared vertices and smooth normals at junctions.
*   **Terrain Fitting**: Uses `Relaxation` algorithms to transition the mesh naturally under boundary constraints.
*   **Interaction Logic**: Listens to Rhino document events to achieve "Real-time generation from hand-drawn sketches".

## 4. Development Status
*   **Phase 1**: [Completed] Basic Quad Mesh logic for road networks, solving automatic layout for junctions (3-way, 4-way).
*   **Phase 2**: [Completed] Implemented SubD algorithms for plot surfaces, ensuring G1/G2 continuity with road boundaries.
*   **Phase 3**: [Completed] Developed adaptive generation for vertical features (Steps, Walls) and new v1.1.0 features (Boardwalks, Scatter System).
*   **Phase 4**: [Completed] Integrated environmental analysis tools (Slope, Solar, Hydrology, Carbon, Wind Shadow) and established data dashboards.
*   **Phase 5**: [In Progress] Advanced GIS integration and detailed construction drawing generation.

## 5. Branch Strategy

*   **`main`**: **Stable / Release**. Only contains verified, stable versions. Updates coincide with new version releases.
*   **`dev`**: **Development / Bleeding Edge**. Contains the latest features and improvements. Updates frequently.

## 6. Documentation Index

### User Guides
*   [**UI Design Standards**](Dev_Guides/UI_Design_Standards.md)
*   [**Component Reference**](Components/README.md)
*   [**Workflows**](Workflows/README.md)

### Core Technology
*   [**Core Logic**](Core_Logic/README.md)
    *   [Roads Implementation](Core_Logic/Roads_Implementation.md)
    *   [Surfaces Implementation](Core_Logic/Surfaces_Implementation.md)
    *   [Features Implementation](Core_Logic/Features_Implementation.md)
    *   [Analysis Implementation](Core_Logic/Analysis_Implementation.md)

---
*Maintained by: Landscape Toolkit Dev Team*
*Last Updated: 2026-02-20*
