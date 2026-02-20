# Workflow: From Sketch to Road Network (从手绘到路网)

本教程将指导您如何使用 Landscape Toolkit 将一张粗略的手绘草图转化为高质量的三维路网。

## 1. 准备工作 (Preparation)
*   **输入**: 一组二维曲线 (Curves)。可以是 Rhino 中手绘的线条，也可以是从 CAD/GIS 导入的线稿。
*   **要求**: 曲线可以在同一平面上，也可以有高差（组件会自动处理）。

## 2. 步骤详解 (Step-by-Step)

### Step 1: 路径优化 (Path Optimization)
使用 **Bio-Path Optimizer** 对原始曲线进行平滑处理。
*   **组件**: `BioPath`
*   **参数**:
    *   `Iterations`: 10-20
    *   `Strength`: 0.5
*   **目的**: 消除手绘线条的抖动，使路径更符合人流自然的行走轨迹。

### Step 2: 生成路网 (Generate Network)
将优化后的曲线输入到 **Quad Road Network** 组件。
*   **组件**: `QuadRoad`
*   **参数**:
    *   `Widths`: 设置道路宽度（单一数值如 6.0m，或列表以实现变宽路网）。
    *   `Fillet`: 设置路口倒角半径（如 3.0m）。
*   **结果**: 生成纯四边面拓扑的路网网格。

### Step 3: 细分平滑 (Subdivision)
使用 Grasshopper 原生的 `SubD from Mesh` 组件。
*   **输入**: `QuadRoad` 输出的 `QuadMesh`。
*   **结果**: 获得最终光滑的 SubD 路面模型。

## 3. 进阶技巧 (Pro Tips)
*   **复杂路口**: 如果路口过于复杂（如 5 岔路口），建议在输入前手动调整曲线端点，使其汇聚于一点，或依赖组件的自动容差合并功能。
