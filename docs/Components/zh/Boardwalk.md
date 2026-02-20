# Boardwalk (栈道生成器)

**Category:** Landscape > Modeling
**Nickname:** Boardwalk

## 1. 简介 (Introduction)
Boardwalk 运算器用于从路径曲线快速生成架空的木栈道模型，包含支撑柱和扶手结构。特别适用于湿地、水面或起伏地形上的人行通道设计。

## 2. 核心算法 (Core Algorithm)
1.  **路径框架 (Path Framing)**: 沿输入曲线提取垂直断面，定义栈道的横截面。
2.  **桥面生成 (Deck Generation)**: 沿路径挤出横截面，生成主要的行走表面。
3.  **支撑布置 (Support Placement)**: 沿路径按固定间距向下投射射线，寻找与地形网格的交点，生成垂直支撑柱。
4.  **栏杆系统 (Railing System)**: 垂直偏移路径边缘以生成扶手和立柱。

## 3. 输入参数 (Input Parameters)

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Path** | P | Curve | - | 栈道的中心线路径。 |
| **Terrain** | T | Mesh | (可选) | 用于确定支撑柱高度的地形网格。若省略，则不生成支撑。 |
| **Width** | W | Number | 2.0 | 栈道桥面的总宽度。 |
| **SupportSpacing** | S | Number | 3.0 | 沿路径布置支撑柱的间距。 |

## 4. 输出参数 (Output Parameters)

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Deck** | D | Brep | 栈道的主要行走桥面。 |
| **Supports** | S | Brep (List) | 延伸至地形的垂直支撑柱。 |
| **Railings** | R | Brep (List) | 两侧的栏杆系统（包含立柱和扶手）。 |

## 5. 使用技巧 (Tips)
*   **地形对齐**: 请确保输入路径位于地形网格上方，以便程序能正确计算支撑柱的高度。
*   **曲率控制**: 避免在输入路径中使用极度尖锐的转弯，这可能导致桥面几何体自相交。
