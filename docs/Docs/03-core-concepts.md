# 核心概念

## Tween 上下文

所有动画都以 `ITweenContext` 表示。它既是状态句柄，也是回调来源：

```csharp
public interface ITweenContext
{
    string id { get; }
    bool isDone { get; }
    bool autoCycle { get; }
    bool paused { get; }
    TweenContextState state { get; }
    float GetPercent();
}
```

数值 Tween 返回 `ITweenContext<T, Target>`。该泛型接口用于在编译期保留值类型和目标类型，从而开放 `SetEase`、`SetLoop`、`SetStrength` 等数值配置扩展。

组合 Tween 返回 `ITweenGroup`：

```csharp
public interface ITweenGroup : ITweenContext
{
    ITweenGroup NewContext(Func<ITweenContext> func);
    void SetLoops(int loops);
}
```

## 状态

`TweenContextState` 包含四个值：

| 状态 | 含义 |
| --- | --- |
| `Allocate` | 已从池中取得并完成初始化，但尚未开始或已经 Rewind |
| `Run` | 正在运行，已加入调度器 |
| `Pause` | 保留运行上下文但暂停推进 |
| `Sleep` | 已返回对象池，不应继续使用旧引用 |

`state` 不是唯一的终止判断。还应结合 `isDone`、`paused` 和 `Tween.IsRunning(context)`：

```csharp
if (context != null && Tween.IsRunning(context))
{
    Debug.Log($"state={context.state}, paused={context.paused}");
}
```

## autoRun

核心创建 API 的最后一个参数是 `autoRun`，默认 `true`。

### autoRun=true

上下文创建后进入调度器的待运行队列。在调度器更新时调用 Run，所以同一调用链中的配置会在 OnBegin 之前完成：

```csharp
Tween.DoGoto(..., autoRun: true)
    .SetEase(Ease.OutQuad)
    .OnBegin(_ => Debug.Log("configured before begin"));
```

### autoRun=false

上下文只分配和配置，不进入待运行队列。需要显式调用 `.Run()`：

```csharp
var tween = Tween.DoGoto(..., autoRun: false)
    .SetEase(Ease.OutQuad)
    .SetAutoCycle(false);

tween.Run();
```

Transform 等便捷扩展当前不暴露 `autoRun`，默认使用核心 API 的自动运行行为。

Sequence 和 Parallel 不带 `autoRun` 参数，也不会自动启动，必须显式 `.Run()`。

## autoCycle 与对象池

每种闭合泛型上下文类型都有独立对象池，例如 `TweenContext<Vector3, Transform>` 和 `TweenContext<float, CanvasGroup>` 分属不同池。Sequence 与 Parallel 也各有自己的池。

默认 `autoCycle=true`：

- 自然完成后回池；
- Cancel、Stop 或强制 Complete 后回池；
- 回池时清除目标、getter、setter、点数组、曲线和回调引用；
- 同一类型之后创建的新 Tween 可能复用该对象。

因此下面的写法不安全：

```csharp
var tween = transform.DoPosition(Vector3.right, 0.1f);
// 动画完成并回池后：
tween.Rewind(); // tween 可能已失效，甚至已被其他动画复用
```

需要保留时：

```csharp
var tween = transform
    .DoPosition(Vector3.right, 0.1f)
    .SetAutoCycle(false);

// 完成后仍可使用
tween.Rewind();
tween.ReStart();

// 不再需要时显式释放
tween.Recycle();
```

不要在对象已经进入 `Sleep` 后继续调用配置或生命周期方法。

## 延迟回收

Tween 可能在自己的 Update、OnTick、OnComplete 或组合回调中终止。如果立即把对象放回池，同一个调用栈内创建的新 Tween 可能拿到尚未退出 Update 的旧对象。

WooTween 在调度器更新期间采用帧末延迟回收：

1. 从运行/等待/组合列表摘除上下文；
2. 标记为待回收；
3. 当前调度遍历结束；
4. 批量返回对象池并发送编辑器回收通知。

这保证完成回调中创建同类型 Tween 时，不会复用仍在执行的实例。

## getter、setter 与当前源值

每次采样执行：

1. getter 读取目标当前值；
2. evaluator 把时间比例变换为缓动比例；
3. ValueCalculator 根据模式算出目标值；
4. 可选地结合 `sourceDelta` 与当前值；
5. 比较当前值和结果；
6. 仅当不同才调用 setter。

getter 不应有副作用。setter 应只写入对应属性，避免在 setter 内再次配置同一个采样缓存或递归启动无法控制的动画。

## 起点重载

扩展方法通常有两类重载：

```csharp
target.DoPosition(start, end, duration); // Direct
target.DoPosition(end, duration);        // 立即读取当前值作为 start
```

只传 `end` 的重载在调用扩展方法时读取当前值，不是在下一帧正式 Run 时读取。Sequence 的工厂函数是惰性调用，因此把该重载放在 `NewContext(() => ...)` 中，仍然可以在轮到该段时读取最新值。

## 时间与 timeScale

运行时使用 `Time.deltaTime`，每帧实际推进量为：

```text
deltaTime * context.timeScale
```

`SetTimeScale(0)` 会让数值时间不再前进，但它不等同于 Pause：状态仍是 Run，调度器仍会调用 Update。需要真正跳过更新时应使用 `Pause()`。

负 timeScale 没有专门的反向播放逻辑。当前 MoveNext 主要针对正向时间设计；需要回退到起点请使用 `Rewind()`，不要依赖负值。

## id 与 owner

`id` 用于识别和 TweenWatcher 展示：

```csharp
tween.SetId("panel-open");
```

当前没有公开的按 id 查询或 Kill API。

`owner` 用于批量终止：

```csharp
tween.SetOwner(this);
this.KillTweens();
```

owner 使用对象引用相等比较。建议传入负责管理生命周期的 MonoBehaviour、GameObject 或专用控制器对象。

## 进度

普通数值 Tween 的 `GetPercent()` 基于内部 `time / duration` 并限制到 `[0, 1]`。完成帧内部时间会为循环逻辑重置，因此判断完成应使用 `isDone`，不要只依赖完成后的 `GetPercent()`。

Sequence 进度按当前轮中“已创建段数 + 当前段进度”除以工厂总数估算；Parallel 取尚未完成子 Tween 的最小进度。包含不同 duration、delay、内部 loops 或返回 null 的工厂时，组进度是调度展示值，不是严格的实际耗时比例。

## 支持类型与 ValueCalculator

计算器负责以下操作：

- Lerp
- Add / Minus
- 乘以或除以标量
- Snap
- Shake 随机范围
- strength 分量乘法

如果 `T` 未注册，WooTween 会输出 `Tween Not support Type`，随后在采样计算时无法继续。自定义类型步骤见[自定义属性与值类型](12-customization.md)。
