<div class="lang-en">

# Changelog

All notable changes to the **Landscape Toolkit** project will be documented in this file.
</div>
<div class="lang-zh">

# 更新日志

本项目 **Landscape Toolkit** 的所有重要更改都将记录在此文件中。
</div>

## [1.2.3] - 2026-02-21

### <span class="lang-en">Added</span><span class="lang-zh">新增</span>
- **<span class="lang-en">Documentation Assets</span><span class="lang-zh">文档资源</span>**:
    - <span class="lang-en">Added `glossary.md` with key domain definitions.</span><span class="lang-zh">新增包含关键领域定义的术语表 (`glossary.md`)。</span>
    - <span class="lang-en">Added `_navbar.md` for improved navigation structure.</span><span class="lang-zh">新增用于改善导航结构的 `_navbar.md`。</span>
    - <span class="lang-en">Added new icon resources for documentation UI.</span><span class="lang-zh">添加用于文档界面的新图标资源。</span>
- **<span class="lang-en">Dev Tools</span><span class="lang-zh">开发工具</span>**:
    - <span class="lang-en">Added `tools/check_links.py` for automated link validation in documentation.</span><span class="lang-zh">新增 `tools/check_links.py` 用于自动化文档链接验证。</span>

### <span class="lang-en">Changed</span><span class="lang-zh">变更</span>
- **<span class="lang-en">Documentation Architecture</span><span class="lang-zh">文档架构</span>**:
    - <span class="lang-en">Transitioned to a unified bilingual file structure, merging separate English/Chinese directories.</span><span class="lang-zh">过渡到统一的双语文件结构，合并了独立的中英文目录。</span>
    - <span class="lang-en">Standardized version numbers across all documentation files to v1.2.3.</span><span class="lang-zh">将所有文档文件的版本号标准化为 v1.2.3。</span>
- **<span class="lang-en">Quality Assurance</span><span class="lang-zh">质量保证</span>**:
    - <span class="lang-en">Fixed broken relative links in changelog and component docs.</span><span class="lang-zh">修复了更新日志和组件文档中的断裂相对链接。</span>
    - <span class="lang-en">Verified completeness of Chinese translations across all components.</span><span class="lang-zh">验证了所有组件中文翻译的完整性。</span>

### <span class="lang-en">Removed</span><span class="lang-zh">移除</span>
- **<span class="lang-en">Legacy Files</span><span class="lang-zh">遗留文件</span>**: <span class="lang-en">Deleted obsolete `docs/en` and `docs/zh` directories.</span><span class="lang-zh">删除了过时的 `docs/en` 和 `docs/zh` 目录。</span>

## [1.2.2] - 2026-02-20

### <span class="lang-en">Changed</span><span class="lang-zh">变更</span>
- **<span class="lang-en">Documentation</span><span class="lang-zh">文档</span>**:
    - <span class="lang-en">Synchronized version numbers across all documentation files (v1.2.2).</span><span class="lang-zh">同步所有文档文件的版本号 (v1.2.2)。</span>
    - <span class="lang-en">Added missing documentation for **Multi-Level Road Network** and **Space Syntax Analysis**.</span><span class="lang-zh">补充了 **多级路网** 和 **空间句法分析** 的文档。</span>
    - <span class="lang-en">Updated project introductory text and progress tracking.</span><span class="lang-zh">更新了项目介绍文本和进度跟踪。</span>
    - <span class="lang-en">Verified Chinese/English translations and file links.</span><span class="lang-zh">验证了中英文翻译和文件链接。</span>

## [1.2.1] - 2026-02-20

### <span class="lang-en">Changed</span><span class="lang-zh">变更</span>
- **<span class="lang-en">UI & Iconography</span><span class="lang-zh">UI 与图标</span>**:
    - **<span class="lang-en">Unified Design Language</span><span class="lang-zh">统一设计语言</span>**: <span class="lang-en">Adopted a "Tile" style with category-specific rounded rectangle backgrounds.</span><span class="lang-zh">采用“磁贴”风格，使用特定类别的圆角矩形背景。</span>
    - **<span class="lang-en">3D Visual Effects</span><span class="lang-zh">3D 视觉效果</span>**: <span class="lang-en">Added linear gradients and inset borders to icons for a subtle 3D/tactile feel.</span><span class="lang-zh">为图标添加线性渐变和内嵌边框，增加微妙的 3D 触感。</span>
    - **<span class="lang-en">Color Coding</span><span class="lang-zh">颜色编码</span>**: <span class="lang-en">Standardized colors for Modeling (Green), Analysis (OrangeRed), Hydrology (Blue), Optimization (Purple), and Utility (Gray).</span><span class="lang-zh">标准化颜色编码：建模（绿色）、分析（橙红）、水文（蓝色）、优化（紫色）和工具（灰色）。</span>
    - **<span class="lang-en">High-Res Support</span><span class="lang-zh">高清支持</span>**: <span class="lang-en">All icons are now programmatically generated at high resolution (scalable to 128x128) for consistent web documentation.</span><span class="lang-zh">所有图标均以高分辨率编程生成（可缩放至 128x128），以确保网页文档的一致性。</span>
