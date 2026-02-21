<div class="lang-en">

# Wind Shadow Analysis

**Category:** Landscape > Analysis
**Nickname:** WindShadow

## 1. Introduction
The **Wind Shadow Analysis** component provides a simplified wind environment analysis based on raycasting to identify sheltered areas behind windbreaks.

## 2. Inputs
*   **Obstacles (O)**: Obstacle geometry (Buildings, Walls, Tree Canopies).
*   **WindDirection (D)**: Dominant wind direction vector.
*   **TestPoints (P)**: Grid of points for analysis (Usually a plane grid 1.5m above ground).
*   **WakeLength (L)**: Maximum shelter distance behind obstacles (meters).

## 3. Outputs
*   **Exposure (E)**: Wind exposure coefficient (0.0 = Fully Sheltered, 1.0 = Fully Exposed).
*   **Rays (R)**: Visualized rays (Shows only blocked paths).

## 4. Notes
This is not a CFD simulation; it is based solely on geometric occlusion relationships, suitable for early design exploration.


</div>

<div class="lang-zh">

# Wind Shadow Analysis (风影分析)

**分类:** Landscape > Analysis
**别名:** WindShadow

## 1. 简介 (Introduction)
**Wind Shadow Analysis** 组件提供基于射线投射的简易风环境分析，用于识别挡风设施背后的遮蔽区域。

## 2. 输入 (Inputs)
*   **Obstacles (O)**: 障碍物几何体（建筑、墙体、树冠）。
*   **WindDirection (D)**: 主导风向向量。
*   **TestPoints (P)**: 需要分析的点阵（通常是离地 1.5m 的平面网格点）。
*   **WakeLength (L)**: 障碍物背后的最大遮蔽距离（米）。

## 3. 输出 (Outputs)
*   **Exposure (E)**: 风暴露系数 (0.0 = 完全遮蔽, 1.0 = 完全暴露)。
*   **Rays (R)**: 可视化射线（仅显示被遮挡的路径）。

## 4. 注意事项 (Notes)
这不是 CFD 模拟，仅基于几何遮挡关系，适用于早期方案推敲。


</div>
