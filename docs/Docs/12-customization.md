# 自定义属性与值类型

WooTween 的扩展层分为三档：

1. 为已有支持值类型封装新的目标属性；
2. 实现新的缓动 evaluator；
3. 修改核心程序集，增加新的值类型计算器。

优先选择第一档，改动最少且不会影响包升级。

## 为自定义组件添加扩展

假设组件：

```csharp
public sealed class HealthBar : MonoBehaviour
{
    public float DisplayValue;
}
```

扩展：

```csharp
using WooTween;

public static class HealthBarTweenExtensions
{
    public static ITweenContext<float, HealthBar> DoValue(
        this HealthBar target,
        float start,
        float end,
        float duration,
        bool snap = false)
    {
        return Tween.DoGoto(
            target,
            start,
            end,
            duration,
            static value => value.DisplayValue,
            static (value, current) => value.DisplayValue = current,
            snap);
    }

    public static ITweenContext<float, HealthBar> DoValue(
        this HealthBar target,
        float end,
        float duration,
        bool snap = false)
    {
        return target.DoValue(target.DisplayValue, end, duration, snap);
    }
}
```

建议同时提供显式 start/end 和当前值/end 两种重载，与内置扩展保持一致。

## 避免闭包

固定属性 getter/setter 使用 static lambda：

```csharp
static target => target.DisplayValue
static (target, value) => target.DisplayValue = value
```

以下写法捕获 `scale`，每次调用可能创建闭包：

```csharp
float scale = 100f;
Tween.DoGoto(
    target,
    0f,
    1f,
    1f,
    value => value.DisplayValue / scale,
    (value, current) => value.DisplayValue = current * scale,
    false);
```

可以把额外参数放入 Target 包装对象，或为固定业务场景缓存委托。

## 自定义 IValueEvaluator

无状态 evaluator：

```csharp
public sealed class SmoothStepEvaluator : IValueEvaluator
{
    public static readonly SmoothStepEvaluator Instance = new SmoothStepEvaluator();

    private SmoothStepEvaluator() { }

    public float Evaluate(float percent, float time, float duration)
    {
        return percent * percent * (3f - 2f * percent);
    }
}
```

```csharp
tween.SetEvaluator(SmoothStepEvaluator.Instance);
```

缓存实例避免反复 new。Evaluator 应满足：

- 主线程快速执行；
- 不做 IO；
- 不分配临时对象；
- 对 percent=0 和 percent=1 给出明确结果；
- duration 接近 0 时不自行产生除零；
- 共享实例时不存每个 Tween 的可变进度。

## 自定义 TweenComponentActor

```csharp
[System.Serializable]
public sealed class DoHealthActor
    : TweenComponentActor<float, HealthBar>
{
    public StartValueType startType;
    public float start;
    public float end = 100f;

    protected override ITweenContext<float, HealthBar> OnCreate()
    {
        if (startType == StartValueType.Relative)
            return target.DoValue(end, duration, snap);
        return target.DoValue(start, end, duration, snap);
    }
}
```

基类会自动处理：

- Target 为空时从 TweenComponent 所在 GameObject 获取 HealthBar；
- SetLoop；
- SetDelay；
- SetSnap；
- SetDuration；
- SetSourceDelta；
- SetId；
- Ease 或 AnimationCurve。

OnCreate 只负责创建正确属性 Tween。

## 自定义组 Actor

```csharp
[System.Serializable]
public sealed class DoFlashActor
    : TweenGroupComponentActor<CanvasGroup>
{
    protected override ITweenGroup OnCreate()
    {
        return Tween.Sequence()
            .NewContext(() => target.DoAlpha(0f, duration * 0.5f))
            .NewContext(() => target.DoAlpha(1f, duration * 0.5f))
            .Run();
    }
}
```

`TweenGroupComponentActor<TTarget>` 会给返回组设置 id 和 loops。自定义 OnCreate 返回前应启动内层组。

## 自定义 ActorEditor

