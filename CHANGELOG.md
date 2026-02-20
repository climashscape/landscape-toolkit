# Changelog

All notable changes to the **Landscape Toolkit** project will be documented in this file.

## [1.2.2] - 2026-02-20

### Changed (更新)
- **Documentation**:
    - Synchronized version numbers across all documentation files (v1.2.2).
    - Added missing documentation for **Multi-Level Road Network** and **Space Syntax Analysis**.
    - Updated project introductory text and progress tracking.
    - Verified Chinese/English translations and file links.

## [1.2.1] - 2026-02-20

### Changed (界面更新)
- **UI & Iconography**:
    - **Unified Design Language**: Adopted a "Tile" style with category-specific rounded rectangle backgrounds.
    - **3D Visual Effects**: Added linear gradients and inset borders to icons for a subtle 3D/tactile feel.
    - **Color Coding**: Standardized colors for Modeling (Green), Analysis (OrangeRed), Hydrology (Blue), Optimization (Purple), and Utility (Gray).
    - **High-Res Support**: All icons are now programmatically generated at high resolution (scalable to 128x128) for consistent web documentation.
- **Documentation**:
    - Updated [UI Design Standards](docs/Dev_Guides/UI_Design_Standards.md) with new specifications.
    - Added Chinese translation for UI Standards ([UI_Design_Standards_zh.md](docs/Dev_Guides/UI_Design_Standards_zh.md)).

## [1.2.0] - 2026-02-20

### Added (新增功能)
- **Integration**:
    - `RhinoPickerComponent`: Select Rhino objects by Layer, Name, or Type directly within Grasshopper.
- **Analysis**:
    - `CarbonAnalysisComponent`: Estimates carbon sequestration based on trees and green areas.
    - `WindShadowAnalysisComponent`: Simplified wind analysis using raycasting to visualize wind shadows.
- **Road Network System**:
    - `RoadNetworkComponent`: Added `Junctions` and `Streets` outputs to support better hierarchy management and material assignment.

## [1.1.0] - 2026-02-20

### Added (新增功能)
- **Features & Planting**:
    - `BoardwalkComponent`: Generates raised boardwalks with customizable width, supports, and railings.
    - `ScatterComponent`: Rule-based distribution system for trees, street lights, and benches along paths or surfaces.
- **Surface Modeling**:
    - `MinimalSurfaceComponent`: Creates minimal surfaces (tensile structures) from boundary curves using relaxation algorithms.
- **Integration**:
    - `GISConnector`: Initial framework for future GIS data integration (Placeholder).

## [1.0.0] - 2026-02-20

### Added (新增功能)
- **Optimization Module**:
    - `WoolyPathOptimizer`: Added bio-mimetic path finding algorithm (Slime Mold) for organic path generation.
    - `PathOptimizer`: Added Laplacian smoothing for curve optimization.
- **Road Network System**:
    - `QuadRoadGenerator`: Core logic for generating clean Quad-Mesh based road networks.
    - `RoadNetworkComponent`: Updated to support **variable road widths** (Road Hierarchy).
    - `Junction Logic`: Implemented robust handling for 3-way, 4-way, and N-way intersections with specific topology patches.
- **Terrain & Surfaces**:
    - `PlotGenerator`: Added algorithm to identify enclosed regions and generate quad mesh plots.
    - `TerrainComponent`: Implemented Delaunay triangulation + QuadRemesh workflow for high-quality terrain.
- **Landscape Features**:
    - `StepsComponent`: Parametric step generation adapting to path slope.
    - `WallComponent`: Vertical wall generation with thickness and path following.
- **Environmental Analysis**:
    - `SlopeAnalysis`: Real-time slope visualization with color gradient.
    - `SolarAnalysis`: Solar exposure estimation based on surface normals.
    - `Hydrology`: Surface runoff simulation using steepest descent algorithm (Raindrop tracing).

### Changed (优化与重构)
- **Architecture**:
    - Refactored project structure to separate `Core`, `Data`, `Modeling`, `Analysis`, and `Optimization` namespaces.
    - Moved `RoadGraph` and related data models to `src/Data/Graph`.
- **Documentation**:
    - Restructured `docs/` folder into `Components`, `Workflows`, and `Core_Logic`.
    - Added comprehensive Markdown manuals for all 8 Grasshopper components.
    - Added workflow tutorials ("Sketch to Road", "Terrain & Planting").
- **Dev Tools**:
    - Initialized `.trae` folder with spec templates and AI prompts to standardize development.

### Fixed (修复)
- Resolved namespace dependencies between Data and Modeling layers.
- Fixed `QuadRoadGenerator` to handle variable width inputs correctly.
