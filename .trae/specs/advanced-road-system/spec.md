# Advanced Road System & Project Optimization Spec

## Why
用户希望将项目升级为一个功能完善、结构清晰、具备高级生成逻辑的景观设计系统。核心痛点在于：
1.  **缺乏分级体系**：现有路网无法区分主次干道，材质和图层单一。
2.  **缺乏衔接逻辑**：道路与广场（硬质铺装）之间缺乏自然的过渡和倒角处理。
3.  **缺乏智能优化**：线稿依赖手工绘制，缺乏仿生学（如羊毛算法/最短路径聚合）的自动优化能力。
4.  **文档晦涩**：缺乏详细的中文原理解析，用户难以理解底层逻辑。

## What Changes
- **Project Structure Optimization**:
  - `src/Optimization`: 新增优化算法模块（如曲线松弛、路径聚合）。
  - `src/Data`: 新增数据处理模块（图层管理、属性映射）。
  - `docs/cn`: 新增详细的中文原理文档。
- **New Features**:
  - **Road Hierarchy System**: 支持定义道路等级（宽度、倒角半径、材质ID），并自动分图层输出。
  - **Plaza Connection**: 自动识别道路端点与广场边界的接触，生成平滑的倒角连接。
  - **Bio-Path Optimizer**: 实现基于力导向或松弛算法的线稿优化，模拟人流自然路径（仿生学基础）。
  - **Data Integration Interface**: 预留接口，支持从 CSV/JSON 导入人流数据来驱动路网宽度。

## Impact
- **Affected Specs**: 扩展 `Modeling` 和 `Core` 模块。
- **Affected Code**: 
  - `src/Modeling/AdvancedRoadComponent.cs` (New)
  - `src/Optimization/PathOptimizer.cs` (New)
  - `src/Data/LayerManager.cs` (New)
  - `README.md` (Rewrite)

## ADDED Requirements
### Requirement: Advanced Road Network
系统应提供支持分级的路网生成器。
#### Scenario: Hierarchy
- **WHEN** 用户输入多组曲线，分别标记为 "Primary" (主路) 和 "Secondary" (次路)。
- **THEN** 主路宽度更大，次路宽度较小。
- **THEN** 主次路交叉处生成符合等级的倒角。
- **THEN** 输出模型自动分层（如 "L-Road-Primary", "L-Road-Secondary"）。

### Requirement: Plaza Integration
系统应处理道路与广场的连接。
#### Scenario: Road meets Plaza
- **WHEN** 道路中心线末端接近广场边缘。
- **THEN** 道路轮廓与广场轮廓进行布尔并集（Union）。
- **THEN** 连接处生成倒角（Fillet），半径可调。

### Requirement: Bio-Inspired Path Optimization
系统应提供线稿优化工具。
#### Scenario: Wooly Path
- **WHEN** 用户输入一组杂乱的路径线稿。
- **THEN** 算法应用松弛（Relaxation）或聚合逻辑，使线条更加顺滑、符合自然行走趋势。

## MODIFIED Requirements
- `README.md`: 必须包含核心算法的中文图解说明（如 QuadRemesh 原理、力导向算法原理）。
