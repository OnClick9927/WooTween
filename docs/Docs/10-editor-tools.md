# 编辑器工具

WooTween.Editor 程序集只在 Unity Editor 中加载，提供 TweenComponent 自定义 Inspector、Scene 路径编辑和 TweenWatcher。

## Edit Mode 调度器

编辑器加载时，WooTween：

1. 创建独立 `TweenScheduler`；
2. 把 scheduler.Update 注册到 `EditorApplication.update`；
3. 计算编辑器帧间隔；
4. 将单次 delta 限制为最多 `0.02f`；
5. 在 ExitingEditMode 时 Kill 编辑器 Tween。

这使 TweenComponent 的 Play 按钮可以在未进入 Play Mode 时工作。

### 编辑器预览限制

- 不使用 `Time.deltaTime`；
- 编辑器卡顿后不会一次跳过很长时间，因为 delta 有上限；
- 预览会真实写入目标属性；
- WooTween 会对 UnityEngine.Object 目标调用 `EditorUtility.SetDirty`；
- 不会自动调用 `Undo.RecordObject`；
- 进入 Play Mode 前编辑器 Tween 会被终止。

需要 Undo 支持的自定义工具应在调用 Sample/Play 前由上层记录对象。

## TweenComponent Inspector

Inspector 包含：

- 默认序列化字段；
- Actor 高级下拉菜单；
- Actor Active 开关；
- 折叠配置区；
- 估算时间条和实时进度底色；
- 删除、上移、下移；
- Events 折叠区；
- Play、Pause/UnPause、Stop、Rewind 工具栏。

Actor 列表通过反射扫描全部程序集中的派生类型。Actor 多时，首次打开 Inspector 的扫描和编辑器实例创建会有一次性成本。

## Scene 路径编辑

`DoPositionArrayActor` 有专用 ActorEditor。在 Inspector 展开该 Actor 时，Scene 视图会：

- 为每个 point 绘制标签；
- 绘制球形 Handle；
- 提供 Position Handle 修改点；
- Direct 模式绘制折线；
- Bezier 模式用 200 个采样点绘制渐变曲线。

路径点当前按世界坐标显示和编辑。使用 Local Position 路径时，应注意 Actor points 与 Scene Handle 可视坐标之间的空间约定，必要时在父物体无变换的环境下编辑，或自行扩展编辑器做 local/world 转换。

## TweenWatcher

打开：

```text
Tools > WooTween > TweenWatcher
```

Watcher 监听内部上下文分配和回收事件，展示：

- ID；
- 分配时的 `Time.time`；
- 具体 TweenContext 类型；
- 当前上下文公共字段/属性；
- 分配调用堆栈，支持文件超链接。

用途：

- 查找未自动回收的上下文；
- 确认 Tween 实际泛型类型；
- 定位创建来源；
- 观察 id 和状态；
- 检查 `SetAutoCycle(false)` 是否有对应 Recycle。

### 如何识别泄漏

1. 打开 TweenWatcher；
2. 重复进入并退出目标 UI/流程；
3. 等待所有动画完成；
4. 观察列表是否持续增长；
5. 选择残留项查看 id、state 和堆栈；
6. 检查创建点是否设置 autoCycle=false、无限循环或未在 OnDisable Kill。

Watcher 在退出 Edit Mode 或 Play Mode 时清空记录。它记录的是 WooTween 分配事件，不是 Unity Profiler 的原生内存快照。

## 自定义 ActorEditor

默认 `TweenActorEditor<T>` 会反射绘制 Actor 字段。需要 Scene Handle 或自定义 Inspector 时可派生：

```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using WooTween;

public sealed class DoIntensityActorEditor
    : TweenActorEditor<DoIntensityActor>
{
    protected override void OnInspectorGUI(DoIntensityActor actor)
    {
        base.OnInspectorGUI(actor);
        EditorGUILayout.HelpBox("Light intensity tween", MessageType.Info);
    }

    protected override void OnSceneGUI(DoIntensityActor actor)
    {
        // 绘制自定义 Handle
    }
}
#endif
```

该类必须位于 Editor 程序集，并引用 `WooTween.Editor`。Inspector 会扫描 `TweenActorEditor<>` 的非抽象派生类型，并按泛型参数匹配 Actor。

## Actor 长度估算

默认编辑器根据 Actor 基类类型估算长度。自定义 Actor 继承数值 Actor 基类时自动使用：

```text
loops * duration + delay
```

自定义组 Actor 使用：

```text
loops * duration
```

如果实际 OnCreate 返回的 Tween 时长与 Actor.duration 不一致，Inspector 时间条也会不一致。自定义 Actor 应把 duration 传入运行时 Tween。

## 打包边界

- `WooTween.Editor.asmdef` 仅包含 Editor 平台；
- Runtime 代码中的 UnityEditor 调用全部受 `#if UNITY_EDITOR` 保护；
- 自定义运行时代码不要直接引用 TweenWatcher、TweenActorEditor 等编辑器类型；
- 自定义 Editor asmdef 应只在 Editor 平台启用。
