# 性能与内存

WooTween 的优化目标是让稳定运行阶段尽量不产生 GC，并减少调度器在大量 Tween 结束时的列表移动成本。本章说明现有机制和调用方应承担的部分。

## 对象池

TweenContext、Sequence、Parallel、点缓冲和 await continuation 队列均使用对象池。

上下文池按具体类型划分：

```text
TweenContext<Vector3, Transform>
TweenContext<float, CanvasGroup>
TweenSequence
TweenParallel
```

池使用 `Stack<T>`，最近回收的对象最先复用。相比 FIFO Queue，这更容易命中仍在 CPU cache 中的对象数据。

## 回池时清理引用

上下文 Reset 会清除：

- target；
- getter / setter；
- start / end / strength；
- points；
- AnimationCurve 引用；
- owner、id；
- 所有回调委托；
- 运行列表索引和待回收标记。

这防止对象池长期保留已经卸载的场景对象、纹理/材质关联对象或大闭包。

## 点缓冲

Array/Bézier 使用 `ArrayBuffer<T>`：

- 初始容量 16；
- 仅在 requiredLength 超过容量时按 2 倍扩容；
- 相同长度和内容时不重复复制；
- 回池时 length 归零；
- 仅当 T 是引用类型或包含引用字段的 struct 时清空已用区间；
- 纯数值 struct 不做无意义 Array.Clear。

“包含引用字段”在每个 T 的静态初始化阶段通过类型字段检查一次。这样兼顾自定义 struct 的引用释放正确性与 Vector/Color 等纯值类型的清理成本。

## Bézier 计算

Bezier 在单次采样中流式递推 Bernstein 系数，不再分配临时 float 数组，也不使用按长度缓存的全局数组字典。

复杂度仍随 points 数量线性增长。Scene 路径需要平滑不等于运行时必须使用大量控制点；优先用较少、语义清晰的点。

## 缓动 evaluator

内置 EaseEvaluator 是 struct，但预先装箱并缓存在静态数组中。`SetEase` 只引用缓存对象，不产生新装箱。

AnimationCurveEvaluator 使用 sealed class：

- 如果 struct 赋给 `IValueEvaluator`，每次会装箱；
- class 在每个 TweenContext 第一次使用曲线时创建一次；
- 后续只替换 curve；
- 回池时 curve=null。

因此它用一次懒分配替代每次 SetAnimationCurve 的装箱分配。

## 调度器列表

运行列表使用稳定 tombstone 摘除：

1. 每个上下文记录 runIndex；
2. 终止时把对应位置设为 null，不执行 `List.Remove` 搬移后续元素；
3. 当前遍历跳过 null；
4. 帧末一次稳定压缩并更新索引。

这避免同一帧大量 Tween 完成时出现 O(n²) 的重复移动。

等待列表也记录 waitIndex，摘除时置 null，处理结束后统一 Clear。

调度器在遍历开始时快照运行数量。回调中新增或重启的 Tween 不会在同一运行循环尾部再次 Update。

## 延迟回收

在 Update 调用栈内结束的对象先摘除并加入待回收列表，遍历结束后统一入池。这样：

- OnComplete 中创建同类型 Tween 不会复用当前对象；
- 子 Tween 回调推进 Sequence 时不会覆盖正在返回的子对象；
- KillTweens 不会因即时列表压缩跳过相邻对象。

待回收列表本身复用容量，不会每帧重新创建。

## 采样缓存

Sample 为每个 `<T, Target>` 使用一个静态上下文，并在 finally 中释放目标、委托和点引用。连续编辑器预览无需反复创建上下文或数组。

采样缓存不是线程安全的，也不支持同泛型组合的递归采样。

## 调用方常见 GC 来源

### 捕获 lambda

```csharp
float factor = 2f;
tween.OnTick((_, time, _) => Debug.Log(time * factor));
```

捕获 factor 会创建闭包。一次性 UI 动画影响通常有限，但每帧/每实体大量创建时应避免。

### 每次创建新的 evaluator

```csharp
tween.SetEvaluator(new MyEvaluator());
```

无状态 evaluator 使用 static readonly 单例。

### OnTick 字符串和日志

字符串插值、Debug.Log、LINQ 和集合创建通常远高于 Tween 本身成本。Profiler 中发现 Tween 回调分配时，应展开调用栈确认是框架还是业务回调。

### DoText

UGUI/TMP `DoText` 使用 `Substring` 构造逐字前缀，字符数变化时会产生新字符串。长文本或大量并行动画应考虑 TMP 的 `maxVisibleCharacters`，以避免反复分配字符串。

### Material 属性名

Material 扩展捕获字符串 name，getter 和 setter 会形成闭包。高频效果可用固定 property ID 和自定义 Target 数据结构优化。

### 每次构建 points

```csharp
transform.DoPositionArray(duration, new[] { a, b, c });
```

每次 `new[]` 都会分配。固定路径缓存数组；动态路径可复用业务数组，WooTween 会在 Run 时复制到内部缓冲。

## autoCycle=false 的成本

关闭自动回收意味着上下文、回调和配置保持有效，直到显式 Recycle。

Sequence/Parallel 为 Rewind 保留第一轮基线和当前/最后一轮子上下文。一个拥有 100 个子项且 autoCycle=false 的组可能保留约两轮子对象；这是正确 Rewind 相对值的内存成本。

建议：

- 仅对确实需要 Rewind/ReStart 的动画关闭 autoCycle；
- 组件 OnDisable 负责回收；
- 业务控制器 OnDestroy 显式 Stop + Recycle；
- 用 TweenWatcher 查找残留；
- 不要对一次性提示动画关闭自动回收。

## 无限循环

`loops=-1` 不会自然回收。所有无限循环必须有明确 owner 或保存引用：

```csharp
var pulse = transform
    .DoLocalScale(Vector3.one * 1.1f, 0.5f)
    .SetLoop(LoopType.PingPong, -1)
    .SetOwner(this);

private void OnDisable()
{
    this.KillTweens();
}
```

## 目标销毁

对象池清理只能在 Tween 终止时释放 target。在 Tween 运行期间销毁 UnityEngine.Object，getter/setter 访问可能触发 MissingReferenceException。

应在目标 OnDisable/OnDestroy 时 Kill 对应 owner，不要等待 setter 自行发现目标为空。

## 线程安全

WooTween 设计为 Unity 主线程使用：

- 调度器列表无锁；
- 对象池无锁；
- UnityEngine 属性通常只能主线程访问；
- SampleCache 是共享静态对象。

不要从 Task.Run、Thread 或 Job 直接创建/控制/采样 Tween。

## 性能检查流程

1. 用 Unity Profiler CPU Timeline 定位 `TweenScheduler_Runtime.Update`；
2. 展开 Tween 回调，区分框架计算和业务代码；
3. 打开 GC.Alloc 列，确认稳定运行帧是否分配；
4. 用 Memory Profiler 检查 autoCycle=false 上下文是否持有场景对象；
5. 用 TweenWatcher 根据分配堆栈定位未回收对象；
6. 压测同时完成、KillTweens、Sequence 多轮和路径控制点数量；
7. 分别测试 Mono 和 IL2CPP 目标平台。

## 优化优先级

通常按以下顺序收益最大：

1. 去掉 OnTick 日志和临时集合；
2. 避免每帧创建 Tween，改为按事件创建；
3. 缓存 points、evaluator 和不捕获委托；
4. 正确回收 autoCycle=false 与无限循环；
5. 避免多个 Tween 竞争同一属性；
6. 减少不必要的 Bézier 控制点；
7. 最后再考虑修改核心调度器。
