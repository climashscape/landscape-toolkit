# Workflow: Terrain & Planting (地形与种植)

本教程介绍如何生成自然起伏的地形并进行植物配置。

## 1. 地形生成 (Terrain Generation)

### Step 1: 准备高程数据
*   **输入**: 等高线 (Contours) 或 散点 (Points)。
*   **组件**: `Terrain`
*   **参数**:
    *   `Target Quad Count`: 2000 (根据场地大小调整)。
    *   `Smooth`: True (消除台阶效应)。

### Step 2: 细分与造型
*   将生成的 `QuadMesh` 转换为 SubD。
*   使用 Rhino 的 Gumball 对 SubD 控制点进行推拉，微调地形起伏。

## 2. 植物散布 (Planting Scatter)

### Step 1: 提取种植区域
*   使用 `Plot Generator` 生成地块网格。
*   或者直接使用 `Terrain` 生成的地形表面。

### Step 2: 定义散布规则
*   **组件**: `Scatter` (需自行实现或使用现有逻辑)
*   **乔木**: 使用泊松盘采样 (Poisson Disk Sampling) 避免树木重叠。
    *   间距: 5m - 8m
*   **灌木/地被**: 使用随机采样或噪波纹理控制密度。

### Step 3: 实例化 (Instancing)
*   使用 Grasshopper 的 `Elefront` 或原生 `Block` 功能，将植物图块实例化到散布点上，保持模型轻量化。
