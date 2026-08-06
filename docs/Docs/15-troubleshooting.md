# 常见问题与排错

## 找不到 WooTween 命名空间

检查：

1. UPM 包是否安装成功；
2. Console 是否先有 asmdef 依赖错误；
3. 脚本是否 `using WooTween;`；
4. 业务 asmdef 是否引用 `WooTween`；
5. 是否误把源码放进 Editor-only 程序集。

如果错误提到 asmdef GUID 未解析，先检查 `com.unity.ugui` 和 WooTween asmdef 是否完整，不要根据旧的工程文件名推断缺失包。

## 找不到 DoPosition / DoAlpha 等扩展

核心 UPM 包不直接编译 `Assets/WooTween.Extend` 源码。导入 `Package Resources/extend.unitypackage`，或使用核心 `Tween.DoGoto`。

自定义 asmdef 看不到已导入扩展时，原因通常是扩展位于 Assembly-CSharp，而命名程序集不能反向引用 Assembly-CSharp。为扩展目录创建 asmdef，并添加 WooTween、UGUI、TMP 等引用。

## Tween 没有开始

逐项检查：

- 核心 API 是否 `autoRun=false`；若是，需要 `.Run()`；
- Sequence/Parallel 是否调用 `.Run()`；
- 上下文是否已经 Stop/Cancel/Recycle；
- 是否 paused；
- timeScale 是否为 0；
- duration 是否有效；
- 目标 GameObject/组件是否有效；
- 是否从非主线程创建；
- Console 是否有 unsupported type 或 null 错误。

可以打开 TweenWatcher 确认 state 和创建堆栈。

## Rewind 空引用或没有效果

最常见原因是完成后上下文已经自动回池：

```csharp
var tween = transform.DoPosition(Vector3.right, 0.2f);
// 默认 autoCycle=true
```

需要完成后 Rewind：

```csharp
var tween = transform
    .DoPosition(Vector3.right, 0.2f)
    .SetAutoCycle(false);
```

Sequence/Parallel 会接管子 Tween，保留第一轮和当前轮的必要上下文。应对父组调用 Rewind，不要回收父组持有的子 Tween。

Rewind 的 OnRewind 回调先于具体起点采样；如果在回调内检查目标值，看到的可能还是回退前状态。

## 完成后 GetPercent 返回 0

普通 Tween 在完成循环时会把内部 time 重置为 0，再设置完成状态。完成判断使用：

```csharp
if (context.isDone) { ... }
```

不要只用完成后的 `GetPercent()==1`。TweenComponent Actor 的编辑器进度通过 OnComplete 单独设为 1。

## Stop 没有触发 OnCancel

这是设计行为：Stop 是静默终止。需要取消通知：

```csharp
tween.Cancel();
```

`KillTweens` 内部使用 Stop + Recycle，也不会触发 OnCancel。

## Complete(true) 没有把属性设到终点

Complete(true) 负责生命周期和 OnComplete，不负责强制 Sample(duration)。需要立即到终点时：

```csharp
target.position = end;
tween.Complete(true);
```

或使用 `Tween.Sample(..., progress: 1f)`。

## await 永远不返回

WooTween awaiter 只监听 OnComplete。上下文被 Stop、Cancel、Rewind 或 Recycle 后不会恢复 continuation。

解决方案：

- 保证流程最终 Complete；
- 自己桥接 OnComplete 和 OnCancel 到 Task/UniTask；
- 不对可能在 OnDisable 取消的动画直接 await。

## Sequence 停在某一段

Sequence 依赖子 Tween 的 OnComplete 或 OnCancel 推进。以下情况会停住：

- 子 Tween 被 Stop；
- 子 Tween 无限循环；
- 工厂返回一个没有 Run 的组；
- 工厂复用已经失效的上下文；
- 子 Tween timeScale=0 或 Pause 后未恢复。

父 Sequence Stop 时停在当前段是预期行为。

## Parallel 永不完成

检查每个子 Tween：

- 是否无限循环；
- 是否只 Pause/Stop 而未 Complete/Cancel；
- 是否返回了不受调度的 autoRun=false 上下文；
- 是否在工厂内异常退出。

