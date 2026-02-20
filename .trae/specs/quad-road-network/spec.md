# Quad Road Network & Plot Spec

## Why
用户需要一套基于平面线稿快速推导景观路网和地块的建模工具，核心要求是生成的模型必须是高质量的 **四边面 (Quad Mesh)** 结构，以便于后续的细化设计、倒角处理和 SubD 转换。这解决了传统 NURBS 建模布线混乱、难以进行有机形态编辑的痛点。

## What Changes
- **新增组件**: `RoadNetworkComponent` (路网生成器)
  - 输入：平面中心线 (Curve List)，路宽 (Number)，厚度/高度 (Number)。
  - 输出：RoadMesh (整体路网)，Junctions (交叉口Mesh列表)，Streets (街道Mesh列表)。
  - 逻辑：基于中心线生成带宽度的平面区域，识别交叉口，分别构建街道段和交叉口段，最终合并或独立输出，便于材质赋予。
- **新增组件**: `PlotGeneratorComponent` (地块生成器)
  - 输入：路网边缘线或围合区域的边界线 (Curve List)。
  - 逻辑：识别封闭区域，生成平面，应用 Quad Remesh 算法将其转化为高质量的四边面网格。
- **技术关键点**:
  - 优先使用 `QuadRemesh` 算法处理不规则平面。
  - 对于规则路段，尝试使用 Sweep/Loft 保持结构化布线（如果可能，或统一使用 Remesh 简化流程）。
  - 确保生成的 Mesh 是单纯的 Quad Mesh，无三角面（或极少）。

## Impact
- **Affected Specs**: 扩展 Modeling 模块能力。
- **Affected Code**: 
  - `src/Modeling/RoadNetworkComponent.cs` (New)
  - `src/Modeling/PlotGeneratorComponent.cs` (New)
  - `src/Core/Utils.cs` (可能需要增加曲线布尔运算辅助函数)

## ADDED Requirements
### Requirement: Quad Road Network Generation
系统应提供一个组件，接受一组曲线作为道路中心线。
#### Scenario: Basic Road
- **WHEN** 用户输入一组相交或不相交的曲线，并设置路宽为 5m。
- **THEN** 系统生成宽度为 5m 的道路网格，且网格拓扑为四边面。
- **THEN** 交叉口应平滑处理或布尔合并。

#### Scenario: Component Separation
- **WHEN** 用户需要对路口和路段赋予不同材质（如路口斑马线，路段沥青）。
- **THEN** 组件输出独立的 `Junctions` 和 `Streets` 网格列表。
- **THEN** 这些网格在几何上是连续的，且都为 Quad Mesh。

### Requirement: Quad Plot Generation (Infill)
系统应提供一个组件，识别由曲线围合的封闭区域，并生成四边面网格。
#### Scenario: Enclosed Area
- **WHEN** 用户输入一组构成封闭环的曲线（如路网边缘）。
- **THEN** 系统生成该区域的覆盖网格。
- **THEN** 网格结构为均匀的四边面（Quad Mesh），边缘贴合输入曲线。

## MODIFIED Requirements
无（这是全新功能模块）。
