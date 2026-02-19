# Tasks

- [x] Task 1: Create detailed project documentation and restructuring.
  - [x] SubTask 1.1: Rewrite `README.md` in Chinese with detailed explanations of core algorithms (Quad Remesh, Delaunay, Road Generation logic).
  - [x] SubTask 1.2: Create `docs/cn` folder and add specific documentation for key modules.
  - [x] SubTask 1.3: Update project folder structure to include `src/Optimization` and `src/Data`.
- [x] Task 2: Implement Advanced Road Network with Hierarchy.
  - [x] SubTask 2.1: Create `RoadType` enum/class (Width, FilletRadius, LayerName).
  - [x] SubTask 2.2: Implement `AdvancedRoadComponent` supporting multiple road types and layer separation.
  - [x] SubTask 2.3: Implement plaza connection logic (Boolean Union + Fillet).
- [x] Task 3: Implement Bio-Inspired Path Optimization.
  - [x] SubTask 3.1: Create `PathOptimizer` class in `src/Optimization`.
  - [x] SubTask 3.2: Implement basic "Relaxation" algorithm (force-directed graph or simple curve smoothing) to simulate natural pathfinding.
  - [x] SubTask 3.3: Create `PathOptimizerComponent` in Grasshopper to expose this functionality.
- [x] Task 4: Integrate and Verify.
  - [x] SubTask 4.1: Test road hierarchy and layer output.
  - [x] SubTask 4.2: Test path optimization on sample curves.
