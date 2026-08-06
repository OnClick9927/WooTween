# WooTween 简介

WooTween 是一个运行于 Unity 主线程的轻量级 Tween 动画库。它把“读取当前值、计算插值、写回目标”抽象为一个通用上下文，并在此基础上提供常见 Unity 对象扩展、串行与并行动画组、可视化组件、编辑器预览以及按进度采样。

本文档对应仓库 `main` 分支和包版本 `1.1.8`。

## 适用场景

WooTween 适合以下工作：

- Transform 位移、旋转、缩放；
- UI 颜色、透明度、填充量、Slider 数值和文本逐字显示；
- Camera、AudioSource、Material、SpriteRenderer 等组件属性动画；
- Rigidbody 和 Rigidbody2D 的位置或旋转插值；
- Shake、Punch、Jump 等程序化反馈；
- 多段动画的 Sequence 串行编排；
- 多个动画的 Parallel 并行编排；
- 编辑器非运行模式预览；
- 根据归一化进度直接采样属性，供时间轴或 ActionEditor 使用；
- 用 getter/setter 快速动画化自己的组件字段。

WooTween 不是完整的 Timeline 替代品，也不会接管 Unity Animator。它更适合由代码或轻量组件驱动、生命周期明确、需要快速组合的属性动画。

## 主要能力

### 通用泛型插值

核心入口 `Tween.DoGoto<T, Target>` 不依赖具体 Unity 组件：

```csharp
Tween.DoGoto(
    target: healthBar,
    start: 0f,
    end: 100f,
    duration: 0.5f,
    getter: static bar => bar.Value,
    setter: static (bar, value) => bar.Value = value,
    snap: false);
```

`T` 是被插值的值类型，`Target` 是持有该属性的目标类型。getter 在每次采样时读取目标当前值，setter 在计算结果变化后写回。

### 六类数值模式和等待

| 模式 | API | 用途 |
| --- | --- | --- |
| Normal | `DoGoto` | 从起点插值到终点 |
| Shake | `DoShake` | 在基础插值上增加随机衰减扰动 |
| Punch | `DoPunch` | 在基础插值上增加确定性的回弹振荡 |
| Jump | `DoJump` | 按跳跃次数和衰减叠加抛物线偏移 |
| Array | `DoArray` | 沿一组点做分段线性插值 |
| Bezier | `DoBezier` | 沿一组控制点计算 Bézier 曲线 |
| Wait | `DoWait` | 只等待时间，不读写目标 |

### 生命周期与对象池

每个动画返回 `ITweenContext`。上下文提供暂停、停止、取消、强制完成、回退、重启、标识、所有者和回调等能力。

默认 `autoCycle=true`：动画结束或被终止后会上交对象池。对象池能减少反复播放动画时的托管分配，但也意味着完成后的引用不能当作永久对象使用。需要完成后执行 `Rewind` 或 `ReStart` 时，应在开始前调用 `SetAutoCycle(false)`。

### 组合动画

`Tween.Sequence()` 按注册顺序逐个执行工厂函数，`Tween.Parallel()` 同时启动所有工厂函数。工厂函数是惰性执行的，因此下一段动画可以在真正开始时读取目标的最新值。

```csharp
Tween.Sequence()
    .NewContext(() => transform.DoPosition(Vector3.right * 2f, 0.4f))
    .NewContext(() => transform.DoLocalScale(Vector3.one * 1.1f, 0.2f))
    .Run();
```

### 运行时和编辑器共用调度

- Play Mode 中，WooTween 创建一个名为 `Tween` 的隐藏式运行组件，并通过其 `Update` 使用 `Time.deltaTime` 推进动画。
- Edit Mode 中，编辑器调度器挂接 `EditorApplication.update`，使用上限为 `0.02` 秒的编辑器帧间隔推进预览。
- 进入 Play Mode 前，编辑器调度器会停止现有编辑器动画。

## 内置支持的值类型

核心程序集默认注册以下 `ValueCalculator<T>`：

- `float`
- `Vector2`
- `Vector3`
- `Vector4`
- `Color`
- `Rect`

可以通过 `Tween.GetSupportTypes()` 查询当前注册结果。仓库目前没有公开的运行时注册方法；增加自定义值类型时，需要实现计算器并在 `Tween.value_calcs` 初始化表中注册，详见[自定义属性与值类型](12-customization.md)。

## 核心包与扩展包

UPM 分支的核心包包含：

- `Runtime/`：上下文、调度器、缓动、值计算器和 TweenComponent；
- `Editor/`：TweenComponent Inspector、Scene 编辑和 TweenWatcher；
- `Package Resources/extend.unitypackage`：可选的 Unity 组件扩展。

扩展包提供 Transform、RectTransform、UGUI、TextMeshPro、Camera、AudioSource、Material、SpriteRenderer、Rigidbody 和 Rigidbody2D 的快捷方法及 Actor 类型。核心 API 不依赖这些扩展，可以只使用 `Tween.DoGoto` 等泛型入口。

## 阅读路线

第一次使用建议按以下顺序阅读：

1. [安装与目录](01-installation.md)
2. [快速开始](02-quick-start.md)
3. [核心概念](03-core-concepts.md)
4. [创建 Tween 与动画模式](04-creating-and-modes.md)
5. [生命周期与回调](06-lifecycle-and-callbacks.md)
6. [Sequence 与 Parallel](07-groups.md)

需要可视化制作动画时继续阅读 [TweenComponent](09-tween-component.md) 和 [编辑器工具](10-editor-tools.md)。准备上线或进行大量动画压测前，阅读[性能与内存](13-performance.md)。

## 版本与反馈

- 仓库：[OnClick9927/WooTween](https://github.com/OnClick9927/WooTween)
- 作者主页：[OnClick9927](https://github.com/OnClick9927)
- 相关项目：[ActionEditor](https://github.com/OnClick9927/ActionEditor)、[WooAsset](https://github.com/OnClick9927/WooAsset)

发现行为与本文档不一致时，请优先以当前分支源码为准，并在仓库 Issue 中附上 Unity 版本、最小复现代码、调用顺序和完整异常堆栈。
