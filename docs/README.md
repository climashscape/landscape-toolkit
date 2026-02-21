<div class="lang-en">

# Landscape Toolkit - Project Documentation

**Current Version: 1.2.3**

 > **New in v1.2.3**: Unified bilingual documentation structure, added Glossary/Navbar, and cleaned up legacy directories.

## 1. Project Vision
This project aims to provide a set of efficient modeling and analysis tools for landscape designers based on Rhino/Grasshopper. The core philosophy is to generate high-quality **3D Quad Mesh Road Networks**, terrain, and landscape structures in real-time from **2D Sketches** through parametric algorithms, integrating multi-dimensional environmental analysis to achieve a "Design-Analysis-Optimization" closed-loop workflow.

## 2. Core Modules

### 2.1 Road Network Generation
*   **Input**: 2D Road Centerlines (Curve).
*   **Core Algorithms**:
    *   **Quad Topology**: Abandoning traditional Trimmed Surfaces in favor of pure mesh modeling ensures lightweight models that are easy to subdivide (SubD).
    *   **Real-time Derivation**: Drag centerlines to update road width, fillets, and junction connections in real-time.
    *   **Hierarchy Management**: Supports material and construction transitions for different road levels (Primary, Secondary, Trails).
    *   **Multi-Level Logic**: Dedicated component for L1/L2/L3 priority networks with bell-mouth connections.
    *   **Bio-Mimetic Tuning (Wooly Path)**: Introduces Wooly/Slime Mold algorithms to naturally smooth rigid straight lines.

### 2.2 Surfaces & Terrain
*   **Enclosure Generation**: Automatically identifies closed areas enclosed by the road network to generate Plots.
*   **Minimal Surface**: Generates smooth surfaces based on boundary lines, supporting SubD subdivision to ensure perfect transitions with road edges.
*   **Vertical Design**: Supports real-time adjustment of terrain undulation via control points or slope parameters.

### 2.3 Landscape Features
*   **Parametric Components**:
    *   **Steps**: Automatically generates steps adapting to slope, balancing tread width and riser height calculations.
    *   **Retaining Walls**: Automatically generated based on elevation differences.
    *   **Boardwalks**: Generates elevated structures along lines.
    *   **Scatter System**: Automatically places streetlights, benches, and street trees along road edges.

### 2.4 Environmental Analysis
*   **Multi-Objective Optimization**:
    *   **Slope/Earthwork**: Calculates cut and fill volumes to optimize site grading.
    *   **Space Syntax**: Analyzes network integration and choice to evaluate accessibility.
    *   **Hydrology**: Catchment area identification, runoff simulation.
    *   **Microclimate**: Wind environment (simplified CFD), Thermal Comfort (UTCI), Solar Hours.
    *   **Carbon Sequestration**: Estimates ecological benefits based on biomass.

## 3. Technical Architecture

### 3.1 File Structure
```
src/
├── Core/               # Geometry Kernels
├── Data/               # Data Models
├── Modeling/           # Modeling Modules
│   ├── Roads/          # Road Algorithms (RoadNetwork, Junctions)
│   ├── Surfaces/       # Terrain & Plot Generation (Terrain, PlotGenerator)
│   └── Features/       # Feature Generation (Steps, Walls, Scatter)
├── Analysis/           # Analysis Modules (Slope, Solar, Hydro, Carbon)
├── Optimization/       # Optimization Algorithms (PathFinder, Multi-Objective)
└── UI/                 # User Interface Logic
```

### 3.2 Key Algorithm Implementation
*   **Road Network**: Uses `Half-Edge` data structure to manage mesh topology, ensuring shared vertices and smooth normals at junctions.
*   **Terrain Fitting**: Uses `Relaxation` algorithms to transition the mesh naturally under boundary constraints.
*   **Interaction Logic**: Listens to Rhino document events to achieve "Real-time generation from hand-drawn sketches".

## 4. Development Status
*   **Phase 1**: [Completed] Basic Quad Mesh logic for road networks, solving automatic layout for junctions (3-way, 4-way).
*   **Phase 2**: [Completed] Implemented SubD algorithms for plot surfaces, ensuring G1/G2 continuity with road boundaries.
*   **Phase 3**: [Completed] Developed adaptive generation for vertical features (Steps, Walls) and new v1.1.0 features (Boardwalks, Scatter System).
*   **Phase 4**: [Completed] Integrated environmental analysis tools (Slope, Solar, Hydrology, Carbon, Wind Shadow) and established data dashboards.
*   **Phase 5**: [In Progress] Advanced GIS integration and detailed construction drawing generation.

## 5. Branch Strategy

*   **`main`**: **Stable / Release**. Only contains verified, stable versions. Updates coincide with new version releases.
*   **`dev`**: **Development / Bleeding Edge**. Contains the latest features and improvements. Updates frequently.

---
*Maintained by: Landscape Toolkit Dev Team*
*Last Updated: 2026-02-21*


</div>

<div class="lang-zh">

# Landscape Toolkit (景观工具箱) - 工程规划文档

**当前版本: 1.2.3**

 > **v1.2.3 更新**: 统一双语文档结构，新增术语表与导航栏，并清理了旧版目录。

