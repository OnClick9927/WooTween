# WooTween

WooTween 是一个面向 Unity 的轻量级 Tween 动画库，提供通用泛型插值、常见 Unity 组件扩展、串行与并行动画组、编辑器预览、采样以及可视化 `TweenComponent`。

## 文档

- [在线完整文档](https://onclick9927.github.io/WooTween/)
- [GitHub 仓库](https://github.com/OnClick9927/WooTween)
- [仓库内文档](https://github.com/OnClick9927/WooTween/tree/main/docs)

## 安装

```text
https://github.com/OnClick9927/WooTween.git#upm
```

通过 Unity Package Manager 的 **Add package from git URL** 安装。常见 Unity 组件扩展位于 `Package Resources/extend.unitypackage`，需要时再导入。

## 最小示例

```csharp
using UnityEngine;
using WooTween;

public class TweenExample : MonoBehaviour
{
    private void Start()
    {
        transform
            .DoPosition(new Vector3(5f, 0f, 0f), 0.8f)
            .SetEase(Ease.OutCubic)
            .OnComplete(_ => Debug.Log("Completed"));
    }
}
```

核心库支持 `float`、`Vector2`、`Vector3`、`Vector4`、`Color`、`Rect`，并可通过 `ValueCalculator<T>` 扩展其他值类型。详细的生命周期、组合动画、采样、组件、编辑器、性能和排错说明见[在线文档](https://onclick9927.github.io/WooTween/)。
