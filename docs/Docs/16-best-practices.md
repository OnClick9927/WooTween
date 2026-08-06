# 实践建议

## 一次性动画：使用默认自动回收

```csharp
transform
    .DoPosition(targetPosition, 0.3f)
    .SetEase(Ease.OutCubic)
    .SetOwner(this);
```

不需要完成后重用上下文时，不要关闭 autoCycle。

## 可逆动画：明确持有和释放

```csharp
private ITweenContext panelTween;

private void Awake()
{
    panelTween = panel
        .DoAnchoredPosition(Vector2.zero, 0.25f)
        .SetAutoCycle(false);
}

public void ResetPanel()
{
    panelTween.Rewind();
}

private void OnDestroy()
{
    panelTween?.Stop();
    panelTween?.Recycle();
    panelTween = null;
}
```

把 `SetAutoCycle(false)` 和清理代码放在同一个类型中，避免所有权分散。

## 用 owner 管理页面或实体

```csharp
private void PlayTweens()
{
    title.DoAlpha(1f, 0.2f).SetOwner(this);
    panel.DoAnchoredPosition(Vector2.zero, 0.3f).SetOwner(this);
}

private void OnDisable()
{
    this.KillTweens();
}
```

owner 比全局 Kill 更安全，也比维护多个一次性字段更简洁。

## Sequence 工厂中读取最新起点

```csharp
Tween.Sequence()
    .NewContext(() => transform.DoPosition(firstTarget, 0.3f))
    .NewContext(() => transform.DoPosition(secondTarget, 0.3f))
    .Run();
```

不要在组外提前创建子 Tween。工厂的惰性执行能保证第二段从第一段最终位置开始。

## 配置顺序

推荐顺序：

```csharp
var tween = Tween.DoGoto(..., autoRun: false)
    .SetDuration(duration)
    .SetDelay(delay)
    .SetEase(ease)
    .SetLoop(loopType, loops)
    .SetAutoCycle(autoCycle)
    .SetOwner(owner)
    .SetId(id)
    .OnBegin(OnBegin)
    .OnTick(OnTick)
    .OnComplete(OnComplete)
    .OnCancel(OnCancel);

tween.Run();
```

autoRun=true 时也应在同一个同步调用链中完成配置。

## 一个属性只由一个动画源控制

避免：

```csharp
Tween.Parallel()
    .NewContext(() => transform.DoPosition(a, 1f))
    .NewContext(() => transform.DoPosition(b, 1f))
    .Run();
```

每帧后写入者覆盖前写入者。把路径合并成 Array/Bézier，或用 Sequence 明确时序。

## UI 打开/关闭

快速重复点击可能让多个 Tween 竞争。先 Kill owner，再启动新状态：

```csharp
public void Open()
{
    this.KillTweens();
    panel.DoAnchoredPosition(Vector2.zero, 0.25f)
        .SetEase(Ease.OutCubic)
        .SetOwner(this);
}

public void Close()
{
    this.KillTweens();
    panel.DoAnchoredPosition(hiddenPosition, 0.2f)
        .SetEase(Ease.InCubic)
        .SetOwner(this);
}
```

如果需要无跳变接续，使用只传 end 的重载，让新 Tween 从当前属性值开始。

## 无限循环

```csharp
private void OnEnable()
{
    icon.DoLocalScale(Vector3.one * 1.1f, 0.5f)
        .SetLoop(LoopType.PingPong, -1)
        .SetOwner(this);
}

private void OnDisable()
{
    this.KillTweens();
}
```

每个 `-1` 必须能在代码审查中找到对应清理路径。

## 低 GC 回调

```csharp
private void HandleComplete(ITweenContext context)
{
    completed = true;
}

tween.OnComplete(HandleComplete);
```

实例方法委托仍会创建委托对象，但不会捕获额外闭包。不要在 OnTick 中使用 LINQ、字符串插值或创建新集合。

## 文本逐字显示

短文本可直接 DoText。长文本、富文本或本地化文本建议动画 TMP `maxVisibleCharacters`，避免 Substring 分配并保持富文本标签结构。

## 物理对象

动态物理对象优先让物理系统控制。若使用现有 Rigidbody 扩展：

- 尽量用于 isKinematic；
- 避免同时施加力；
- 检查碰撞插值表现；
- 明确普通 Update 与 FixedUpdate 的差异。

## 编辑器工具

时间轴或 Inspector 滑块使用 Sample，而不是每次变更创建并 Run 一个零时长 Tween。上层负责 Undo：

```csharp
#if UNITY_EDITOR
Undo.RecordObject(target, "Preview Tween");
Tween.Sample(...);
#endif
```

## 错误输入前置校验

```csharp
if (target == null)
    return;
if (duration <= 0f)
    return;
if (points == null || points.Length < 3)
    return;
```

核心部分配置只记录错误，不会为所有非法输入提供恢复策略。公共业务封装应更早验证。

## 测试清单

每个重要动画流程至少验证：

- 正常完成；
- 开始前取消；
- 运行中 Pause/UnPause；
- 运行中 Stop；
- 完成后 Rewind（若支持）；
- ReStart；
- GameObject OnDisable；
- Scene 切换；
- 快速重复触发；
- timeScale=0 和非 1；
- loops=1、多轮、无限循环；
- Sequence 子项取消；
- Parallel 不同时长；
- 回调中创建/停止新 Tween；
- 目标对象提前销毁；
- Edit Mode 预览进入 Play Mode；
- IL2CPP 构建。

## 代码审查清单

- 是否错误持有 autoCycle=true 完成后的上下文；
- autoCycle=false 是否显式 Recycle；
- 无限循环是否有 owner；
- 组子项是否通过工厂创建；
- points 是否至少三个；
- duration 是否为正；
- 是否多个 Tween 写同一属性；
- 是否在 OnTick 分配；
- 是否从后台线程调用；
- 是否错误假设 Stop 会触发 OnCancel；
- 是否错误假设 Cancel 会恢复 await；
- 是否依赖完成后的 GetPercent==1；
- 是否在 PackageCache 直接改源码。

## 发布前验证

1. 清空 Console；
2. 在目标 Unity LTS 版本重新导入；
3. 运行核心场景；
4. 打开 TweenWatcher 检查残留；
5. Profiler 检查稳定帧 GC.Alloc；
6. 测试 Mono 和 IL2CPP；
7. 测试目标平台的暂停/恢复和场景切换；
8. 固定 UPM 提交；
9. 记录 WooTween 版本和本地扩展修改。
