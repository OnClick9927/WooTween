# 采样与异步等待

## 采样是什么

采样 API 根据指定 `progress` 立即计算并写回一次属性，不加入运行调度器，不依赖后续 Update。它适合：

- 编辑器滑块预览；
- 自定义 Timeline/ActionEditor；
- 录像、截图或离线生成关键帧；
- 手动驱动的动画系统；
- 根据滚动进度、音频时间等外部时钟映射属性。

## Sample

```csharp
Tween.Sample(
    target,
    start,
    end,
    duration,
    getter,
    setter,
    snap,
    progress);
```

示例：

```csharp
[ExecuteAlways]
public sealed class TweenPreview : MonoBehaviour
{
    [Range(0f, 1f)] public float progress;

    private void Update()
    {
        Tween.Sample(
            transform,
            Vector3.zero,
            Vector3.right * 4f,
            1f,
            static target => target.localPosition,
            static (target, value) => target.localPosition = value,
            false,
            progress);
    }
}
```

内部使用 `progress * duration` 作为采样时间。普通时间比例在 TweenContext.Sample 中 Clamp 到 `[0, 1]`。

## 模式采样 API

| 运行 API | 采样 API |
| --- | --- |
| `DoGoto` | `Sample` |
| `DoWait` | `SampleWait` |
| `DoShake` | `SampleShake` |
| `DoPunch` | `SamplePunch` |
| `DoJump` | `SampleJump` |
| `DoArray` | `SampleArray` |
| `DoBezier` | `SampleBezier` |

各模式参数与运行版本一致，区别是 `autoRun` 被替换为 `progress`。

```csharp
Tween.SampleBezier(
    transform,
    1f,
    static target => target.position,
    static (target, value) => target.position = value,
    points,
    snap: false,
    progress: previewProgress);
```

`SampleWait` 没有目标属性，因此不会产生可见变化，主要用于保持上层采样接口结构一致。

## 采样缓存

WooTween 为每个 `<T, Target>` 组合保留一个静态采样上下文：

```text
SampleCache<Vector3, Transform>
SampleCache<float, CanvasGroup>
...
```

每次采样复用对应上下文。采样结束后会释放：

- target；
- start/end/strength；
- getter/setter；
- points 和内部点缓冲。

这减少持续拖动预览滑块时的 GC，并避免静态缓存长期持有场景对象。

## 采样限制

- 同一 `<T, Target>` 缓存不是并发容器，应只在 Unity 主线程调用；
- 不要在同一 setter 中递归调用相同 `<T, Target>` 的 Sample；
- Sample 使用缓存上下文当前的默认 evaluator，公共采样 API 没有返回上下文供调用者 SetEase；
- Shake 依赖 UnityEngine.Random，相同 progress 多次采样不保证相同随机结果；
- points 必须至少三个；
- duration 应大于 0；
- Sample 不产生 OnBegin、OnTick、OnComplete 等生命周期回调。

需要可重复的 Shake 离线采样时，应改用确定性 evaluator/模式，或在外层固定并恢复 Unity 随机状态。

## await 支持

`ITweenContext` 提供扩展 awaiter，因此可以直接 await：

```csharp
private async void PlayIntro()
{
    await transform.DoPosition(Vector3.zero, 0.4f);
    await transform.DoLocalScale(Vector3.one, 0.25f);
}
```

awaiter 在构造时向上下文注册 OnComplete，并把 continuation 放入池化队列。完成回调中依次执行 continuation，然后归还队列。

## await 的完成条件

awaiter 只监听 OnComplete：

- 自然完成：恢复；
- `Complete(true)`：恢复；
- Stop：不恢复；
- Cancel / `Complete(false)`：不恢复；
- Rewind：不恢复；
- Recycle：不恢复。

因此不要把可能被取消的 Tween 直接作为唯一等待条件：

```csharp
// 如果 OnDisable 中 Cancel，这个 async 流程不会继续。
await tween;
```

当前 awaiter 不支持 CancellationToken，也不会在取消时抛出 OperationCanceledException。

## 安全的异步使用方式

### 确保最终 Complete

```csharp
private async void PlayNonCancelable()
{
    var tween = transform.DoPosition(Vector3.right, 0.5f);
    await tween;
    // 这里只在 Complete 后执行
}
```

### 自己桥接取消

```csharp
private Task<bool> WaitTween(ITweenContext tween)
{
    var source = new TaskCompletionSource<bool>();
    tween.OnComplete(_ => source.TrySetResult(true));
    tween.OnCancel(_ => source.TrySetResult(false));
    return source.Task;
}
```

使用 `TaskCompletionSource` 会产生额外分配，但能得到明确的完成/取消结果。项目使用 UniTask 时也可以写对应桥接扩展。

### 组件销毁

async void 方法在 MonoBehaviour 销毁后不会自动停止。恢复后访问 Unity 对象前检查：

```csharp
await tween;
if (this == null)
    return;
```

## await 与对象池

默认 autoCycle=true 时，OnComplete continuation 在上下文正式回池前执行，但调度帧结束后对象可能马上变为 Sleep。不要在 await 后长期保存返回上下文并假设它仍然有效。

```csharp
var tween = transform.DoPosition(Vector3.right, 0.5f);
await tween;

// 可以读取业务目标最终值。
Debug.Log(transform.position);

// 不要在之后依赖 tween.Rewind()，除非创建时 SetAutoCycle(false)。
```

需要 await 后回退：

```csharp
var tween = transform
    .DoPosition(Vector3.right, 0.5f)
    .SetAutoCycle(false);

await tween;
tween.Rewind();

// 最终清理
tween.Recycle();
```

## 外部时间轴设计建议

用 Sample 构建时间轴时：

1. 时间轴保存纯配置，不保存采样上下文；
2. 每帧计算归一化 progress；
3. 对每个轨道调用对应 Sample；
4. 明确多个轨道写同一属性的优先级；
5. 编辑器采样完成后标记 UnityEngine.Object dirty；WooTween 在非 Play Mode 已对 UnityEngine.Object 目标调用 `EditorUtility.SetDirty`；
6. 在 Undo/Redo 场景中由上层工具负责 `Undo.RecordObject`，WooTween Sample 本身不记录 Undo。
