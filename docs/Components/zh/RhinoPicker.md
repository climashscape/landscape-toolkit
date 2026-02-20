# Rhino Picker (对象拾取)

**Rhino Picker** 组件允许用户直接在 Grasshopper 中通过图层、名称或类型筛选并拾取 Rhino 文档中的几何体。

## 输入 (Inputs)
*   **Layer (L)**: 图层名称列表（可选）。
*   **Name (N)**: 对象名称过滤器（支持 `*` 通配符）。
*   **Type (T)**: 对象类型（Curve, Surface, Brep, Mesh, Point, Text 等，默认 All）。
*   **Refresh (R)**: 布尔值，用于强制刷新选择。

## 输出 (Outputs)
*   **Geometry (G)**: 拾取到的几何体列表。
*   **Count (C)**: 拾取到的对象数量。

## 用法示例
1.  输入 "Roads" 到 Layer端口，获取所有道路曲线。
2.  输入 "Building_*" 到 Name 端口，获取所有以 Building_ 开头的对象。
