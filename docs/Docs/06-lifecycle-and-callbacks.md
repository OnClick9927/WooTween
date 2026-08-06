# 生命周期与回调

## 生命周期方法总表

| 方法 | 是否继续调度 | 回调 | 默认是否回池 | 主要用途 |
| --- | --- | --- | --- | --- |
| `Pause()` | 保留但跳过推进 | 无 | 否 | 暂停 |
| `UnPause()` | 恢复推进 | 无 | 否 | 继续 |
| `Stop()` | 否 | 不触发 OnCancel/OnComplete | 是 | 静默终止 |
| `Cancel()` | 否 | OnCancel | 是 | 带取消通知地终止 |
| `Complete(true)` | 否 | OnComplete | 是 | 强制标记完成 |
| `Complete(false)` | 否 | OnCancel | 是 | 与 Cancel 等价的底层入口 |
| `Rewind()` | 否 | OnRewind | 否 | 恢复到起始采样并保留配置 |
| `ReStart()` | 是 | OnBegin，之后按正常流程 | 取决于结果 | 停止并重新运行 |
| `Recycle()` | 否 | 不触发生命周期回调 | 是 | 显式归还对象池 |

“默认是否回池”受 `autoCycle` 控制；显式 `Recycle()` 不受 `autoCycle=false` 阻止。

## Pause 与 UnPause

```csharp
tween.Pause();
tween.UnPause();
```

Pause 设置 `paused=true` 和 `state=Pause`。普通 Tween 仍保留调度器槽位，但 Update 会立即返回。组会把 Pause 传播给当前子 Tween；Parallel 会传播给当前轮全部子 Tween。

UnPause 恢复 `state=Run`。对已经 Stop、Cancel、Complete、Recycle 的上下文调用 UnPause 不会重新加入调度器；需要重新播放应使用 ReStart 或创建新 Tween。

## Stop 与 Cancel 的差异

```csharp
tween.Stop();   // 静默
tween.Cancel(); // 触发 OnCancel
```

Stop：

- 标记 canceled；
- 停止当前子动画；
- 从等待、运行或组列表摘除；
- 不调用 OnCancel；
- `autoCycle=true` 时回池。

Cancel 实际调用 `Complete(false)`：

- 先调用 OnCancel；
- 再停止子动画、摘除和尝试回池。

如果业务依赖“操作被取消”的通知，使用 Cancel。如果只是 OnDisable 清理且不希望触发业务回调，可以 Stop 后显式 Recycle。

## 自然完成与强制完成

普通数值 Tween 到达最终 loop 时会：

1. 采样 duration 对应的最终值；
2. 标记 `isDone=true`；
3. 调用 OnComplete；
4. 停止子级（组上下文）；
5. 从调度器摘除；
6. 根据 autoCycle 回池。

最终完成帧不会再调用 OnTick。需要确保最终状态时，应在 OnComplete 中读取目标属性或使用 `isDone`，不要等待最后一个 OnTick。

`Complete(true)` 只改变生命周期并触发 OnComplete，不会主动把普通数值 Tween 采样到终点。需要视觉上立即到终点时，应先设置目标值或使用采样 API，再 Complete。

## Rewind

```csharp
var tween = transform
    .DoPosition(Vector3.right * 3f, 1f)
    .SetAutoCycle(false);

// 运行中或完成后
tween.Rewind();
```

Rewind 的流程：

1. 临时关闭 autoCycle，避免 Stop 把当前对象回池；
2. Stop 并从调度器摘除；
3. 清除 paused、canceled、isDone；
4. state 恢复为 Allocate；
5. 调用外部 OnRewind 回调；
6. 执行具体上下文的回退采样；
7. 恢复原 autoCycle 设置。

注意 OnRewind 回调发生在实际属性采样到起点之前。如果回调需要读取回退后的目标值，应把读取延迟到下一段代码或自行安排调用顺序。

Rewind 不会自动重新播放。调用后使用 `Run()` 或 `ReStart()`。

## ReStart

```csharp
tween.ReStart();
```

