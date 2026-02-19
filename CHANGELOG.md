# Changelog

All notable changes to the **Landscape Toolkit** project will be documented in this file.

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
