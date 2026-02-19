# Landscape Toolkit (景观工具箱) - 工程规划文档

## 1. 项目愿景 (Project Vision)
本项目旨在为景观设计师提供一套基于 Rhino/Grasshopper 的高效建模与分析工具集。核心理念是从**二维线稿**出发，通过参数化算法实时推导生成高质量的**三维四边面 (Quad Mesh) 路网**、地形以及景观构筑物，并集成多维度的环境分析功能，实现“设计-分析-优化”的闭环工作流。

## 2. 核心功能模块 (Core Modules)

### 2.1 路网生成 (Road Network Generation)
*   **输入**: 平面道路中心线 (Curve)。
*   **核心算法**:
    *   **四边面拓扑 (Quad Topology)**: 摒弃传统的 Trimmed Surface，采用纯网格建模，确保模型轻量且易于细分 (SubD)。
    *   **实时推导**: 拖动中心线，路网宽度、倒角、路口连接实时更新。
    *   **层级管理**: 支持不同等级道路（主路、次路、小径）的材质与构造衔接。
    *   **仿生微调 (Wooly Path)**: 引入羊毛线/粘菌算法，对生硬的直线进行自然化平滑处理。

### 2.2 场地与地形 (Surfaces & Terrain)
*   **围合生成**: 自动识别路网围合的封闭区域，生成地块 (Plot)。
*   **最简曲面 (Minimal Surface)**: 基于边界线生成光顺的曲面，支持 SubD 细分，确保边缘与道路完美衔接。
*   **竖向设计**: 支持通过控制点或坡度参数，实时调整地形起伏。

### 2.3 景观构筑物 (Landscape Features)
*   **参数化组件**:
    *   **台阶 (Steps)**: 自动适应坡度生成台阶，支持踏步宽度与高度的平衡计算。
    *   **挡土墙 (Retaining Walls)**: 基于高差自动生成。
    *   **栈道 (Boardwalks)**: 沿线生成架空结构。
    *   **散布系统 (Scatter)**: 基于路网边缘自动布置路灯、座椅、行道树。

### 2.4 环境分析 (Environmental Analysis)
*   **多目标优化**:
    *   **坡度/土方**: 计算填挖方量，优化场地标高。
    *   **水文分析**: 汇水区识别、径流模拟。
    *   **微气候**: 风环境（CFD简易模拟）、热舒适度 (UTCI)、光照时数。
    *   **碳汇估算**: 基于植物量估算生态效益。

## 3. 技术架构 (Technical Architecture)

### 3.1 文件结构 (File Structure)
```
src/
├── Core/               # 核心几何算法库 (Geometry Kernels)
├── Data/               # 数据结构定义 (Data Models)
├── Modeling/           # 建模功能模块
│   ├── Roads/          # 道路生成算法 (RoadNetwork, Junctions)
│   ├── Surfaces/       # 地形与地块生成 (Terrain, PlotGenerator)
│   └── Features/       # 构筑物生成 (Steps, Walls, Scatter)
├── Analysis/           # 分析模块 (Slope, Solar, Hydro, Carbon)
├── Optimization/       # 优化算法 (PathFinder, Multi-Objective)
└── UI/                 # 用户界面逻辑
```

### 3.2 关键算法实现思路
*   **路网生成**: 采用 `Half-Edge` 数据结构管理网格拓扑，确保路口连接处的顶点共享与法线平滑。
*   **地形拟合**: 使用 `Relaxation` (松弛) 算法使网格在边界约束下自然过渡。
*   **交互逻辑**: 监听 Rhino 文档事件，实现“手拉线稿，实时生成”。

## 4. 开发进度 (Development Status)
*   **Phase 1**: [已完成] 基础路网生成的四边面逻辑，解决路口（3-way, 4-way）的自动布线。
*   **Phase 2**: [已完成] 实现地块封面的 SubD 算法，确保与路网边界的 G1/G2 连续。
*   **Phase 3**: [已完成] 开发竖向构件（台阶、墙）的自适应生成。
*   **Phase 4**: [已完成] 集成环境分析工具（坡度、光照、水文），建立数据仪表盘。

## 5. 文档索引 (Documentation Index)

### 用户指南 (User Guides)
*   [**Component Reference (运算器参考手册)**](Components/README.md)
*   [**Workflows (工作流指南)**](Workflows/README.md)

### 核心技术 (Core Technology)
*   [**Core Logic (核心实现逻辑)**](Core_Logic/README.md)
    *   [路网生成 (Roads)](Core_Logic/Roads_Implementation.md)
    *   [地形处理 (Surfaces)](Core_Logic/Surfaces_Implementation.md)
    *   [环境分析 (Analysis)](Core_Logic/Analysis_Implementation.md)

---
*文档维护者: Trae AI Assistant*
*最后更新: 2026-02-19*
