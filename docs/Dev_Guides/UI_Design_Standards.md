<div class="lang-en">

# UI Design & Development Standards Manual

This manual outlines the standards for User Interface (UI) design and development within the **Landscape Toolkit**. It ensures consistency across all Grasshopper components and provides a predictable experience for users.

## 1. Visual Identity (Iconography)

All icons are programmatically generated in `src/Icons.cs` to ensure high resolution and style consistency.

### 1.1 Design Philosophy
The UI follows a **"Tile"** design language:
*   **Background**: A rounded rectangle representing the component's category.
*   **Foreground**: A high-contrast, simplified vector icon representing the tool's function.

### 1.2 Color Coding
We use a strict color-coding system to differentiate component categories:

| Category | Color (RGB/Name) | Hex | Usage |
| :--- | :--- | :--- | :--- |
| **Modeling** | `ForestGreen` (34, 139, 34) | `#228B22` | Geometry generation (Roads, Terrain, Walls). |
| **Analysis** | `OrangeRed` (255, 69, 0) | `#FF4500` | Environmental analysis (Slope, Solar). |
| **Hydrology** | `DodgerBlue` (30, 144, 255) | `#1E90FF` | Water-related analysis. |
| **Optimization** | `Purple` (128, 0, 128) | `#800080` | Algorithms (Path finding, smoothing). |
| **Utility** | `Gray` (128, 128, 128) | `#808080` | Helper tools (Pickers, Converters). |

### 1.3 Icon Style
*   **Size**: 24x24 pixels (Standard GH icon size), scalable to 128x128 for Web.
*   **Background Shape**: Full bleed Rounded Rectangle (4px radius).
*   **Visual Effects**:
    *   **3D Gradient**: Linear Gradient from Top-Left (Base Color + 40 brightness) to Bottom-Right (Base Color - 40 brightness).
    *   **Border**: 1px Inset Border (Dark Color) for definition.
*   **Foreground**:
    *   **Scale**: Scaled by 0.8x and centered to fit within the background.
    *   **Color**: Use **White** (`#FFFFFF`) or high-contrast colors for primary elements.
    *   Avoid complex gradients.
    *   Lines should be at least 1.5px thick (at 24px scale) for readability.

### 1.4 Implementation
To add a new icon:
1.  Open `src/Icons.cs`.
2.  Define the drawing logic in a static `Action<Graphics>`.
3.  Use the `WithBackground` helper to apply the category color.
    ```csharp
    public static Bitmap MyComponent => CreateIcon(
        WithBackground(CategoryColors.Modeling, DrawMyComponent)
    );
    ```

## 2. Component Design (UX)

### 2.1 Naming Conventions
*   **Name**: Full descriptive name (e.g., "Quad Road Network").
*   **Nickname**: Short, camelCase name without spaces (e.g., "QuadRoad").
*   **Category**: "Landscape".
*   **Sub-category**: "Modeling", "Analysis", "Optimization", "Workflow".

### 2.2 Input Parameters
*   **Order**:
    1.  **Main Geometry** (Curves, Points, Breps).
    2.  **Key Parameters** (Width, Height, Count).
    3.  **Options/Settings** (Booleans, Modes).
*   **Defaults**: Always provide sensible default values.

### 2.3 Output Parameters
*   **Order**:
    1.  **Main Result** (Mesh, Curve).
    2.  **Secondary Geometry** (Graph, Junctions).
    3.  **Data/Metrics** (Area, Count, Report).

## 3. Web Documentation
The web documentation (`docs/index.html`) mirrors the plugin UI:
*   Uses the same generated icons (exported via `tools/IconExporter.cs`).
*   Uses the same color scheme for headers and accents.

---
*Maintained by Landscape Toolkit Dev Team*


</div>

<div class="lang-zh">

# UI 设计与开发标准手册