Parallel 会忽略 null 工厂结果，但不会捕获工厂异常。

## NullReferenceException 出现在 getter/setter

常见原因：

- target 为 null；
- UnityEngine.Object 已销毁但 Tween 仍运行；
- TweenComponent 自动 GetComponent 失败；
- 自定义 getter 访问了目标内部空字段；
- points 为 null。

为 Tween 设置 owner，并在目标 OnDisable/OnDestroy Kill：

```csharp
tween.SetOwner(this);

private void OnDisable()
{
    this.KillTweens();
}
```

## Tween Not support Type

内置类型只有：

```text
float, Vector2, Vector3, Vector4, Color, Rect
```

Quaternion、int、自定义 struct 不能直接作为 T。int 属性通常用 float Tween + snap/强制 int setter；Quaternion 可用 Euler Vector3 或 float 进度 + Slerp。

## Array/Bézier 报错

必须满足：

- points 非 null；
- 至少三个点；
- duration > 0；
- T 已注册 ValueCalculator。

日志 `At Least 3 point` 后当前实现仍会继续配置，必须由调用者修正输入。

## 动画被 Layout、Animator 或物理覆盖

WooTween 每帧写属性，但其他系统可能在同帧稍后再次写入：

- Animator 覆盖 Transform；
- LayoutGroup/ContentSizeFitter 覆盖 RectTransform；
- Cinemachine 覆盖 Camera；
- Rigidbody 物理解算覆盖位置；
- 自定义 Update/LateUpdate 覆盖字段。

解决：

- 禁用冲突控制器；
- 动画父级或独立代理属性；
- 改变调度时机；
- 由一个系统统一合成最终值。

## DoText 产生 GC

UGUI/TMP DoText 每次字符数变化都会 Substring。长文本改用 TMP `maxVisibleCharacters` 的自定义 Tween：

```csharp
Tween.DoGoto(
    text,
    0f,
    text.textInfo.characterCount,
    duration,
    static target => target.maxVisibleCharacters,
    static (target, value) => target.maxVisibleCharacters = (int)value,
    true);
```

## TMP spacing 动画写错属性

当前只传 end 的 `DoWordSpacing`、`DoParagraphSpacing`、`DoLineSpacing` 会转调 DoCharacterSpacing。使用显式 start/end 重载：

```csharp
text.DoWordSpacing(text.wordSpacing, end, duration);
```

## DoPitch 从异常值开始

当前 `DoPitch(end, duration)` 使用 volume 作为 start。改用：

```csharp
audioSource.DoPitch(audioSource.pitch, end, duration);
```

## Material DoInt 起点不正确

当前只传 end 的 DoInt 使用 GetFloat 读取起点。使用显式 start：

```csharp
material.DoInt(name, material.GetInteger(name), end, duration);
```

## Editor 预览速度异常

编辑器 delta 上限是 0.02 秒，卡顿或窗口失焦后不会追赶全部真实时间。预览用于视觉编辑，不保证与运行时墙钟完全一致。

## Editor 预览修改了场景

Edit Mode Tween 会直接写属性并 SetDirty。使用 Rewind 恢复，或在预览前记录原值。WooTween 不自动注册 Undo。

## 内存持续增长

检查：

- `SetAutoCycle(false)` 是否最终 Recycle；
- `loops=-1` 是否有 owner 清理；
- TweenComponent 是否一直启用并持有完成组；
- 回调闭包是否捕获大对象；
- Sequence/Parallel 是否拥有大量子项；
- 是否每帧创建新的 points、evaluator 或 Tween；
- TweenWatcher 中上下文数量是否持续增加。

对象池容量增长后通常不会主动收缩，这属于复用策略。应区分“池保留可复用对象”和“活动上下文/闭包泄漏”。

## 如何提交有效 Issue

请提供：

- Unity 版本；
- WooTween 分支/提交；
- 安装方式；
- 目标平台与 Mono/IL2CPP；
- 最小复现代码；
- 完整异常堆栈；
- autoRun、autoCycle、loops 和调用顺序；
- 是否在回调中修改生命周期；
- 是否能在仓库 Example 工程复现。
