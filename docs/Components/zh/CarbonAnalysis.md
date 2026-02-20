# Carbon Analysis (碳汇分析)

**Carbon Analysis** 组件用于估算景观设计方案的碳汇效益（固碳量）。

## 输入 (Inputs)
*   **TreePoints (T)**: 代表乔木的点列表。
*   **GreenAreas (A)**: 代表灌木或草坪的网格/曲面列表。
*   **TreeFactor (TF)**: 单株乔木年固碳量 (kg/year)，默认 22.0。
*   **AreaFactor (AF)**: 单位面积绿地年固碳量 (kg/m2/year)，默认 1.5。

## 输出 (Outputs)
*   **TotalCarbon (C)**: 总固碳量 (kg/year)。
*   **Report (R)**: 详细计算报告（文本）。

## 算法依据
基于常见园林植物的平均固碳能力进行估算。
