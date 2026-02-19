# Bio-Path Optimizer

**Category:** Landscape > Optimization
**Nickname:** BioPath

## 1. Introduction
Bio-Path Optimizer is a lightweight curve optimization tool designed to convert rigid hand-drawn polylines or orthogonal paths into natural, streamlined curves. It uses the **Laplacian Smoothing** algorithm to simulate the process of tension relaxation.

## 2. Core Algorithm
The algorithm achieves smoothing by iteratively moving vertices on the curve:
*   For each internal vertex $P_i$ on the curve, calculate the midpoint $P_{target}$ between its predecessor $P_{i-1}$ and successor $P_{i+1}$.
*   Move $P_i$ towards $P_{target}$ by a certain ratio (controlled by `Strength`).
*   Endpoints remain fixed to preserve the path's start and end locations.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Curves** | C | Curve (List) | The list of original curves to optimize. |
| **Iterations** | I | Integer | 10 | Number of iterations. More iterations result in smoother curves that tend towards straight lines. |
| **Strength** | S | Number | 0.5 | Smoothing strength (0.0 - 1.0). 1.0 means moving fully to the midpoint; 0.0 means no movement. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Optimized Curves** | OC | Curve (List) | The optimized smooth curves. |

## 5. Tips
*   Ideal for processing rough paths imported from GIS or quick sketches drawn by designers.
*   If the curves shrink too much, reduce `Iterations` or `Strength`.
*   This component is typically used as a pre-processing step for `Quad Road Network` to ensure the generated road network is fluid and aesthetically pleasing.
