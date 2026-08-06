# API 总表

命名空间：

```csharp
using WooTween;
```

## 创建 API

### Tween.DoGoto

```csharp
ITweenContext<T, Target> DoGoto<T, Target>(
    Target target,
    T start,
    T end,
    float duration,
    Func<Target, T> getter,
    Action<Target, T> setter,
    bool snap,
    bool autoRun = true)
```

### Tween.DoWait

```csharp
ITweenContext DoWait(float duration, bool autoRun = true)
```

### Tween.DoShake

```csharp
ITweenContext<T, Target> DoShake<T, Target>(
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

### Tween.DoPunch

```csharp
ITweenContext<T, Target> DoPunch<T, Target>(
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

### Tween.DoJump

```csharp
ITweenContext<T, Target> DoJump<T, Target>(
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

### Tween.DoArray

```csharp
ITweenContext<T, Target> DoArray<T, Target>(
    Target target,
    float duration,
    Func<Target, T> getter,
    Action<Target, T> setter,
    T[] points,
    bool snap = false,
    bool autoRun = true)
```

### Tween.DoBezier

```csharp
ITweenContext<T, Target> DoBezier<T, Target>(
    Target target,
    float duration,
    Func<Target, T> getter,
    Action<Target, T> setter,
    T[] points,
    bool snap = false,
    bool autoRun = true)
```

## 组合 API

```csharp
ITweenGroup Tween.Sequence()
ITweenGroup Tween.Parallel()
```

`ITweenGroup`：

```csharp
ITweenGroup NewContext(Func<ITweenContext> func)
void SetLoops(int loops)
```

Sequence/Parallel 需要显式 Run。

## 采样 API

```csharp
void Sample<T, Target>(
    Target target, T start, T end, float duration,
    Func<Target, T> getter, Action<Target, T> setter,
    bool snap, float progress)

void SampleWait(float duration, float progress)

void SampleShake<T, Target>(
    Target target, T start, T end, float duration,
    Func<Target, T> getter, Action<Target, T> setter,
    T strength, int frequency = 10, float dampingRatio = 1,
    bool snap = false, float progress = 1)

void SamplePunch<T, Target>(
    Target target, T start, T end, float duration,
    Func<Target, T> getter, Action<Target, T> setter,
    T strength, int frequency = 10, float dampingRatio = 1,
    bool snap = false, float progress = 1)

void SampleJump<T, Target>(
    Target target, T start, T end, float duration,
    Func<Target, T> getter, Action<Target, T> setter,
    T strength, int jumpCount = 5, float jumpDamping = 2f,
    bool snap = false, float progress = 1)

void SampleArray<T, Target>(
    Target target, float duration,
    Func<Target, T> getter, Action<Target, T> setter,
    T[] points, bool snap = false, float progress = 1)

void SampleBezier<T, Target>(
    Target target, float duration,
    Func<Target, T> getter, Action<Target, T> setter,
    T[] points, bool snap = false, float progress = 1)
```

## 公共上下文控制扩展

以下扩展适用于 `ITweenContext` 和 `ITweenGroup`：

| API | 返回 | 说明 |
| --- | --- | --- |
| `Run()` | 原上下文类型 | 启动并加入调度器 |
| `ReStart()` | 原上下文类型 | Stop 后重新 Run |
| `Pause()` | 原上下文类型 | 暂停 |
| `UnPause()` | 原上下文类型 | 恢复 |
| `Stop()` | 原上下文类型 | 静默终止 |
| `Cancel()` | void | 触发取消回调并终止 |
| `Complete(bool)` | 原上下文类型 | true=完成，false=取消 |
| `Rewind()` | 原上下文类型 | 停止并采样回起点 |
| `Recycle()` | 原上下文类型 | 显式归还池 |
| `SetTimeScale(float)` | 原上下文类型 | 设置时间倍率 |
| `SetAutoCycle(bool)` | 原上下文类型 | 设置自动回池 |
| `SetId(string)` | 原上下文类型 | 设置 id |
| `SetOwner(object)` | 原上下文类型 | 设置批量管理 owner |

## 回调扩展

```csharp
T OnBegin<T>(this T context, Action<ITweenContext> action)
T OnComplete<T>(this T context, Action<ITweenContext> action)
T OnCancel<T>(this T context, Action<ITweenContext> action)
T OnRewind<T>(this T context, Action<ITweenContext> action)
T OnTick<T>(this T context, Action<ITweenContext, float, float> action)
```

多次调用追加委托。

## 数值上下文配置扩展

适用于 `ITweenContext<T, Target>`：

| API | 说明 |
| --- | --- |
| `SetLoop(LoopType type, int loops)` | 设置数值 Tween 循环 |
| `SetDelay(float value)` | 设置首次运行延迟 |
| `SetSourceDelta(float delta)` | 设置当前源值混合 |
| `SetEvaluator(IValueEvaluator)` | 自定义缓动计算器 |
| `SetEase(Ease)` | 内置 Ease |
| `SetAnimationCurve(AnimationCurve)` | Unity AnimationCurve |
| `SetSnap(bool)` | 最终值取整 |
| `SetDuration(float)` | 修改 duration |
| `SetFrequency(int)` | Shake/Punch frequency |
| `SetDampingRatio(float)` | Shake/Punch damping |
| `SetJumpCount(int)` | Jump 次数 |
| `SetJumpDamping(float)` | Jump 衰减 |
| `SetStrength(T)` | Shake/Punch/Jump strength |

## 全局管理 API

```csharp
List<Type> Tween.GetSupportTypes()
bool Tween.IsRunning(ITweenContext context)
void Tween.KillTweens()
void owner.KillTweens()
float Tween.GetDeltaTime()
```

`GetDeltaTime` 在 Play Mode 返回 `Time.deltaTime`；Edit Mode 返回编辑器调度器计算的 delta。

## await

```csharp
Tween.IAwaiter<T> GetAwaiter<T>(this T context)
    where T : ITweenContext
```

只以 OnComplete 为完成信号。

## ITweenContext 属性

| 属性 | 类型 | 说明 |
| --- | --- | --- |
| `id` | string | 调试标识 |
| `isDone` | bool | 是否完成 |
| `autoCycle` | bool | 是否自动回池 |
| `paused` | bool | 是否暂停 |
| `state` | TweenContextState | 当前状态 |
| `GetPercent()` | float | 估算进度 |

## TweenContextState

```csharp
Allocate
Run
Pause
Sleep
```

## TweenType

```csharp
Normal
Shake
Punch
Jump
Bezier
Array
WaitTime
```

通常由创建 API 决定，不需要调用者直接设置。

## LoopType

```csharp
Restart
PingPong
Add
```

## Ease

```csharp
Linear
InSine, OutSine, InOutSine
InQuad, OutQuad, InOutQuad
InCubic, OutCubic, InOutCubic
InQuart, OutQuart, InOutQuart
InQuint, OutQuint, InOutQuint
InExpo, OutExpo, InOutExpo
InCirc, OutCirc, InOutCirc
```

## IValueEvaluator

```csharp
public interface IValueEvaluator
{
    float Evaluate(float percent, float time, float duration);
}
```

## TweenComponent API

| 成员 | 说明 |
| --- | --- |
| `bool paused` | 没有上下文时也返回 true |
| `T FindActor<T>()` | 返回第一个匹配 Actor |
| `Play()` | 首次创建并运行；已有上下文则 ReStart |
| `Pause()` | 暂停组 |
| `UnPause()` | 恢复组 |
| `Stop()` | 静默终止组 |
| `Cancel()` | 取消组 |
| `Rewind()` | 回退组 |
| `ReStart()` | 重启组 |
| `onBegin` | UnityEvent<ITweenContext> |
| `onComplete` | UnityEvent<ITweenContext> |
| `onCancel` | UnityEvent<ITweenContext> |
| `onRewind` | UnityEvent<ITweenContext> |
| `onTick` | UnityEvent<ITweenContext,float,float> |

## Transform 扩展

普通：

```text
DoPosition, DoLocalPosition, DoRotate, DoLocalRotate, DoLocalScale
```

Shake：

```text
DoShakePosition, DoShakeLocalPosition,
DoShakeRotate, DoShakeLocalRotate, DoShakeLocalScale
```

Punch：

```text
DoPunchPosition, DoPunchLocalPosition,
DoPunchRotate, DoPunchLocalRotate, DoPunchLocalScale
```

Jump：

```text
DoJumpPosition, DoJumpLocalPosition,
DoJumpRotate, DoJumpLocalRotate, DoJumpLocalScale
```

路径：

```text
DoPositionArray, DoLocalPositionArray,
DoRotateArray, DoLocalRotateArray, DoLocalScaleArray,
DoPositionArrayBezier, DoLocalPositionArrayBezier,
DoRotateArrayBezier, DoLocalRotateArrayBezier,
DoLocalScaleArrayBezier
```

## 其他 Unity 扩展索引

| 目标 | API |
| --- | --- |
| RectTransform | DoSizeDelta、DoPivot、DoAnchorMin、DoAnchorMax、DoAnchoredPosition |
| Graphic | DoColor、DoGradientColor |
| Image | DoFillAmount |
| Slider | DoValue |
| UGUI Text | DoFontSize、DoText |
| CanvasGroup | DoAlpha |
| TMP_Text | DoFontSize、DoText、DoCharacterSpacing、DoWordSpacing、DoParagraphSpacing、DoLineSpacing |
| Camera | DoAspect、DoNearClipPlane、DoFarClipPlane、DoFieldOfView、DoOrthographicSize、DoBackgroundColor、DoRect、DoPixelRect |
| AudioSource | DoVolume、DoPitch |
| Material | DoFloat、DoInt、DoColor、DoVector |
| SpriteRenderer | DoColor、DoGradientColor |
| Rigidbody | DoPosition、DoRotation、DoShakePosition、DoPunchPosition、DoShakeRotation、DoPunchRotation |
| Rigidbody2D | DoPosition、DoRotation、DoShakePosition、DoPunchPosition、DoShakeRotation、DoPunchRotation |

扩展的完整行为和限制见 [Unity 扩展方法](11-unity-extensions.md)。
