# Solar Analysis (光照分析)

**Category:** Landscape > Analysis
**Nickname:** Solar

## 1. 简介 (Introduction)
Solar Analysis 是一个基于网格法线的快速光照暴露度估算工具。它通过计算地形表面法线与太阳光线的夹角（点积），生成直观的受光面与背光面热力图。适用于场地日照初判和向阳坡分析。

## 2. 核心算法 (Core Algorithm)
1.  **法线计算**: 计算输入网格每个顶点的法向量 (Normal Vector)。
2.  **点积运算 (Dot Product)**: 计算法向量 $\vec{N}$ 与反向太阳光线 $\vec{L}$ 的点积：$Exposure = \max(0, \vec{N} \cdot \vec{L})$。
    *   结果为 1.0 表示垂直受光（最强）。
    *   结果为 0.0 表示侧切光或背光（最弱）。
3.  **颜色映射**: 将暴露度值映射为从蓝色（阴影）到红色（受光）的热力图渐变。

## 3. 输入参数 (Input Parameters)

| 参数名 | 缩写 | 类型 | 默认值 | 说明 |
| :--- | :--- | :--- | :--- | :--- |
| **Mesh** | M | Mesh | 需要分析的地形或建筑网格。 |
| **SunDirection** | S | Vector | (-1, -1, -1) | 太阳光线的方向向量。通常从 Rhino 的太阳面板或 Ladybug 获取。 |

## 4. 输出参数 (Output Parameters)

| 参数名 | 缩写 | 类型 | 说明 |
| :--- | :--- | :--- | :--- |
| **Colored Mesh** | CM | Mesh | 带有顶点颜色的分析结果网格。 |
| **Exposure** | E | Number (List) | 每个顶点的光照暴露度数值 (0.0 - 1.0)。 |

## 5. 使用建议 (Tips)
*   **快速分析**: 这是一个几何估算工具，不包含阴影遮挡计算（即不考虑建筑互遮），计算速度极快，适合设计初期的地形朝向分析。
*   **精确模拟**: 如果需要精确的小时级日照时数或辐射量，建议使用 Ladybug Tools。
