<div class="lang-en">

# Rhino Picker

**Category:** Landscape > Integration
**Nickname:** Picker

## 1. Introduction
The **Rhino Picker** component allows users to filter and select geometry directly from the Rhino document within Grasshopper by Layer, Name, or Type.

## 2. Inputs
*   **Layer (L)**: List of layer names (Optional).
*   **Name (N)**: Object name filter (Supports `*` wildcard).
*   **Type (T)**: Object type (Curve, Surface, Brep, Mesh, Point, Text, etc., Default: All).
*   **Refresh (R)**: Boolean to force refresh the selection.

## 3. Outputs
*   **Geometry (G)**: List of selected geometry.
*   **Count (C)**: Number of objects selected.

## 4. Usage Example
1.  Input "Roads" to the **Layer** port to get all road curves.
2.  Input "Building_*" to the **Name** port to get all objects starting with "Building_".


</div>

<div class="lang-zh">

# Rhino Picker (对象拾取)

**分类:** Landscape > Integration
**别名:** Picker

## 1. 简介 (Introduction)
**Rhino Picker** 组件允许用户直接在 Grasshopper 中通过图层、名称或类型筛选并拾取 Rhino 文档中的几何体。

## 2. 输入 (Inputs)
*   **Layer (L)**: 图层名称列表（可选）。
*   **Name (N)**: 对象名称过滤器（支持 `*` 通配符）。
*   **Type (T)**: 对象类型（Curve, Surface, Brep, Mesh, Point, Text 等，默认 All）。
*   **Refresh (R)**: 布尔值，用于强制刷新选择。

## 3. 输出 (Outputs)
*   **Geometry (G)**: 拾取到的几何体列表。
*   **Count (C)**: 拾取到的对象数量。

## 4. 用法示例 (Usage Example)
1.  输入 "Roads" 到 Layer端口，获取所有道路曲线。
2.  输入 "Building_*" 到 Name 端口，获取所有以 Building_ 开头的对象。


</div>
