# Landscape Steps

**Category:** Landscape > Modeling
**Nickname:** Steps

## 1. Introduction
The Steps component is used to rapidly generate landscape steps along a specified path. It is not just a simple geometric array but a logic-driven parametric generator that automatically calculates the number of steps based on path length, tread depth, and riser height, generating solid models.

## 2. Core Algorithm
1.  **Path Analysis**: Calculates the length of the input path.
2.  **Parameter Calculation**: Calculates the number of steps based on the set `Tread` depth: $Count = Length / Tread$.
3.  **Section Generation**: Generates rectangular sections for the steps at each division point along the path.
4.  **Solid Construction**: Combines sections into closed Brep solids.

## 3. Input Parameters

| Name | Abbr. | Type | Default | Description |
| :--- | :--- | :--- | :--- | :--- |
| **Path** | C | Curve | The center path for the steps (planar or 3D curve). |
| **Width** | W | Number | 2.0 | Total width of the steps. |
| **Tread** | T | Number | 0.3 | Tread depth (Run). Typically 0.3m - 0.45m. |
| **Riser** | R | Number | 0.15 | Riser height (Rise). Typically 0.12m - 0.15m. |

## 4. Output Parameters

| Name | Abbr. | Type | Description |
| :--- | :--- | :--- | :--- |
| **Steps** | S | Brep (List) | List of generated step solids. |

## 5. Tips
*   **Slope Integration**: The generated steps are "horizontal". If the path has a slope, the steps will automatically stack along the Z-axis.
*   **Standard Compliance**: When designing, keep in mind the ergonomic formula $2R + T \approx 600-650mm$.