## 1. 项目愿景 (Project Vision)
本项目旨在为景观设计师提供一套基于 Rhino/Grasshopper 的高效建模与分析工具集。核心理念是从**二维线稿**出发，通过参数化算法实时推导生成高质量的**三维四边面 (Quad Mesh) 路网**、地形以及景观构筑物，并集成多维度的环境分析功能，实现“设计-分析-优化”的闭环工作流。

## 2. 核心功能模块 (Core Modules)

### 2.1 路网生成 (Road Network Generation)
*   **输入**: 平面道路中心线 (Curve)。
*   **核心算法**:
    *   **四边面拓扑 (Quad Topology)**: 摒弃传统的 Trimmed Surface，采用纯网格建模，确保模型轻量且易于细分 (SubD)。
    *   **实时推导**: 拖动中心线，路网宽度、倒角、路口连接实时更新。
    *   **层级管理**: 支持不同等级道路（主路、次路、小径）的材质与构造衔接。
    *   **多级路网**: 专用的多级路网组件，支持 L1/L2/L3 优先路口与喇叭口连接。
    *   **仿生微调 (Wooly Path)**: 引入羊毛线/粘菌算法，对生硬的直线进行自然化平滑处理。

### 2.2 场地与地形 (Surfaces & Terrain)
*   **围合生成**: 自动识别路网围合的封闭区域，生成地块 (Plot)。
*   **最简曲面 (Minimal Surface)**: 基于边界线生成光顺的曲面，支持 SubD 细分，确保边缘与道路完美衔接。
*   **竖向设计**: 支持通过控制点或坡度参数，实时调整地形起伏。

### 2.3 景观构筑物 (Landscape Features)
*   **参数化组件**:
    *   **台阶 (Steps)**: 自动适应坡度生成台阶，平衡踏步宽度与高度计算。
    *   **挡土墙 (Walls)**: 基于高差自动生成挡土墙或种植池边缘。
    *   **栈道 (Boardwalks)**: 沿线生成带有支撑柱和扶手的架空栈道。
    *   **散布系统 (Scatter System)**: 沿路边自动散布路灯、座椅和行道树。

### 2.4 环境分析 (Environmental Analysis)
*   **多目标优化**:
    *   **坡度/土方**: 计算填挖方量以优化场地平整度。
    *   **空间句法**: 分析网络集成度和穿行度，评估可达性。
    *   **水文分析**: 汇水区识别、地表径流模拟。
    *   **微气候**: 风环境（简化 CFD）、热舒适度 (UTCI)、日照时数。
    *   **固碳分析**: 基于生物量估算生态效益。

## 3. 技术架构 (Technical Architecture)

### 3.1 文件结构 (File Structure)
```
src/
├── Core/               # 几何核心算法
├── Data/               # 数据模型
├── Modeling/           # 建模模块
│   ├── Roads/          # 道路算法 (RoadNetwork, Junctions)
│   ├── Surfaces/       # 地形与地块生成 (Terrain, PlotGenerator)
│   └── Features/       # 构筑物生成 (Steps, Walls, Scatter)
├── Analysis/           # 分析模块 (Slope, Solar, Hydro, Carbon)
├── Optimization/       # 优化算法 (PathFinder, Multi-Objective)
└── UI/                 # 用户交互逻辑
```

### 3.2 关键算法实现 (Key Algorithm Implementation)
*   **路网**: 使用 `Half-Edge` 半边数据结构管理网格拓扑，确保路口处的顶点共享与法线平滑。
*   **地形拟合**: 使用 `Relaxation` 松弛算法使网格在边界约束下自然过渡。
*   **交互逻辑**: 监听 Rhino 文档事件，实现“手绘草图即时生成”。

## ⚠️ Development Status (开发状态)

> **Note**: This project is currently in **Alpha** stage. All tools and components are under active development and have not yet undergone rigorous code review or testing. APIs and functionality are subject to change.
>
> **注意**: 本项目目前处于 **Alpha** 开发阶段。所有工具和组件均在积极开发中，尚未经过严格的代码审查或测试。API 和功能可能会随时调整。

- [-] **Phase 1**: Basic Quad Mesh logic & Junctions (基础四边面逻辑与路口解算) - *Under Review / 审查中*
- [-] **Phase 2**: SubD algorithms for Surfaces (曲面 SubD 算法) - *Experimental / 实验性*
- [-] **Phase 3**: Vertical Features - Steps, Walls, Boardwalks (竖向构筑物) - *Refining / 优化中*
- [-] **Phase 4**: Environmental Analysis Tools (环境分析工具) - *Testing / 测试中*
- [ ] **Phase 5**: GIS Integration & Construction Drawings (GIS 集成与施工图生成)

## 5. 分支策略 (Branch Strategy)

*   **`main`**: **稳定版 / 发布版**。仅包含经过验证的稳定版本。更新与新版本发布同步。
*   **`dev`**: **开发版 / 前沿版**。包含最新功能和改进。更新频繁。

---
*维护者: Landscape Toolkit 开发团队*
*最后更新: 2026-02-21*

</div>