- **<span class="lang-en">Documentation</span><span class="lang-zh">文档</span>**:
    - <span class="lang-en">Updated [UI Design Standards](docs/Dev_Guides/UI_Design_Standards.md) with new specifications.</span><span class="lang-zh">更新了 [UI 设计规范](docs/Dev_Guides/UI_Design_Standards.md) 的新规格。</span>
    - <span class="lang-en">Added Chinese translation for UI Standards ([UI_Design_Standards_zh.md](docs/Dev_Guides/UI_Design_Standards.md)).</span><span class="lang-zh">添加了 UI 规范的中文翻译 ([UI_Design_Standards_zh.md](docs/Dev_Guides/UI_Design_Standards.md))。</span>

## [1.2.0] - 2026-02-20

### <span class="lang-en">Added</span><span class="lang-zh">新增</span>
- **<span class="lang-en">Integration</span><span class="lang-zh">集成</span>**:
    - `RhinoPickerComponent`: <span class="lang-en">Select Rhino objects by Layer, Name, or Type directly within Grasshopper.</span><span class="lang-zh">在 Grasshopper 中直接通过图层、名称或类型选择 Rhino 对象。</span>
- **<span class="lang-en">Analysis</span><span class="lang-zh">分析</span>**:
    - `CarbonAnalysisComponent`: <span class="lang-en">Estimates carbon sequestration based on trees and green areas.</span><span class="lang-zh">基于树木和绿地面积估算碳汇。</span>
    - `WindShadowAnalysisComponent`: <span class="lang-en">Simplified wind analysis using raycasting to visualize wind shadows.</span><span class="lang-zh">使用光线投射法简化风环境分析，可视化风影区域。</span>
- **<span class="lang-en">Road Network System</span><span class="lang-zh">路网系统</span>**:
    - `RoadNetworkComponent`: <span class="lang-en">Added `Junctions` and `Streets` outputs to support better hierarchy management and material assignment.</span><span class="lang-zh">添加 `Junctions` 和 `Streets` 输出，以支持更好的层级管理和材质分配。</span>

## [1.1.0] - 2026-02-20

### <span class="lang-en">Added</span><span class="lang-zh">新增</span>
- **<span class="lang-en">Features & Planting</span><span class="lang-zh">构筑物与种植</span>**:
    - `BoardwalkComponent`: <span class="lang-en">Generates raised boardwalks with customizable width, supports, and railings.</span><span class="lang-zh">生成具有可自定义宽度、支撑和栏杆的架空栈道。</span>
    - `ScatterComponent`: <span class="lang-en">Rule-based distribution system for trees, street lights, and benches along paths or surfaces.</span><span class="lang-zh">基于规则的沿路径或表面分布树木、路灯和长椅的散布系统。</span>
- **<span class="lang-en">Surface Modeling</span><span class="lang-zh">曲面建模</span>**:
    - `MinimalSurfaceComponent`: <span class="lang-en">Creates minimal surfaces (tensile structures) from boundary curves using relaxation algorithms.</span><span class="lang-zh">使用松弛算法基于边界曲线创建极小曲面（张拉结构）。</span>
- **<span class="lang-en">Integration</span><span class="lang-zh">集成</span>**:
    - `GISConnector`: <span class="lang-en">Initial framework for future GIS data integration (Placeholder).</span><span class="lang-zh">未来 GIS 数据集成的初始框架（占位符）。</span>

## [1.0.0] - 2026-02-20

### <span class="lang-en">Added</span><span class="lang-zh">新增</span>
- **<span class="lang-en">Optimization Module</span><span class="lang-zh">优化模块</span>**:
    - `WoolyPathOptimizer`: <span class="lang-en">Added bio-mimetic path finding algorithm (Slime Mold) for organic path generation.</span><span class="lang-zh">添加仿生寻路算法（粘菌）用于生成有机路径。</span>
    - `PathOptimizer`: <span class="lang-en">Added Laplacian smoothing for curve optimization.</span><span class="lang-zh">添加用于曲线优化的拉普拉斯平滑。</span>
