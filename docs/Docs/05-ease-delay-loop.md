# 缓动、延迟与循环

## Ease

使用 `SetEase` 选择内置缓动：

```csharp
transform
    .DoPosition(Vector3.right * 3f, 1f)
    .SetEase(Ease.InOutCubic);
```

支持的枚举：

| 类别 | Ease |
| --- | --- |
| 线性 | `Linear` |
| Sine | `InSine`、`OutSine`、`InOutSine` |
| Quad | `InQuad`、`OutQuad`、`InOutQuad` |
| Cubic | `InCubic`、`OutCubic`、`InOutCubic` |
| Quart | `InQuart`、`OutQuart`、`InOutQuart` |
| Quint | `InQuint`、`OutQuint`、`InOutQuint` |
| Expo | `InExpo`、`OutExpo`、`InOutExpo` |
| Circ | `InCirc`、`OutCirc`、`InOutCirc` |

默认值为 `Ease.Linear`。内置 Ease evaluator 已缓存，不会因每次 `SetEase` 产生新的 evaluator 对象。

## AnimationCurve

```csharp
AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

transform
    .DoPosition(Vector3.right * 3f, 1f)
    .SetAnimationCurve(curve);
```

曲线使用归一化进度作为输入：

```text
curve.Evaluate(percent)
```

曲线输出不被再次 Clamp。输出小于 0 或大于 1 时，最终行为取决于具体 `ValueCalculator<T>.Lerp`。Unity 的 `Mathf.Lerp`、`Vector*.Lerp`、`Color.Lerp` 通常会限制 t，因此不要假设所有类型都允许外插。

每个使用过 AnimationCurve 的 TweenContext 会懒创建一个可复用 evaluator 对象。回池时清空 curve 引用，避免池长期持有曲线资源。

## 自定义 IValueEvaluator

```csharp
public sealed class OvershootEvaluator : IValueEvaluator
{
    public float Evaluate(float percent, float time, float duration)
    {
        float x = percent - 1f;
        return 1f + x * x * ((2.5f + 1f) * x + 2.5f);
    }
}
```

使用：

```csharp
tween.SetEvaluator(new OvershootEvaluator());
```

参数含义：

- `percent`：普通时间比例，已限制在 `[0, 1]`；
- `time`：当前 Tween 时间；
- `duration`：配置的持续时间。

高频创建 Tween 时，应缓存无状态 evaluator，避免每次 `new`。有状态 evaluator 不应在多个并行动画间共享可变状态。

## Delay

```csharp
tween.SetDelay(0.3f);
```

行为：

1. Tween 第一次更新时先采样起点；
2. 内部时间累计 delay；
3. 达到 delay 后把动画时间从 0 开始推进；
4. delay 只应用于一次 Run 的开头，不会在每个内部 loop 前重复。

传入非常小或非正值时，当前实现会把 delay 设为 `0.0002f`，而不是严格的 0。通常可以视为无感知的最小延迟，但对逐帧精确测试应考虑这一行为。

## 数值 Tween 循环

```csharp
tween.SetLoop(LoopType.PingPong, 4);
```

`loops` 表示总播放轮数，不是额外重复次数。推荐使用：

- `1`：播放一次；
- `2` 或更大：播放指定总轮数；
- `-1`：无限循环。

不要使用 0 表示“不播放”。当前执行流程仍会进入一次采样；需要禁用动画时应不要创建，或在 TweenComponent 中关闭 Actor 的 active。

### Restart

每轮使用相同起点和终点重新播放：

```text
A -> B, A -> B, A -> B
```

### PingPong

每轮交换起点与终点：

```text
A -> B, B -> A, A -> B
```

Array/Bézier 会在内部缓冲区中交替使用原顺序和反向顺序。

### Add

每轮把上一轮跨度累加到后续起终点：

```text
A -> B, B -> B+(B-A), ...
```

Array/Bézier 会计算末点减首点的 gap，并把 gap 加到每个内部点。

Add 依赖值计算器的 Add 和 Minus。对 Color、Rect、自定义类型等使用前，应确认累加语义符合预期。

## 组循环

Sequence 和 Parallel 使用 `ITweenGroup.SetLoops(int)`，没有 `LoopType`：每一轮都会重新调用全部子 Tween 工厂。

```csharp
var group = Tween.Sequence()
    .NewContext(() => transform.DoPosition(Vector3.right, 0.3f))
    .NewContext(() => transform.DoPosition(Vector3.zero, 0.3f));

group.SetLoops(3);
group.Run();
```

应在 Run 前设置组 loops。虽然非空组通常不会在 `Run()` 调用栈内立刻完成，但预先配置能避免空组或同步终止工厂的时序差异。

## sourceDelta

```csharp
tween.SetSourceDelta(0.5f);
```

WooTween 先计算模式目标值 `dest`，再根据当前 getter 值 `src` 进行一次混合：

```text
sourcePercent = (1 - sourceDelta) + sourceDelta * percent
output = Lerp(src, dest, sourcePercent)
```

典型含义：

| sourceDelta | 行为 |
| --- | --- |
| `0` | sourcePercent 恒为 1，直接使用计算目标值；这是默认值 |
| `1` | sourcePercent 等于时间进度，开始阶段更依赖目标当前值 |
| `0~1` | 在两者之间混合 |

它不是简单的“相对起点”开关。因为 src 每帧由 getter 重新读取，非零 sourceDelta 会形成对当前状态的动态混合。目标同时被其他系统修改时，可用于减弱突变，也可能造成难以预测的竞争。

建议：

- 单一系统控制属性时保持默认 0；
- 与跟随、物理或输入系统共同控制前先做可视化测试；
- 不要把超出 `[0, 1]` 的值当作稳定公共语义，当前 API 不做 Clamp。

## 运行时修改配置

`SetDuration`、`SetFrequency`、`SetStrength` 等会直接修改上下文字段。推荐在正式 Run 前设置。运行中修改可能立即影响后续帧，但不会重建所有已经缓存的运行状态：

- 修改 duration 会改变下一帧的时间比例和完成点；
- 修改 points 没有公开 setter，外部数组变化也不保证当前帧生效；
- 修改 loopType/loops 会影响之后的轮次；
- 修改 evaluator 会影响下一次采样。

需要完整、可预测地应用新配置时，Stop/Rewind 后重新创建 Tween 通常更清晰。
