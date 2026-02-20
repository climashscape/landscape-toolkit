# Landscape Features Spec

## Why
Designers need automated tools to generate common landscape elements like walls, steps, boardwalks, and scattered objects (trees, lights) to quickly populate a scene and test design options without manual modeling.

## What Changes
- **New Components**:
  - `WallComponent`: Generates vertical walls or curbs from curves.
  - `StepsComponent`: Generates parametric steps along a path, adapting to slope.
  - `BoardwalkComponent`: Generates raised boardwalk structures with decking and supports.
  - `ScatterComponent`: Distributes objects (points/planes) along curves or surfaces based on rules.
  - `MinimalSurfaceComponent`: Generates tensile structures using relaxation.

## Impact
- **Affected Specs**: New Landscape Features Module.
- **Affected Code**: 
  - `src/Modeling/Features/Walls/WallComponent.cs`
  - `src/Modeling/Features/Steps/StepsComponent.cs`
  - `src/Modeling/Features/Boardwalks/BoardwalkComponent.cs`
  - `src/Modeling/Features/Scatter/ScatterComponent.cs`
  - `src/Modeling/Surfaces/MinimalSurfaceComponent.cs`

## ADDED Requirements

### Requirement: Wall Generation
System should extrude walls along curves.
#### Scenario: Retaining Wall
- **WHEN** User inputs a curve and height/thickness.
- **THEN** System generates a solid wall mesh.
- **THEN** Wall follows the curve's z-values or projects to terrain.

### Requirement: Steps Generation
System should generate steps on slopes.
#### Scenario: Path Steps
- **WHEN** User inputs a path curve and step dimensions (tread/riser).
- **THEN** System calculates number of steps based on slope.
- **THEN** System generates step geometry fitting the path.

### Requirement: Boardwalk Generation
System should generate raised paths.
#### Scenario: Wetland Walkway
- **WHEN** User inputs a path curve, width, and railing options.
- **THEN** System generates decking, support posts, and railings.

### Requirement: Scatter System
System should populate scenes.
#### Scenario: Tree Planting
- **WHEN** User inputs an area or curve and density rules.
- **THEN** System generates random points with minimum distance constraints (Poisson Disk).
- **THEN** System can align objects to surface normals.

### Requirement: Minimal Surface
System should generate tensile forms.
#### Scenario: Shade Structure
- **WHEN** User inputs boundary curves (some elevated).
- **THEN** System generates a relaxed mesh minimizing surface area.
