# 创建 Tween 与动画模式

## 核心创建 API

所有数值模式都位于 `WooTween.Tween` 静态类。

### DoGoto

```csharp
public static ITweenContext<T, Target> DoGoto<T, Target>(
    Target target,
    T start,
    T end,
    float duration,
    Func<Target, T> getter,
    Action<Target, T> setter,
    bool snap,
    bool autoRun = true)
```

用途：普通起点到终点插值。

```csharp
Tween.DoGoto(
    light,
    0f,
    5f,
    0.4f,
    static target => target.intensity,
    static (target, value) => target.intensity = value,
    snap: false);
```

### DoWait

```csharp
public static ITweenContext DoWait(float duration, bool autoRun = true)
```

Wait 不需要目标、getter 或 setter，只推进时间。它常用于 Sequence 中制造间隔。

```csharp
Tween.Sequence()
    .NewContext(() => OpenPanel())
    .NewContext(() => Tween.DoWait(0.25f))
    .NewContext(() => ShowHint())
    .Run();
```

### DoShake

```csharp
public static ITweenContext<T, Target> DoShake<T, Target>(
    Target target,
    T start,
    T end,
    float duration,
    Func<Target, T> getter,
    Action<Target, T> setter,
    T strength,
    int frequency = 10,
    float dampingRatio = 1,
    bool snap = false,
    bool autoRun = true)
```

Shake 先计算基础插值，再叠加按分量生成的随机扰动。扰动由 frequency 和 dampingRatio 控制，并在起点和终点归零。

- `strength`：每个值分量的最大扰动尺度；
- `frequency`：振荡频率参数，越大变化越密集；
- `dampingRatio`：衰减参数，越大越快减弱；
- Shake 使用 UnityEngine.Random，因此每次播放轨迹可能不同。

```csharp
Tween.DoShake(
    camera.transform,
    camera.transform.localPosition,
    camera.transform.localPosition,
    0.35f,
    static target => target.localPosition,
    static (target, value) => target.localPosition = value,
    new Vector3(0.12f, 0.12f, 0f),
    frequency: 15,
    dampingRatio: 1.2f);
```

### DoPunch

Punch 的签名与 Shake 基本相同。区别在于它叠加确定性的余弦回弹，而不是随机范围。

```csharp
Tween.DoPunch(
    transform,
    Vector3.one,
    Vector3.one,
    0.3f,
    static target => target.localScale,
    static (target, value) => target.localScale = value,
    Vector3.one * 0.15f,
    frequency: 8,
    dampingRatio: 1f);
```

当 start 和 end 相同，Punch 表现为从基准值向 strength 方向弹出并回到基准值。

### DoJump

```csharp
public static ITweenContext<T, Target> DoJump<T, Target>(
    Target target,
    T start,
    T end,
    float duration,
    Func<Target, T> getter,
    Action<Target, T> setter,
    T strength,
    int jumpCount = 5,
    float jumpDamping = 2f,
    bool snap = false,
    bool autoRun = true)
```

Jump 在基础插值上叠加多个归一化抛物线段：

- `jumpCount`：完整 duration 内的跳跃次数；
- `strength`：第一跳的偏移方向和幅度；
- `jumpDamping`：每完成一跳，strength 会除以该值。

`jumpCount` 应大于 0，`jumpDamping` 不应为 0。

```csharp
Tween.DoJump(
    transform,
    transform.position,
    transform.position + Vector3.right * 3f,
    1f,
    static target => target.position,
    static (target, value) => target.position = value,
    strength: Vector3.up * 2f,
    jumpCount: 2,
    jumpDamping: 1.5f);
```

### DoArray

```csharp
public static ITweenContext<T, Target> DoArray<T, Target>(
    Target target,
    float duration,
    Func<Target, T> getter,
    Action<Target, T> setter,
    T[] points,
    bool snap = false,
    bool autoRun = true)
```

Array 把总进度均匀分到相邻点之间，并在每段做线性插值。四个点会产生三段，每段占总进度的三分之一。

```csharp
Tween.DoArray(
    transform,
    1.5f,
    static target => target.position,
    static (target, value) => target.position = value,
    new[]
    {
        Vector3.zero,
        Vector3.right,
        Vector3.right + Vector3.up,
        Vector3.up
    });
```

### DoBezier

DoBezier 参数与 DoArray 相同，但使用全部点计算 Bernstein 多项式。首尾点是轨迹端点，中间点是控制点。

```csharp
Tween.DoBezier(
    transform,
    1.5f,
    static target => target.position,
    static (target, value) => target.position = value,
    new[]
    {
        Vector3.zero,
        new Vector3(1f, 2f, 0f),
        new Vector3(3f, 2f, 0f),
        new Vector3(4f, 0f, 0f)
    });
```

## points 约束

Array 和 Bézier 配置在点数组为 null 或点数不大于 2 时会输出 `At Least 3 point`。当前实现只记录错误，不会自动修复输入；继续运行可能产生空引用或索引越界。

调用者必须保证：

- `points != null`；
- `points.Length >= 3`；
- 数组元素本身是有效值；
- duration 大于 0。

动画开始时，点数组被复制到内部池化 `ArrayBuffer<T>`。这避免外部数组在运行中被循环逻辑直接修改，并允许 PingPong/Add 调整内部点。

## duration 约束

所有数值 Tween 都应使用正 duration。duration 为 0 时，时间比例存在 `0 / 0` 的风险，缓动和自定义 evaluator 可能收到 NaN。需要立即赋值时，直接写属性或使用采样 API，不要依赖零时长 Tween。

## snap

`snap=true` 会在计算结束后执行值类型计算器的 Snap：

- float：`Mathf.RoundToInt`；
- Vector2/3/4：逐分量取整；
- Color：逐通道取整；
- Rect：x、y、width、height 取整。

Snap 发生在所有模式偏移和 `sourceDelta` 混合之后。对于世界坐标、UI 像素或整数式属性很有用；颜色、比例等连续属性通常保持 false。

## autoRun 的配置窗口

当 `autoRun=true` 时，Tween 通常在下一次调度更新开始。链式调用是设置参数的正常方式：

```csharp
Tween.DoShake(...)
    .SetFrequency(18)
    .SetDampingRatio(1.4f)
    .SetStrength(strength)
    .SetEase(Ease.OutQuad);
```

需要完全控制开始时机时：

```csharp
var tween = Tween.DoGoto(..., autoRun: false);
tween.SetEase(Ease.InOutCubic);
tween.OnBegin(OnBegin);
tween.Run();
```

## getter/setter 设计建议

推荐：

```csharp
static target => target.Value
static (target, value) => target.Value = value
```

避免：

- getter 修改状态；
- getter 每帧创建临时集合或字符串；
- setter 启动另一个无条件 Tween，形成每帧递归创建；
- 捕获大对象的 lambda 长期保留在 `autoCycle=false` 上下文中；
- getter 和 setter 操作不同的属性。

## 何时使用扩展方法

扩展方法减少 getter/setter 重复代码，并使用 `static` lambda 避免闭包。业务属性没有现成扩展时，优先写一个项目级扩展封装 `DoGoto`，而不是在每个调用点复制 getter/setter。详见[自定义属性与值类型](12-customization.md)。