默认反射 Inspector 足以绘制 public 字段。需要 Scene Handle 时：

```csharp
#if UNITY_EDITOR
public sealed class DoHealthActorEditor
    : TweenActorEditor<DoHealthActor>
{
    protected override void OnSceneGUI(DoHealthActor actor)
    {
        // 自定义场景绘制
    }
}
#endif
```

放入 Editor 程序集，并引用 WooTween.Editor。

## 新增值类型

核心默认只支持 float、Vector2、Vector3、Vector4、Color 和 Rect。`ValueCalculator<T>` 与注册字典当前不是公开扩展点，所以外部程序集无法仅通过一个公共 API 注册新类型。

需要 Quaternion、自定义 struct 等类型时有两种策略。

### 策略一：映射到已支持类型

Quaternion 可映射为 Euler Vector3：

```csharp
Tween.DoGoto(
    transform,
    start.eulerAngles,
    end.eulerAngles,
    duration,
    static target => target.rotation.eulerAngles,
    static (target, value) => target.rotation = Quaternion.Euler(value),
    false);
```

或者动画化一个 float 进度，并在 setter 中调用专用插值：

```csharp
Quaternion from = transform.rotation;
Quaternion to = Quaternion.LookRotation(Vector3.forward);

Tween.DoGoto(
    transform,
    0f,
    1f,
    duration,
    static _ => 0f,
    (target, value) => target.rotation = Quaternion.Slerp(from, to, value),
    false);
```

第二种写法捕获 from/to，会产生闭包，但能使用 Quaternion.Slerp 的正确旋转语义。可以把参数放进专用 Target 类以消除闭包。

### 策略二：扩展核心源码

必须让自定义类型对 WooTween Runtime 程序集可见，然后在同一程序集实现计算器：

```csharp
internal sealed class ValueCalculator_MyValue
    : ValueCalculator<MyValue>
{
    public override MyValue Lerp(MyValue a, MyValue b, float t)
        => MyValue.Lerp(a, b, t);

    public override MyValue Add(MyValue a, MyValue b) => a + b;
    public override MyValue Minus(MyValue a, MyValue b) => a - b;
    public override MyValue Multi(MyValue value, float scale) => value * scale;
    public override MyValue Dev(MyValue value, float divisor) => value / divisor;
    public override MyValue Snap(MyValue value) => value.Round();
    public override MyValue MultiStrength(MyValue strength, MyValue value)
        => strength.ComponentMultiply(value);
    public override MyValue RangeValue(MyValue value)
        => value * Range();
}
```

然后修改 `Tween.value_calcs` 初始化表：

```csharp
{ typeof(MyValue), new ValueCalculator_MyValue() },
```

所有抽象运算都必须正确实现，因为 Normal 之外的模式和 LoopType.Add 依赖这些方法。

这种方案会修改包源码。建议维护 fork 或把 WooTween 嵌入项目，不要直接改 PackageCache。

## 设计 ValueCalculator 的检查表

- Lerp 在 t=0 返回 start，t=1 返回 end；
- Add/Minus 互为合理逆运算；
- Multi/Dev 对标量行为一致；
- Dev 的 divisor=0 有明确防护；
- Snap 不改变不需要取整的语义；
- MultiStrength 是逐分量还是其他领域语义；
- RangeValue 的随机范围符合 Shake 预期；
- 类型包含托管引用时，池化点缓冲会在 Clear 时清除引用；
- EqualityComparer<T>.Default 能合理判断结果是否变化。

## 自定义调度时机

当前公共 API 固定使用 TweenScheduler_Runtime.Update。TweenScheduler 是内部实现的一部分，不建议业务直接依赖。

需要 FixedUpdate、UnscaledTime 或手动时钟时，可以：

- 使用 Sample 由自己的时钟驱动；
- 在 fork 中抽象 GetDeltaTime/调度器；
- 为特定属性实现业务层更新器。

仅设置 timeScale 不能切换到 `Time.unscaledDeltaTime`。
