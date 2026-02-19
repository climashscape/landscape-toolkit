# Landscape Terrain (景观地形)

**Category:** Landscape > Modeling
**Nickname:** Terrain

## 1. 简介 (Introduction)
Terrain 组件致力于生成高质量的景观微地形。与传统的 GIS 地形不同，该组件专注于设计尺度的土方造型，支持从等高线或散点生成光滑、流动且布线讲究的四边面地形，也就是俗称的 "Class-A" 地形曲面。

## 2. 核心算法 (Core Algorithm)
1.  **德劳内三角化 (Delaunay Triangulation)**: 首先基于输入点或等高线采样点构建基础的不规则三角网 (TIN)。
2.  **平滑处理 (Smoothing)**: 对初始三角网进行拉普拉斯平滑，消除等高线常见的“阶梯效应”。
3.  **四边面重构 (Quad Remesh)**: 将平滑后的模型重构为四边面网格，优化布线流向。
4.  **SubD 转换 (SubD Conversion)**: (可选) 输出 SubD 曲面，获得最终的完美光顺效果。

## 3. 输入参数 (Input Parameters)

| 参数名 | 缩写 | 类型 | 默认值 | 说明 |
| :--- | :--- | :--- | :--- | :--- |
| **Input** | G | Geometry (List) | 输入的等高线 (Curve) 或高程点 (Point)。 |
| **Target Quad Count** | QC | Integer | 2000 | 目标网格面数。数值越大细节越多。 |
| **Smooth** | S | Boolean | True | 是否在重构前对初始地形进行平滑。对于阶梯状等高线建议开启。 |

## 4. 输出参数 (Output Parameters)

| 参数名 | 缩写 | 类型 | 说明 |
| :--- | :--- | :--- | :--- |
| **Quad Mesh** | QM | Mesh | 重构后的四边面网格。 |
| **SubD** | SD | SubD | 转换后的细分曲面（高精度光滑模型）。 |

## 5. 使用建议 (Tips)
*   **等高线修复**: 确保输入的等高线没有自交或重叠，否则可能导致初始三角化失败。
*   **微地形**: 非常适合制作起伏的草坡、土丘。可以通过控制输入点的密度来控制地形的局部特征。
