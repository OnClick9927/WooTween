# WooTween

WooTween 是一个面向 Unity 的轻量级 Tween 动画库，提供通用泛型插值、常见 Unity 组件扩展、串行与并行动画组、编辑器预览、采样以及可视化 `TweenComponent`。

- 支持 Unity 2019.4 及以上版本
- 当前包版本：`1.1.8`
- 内置值类型：`float`、`Vector2`、`Vector3`、`Vector4`、`Color`、`Rect`
- 动画模式：Normal、Shake、Punch、Jump、Bezier、Array、Wait
- 组合模式：Sequence、Parallel
- 生命周期：Pause、UnPause、Stop、Cancel、Complete、Rewind、ReStart、Recycle
- 编辑器能力：非运行模式预览、时间轴展示、路径 Scene Handle、TweenWatcher

## 文档

- [在线完整文档](https://onclick9927.github.io/WooTween/)
- [仓库内文档入口](docs/README.md)
- [API 总表](docs/Docs/14-api-reference.md)
- [常见问题与排错](docs/Docs/15-troubleshooting.md)

## 安装

在 Unity Package Manager 中选择 **Add package from git URL**，输入：

```text
https://github.com/OnClick9927/WooTween.git#upm
```

UPM 包包含 WooTween 核心运行时和编辑器工具。Transform、UGUI、TMP、Camera、AudioSource、Material、Rigidbody 等便捷扩展位于包内的 `Package Resources/extend.unitypackage`，按需导入即可。

## 快速开始

```csharp
using UnityEngine;
using WooTween;

public class TweenExample : MonoBehaviour
{
    private ITweenContext moveTween;

    private void Start()
    {
        moveTween = transform
            .DoPosition(new Vector3(5f, 0f, 0f), 0.8f)
            .SetEase(Ease.OutCubic)
            .OnComplete(_ => Debug.Log("Move completed"));
    }

    private void OnDisable()
    {
        moveTween?.Cancel();
    }
}
```

不使用扩展方法时，可以通过通用 API 为任意目标属性创建 Tween：

```csharp
Tween.DoGoto(
    target: transform,
    start: transform.position,
    end: new Vector3(5f, 0f, 0f),
    duration: 0.8f,
    getter: static target => target.position,
    setter: static (target, value) => target.position = value,
    snap: false);
```

## 组合动画

```csharp
var sequence = Tween.Sequence()
    .NewContext(() => transform.DoPosition(Vector3.right * 3f, 0.5f))
    .NewContext(() => transform.DoLocalScale(Vector3.one * 1.2f, 0.25f))
    .NewContext(() => Tween.DoWait(0.2f))
    .SetAutoCycle(false)
    .OnComplete(_ => Debug.Log("Sequence completed"))
    .Run();

// SetAutoCycle(false) 后可以保留上下文并回到初始状态。
sequence.Rewind();
sequence.ReStart();
```

> 完整的创建参数、循环规则、回调时序、对象池约束、Sequence/Parallel 所有权、采样、异步等待、组件配置和扩展 API，请阅读[在线文档](https://onclick9927.github.io/WooTween/)。

## 编辑器预览

为 GameObject 添加 `IFramework/TweenComponent`，选择 Sequence 或 Parallel，添加 Actor 后即可在 Inspector 中执行 Play、Pause、Stop 和 Rewind。路径类 Actor 展开时还可以在 Scene 视图中直接编辑控制点。

<img width="755" height="611" alt="WooTween Inspector" src="https://github.com/user-attachments/assets/c2904f06-f670-4f1b-a199-d32560a3e065" />

## 相关项目

- [ActionEditor](https://github.com/OnClick9927/ActionEditor)：可结合 WooTween 采样 API 实现类似 Unity Animation 窗口的效果。
- [WooAsset](https://github.com/OnClick9927/WooAsset)：同作者的 AssetBundle 管理工具。
