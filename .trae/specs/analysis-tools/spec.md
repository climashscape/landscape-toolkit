# Analysis Tools Spec

## Why
Landscape design requires not only geometric modeling but also environmental and ecological performance analysis. Designers need real-time feedback on slope, solar exposure, wind comfort, and carbon sequestration to make informed decisions early in the design process.

## What Changes
- **New Components**:
  - `SlopeAnalysisComponent`: Visualizes terrain slope with color gradients.
  - `SolarAnalysisComponent`: Estimates solar exposure based on surface normals and sun direction.
  - `WindShadowAnalysisComponent`: Simulates wind shadow areas behind obstacles using raycasting.
  - `CarbonAnalysisComponent`: Calculates carbon sequestration potential of planting designs.
  - `HydrologyComponent`: Simulates surface runoff and water accumulation paths.

## Impact
- **Affected Specs**: New Analysis Module.
- **Affected Code**: 
  - `src/Analysis/SlopeAnalysisComponent.cs`
  - `src/Analysis/SolarAnalysisComponent.cs`
  - `src/Analysis/WindShadowAnalysisComponent.cs`
  - `src/Analysis/CarbonAnalysisComponent.cs`
  - `src/Analysis/HydrologyComponent.cs`

## ADDED Requirements

### Requirement: Slope Analysis
System should visualize terrain steepness.
#### Scenario: Terrain Evaluation
- **WHEN** User inputs a terrain mesh or surface.
- **THEN** System calculates slope angle for each face/point.
- **THEN** System colors the mesh based on slope ranges (e.g., 0-5% Green, >25% Red).

### Requirement: Solar Analysis
System should estimate solar exposure.
#### Scenario: Sun Exposure
- **WHEN** User inputs geometry and a sun vector.
- **THEN** System calculates dot product between face normals and sun vector.
- **THEN** System visualizes exposure levels (Shadow vs. Sunlit).

### Requirement: Wind Shadow Analysis
System should identify sheltered areas.
#### Scenario: Windbreak Effect
- **WHEN** User inputs obstacles (buildings/trees) and wind direction.
- **THEN** System casts rays to determine areas blocked from the wind.
- **THEN** System outputs a grid of exposure values (0=Sheltered, 1=Exposed).

### Requirement: Carbon Analysis
System should calculate ecological benefits.
#### Scenario: Planting Carbon Sequestration
- **WHEN** User inputs tree points and green area surfaces.
- **THEN** System calculates total carbon sequestration based on species factors and area.
- **THEN** System outputs total kg/year and a detailed report.

### Requirement: Hydrology Analysis
System should simulate water flow.
#### Scenario: Surface Runoff
- **WHEN** User inputs a terrain mesh and rainfall points.
- **THEN** System traces the path of steepest descent for each point.
- **THEN** System visualizes flow lines accumulating in low points.
