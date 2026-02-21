# Landscape Toolkit Tests

此目录包含 `LandscapeToolkit` 项目的单元测试。

## 环境要求

- .NET Framework 4.8 开发包
- Rhino 7 (用于运行依赖 RhinoCommon 的集成测试)

## 运行测试

使用 Visual Studio 的测试资源管理器或命令行运行测试：

```powershell
dotnet test
```

## 关于 RhinoCommon 的测试

大多数涉及几何计算的测试都需要 Rhino 运行时环境。
如果直接运行 `dotnet test`，可能会因为找不到 Rhino 的原生 DLL 而失败。

### 解决方案

1. **分离逻辑**: 尽可能将纯算法逻辑与 RhinoCommon 类型分离。
2. **使用 Rhino.Testing**: 对于必须依赖 Rhino 的测试，可以使用 `Rhino.Testing` 包来启动 Rhino 核心。
   (目前尚未集成，如需集成请参考 [Rhino.Testing GitHub](https://github.com/mcneel/Rhino.Testing))

### 当前状态

目前 `BasicTests.cs` 中包含一个简单的数学测试用于验证环境配置。
依赖 `Rhino.Geometry` 的测试已被注释，以免在没有 Rhino 环境的机器上报错。