ReStart 会保留当前配置和回调，执行 Stop 后重新 Run。为了避免 Stop 阶段把对象回池，内部会临时关闭 autoCycle，并在启动完成后恢复原设置。

对于 Sequence/Parallel，ReStart 会释放旧子上下文并重新调用工厂，因此“只传 end 的扩展重载”会重新读取当时的目标值。

ReStart 不是 Rewind + Run：它不会保证先把属性采样到初始值。如果需要先恢复视觉起点，再开始，可以显式：

```csharp
tween.Rewind();
tween.Run();
```

## Recycle

```csharp
tween.Recycle();
```

Recycle 立即或在当前调度帧末：

- 从调度器摘除；
- 清除回调、owner、id、目标和曲线等引用；
- 把对象压回对应类型的池；
- state 变为 Sleep。

显式 Recycle 不发送 OnCancel 或 OnComplete。通常先 Stop/Cancel，再 Recycle；对已经完成且 `autoCycle=false` 的上下文，可以直接 Recycle。

## 回调

```csharp
tween
    .OnBegin(OnBegin)
    .OnTick(OnTick)
    .OnComplete(OnComplete)
    .OnCancel(OnCancel)
    .OnRewind(OnRewind);
```

签名：

```csharp
Action<ITweenContext>                         // Begin/Complete/Cancel/Rewind
Action<ITweenContext, float, float>           // Tick: context, time, delta
```

多次注册会使用委托 `+=` 追加，按注册顺序调用。回池 Reset 时全部清空。

### OnBegin

由 Run 触发。它发生在上下文切换到 Run 状态之后。回调内 Stop、Cancel、Complete、Rewind 或 Pause 会被 Run 的收尾检查识别；已终止上下文不会被重新加入调度器。

### OnTick

仅在本帧 MoveNext 返回“尚未完成”时调用。`time` 是当前 Tween 内部时间，`delta` 是调度器传入的原始 deltaTime，不包含 timeScale 乘法。

### OnComplete

isDone 已经为 true，但上下文尚未回池。可以在回调中读取 id、进度或目标，也可以创建新 Tween。调度器更新期间的回收会延迟到遍历结束，避免新对象复用当前调用栈中的实例。

### OnCancel

Cancel 标记已经设置，但子动画尚未全部停止。不要在 OnCancel 中假设对象会永久保留。

### OnRewind

上下文状态已经重置，但具体属性回退采样在回调之后执行。

## 回调重入

以下模式受到生命周期保护：

```csharp
tween.OnComplete(context => context.Rewind());
tween.OnComplete(context => context.ReStart());
tween.OnTick((context, _, _) => context.Stop());
```

外层 Complete/Update 在回调返回后会重新检查 valid、待回收标记和终止状态，避免覆盖回调内的新状态。调度器遍历使用帧开始时的运行数量，因此回调中重新 Run 的上下文不会在同一遍历尾部再次 Update。

仍应避免无条件递归，例如 OnBegin 中再次对同一对象 Run，或 OnComplete 中立即 ReStart 一个零时长、同步完成的上下文。

## MonoBehaviour 生命周期模板

自动回收、只关心播放过程：

```csharp
private ITweenContext tween;

private void OnEnable()
{
    tween = transform.DoPosition(Vector3.right, 0.5f).SetOwner(this);
}

private void OnDisable()
{
    this.KillTweens();
    tween = null;
}
```

需要 Rewind/ReStart：

```csharp
private ITweenContext tween;

private void Awake()
{
    tween = transform
        .DoPosition(Vector3.right, 0.5f)
        .SetAutoCycle(false);
}

private void OnDestroy()
{
    tween?.Stop();
    tween?.Recycle();
    tween = null;
}
```

## 批量终止

```csharp
Tween.KillTweens(); // 全部
owner.KillTweens(); // 指定 owner
```

Kill 会 Stop 并 Recycle，因 Stop 是静默终止，所以不会触发 OnCancel。全局 Kill 也会终止其他系统创建的 Tween，除非在关卡重置、应用退出或测试清理阶段，否则优先使用 owner。