- **<span class="lang-en">Road Network System</span><span class="lang-zh">路网系统</span>**:
    - `QuadRoadGenerator`: <span class="lang-en">Core logic for generating clean Quad-Mesh based road networks.</span><span class="lang-zh">生成基于洁净四边面网格路网的核心逻辑。</span>
    - `RoadNetworkComponent`: <span class="lang-en">Updated to support **variable road widths** (Road Hierarchy).</span><span class="lang-zh">更新以支持 **可变路宽**（道路层级）。</span>
    - `Junction Logic`: <span class="lang-en">Implemented robust handling for 3-way, 4-way, and N-way intersections with specific topology patches.</span><span class="lang-zh">实现了针对三岔、十字和多岔路口的稳健拓扑修补逻辑。</span>
- **<span class="lang-en">Terrain & Surfaces</span><span class="lang-zh">地形与地表</span>**:
    - `PlotGenerator`: <span class="lang-en">Added algorithm to identify enclosed regions and generate quad mesh plots.</span><span class="lang-zh">添加识别封闭区域并生成四边面网格地块的算法。</span>
    - `TerrainComponent`: <span class="lang-en">Implemented Delaunay triangulation + QuadRemesh workflow for high-quality terrain.</span><span class="lang-zh">实现了 Delaunay 三角剖分 + QuadRemesh 工作流以生成高质量地形。</span>
- **<span class="lang-en">Landscape Features</span><span class="lang-zh">景观构筑物</span>**:
    - `StepsComponent`: <span class="lang-en">Parametric step generation adapting to path slope.</span><span class="lang-zh">适应路径坡度的参数化台阶生成。</span>
    - `WallComponent`: <span class="lang-en">Vertical wall generation with thickness and path following.</span><span class="lang-zh">具有厚度和路径跟随的垂直墙体生成。</span>
- **<span class="lang-en">Environmental Analysis</span><span class="lang-zh">环境分析</span>**:
    - `SlopeAnalysis`: <span class="lang-en">Real-time slope visualization with color gradient.</span><span class="lang-zh">具有颜色渐变的实时坡度可视化。</span>
    - `SolarAnalysis`: <span class="lang-en">Solar exposure estimation based on surface normals.</span><span class="lang-zh">基于曲面法线的日照暴露估算。</span>
    - `Hydrology`: <span class="lang-en">Surface runoff simulation using steepest descent algorithm (Raindrop tracing).</span><span class="lang-zh">使用最速下降算法（雨滴追踪）的地表径流模拟。</span>

### <span class="lang-en">Changed</span><span class="lang-zh">变更</span>
- **<span class="lang-en">Architecture</span><span class="lang-zh">架构</span>**:
    - <span class="lang-en">Refactored project structure to separate `Core`, `Data`, `Modeling`, `Analysis`, and `Optimization` namespaces.</span><span class="lang-zh">重构项目结构，分离 `Core`、`Data`、`Modeling`、`Analysis` 和 `Optimization` 命名空间。</span>
    - <span class="lang-en">Moved `RoadGraph` and related data models to `src/Data/Graph`.</span><span class="lang-zh">将 `RoadGraph` 和相关数据模型移动到 `src/Data/Graph`。</span>
- **<span class="lang-en">Documentation</span><span class="lang-zh">文档</span>**:
    - <span class="lang-en">Restructured `docs/` folder into `Components`, `Workflows`, and `Core_Logic`.</span><span class="lang-zh">将 `docs/` 文件夹重组为 `Components`、`Workflows` 和 `Core_Logic`。</span>
    - <span class="lang-en">Added comprehensive Markdown manuals for all 8 Grasshopper components.</span><span class="lang-zh">为所有 8 个 Grasshopper 组件添加了详细的 Markdown 手册。</span>
    - <span class="lang-en">Added workflow tutorials ("Sketch to Road", "Terrain & Planting").</span><span class="lang-zh">添加了工作流教程（“草图转路网”、“地形与种植”）。</span>
- **<span class="lang-en">Dev Tools</span><span class="lang-zh">开发工具</span>**:
    - <span class="lang-en">Initialized `.trae` folder with spec templates and AI prompts to standardize development.</span><span class="lang-zh">初始化 `.trae` 文件夹，包含规范模板和 AI 提示词以标准化开发。</span>

### <span class="lang-en">Fixed</span><span class="lang-zh">修复</span>
- <span class="lang-en">Resolved namespace dependencies between Data and Modeling layers.</span><span class="lang-zh">解决了数据层和建模层之间的命名空间依赖关系。</span>
