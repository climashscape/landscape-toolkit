# Wind Shadow Analysis

**Wind Shadow Analysis** component provides a simplified wind environment analysis based on raycasting to identify sheltered areas behind windbreaks.

## Inputs
*   **Obstacles (O)**: Obstacle geometry (Buildings, Walls, Tree Canopies).
*   **WindDirection (D)**: Dominant wind direction vector.
*   **TestPoints (P)**: Grid of points for analysis (Usually a plane grid 1.5m above ground).
*   **WakeLength (L)**: Maximum shelter distance behind obstacles (meters).

## Outputs
*   **Exposure (E)**: Wind exposure coefficient (0.0 = Fully Sheltered, 1.0 = Fully Exposed).
*   **Rays (R)**: Visualized rays (Shows only blocked paths).

## Notes
This is not a CFD simulation; it is based solely on geometric occlusion relationships, suitable for early design exploration.
