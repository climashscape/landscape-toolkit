# Wind Shadow Analysis (风影分析)

**Wind Shadow Analysis** 组件提供基于射线投射的简易风环境分析，用于识别挡风设施背后的遮蔽区域。

## 输入 (Inputs)
*   **Obstacles (O)**: 障碍物几何体（建筑、墙体、树冠）。
*   **WindDirection (D)**: 主导风向向量。
*   **TestPoints (P)**: 需要分析的点阵（通常是离地 1.5m 的平面网格点）。
*   **WakeLength (L)**: 障碍物背后的最大遮蔽距离（米）。

## 输出 (Outputs)
*   **Exposure (E)**: 风暴露系数 (0.0 = 完全遮蔽, 1.0 = 完全暴露)。
*   **Rays (R)**: 可视化射线（仅显示被遮挡的路径）。

## 注意事项
这不是 CFD 模拟，仅基于几何遮挡关系，适用于早期方案推敲。
