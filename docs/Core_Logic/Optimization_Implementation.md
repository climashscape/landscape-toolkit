<div class="lang-en">

# Optimization Algorithms (Optimization Implementation)

## 1. Bio-Mimetic / Wooly Path Algorithm
The "Wooly Algorithm" or "Slime Mold Algorithm" is primarily used for path optimization, simulating organic pathfinding behaviors found in nature.

*   **Principle**: Simulates the natural curvature of wool threads when pulled, or the shortest path finding of slime mold foraging.
*   **Implementation**:
    1.  **Agent-Based**: Release a large number of **Agents** walking between start and end points.
    2.  **Trail Formation**: Agents leave "Pheromones" where they walk.
    3.  **Path Extraction**: Extract paths with the highest pheromone concentration.
    4.  **Curve Optimization**: Smooth the extracted paths so they conform to shortest path principles while possessing organic curvature aesthetics.

## 2. Curve Relaxation (Path Optimizer)
For general curve smoothing, we implement a Laplacian Smoothing algorithm.

*   **Logic**:
    *   Iteratively move each control point of a curve towards the average position of its neighbors.
    *   $P_i^{new} = P_i + \lambda \cdot (P_{i-1} + P_{i+1} - 2P_i)$
    *   **Constraints**: Keep start and end points fixed to preserve connectivity.

## 3. Code Reference
*   `src/Optimization/WoolyPathOptimizerComponent.cs` - Implementation of the bio-mimetic agent system.
*   `src/Optimization/PathOptimizerComponent.cs` - Implementation of the curve relaxation logic.

</div>

<div class="lang-zh">

# 优化算法 (Optimization Implementation)

## 1. 仿生微调算法 (Bio-Mimetic / Wooly Path)
用户提到的“羊毛算法”或“粘菌算法”主要用于路径优化，模拟自然界中的有机寻路行为。

*   **原理**: 模拟羊毛线被拉扯时的自然弯曲，或粘菌寻找食物的最短路径。
*   **实现**:
    1.  **Agent-Based**: 释放大量智能体 (Agents) 在起点和终点之间行走。
    2.  **Trail Formation**: 智能体走过的地方留下“信息素” (Pheromone)。
    3.  **Path Extraction**: 提取信息素浓度最高的路径。
    4.  **Curve Optimization**: 对提取的路径进行平滑，使其既符合最短路径原则，又具有有机的弯曲美感。

## 2. 曲线松弛 (Path Optimizer)
针对通用的曲线平滑，我们实现了拉普拉斯平滑 (Laplacian Smoothing) 算法。

*   **逻辑**:
    *   迭代地将曲线上的每个控制点向其邻居的平均位置移动。
    *   $P_i^{new} = P_i + \lambda \cdot (P_{i-1} + P_{i+1} - 2P_i)$
    *   **约束**: 保持起点和终点固定，以维持连接性。

## 3. 代码参考
*   `src/Optimization/WoolyPathOptimizerComponent.cs` - 仿生智能体系统的实现。
*   `src/Optimization/PathOptimizerComponent.cs` - 曲线松弛逻辑的实现。

</div>
