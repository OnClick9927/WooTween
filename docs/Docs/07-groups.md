# Sequence 与 Parallel

## 创建和启动

组不会自动运行：

```csharp
ITweenGroup group = Tween.Sequence();
group.NewContext(CreateFirst);
group.NewContext(CreateSecond);
group.SetLoops(2);
group.Run();
```

`NewContext` 参数是 `Func<ITweenContext>`。工厂应每次返回一个新建或已正确重启的有效上下文。

## 为什么使用工厂

错误做法：

```csharp
var move = transform.DoPosition(Vector3.right, 0.5f);
group.NewContext(() => move);
```

`move` 在加入组之前已经处于自动运行队列；多轮组还会反复返回同一个对象，导致状态和对象池所有权混乱。

推荐：

```csharp
group.NewContext(() => transform.DoPosition(Vector3.right, 0.5f));
```

每次调用工厂都会创建一轮对应的子上下文。

## Sequence

Sequence 每次只维护一个当前活动子 Tween：

```csharp
var sequence = Tween.Sequence()
    .NewContext(() => transform.DoPosition(Vector3.right * 2f, 0.5f))
    .NewContext(() => Tween.DoWait(0.1f))
    .NewContext(() => transform.DoLocalScale(Vector3.one * 1.2f, 0.3f));

sequence.OnComplete(_ => Debug.Log("done"));
sequence.Run();
```

执行顺序：

1. 调用第一个工厂；
2. 监听子 Tween 的 OnComplete 和 OnCancel；
3. 子 Tween 完成或取消时调用下一个工厂；
4. 一轮工厂耗尽后递增组循环；
5. 达到组 loops 后完成。

子 Tween 的 Stop 不触发 OnCancel，因此会停止整个推进链；这是父组 Stop/Rewind 能冻结当前段而不会错误开始下一段的基础。

工厂返回 null 时，Sequence 会跳过该项并继续寻找下一项。虽然代码能处理 null，推荐在工厂内部明确处理条件，便于定位配置错误。

## Parallel

Parallel 在每轮开始时创建全部子 Tween：

```csharp
var parallel = Tween.Parallel()
    .NewContext(() => transform.DoPosition(Vector3.right * 2f, 0.5f))
    .NewContext(() => transform.DoLocalScale(Vector3.one * 1.2f, 0.8f))
    .NewContext(() => canvasGroup.DoAlpha(1f, 0.3f));

parallel.Run();
```

每个子 Tween 的 Complete 或 Cancel 都会增加完成计数。当完成计数达到有效上下文数量时，本轮结束。返回 null 的工厂会被忽略；如果没有任何有效子 Tween，组会立即完成。

## 子 Tween 所有权

组创建子 Tween 后会把子 Tween 的 `autoCycle` 设为 false，并接管其生命周期：

- 子 Tween 完成后从调度器摘除，但不会提前回池；
- 组保留必要的子上下文用于 Rewind；
- 组重启、回池或淘汰中间循环时统一 Recycle；
- 调度器 Update 中发生释放时使用延迟回收，避免调用栈内复用。

不要在外部对组正在持有的子上下文调用 Recycle。外部提前回收会使组跳过失效子项，并可能破坏业务预期。

## 组循环与 Rewind 基线

```csharp
group.SetLoops(3);
```

每轮重新执行工厂。为了正确回退相对起点，组会保留：

- 第一轮子上下文，作为最初 Rewind 基线；
- 当前或最后一轮子上下文，恢复最新一轮造成的变化。

中间轮上下文在安全时机回池，所以有限和无限循环的保留量不会随轮数无限增长，通常上限约为两轮子上下文集合。

Rewind 以逆序处理子上下文：

1. 回退当前/最后一轮；
2. 若当前轮不是第一轮，再回退第一轮基线。

Sequence 的逆序保证后执行的属性先撤销；Parallel 也逆序调用，便于多个子项写同一属性时尽可能恢复组开始前状态。最佳实践仍是避免并行子 Tween 竞争同一个属性。

## 组的 autoCycle

默认组完成后自动回池，并同时释放子 Tween：

```csharp
Tween.Sequence()
    .NewContext(...)
    .Run();
```

完成后需要 Rewind：

```csharp
var sequence = Tween.Sequence()
    .NewContext(...)
    .SetAutoCycle(false)
    .Run();

sequence.Rewind();
```

不再需要时：

```csharp
sequence.Stop();
sequence.Recycle();
```

## 暂停、时间缩放和停止传播

Sequence：

- Pause/UnPause 传播到当前 `inner`；
- SetTimeScale 传播到当前 `inner`；
- Stop 只停止当前 `inner`，历史子项已经结束并摘除。

Parallel：

- Pause/UnPause 传播到当前轮 contexts；
- SetTimeScale 传播到当前轮 contexts；
- Stop 遍历并停止当前轮全部 contexts。

组开始下一轮时，会把组的当前 timeScale 设置到新建子 Tween。

## 进度

Sequence：

```text
(当前轮已创建子项数 - 1 + 当前子项进度) / 工厂总数
```

Parallel：取所有尚未完成、未取消子 Tween 的最小进度。

这些值适合简单进度条，但不是严格的耗时加权：

- Sequence 中每段 duration 不同仍按段数均分；
- Parallel 的最长项主导进度；
- 子 Tween delay/loops 会改变实际耗时；
- 返回 null 的工厂仍可能影响 Sequence 的分母。

需要精确总时长时，应在业务层根据每段 duration、delay 和 loops 计算。

## 嵌套组

工厂可以返回另一个组，但内层组也必须启动：

```csharp
ITweenContext CreateParallelBlock()
{
    return Tween.Parallel()
        .NewContext(() => transform.DoPosition(Vector3.right, 0.4f))
        .NewContext(() => transform.DoLocalScale(Vector3.one * 1.1f, 0.4f))
        .Run();
}

Tween.Sequence()
    .NewContext(CreateParallelBlock)
    .NewContext(() => Tween.DoWait(0.2f))
    .Run();
```

外层组会把返回的内层组设为非自动回收并接管。避免让同一个内层组同时属于多个父组。

## 条件分支

```csharp
sequence.NewContext(() =>
{
    if (!gameObject.activeInHierarchy)
        return null;
    return transform.DoPosition(Vector3.right, 0.3f);
});
```

对复杂分支，更推荐返回一个明确的 `DoWait(0.0001f)` 或拆分组构建逻辑，以便进度和调试工具能看到实际结构。

## 常见错误

### 忘记 Run

`Tween.Sequence()` 和 `Tween.Parallel()` 只分配，不自动启动。

### Run 后才配置空组 loops

空组会在 Run 调用中立即 Complete。应先 SetLoops、注册回调，再 Run。

### 工厂复用已完成对象

每一轮应创建新上下文，除非你完全管理旧对象的 Rewind/ReStart 和 autoCycle。

### 并行动画写同一属性

最终值取决于调度顺序，每帧 setter 会互相覆盖。把它们合并成单个自定义 Tween，或改为 Sequence。

### 无限组未清理

`SetLoops(-1)` 永不自然完成。必须保存组引用，或设置 owner 并在场景/对象生命周期结束时 Kill。
