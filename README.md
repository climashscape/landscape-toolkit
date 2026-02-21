# <img src="docs/assets/icons/logo.png" width="32" valign="middle"/> Landscape Toolkit (景观工具箱)

[![Version](https://img.shields.io/badge/Version-1.2.3-blue.svg)](CHANGELOG.md)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Documentation](https://img.shields.io/badge/Docs-Interactive-green.svg)](https://climashscape.github.io/landscape-toolkit/)

**High-Performance Landscape Architecture Toolkit for Rhino + Grasshopper**
**专为景观设计师打造的全流程参数化设计系统**

> **New in v1.2.3**: Unified bilingual documentation structure, added Glossary/Navbar, and cleaned up legacy directories.

---

### 🚀 [**Click Here for Interactive Documentation & Showcase**](https://climashscape.github.io/landscape-toolkit/)
访问交互式文档主页，体验更直观的工具展示与版本历史。

---

## 🔭 Project Vision (项目愿景)

This project aims to provide a set of efficient modeling and analysis tools for landscape designers based on Rhino/Grasshopper. The core philosophy is to generate high-quality **3D Quad Mesh Road Networks**, terrain, and landscape structures in real-time from **2D Sketches** through parametric algorithms, integrating multi-dimensional environmental analysis to achieve a "Design-Analysis-Optimization" closed-loop workflow.

本项目旨在为景观设计师提供一套基于 Rhino/Grasshopper 的高效建模与分析工具集。核心理念是从**二维线稿**出发，通过参数化算法实时推导生成高质量的**三维四边面 (Quad Mesh) 路网**、地形以及景观构筑物，并集成多维度的环境分析功能，实现“设计-分析-优化”的闭环工作流。

## 🛠️ Toolbox Showcase (工具箱展示)

### 🛣️ Modeling (核心建模)

| Icon | Component | Description (English/中文) | Docs |
| :---: | :--- | :--- | :--- |
| <img src="docs/assets/icons/road_network.png" width="40"/> | **Quad Road Network** | Generate high-quality quad mesh road networks with SubD support.<br>生成高质量的全四边面路网，支持分级与 SubD 工作流。 | [Link](docs/Components/QuadRoadNetwork.md) |
| <img src="docs/assets/icons/road_network.png" width="40"/> | **Multi-Level Road** | L1/L2/L3 hierarchy road generation with priority junctions.<br>支持 L1/L2/L3 分级的多级路网生成系统，具备优先路口处理。 | [Link](docs/Components/MultiLevelRoad.md) |
| <img src="docs/assets/icons/terrain.png" width="40"/> | **Landscape Terrain** | Generate smooth "Class-A" terrain surfaces from contours/points.<br>从等高线或散点生成光顺的 "Class-A" 地形曲面。 | [Link](docs/Components/Terrain.md) |
| <img src="docs/assets/icons/plot_generator.png" width="40"/> | **Plot Generator** | Automatically extract plots from road networks.<br>自动提取路网围合区域，生成规整的地块网格。 | [Link](docs/Components/PlotGenerator.md) |
| <img src="docs/assets/icons/steps.png" width="40"/> | **Landscape Steps** | Parametric steps generation along paths adapted to slope.<br>沿路径自动生成适应坡度的参数化台阶。 | [Link](docs/Components/Steps.md) |
| <img src="docs/assets/icons/wall.png" width="40"/> | **Landscape Wall** | Quick generation of retaining walls with thickness.<br>快速生成具有厚度的挡土墙或种植池边缘。 | [Link](docs/Components/Wall.md) |
| <img src="docs/assets/icons/boardwalk.png" width="40"/> | **Boardwalk** | Elevated boardwalks with supports and railings.<br>生成带有支撑柱和扶手的架空栈道。 | [Link](docs/Components/Boardwalk.md) |
| <img src="docs/assets/icons/scatter.png" width="40"/> | **Scatter Elements** | Rule-based scattering of trees, lights, or benches.<br>依据规则随机散布乔木、路灯或座椅等配景。 | [Link](docs/Components/Scatter.md) |
| <img src="docs/assets/icons/minimal_surface.png" width="40"/> | **Minimal Surface** | Tensile structures based on relaxation algorithms.<br>基于松弛算法生成极小曲面张拉结构。 | [Link](docs/Components/MinimalSurface.md) |
| <img src="docs/assets/icons/rhino_picker.png" width="40"/> | **Rhino Picker** | Select Rhino objects by Layer or Name directly in GH.<br>直接在 GH 中按图层或名称拾取 Rhino 对象。 | [Link](docs/Components/RhinoPicker.md) |

### 🦠 Optimization (仿生优化)

| Icon | Component | Description (English/中文) | Docs |
| :---: | :--- | :--- | :--- |
| <img src="docs/assets/icons/wooly_path_optimizer.png" width="40"/> | **Wooly Path Optimizer** | Slime Mold algorithm for organic path networks.<br>基于粘菌算法 (Slime Mold) 生成有机的仿生路径网络。 | [Link](docs/Components/WoolyPathOptimizer.md) |
| <img src="docs/assets/icons/path_optimizer.png" width="40"/> | **Bio-Path Optimizer** | Laplacian smoothing to fix hand-drawn jitters.<br>基于拉普拉斯平滑的曲线优化工具，修复手绘抖动。 | [Link](docs/Components/PathOptimizer.md) |

### 📐 Analysis (环境分析)

| Icon | Component | Description (English/中文) | Docs |
| :---: | :--- | :--- | :--- |
| <img src="docs/assets/icons/slope_analysis.png" width="40"/> | **Slope Analysis** | Real-time terrain slope visualization.<br>实时地形坡度可视化分析。 | [Link](docs/Components/SlopeAnalysis.md) |
| <img src="docs/assets/icons/analysis_impl.png" width="40"/> | **Space Syntax** | Integration, Choice, and Depth analysis for accessibility.<br>空间句法分析（集成度、穿行度、深度），评估路网可达性。 | [Link](docs/Components/SpaceSyntax.md) |
| <img src="docs/assets/icons/solar_analysis.png" width="40"/> | **Solar Exposure** | Fast solar exposure estimation based on normals.<br>基于法线的快速光照暴露度估算。 | [Link](docs/Components/SolarAnalysis.md) |
| <img src="docs/assets/icons/wind_shadow_analysis.png" width="40"/> | **Wind Shadow** | Simplified wind environment/occlusion analysis.<br>基于射线投射的简易风环境/遮挡分析。 | [Link](docs/Components/WindShadowAnalysis.md) |
| <img src="docs/assets/icons/carbon_analysis.png" width="40"/> | **Carbon Analysis** | Estimate carbon sequestration of trees and green space.<br>估算乔木与绿地的固碳效益。 | [Link](docs/Components/CarbonAnalysis.md) |
| <img src="docs/assets/icons/hydrology.png" width="40"/> | **Runoff Simulation** | Surface runoff and catchment paths (Steepest Descent).<br>模拟地表径流与汇水路径 (最速下降法)。 | [Link](docs/Components/Hydrology.md) |

## ⚠️ Development Status (开发状态)

> **Note**: This project is currently in **Beta** stage. Core features are stable, but APIs may still undergo minor changes.
>
> **注意**: 本项目目前处于 **Beta** 开发阶段。核心功能已趋于稳定，但 API 仍可能进行微调。

- [x] **Phase 1**: Basic Quad Mesh logic & Junctions (基础四边面逻辑与路口解算) - *Completed / 已完成*
- [x] **Phase 2**: SubD algorithms for Surfaces (曲面 SubD 算法) - *Completed / 已完成*
- [x] **Phase 3**: Vertical Features - Steps, Walls, Boardwalks (竖向构筑物) - *Completed / 已完成*
- [x] **Phase 4**: Environmental Analysis Tools (环境分析工具) - *Completed / 已完成*
- [ ] **Phase 5**: GIS Integration & Construction Drawings (GIS 集成与施工图生成) - *In Progress / 开发中*

## 🔄 Core Workflow (核心工作流)

1.  **Sketch**: 设计师绘制粗略的路径草图。
2.  **Optimize**: 使用 `Bio-Path` 或 `Wooly Path` 优化路径形态。
3.  **Network**: 输入 `Quad Road Network` 生成三维路网。
4.  **Plots**: 使用 `Plot Generator` 提取地块。
5.  **Terrain**: 使用 `Terrain` 生成光顺地形，并与路网缝合。
6.  **Features**: 自动生成台阶 (`Steps`) 和挡墙 (`Wall`)。
7.  **Analyze**: 实时评估坡度 (`Slope`) 和排水 (`Hydrology`)，反哺设计调整。

## 📦 Build & Release (构建与发布)

To build the project and create a release package:
运行以下命令构建项目并生成发布包：

```powershell
.\build.ps1
```

The output `.gha` and `.zip` files will be in the `dist/` directory.
输出文件位于 `dist/` 目录。

## 🌿 Branch Strategy (分支策略)

*   **`main`**: **Stable / Release**. Only contains verified, stable versions. Updates coincide with new version releases.
*   **`dev`**: **Development / Bleeding Edge**. Contains the latest features and improvements. Updates frequently.

---

*Maintained by Landscape Toolkit Dev Team*
