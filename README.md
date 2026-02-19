# Landscape Toolkit (景观工具箱) - 项目总览

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

**High-Performance Landscape Architecture Toolkit for Rhino + Grasshopper**
**专为景观设计师打造的全流程参数化设计系统**

---

## 1. 项目愿景 (Project Vision)
本项目旨在重新定义景观参数化设计的工作流，从**二维线稿的仿生优化**开始，无缝衔接到**三维四边面路网 (Quad Mesh)** 与**最简曲面地形 (Minimal Surface)** 的生成，最终通过**多维环境分析**反哺设计。

这是一个面向未来的大型项目，不仅涵盖现有的建模功能，还将集成 GIS 大数据与 AI 辅助设计，实现从“手绘草图”到“数字孪生”的完整闭环。

---

## 2. 核心工作流 (Core Workflow)

我们的工具链严格遵循以下逻辑顺序，确保设计推演的严密性：

### **Step 1: 平面流线优化 (Path Optimization)**
*   **输入**: 设计师手绘的粗略中心线 (Centerlines)。
*   **算法**: 引入 **Wooly Path (羊毛算法)** 与 **Slime Mold (粘菌算法)**。
*   **功能**: 对线稿进行仿生微调，模拟人流寻找最短路径的自然趋势，使生硬的折线转化为有机的流畅曲线。

### **Step 2: 四边面路网生成 (Quad Road Network)**
*   **核心技术**: **实时推导**与**纯四边面拓扑 (Pure Quad Topology)**。
*   **功能详解**:
    *   **实时交互**: 拖动中心线，路网宽度、倒角实时更新。
    *   **分级系统**: 支持主路、次路、小径等多级道路系统，不同等级拥有不同的宽度与材质 ID。
    *   **智能接口**: 自动处理路口连接 (3-Way, 4-Way)，保证车流线顺滑。
    *   **竖向拟合**: 道路中心线自动拟合地形高程，并控制纵坡在规范范围内。

### **Step 3: 围合与最简曲面 (Enclosed Minimal Surfaces)**
*   **核心痛点解决**: 解决 SubD 边界收缩导致的与路网不贴合问题。
*   **功能详解**:
    *   **地块识别**: 自动提取路网围合的封闭区域。
    *   **最简曲面**: 基于路网边界生成光顺的 **Minimal Surface** (平均曲率=0)，确保与道路边缘 **G1/G2 连续**。
    *   **边界锁定**: 采用特殊的网格松弛算法 (Relaxation)，严格锁定边界顶点，防止 SubD 细分时产生缝隙。

### **Step 4: 地形与构筑物 (Terrain & Features)**
在生成的曲面上进行二次深化：
*   **内部地形**: 通过 Point/Curve Attractor 微调地块内部起伏（如土丘、下凹绿地）。
*   **坡度控制**: 区分草坡（<25%）、灌木坡等不同坡度要求。
*   **台阶 (Steps)**: 自动检测高差，在陡坡路径上生成台阶，自动计算踏步数。
*   **挡土墙 (Walls)**: 基于地形高差自动生成重力式或悬臂式挡土墙。
*   **栈道 (Boardwalks)**: 生成架空栈道系统。

### **Step 5: 散布系统 (Scatter System)**
*   **功能**: 基于路网边缘和地块属性自动布置配景。
*   **内容**: 路灯（沿路分布）、座椅（节点分布）、行道树（阵列分布）、地被植物（泊松盘采样分布）。

### **Step 6: 环境分析与迭代 (Analysis & Feedback)**
分析结果不是终点，而是优化的起点：
*   **多目标优化**: 土方平衡 vs 日照时长 vs 路径长度。
*   **物理环境模拟**: 风环境 (CFD)、热舒适度 (UTCI)、光环境。
*   **生态分析**: 碳汇估算、汇水分析 (Hydrology)、坡度/坡向分析。
*   **未来集成**: 接入 GIS 大数据，实现基于城市数据的自动路网生成。

---

## 3. 技术架构 (Technical Architecture)

```
G:\CODE\LANDSCAPE TOOLKIT
├─docs/                         # 技术文档中心
│      Roads_Implementation.md  # 路网与分级系统详解
│      Surfaces_Implementation.md # 地形、SubD边界与构筑物详解
│      Analysis_Implementation.md # 全维分析与优化详解
│
├─src/
│  ├─Core/
│  │  ├─Interfaces/             # [新增] 核心接口定义 (IRoadGenerator等)
│  │  └─Utils.cs                # 通用工具类
│  │
│  ├─Data/
│  │  ├─Graph/                  # [新增] 路网拓扑数据结构 (Nodes, Edges)
│  │  └─Models/                 # 数据模型 (RoadType, PlantData)
│  │
│  ├─Modeling/
│  │  ├─Roads/                  # [核心] QuadRoadGenerator (四边面路网)
│  │  ├─Surfaces/               # [核心] MinimalSurfaceGenerator (最简曲面)
│  │  ├─Features/               # 台阶、墙、栈道
│  │  │  ├─Steps/               # [重构] StepGenerator逻辑类
│  │  │  └─Scatter/             # [新增] 散布系统
│  │
│  ├─Analysis/                  # 坡度、土方、碳汇、水文、微气候
│  ├─Optimization/              # [核心] WoolyPathOptimizer (仿生优化)
│  └─Integration/               # [未来] GIS与大数据接口
```

---

## 4. 快速索引 (Documentation Index)

*   [路网生成与分级系统 (Roads & Hierarchy)](docs/Roads_Implementation.md)
*   [地形、SubD与边界处理 (Terrain & Boundaries)](docs/Surfaces_Implementation.md)
*   [环境分析与多目标优化 (Analysis & Optimization)](docs/Analysis_Implementation.md)

---
*Maintained by Trae AI Assistant*
