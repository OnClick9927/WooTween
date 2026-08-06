# TweenComponent

`TweenComponent` 为 WooTween 提供可序列化、可预览的组件工作流。添加菜单路径：

```text
Add Component > IFramework > TweenComponent
```

同一个 GameObject 只允许一个 TweenComponent。

## 顶层配置

| 字段 | 含义 |
| --- | --- |
| Mode | `Sequence` 串行或 `Parallel` 并行 |
| Time Scale | 传递给组，并在创建子 Tween 时传递给子项 |
| Loops | 整个组的循环次数，`-1` 为无限 |
| Id | 组上下文 id，用于识别和 TweenWatcher |
| Play On Awake | Awake 时调用 Play |

组件内部把 Actor 列表转换为一个 ITweenGroup，并固定设置 `SetAutoCycle(false)`，以支持 Inspector 的 Stop、Rewind 和再次 Play。

## 添加 Actor

Inspector 的 Actors 下拉菜单会扫描所有非抽象 `TweenComponentActor` 子类，并按目标类型分组，例如：

- Common/DoWait
- Transform/DoPosition
- RectTransform/DoAnchoredPosition
- Graphic/DoColor
- Camera/DoFieldOfView
- Rigidbody/DoPosition

选择后 Actor 被存入 `[SerializeReference] List<TweenComponentActor>`。SerializeReference 使不同派生类型能够共存在一个列表中。

## Actor 公共字段

所有 Actor：

| 字段 | 含义 |
| --- | --- |
| Active | 是否参与本次组构建 |
| ID | 子 Tween id |
| Duration | 单轮持续时间 |

数值 Actor 额外提供：

| 字段 | 含义 |
| --- | --- |
| Target | 可显式指定；为空时从 TweenComponent 所在 GameObject 获取 |
| Snap | 最终值是否取整 |
| Source Delta | 当前源值动态混合参数 |
| Delay | 本 Actor 首轮开始前延迟 |
| Loop Type | Restart、PingPong、Add |
| Loops | Actor 自身循环数，`-1` 无限 |
| Curve Type | Ease 或 AnimationCurve |
| Ease / AnimationCurve | 对应缓动配置 |

具体 Actor 再增加 start/end、space、strength、frequency、points 等字段。

## Start Value Type

多数扩展 Actor 使用 `StartValueType`：

- `Direct`：使用 Inspector 中显式填写的 start 和 end；
- `Relative`：调用只传 end 的扩展重载，在 Actor 工厂执行时读取目标当前属性作为 start。

这里的 Relative 表示“相对当前状态取得起点”，不是“把 end 当作增量”。例如 Position 的 end 仍是绝对目标坐标。

## Transform 空间

Transform Actor 的 `TransformActSpace`：

- World：position / eulerAngles；
- Local：localPosition / localEulerAngles；
- Scale 始终使用 localScale。

路径 Actor 还提供：

- `ArrayTweenType.Direct`：分段线性路径；
- `ArrayTweenType.Bezier`：Bézier 路径。

## 列表顺序与时间条

Sequence 模式中：

- Actor 按列表从上到下执行；
- 上移/下移按钮会改变执行顺序；
- 时间条起点是之前 Actor 估算长度之和。

Parallel 模式中：

- 全部 Active Actor 同时启动；
- 时间条都从 0 开始；
- 总长度取最长 Actor 的估算长度。

估算长度：

```text
数值 Actor：loops * duration + delay
组 Actor：loops * duration
普通 Actor：duration
```

无限循环显示为极大长度。估算没有考虑自定义工厂的同步终止或运行时参数变化。

## 事件

Events 折叠区包含 UnityEvent：

- onBegin
- onCancel
- onRewind
- onComplete
- onTick(context, time, delta)

这些事件注册到顶层组，不是单个 Actor。Actor 在编辑器内还会注册内部 Tick/Complete 回调，用于更新 Inspector 进度显示。

## 公共控制方法

```csharp
TweenComponent component;

component.Play();
component.Pause();
component.UnPause();
component.Stop();
component.Cancel();
component.Rewind();
component.ReStart();
```

### Play

第一次调用：

1. 根据 Mode 创建 Sequence 或 Parallel；
2. 遍历 Active Actor，给 Actor 注入当前 Transform；
3. 把 `actor.Create` 注册为组工厂；
4. 绑定组件事件、timeScale、id；
5. 设置组 autoCycle=false；
6. Run 并设置组 loops。

运行模式下再次 Play，如果已有 context，会调用 ReStart。

编辑模式下每次 Play 会先取消并回收旧 context，再按当前 Inspector 配置重建，便于修改参数后重新预览。

### Rewind

停止当前组并按逆序回退已创建子 Tween。编辑模式还会重置 Actor 的预览进度。组件组保持 autoCycle=false，因此完成后仍可 Rewind。

### OnDisable

组件禁用时：

1. 把组 autoCycle 设回 true；
2. Cancel；
3. 显式 Recycle；
4. 清空 context 引用。

显式 Recycle 能处理“组已经完成，Cancel 因 isDone 不再执行”的情况。

## 查找 Actor

```csharp
var positionActor = component.FindActor<TweenEx_Transform.DoPositionActor>();
```

返回列表中第一个匹配类型，找不到返回 null。Actor 字段是序列化配置；运行中修改已创建 Tween 不一定立即生效，通常在 ReStart 重建工厂后生效。

## 自定义 Actor

```csharp
[System.Serializable]
public sealed class DoIntensityActor
    : TweenComponentActor<float, Light>
{
    public float start;
    public float end = 1f;

    protected override ITweenContext<float, Light> OnCreate()
    {
        return Tween.DoGoto(
            target,
            start,
            end,
            duration,
            static value => value.intensity,
            static (value, intensity) => value.intensity = intensity,
            snap);
    }
}
```

编译后 Inspector 会通过类型扫描自动把它加入 Actor 菜单。目标类型 Light 会作为菜单分组名。

## 组件使用注意事项

- Target 为空且同一 GameObject 没有对应组件时会记录错误，随后 OnCreate 仍可能发生空引用；配置阶段应确保目标存在。
- `loops=-1` 的组件只会在 Stop、Cancel、OnDisable 或显式 Recycle 时结束。
- 多个 Parallel Actor 写同一属性会互相覆盖。
- Edit Mode 预览会直接修改场景对象；提交场景前确认已 Rewind 到期望状态。
- Inspector 在 Play Mode 被禁用；运行时控制应通过脚本或 UnityEvent 调用公共方法。
- SerializeReference 类型被重命名或删除后，已有场景可能丢失对应 Actor 数据。
