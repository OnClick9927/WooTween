# 快速开始

本章从一个最小动画开始，逐步加入配置、生命周期和组合。

## 第一个 Tween

如果已经导入 Transform 扩展：

```csharp
using UnityEngine;
using WooTween;

public sealed class MoveExample : MonoBehaviour
{
    private void Start()
    {
        transform.DoPosition(new Vector3(3f, 0f, 0f), 1f);
    }
}
```

`DoPosition(end, duration)` 会在创建时读取 `transform.position` 作为起点。默认 `autoRun=true`，上下文先进入待运行队列，并在调度器更新时开始执行。因此可以在创建表达式后继续链式配置。

## 使用核心泛型 API

没有导入扩展包时，等价写法为：

```csharp
Tween.DoGoto(
    transform,
    transform.position,
    new Vector3(3f, 0f, 0f),
    1f,
    static target => target.position,
    static (target, value) => target.position = value,
    snap: false);
```

尽量使用不捕获局部变量的 `static` lambda。这样不会为每次 Tween 创建闭包对象。

## 链式配置

```csharp
ITweenContext<Vector3, Transform> context = transform
    .DoPosition(new Vector3(3f, 0f, 0f), 1f)
    .SetDelay(0.2f)
    .SetEase(Ease.OutCubic)
    .SetLoop(LoopType.PingPong, 2)
    .SetTimeScale(1f)
    .SetId("player-enter")
    .SetOwner(gameObject)
    .OnBegin(_ => Debug.Log("Begin"))
    .OnTick((tween, time, delta) =>
    {
        Debug.Log($"{tween.GetPercent():P0}");
    })
    .OnComplete(_ => Debug.Log("Complete"));
```

注意两类配置的返回类型：

- 所有上下文共有：`SetTimeScale`、`SetAutoCycle`、`SetId`、`SetOwner` 和回调；
- 数值上下文专有：`SetEase`、`SetAnimationCurve`、`SetLoop`、`SetDelay`、`SetSnap` 等。

## 保存引用并控制

```csharp
private ITweenContext move;

private void Start()
{
    move = transform
        .DoPosition(Vector3.right * 5f, 2f)
        .SetAutoCycle(false);
}

public void PauseMove() => move?.Pause();
public void ResumeMove() => move?.UnPause();
public void StopMove() => move?.Stop();
public void RewindMove() => move?.Rewind();
public void RestartMove() => move?.ReStart();

private void OnDestroy()
{
    move?.Cancel();
    move?.Recycle();
    move = null;
}
```

这里设置 `autoCycle=false`，是因为需要在完成后继续使用同一个上下文。如果保持默认值，完成后对象会进入池，旧引用不再可用。

## 取消同一所有者的动画

创建时给多个 Tween 设置同一个 owner：

```csharp
transform.DoPosition(Vector3.right * 2f, 1f).SetOwner(this);
transform.DoLocalScale(Vector3.one * 1.2f, 1f).SetOwner(this);
```

统一终止：

```csharp
this.KillTweens();
```

`KillTweens` 会 Stop 并回收所有 owner 与传入对象相同的上下文。不要把 `SetId` 当作批量终止条件；当前公开 API 只支持按 owner 批量终止。

## 串行动画

```csharp
ITweenGroup sequence = Tween.Sequence()
    .NewContext(() => transform.DoPosition(Vector3.right * 2f, 0.5f))
    .NewContext(() => Tween.DoWait(0.15f))
    .NewContext(() => transform.DoLocalScale(Vector3.one * 1.2f, 0.25f))
    .OnComplete(_ => Debug.Log("Sequence complete"))
    .Run();
```

`NewContext` 接收工厂函数，而不是已经创建的上下文。Sequence 到达对应位置时才调用工厂，因此第二段、第三段可以使用那一刻的最新目标值。

## 并行动画

```csharp
ITweenGroup parallel = Tween.Parallel()
    .NewContext(() => transform.DoPosition(Vector3.right * 2f, 0.5f))
    .NewContext(() => transform.DoLocalScale(Vector3.one * 1.2f, 0.5f))
    .OnComplete(_ => Debug.Log("Parallel complete"))
    .Run();
```

Parallel 会在每轮开始时调用全部工厂。当所有有效子 Tween 都完成或取消后，本轮结束。

## Shake、Punch 和 Jump

Transform 扩展示例：

```csharp
transform.DoShakePosition(
    end: transform.position,
    duration: 0.5f,
    strength: new Vector3(0.2f, 0.2f, 0f),
    frequency: 12,
    dampingRatio: 1f);

transform.DoPunchLocalScale(
    end: Vector3.one,
    duration: 0.35f,
    strength: Vector3.one * 0.2f,
    frequency: 8,
    dampingRatio: 1f);

transform.DoJumpPosition(
    end: transform.position + Vector3.right * 2f,
    duration: 0.8f,
    strength: Vector3.up,
    jumpCount: 2,
    jumpDamping: 2f);
```

参数含义见[创建 Tween 与动画模式](04-creating-and-modes.md)。

## 路径数组与 Bézier

```csharp
Vector3[] path =
{
    transform.position,
    new Vector3(1f, 2f, 0f),
    new Vector3(3f, 1f, 0f),
    new Vector3(4f, 0f, 0f)
};

transform.DoPositionArray(1.5f, path);       // 分段线性
transform.DoPositionArrayBezier(1.5f, path); // Bézier 曲线
```

Array 和 Bézier 至少应提供三个点。WooTween 在动画开始时把点复制到内部池化缓冲区，因此运行中修改原数组不会立即改变当前轨迹；重新 Run 或 Rewind 时会重新检查点数据。

## 按进度采样

```csharp
[Range(0f, 1f)] public float preview;

private void OnValidate()
{
    Tween.Sample(
        transform,
        Vector3.zero,
        Vector3.right * 5f,
        1f,
        static target => target.localPosition,
        static (target, value) => target.localPosition = value,
        false,
        preview);
}
```

采样不会把上下文加入运行调度器，适合编辑器滑块和外部时间轴。`progress` 会用于计算 `progress * duration`，核心采样逻辑会把普通时间比例限制在 `[0, 1]`。

## 等待完成

WooTween 上下文实现了 awaiter：

```csharp
private async void PlayAnimation()
{
    await transform.DoPosition(Vector3.right * 2f, 0.5f);
    await transform.DoLocalScale(Vector3.one * 1.2f, 0.25f);
}
```

awaiter 只监听完成回调。Stop 或 Cancel 不会使等待自动结束，因此异步流程必须自行保证上下文能够 Complete。详见[采样与异步等待](08-sampling-and-await.md)。

## 下一步

- 理解 `autoRun`、`autoCycle`、state 和对象池：[核心概念](03-core-concepts.md)
- 了解每一种模式：[创建 Tween 与动画模式](04-creating-and-modes.md)
- 正确处理销毁与回调：[生命周期与回调](06-lifecycle-and-callbacks.md)
- 使用 Inspector 制作动画：[TweenComponent](09-tween-component.md)
