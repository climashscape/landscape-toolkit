# Wooly Path Optimizer (仿生路径优化器)

**Category:** Landscape > Optimization
**Nickname:** WoolyPath

## 1. 简介 (Introduction)
Wooly Path Optimizer 是一个基于 **Physarum Polycephalum (粘菌)** 仿生算法的路径优化工具。它模拟生物体在环境中寻找食物和建立高效运输网络的行为，用于生成符合自然人流习惯的有机路径系统。

与传统的“最短路径”算法不同，该组件生成的路径具有有机的形态，能够自适应地避开障碍（未来功能），并形成主次分明的层级结构。

## 2. 核心算法 (Core Algorithm)
该组件运行一个基于智能体 (Agent-based) 的模拟系统：
1.  **智能体 (Agents)**: 数千个虚拟智能体在画布上随机移动。
2.  **感知 (Sensory)**: 智能体通过传感器探测前方的“信息素 (Pheromone)”浓度。
3.  **决策 (Decision)**: 智能体倾向于转向信息素浓度更高的方向（正反馈循环）。
4.  **沉积 (Deposition)**: 智能体在经过的路径上留下信息素。
5.  **扩散与衰减 (Diffusion & Decay)**: 信息素会向周围扩散并随时间衰减，模拟自然界的化学信号传播。

## 3. 输入参数 (Input Parameters)

| 参数名 | 缩写 | 类型 | 默认值 | 说明 |
| :--- | :--- | :--- | :--- | :--- |
| **Bounds** | B | Rectangle | - | 模拟的边界范围。智能体被限制在此区域内活动。 |
| **Resolution** | Res | Integer | 100 | 网格分辨率。数值越高，路径越细腻，但计算越慢。建议 100-300。 |
| **AgentCount** | N | Integer | 1000 | 智能体数量。数量越多，路径越清晰，网络越密集。 |
| **SensorDistance** | SD | Number | 5.0 | 传感器的探测距离。决定了智能体“看”多远。 |
| **SensorAngle** | SA | Number | 45° (π/4) | 传感器相对于前进方向的角度。 |
| **TurnAngle** | TA | Number | 45° (π/4) | 智能体的最大转向角度。 |
| **Reset** | R | Boolean | False | 重置模拟状态（清空画布和智能体位置）。 |
| **Run** | Run | Boolean | False | 激活模拟。设置为 True 时，模拟将逐帧运行。 |

## 4. 输出参数 (Output Parameters)

| 参数名 | 缩写 | 类型 | 说明 |
| :--- | :--- | :--- | :--- |
| **TrailMap** | M | Mesh | 信息素地图的可视化网格。颜色越白表示路径越常被经过。 |
| **Agents** | A | Point (List) | 当前所有智能体的实时位置（用于显示粒子效果）。 |
| **Iteration** | I | Integer | 当前模拟的迭代步数。 |

## 5. 使用建议 (Tips)
*   **分辨率设置**: 初步测试时使用低分辨率 (50-100)，最终出图时提高到 200+。
*   **参数调试**: 
    *   增加 `SensorDistance` 会产生更长、更直的路径。
    *   减小 `TurnAngle` 会使路径更平滑。
*   **结合使用**: 配合 `Trace` 组件（开发中）可将 TrailMap 转化为矢量曲线，进而输入到 `Quad Road Network` 生成实体路网。
