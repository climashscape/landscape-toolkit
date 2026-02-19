# Hydrology Analysis (水文径流模拟)

**Category:** Landscape > Analysis
**Nickname:** Hydro

## 1. 简介 (Introduction)
Hydrology Analysis 是一个基于网格的最速下降法 (Steepest Descent) 水流模拟工具。它通过在地形上随机生成“雨滴”并模拟其沿重力方向的流动路径，帮助设计师快速识别汇水区、排水沟位置和积水风险点。

## 2. 核心算法 (Core Algorithm)
1.  **随机降雨 (Random Drop)**: 在地形上方随机生成起始点，并投影到网格表面。
2.  **最速下降 (Steepest Descent)**:
    *   在当前点寻找局部最陡峭的下降方向（即重力在表面法线切平面上的投影）。
    *   $\vec{Slope} = \vec{g} - (\vec{g} \cdot \vec{n}) \vec{n}$，其中 $\vec{g} = (0, 0, -1)$。
3.  **路径追踪 (Path Tracing)**: 沿下降方向迭代移动，直到到达局部最低点或地形边缘。

## 3. 输入参数 (Input Parameters)

| 参数名 | 缩写 | 类型 | 默认值 | 说明 |
| :--- | :--- | :--- | :--- | :--- |
| **Mesh** | M | Mesh | 需要分析的地形网格。确保网格具有良好的拓扑和法线。 |
| **Raindrops** | N | Integer | 500 | 模拟的雨滴数量。数量越多，结果越接近真实的汇水分布。 |

## 4. 输出参数 (Output Parameters)

| 参数名 | 缩写 | 类型 | 说明 |
| :--- | :--- | :--- | :--- |
| **FlowLines** | F | Curve (List) | 生成的水流路径曲线。线条密集的区域即为汇水线（谷线）。 |

## 5. 使用建议 (Tips)
*   **网格质量**: 输入的 Mesh 不需要非常精细，但必须是单层流形网格。孔洞或重叠面会导致模拟中断。
*   **汇水线提取**: 通过观察 FlowLines 的聚集程度，可以直观地画出场地的主要排水沟位置。
*   **坑洼识别**: 路径终点聚集的地方通常是局部低洼点，可能存在积水风险，需设计排水口。
