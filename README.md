# Landscape Toolkit (景观工具箱)

[![Version](https://img.shields.io/badge/Version-1.2.2-blue.svg)](CHANGELOG.md)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Documentation](https://img.shields.io/badge/Docs-Interactive-green.svg)](https://climashscape.github.io/landscape-toolkit/)

**High-Performance Landscape Architecture Toolkit for Rhino + Grasshopper**
**专为景观设计师打造的全流程参数化设计系统**

> **New in v1.2.2**: Comprehensive documentation update, version synchronization, and content consistency checks.

---

### 🚀 [**Click Here for Interactive Documentation & Showcase**](https://climashscape.github.io/landscape-toolkit/)
访问交互式文档主页，体验更直观的工具展示与版本历史。

---

## 🌿 Branch Strategy (分支策略)

*   **`main`**: **Stable / Release**. Only contains verified, stable versions. Updates coincide with new version releases.
*   **`dev`**: **Development / Bleeding Edge**. Contains the latest features and improvements. Updates frequently.

## 📦 Build & Release (构建与发布)

To build the project and create a release package:
运行以下命令构建项目并生成发布包：

```powershell
.\build.ps1
```

The output `.gha` and `.zip` files will be in the `dist/` directory.
输出文件位于 `dist/` 目录。

## 🛠️ Toolbox Showcase (工具箱展示)

### 🛣️ Modeling (核心建模)
| Component | Description | Docs |
| :--- | :--- | :--- |
| **Quad Road Network** | 生成高质量的全四边面路网，支持分级与 SubD 工作流。 | [English](docs/Components/en/QuadRoadNetwork.md) / [中文](docs/Components/zh/QuadRoadNetwork.md) |
| **Multi-Level Road** | 支持 L1/L2/L3 分级的多级路网生成系统，具备优先路口处理。 | [English](docs/Components/en/MultiLevelRoad.md) / [中文](docs/Components/zh/MultiLevelRoad.md) |
| **Landscape Terrain** | 从等高线或散点生成光顺的 "Class-A" 地形曲面。 | [English](docs/Components/en/Terrain.md) / [中文](docs/Components/zh/Terrain.md) |
| **Plot Generator** | 自动提取路网围合区域，生成规整的地块网格。 | [English](docs/Components/en/PlotGenerator.md) / [中文](docs/Components/zh/PlotGenerator.md) |
| **Landscape Steps** | 沿路径自动生成适应坡度的参数化台阶。 | [English](docs/Components/en/Steps.md) / [中文](docs/Components/zh/Steps.md) |
| **Landscape Wall** | 快速生成具有厚度的挡土墙或种植池边缘。 | [English](docs/Components/en/Wall.md) / [中文](docs/Components/zh/Wall.md) |
| **Boardwalk** | 生成带有支撑柱和扶手的架空栈道。 | [English](docs/Components/en/Boardwalk.md) / [中文](docs/Components/zh/Boardwalk.md) |
| **Scatter System** | 依据规则随机散布乔木、路灯或座椅等配景。 | [English](docs/Components/en/Scatter.md) / [中文](docs/Components/zh/Scatter.md) |
| **Minimal Surface** | 基于松弛算法生成极小曲面张拉结构。 | [English](docs/Components/en/MinimalSurface.md) / [中文](docs/Components/zh/MinimalSurface.md) |
| **Rhino Picker** | 直接在 GH 中按图层或名称拾取 Rhino 对象。 | [English](docs/Components/en/RhinoPicker.md) / [中文](docs/Components/zh/RhinoPicker.md) |

### 🦠 Optimization (仿生优化)
| Component | Description | Docs |
| :--- | :--- | :--- |
| **Wooly Path Optimizer** | 基于粘菌算法 (Slime Mold) 生成有机的仿生路径网络。 | [English](docs/Components/en/WoolyPathOptimizer.md) / [中文](docs/Components/zh/WoolyPathOptimizer.md) |
| **Bio-Path Optimizer** | 基于拉普拉斯平滑的曲线优化工具，修复手绘抖动。 | [English](docs/Components/en/PathOptimizer.md) / [中文](docs/Components/zh/PathOptimizer.md) |

### 📐 Analysis (环境分析)
| Component | Description | Docs |
| :--- | :--- | :--- |
| **Slope Analysis** | 实时地形坡度可视化分析。 | [English](docs/Components/en/SlopeAnalysis.md) / [中文](docs/Components/zh/SlopeAnalysis.md) |
| **Space Syntax** | 空间句法分析（集成度、穿行度、深度），评估路网可达性。 | [English](docs/Components/en/SpaceSyntax.md) / [中文](docs/Components/zh/SpaceSyntax.md) |
| **Solar Analysis** | 基于法线的快速光照暴露度估算。 | [English](docs/Components/en/SolarAnalysis.md) / [中文](docs/Components/zh/SolarAnalysis.md) |
| **Wind Shadow** | 基于射线投射的简易风环境/遮挡分析。 | [English](docs/Components/en/WindShadowAnalysis.md) / [中文](docs/Components/zh/WindShadowAnalysis.md) |
| **Carbon Analysis** | 估算乔木与绿地的固碳效益。 | [English](docs/Components/en/CarbonAnalysis.md) / [中文](docs/Components/zh/CarbonAnalysis.md) |
| **Hydrology Analysis** | 模拟地表径流与汇水路径 (最速下降法)。 | [English](docs/Components/en/Hydrology.md) / [中文](docs/Components/zh/Hydrology.md) |

---

## 🔄 Core Workflow (核心工作流)

1.  **Sketch**: 设计师绘制粗略的路径草图。
2.  **Optimize**: 使用 `Bio-Path` 或 `Wooly Path` 优化路径形态。
3.  **Network**: 输入 `Quad Road Network` 生成三维路网。
4.  **Plots**: 使用 `Plot Generator` 提取地块。
5.  **Terrain**: 使用 `Terrain` 生成光顺地形，并与路网缝合。
6.  **Features**: 自动生成台阶 (`Steps`) 和挡墙 (`Wall`)。
7.  **Analyze**: 实时评估坡度 (`Slope`) 和排水 (`Hydrology`)，反哺设计调整。

---

## 📚 Documentation Index (文档索引)

*   [**Interactive Homepage (交互式主页)**](docs/index.html)
*   [**Component Reference (运算器参考手册)**](docs/Components/README.md)
*   [**Workflows (工作流指南)**](docs/Workflows/README.md)
*   [**Core Logic (核心技术实现)**](docs/Core_Logic/README.md)
*   [**Changelog (更新日志)**](CHANGELOG.md)

---
*Maintained by Landscape Toolkit Dev Team*