本手册概述了 **Landscape Toolkit** 界面 (UI) 设计与开发的标准。它旨在确保所有 Grasshopper 组件的一致性，并为用户提供可预测的体验。

## 1. 视觉识别 (图标设计)

所有图标均在 `src/Icons.cs` 中以编程方式生成，以确保持高分辨率和风格统一。

### 1.1 设计理念
UI 遵循 **"卡片/磁贴 (Tile)"** 设计语言：
*   **背景**：代表组件分类的圆角矩形。
*   **前景**：代表工具功能的高对比度矢量图标。

### 1.2 颜色编码 (Color Coding)
我们使用严格的颜色编码系统来区分组件类别：

| 类别 (Category) | 颜色 (Color) | 十六进制 (Hex) | 用途 (Usage) |
| :--- | :--- | :--- | :--- |
| **建模 (Modeling)** | `ForestGreen` (森林绿) | `#228B22` | 几何生成 (道路, 地形, 墙体)。 |
| **分析 (Analysis)** | `OrangeRed` (橙红) | `#FF4500` | 环境分析 (坡度, 日照)。 |
| **水文 (Hydrology)** | `DodgerBlue` (宝蓝) | `#1E90FF` | 水分析相关。 |
| **优化 (Optimization)** | `Purple` (紫色) | `#800080` | 算法 (路径寻找, 平滑)。 |
| **工具 (Utility)** | `Gray` (灰色) | `#808080` | 辅助工具 (拾取器, 转换器)。 |

### 1.3 图标风格
*   **尺寸**：24x24 像素 (标准 GH 图标尺寸)，可缩放至 128x128 用于网页。
*   **背景形状**：全尺寸圆角矩形 (4px 圆角)。
*   **视觉效果**：
    *   **3D 渐变**：从左上角 (基色 +40 亮度) 到右下角 (基色 -40 亮度) 的线性渐变。
    *   **描边**：1px 内嵌 (Inset) 深色描边，增强轮廓感。
*   **前景**：
    *   **缩放**：缩放 0.8 倍并居中显示，以适应背景。
    *   **颜色**：主要元素使用 **白色** (`#FFFFFF`) 或高对比度颜色。
    *   避免复杂的渐变。
    *   线条宽度至少为 1.5px (在 24px 比例下) 以保证可读性。

### 1.4 实现方法
添加新图标的步骤：
1.  打开 `src/Icons.cs`。
2.  在静态 `Action<Graphics>` 中定义绘图逻辑。
3.  使用 `WithBackground` 辅助方法应用分类颜色。
    ```csharp
    public static Bitmap MyComponent => CreateIcon(
        WithBackground(CategoryColors.Modeling, DrawMyComponent)
    );
    ```

## 2. 组件设计 (UX)

### 2.1 命名规范
*   **名称 (Name)**：完整的描述性名称 (例如 "Quad Road Network")。
*   **昵称 (Nickname)**：简短的驼峰式名称，无空格 (例如 "QuadRoad")。
*   **主类别**："Landscape"。
*   **子类别**："Modeling" (建模), "Analysis" (分析), "Optimization" (优化), "Workflow" (工作流)。

### 2.2 输入参数
*   **顺序**：
    1.  **主要几何体** (曲线, 点, Brep)。
    2.  **关键参数** (宽度, 高度, 数量)。
    3.  **选项/设置** (布尔值, 模式)。
*   **默认值**：始终为数值输入提供合理的默认值。

### 2.3 输出参数
*   **顺序**：
    1.  **主要结果** (网格, 曲线)。
    2.  **次要几何体** (图表, 连接点)。
    3.  **数据/指标** (面积, 数量, 报告)。

## 3. 网页文档
网页文档 (`docs/index.html`) 与插件 UI 保持一致：
*   使用相同的生成图标 (通过 `tools/IconExporter.cs` 导出)。
*   标题和强调色使用相同的配色方案。

---
*Landscape Toolkit 开发团队维护*


</div>
