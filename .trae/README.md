# .trae Project Configuration (项目配置中心)

本目录用于存放 Trae IDE 的项目特定配置、开发规范与辅助提示词。

## Directory Structure (目录结构)

*   [**specs/**](specs/): **Feature Specifications** (功能需求文档)。
    *   存放所有待开发或正在开发的功能的详细设计文档。
    *   每个子目录代表一个独立的 Feature 或 Module。
*   [**templates/**](templates/): **Standard Templates** (标准模板)。
    *   包含需求文档、Bug 报告等标准格式模板，确保开发流程的一致性。
*   [**prompts/**](prompts/): **AI Prompts** (AI 提示词库)。
    *   存放针对代码审查、文档生成等任务的预设提示词，提升 AI 辅助效率。

## Usage (使用说明)

1.  **新建功能**: 复制 `templates/feature_spec_template.md` 到 `specs/` 下的新目录，并填写需求。
2.  **代码审查**: 使用 `prompts/code_review.md` 中的内容作为 Prompt，让 AI 对代码进行质量检查。
3.  **文档生成**: 使用 `prompts/doc_generation.md` 快速生成组件说明书。

## Active Specs (进行中的需求)

| Spec Name | Status | Description |
| :--- | :--- | :--- |
| **Advanced Road System** | ✅ Completed | 包含路网分级、广场连接与仿生路径优化。 |
| **Quad Road Network** | ✅ Completed | 基础的四边面路网生成与地块填充。 |
| **Landscape Features** | ✅ Completed | 包含台阶、挡墙、栈道、散布系统与极小曲面。 |
| **Analysis Tools** | ✅ Completed | 包含坡度、光照、风影、碳汇与水文分析。 |
| **Integration Tools** | ✅ Completed | 包含 Rhino 对象拾取器 (RhinoPicker) 与 GIS 接口。 |
| **UI Unification** | ✅ Completed (v1.2.1) | 统一插件与网页 UI，采用 3D 卡片式设计与标准色系。 |

---
*Maintained by Landscape Toolkit Dev Team*
