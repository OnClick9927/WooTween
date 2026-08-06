# 安装与目录

## 环境要求

`package.json` 声明的最低 Unity 版本为 `2019.4`。当前仓库使用 Unity `2021.3.33f1` 维护。

核心运行时依赖 UnityEngine，并在 asmdef 中引用 Unity UI 相关程序集。Unity 2019.4 项目通常已经包含内置 UGUI 包；如果导入后出现程序集 GUID 引用缺失，请先确认 Package Manager 中的 `com.unity.ugui` 可用，并检查项目是否修改过 WooTween 的 asmdef。

## 使用 Git URL 安装

1. 打开 Unity 的 **Window > Package Manager**。
2. 点击左上角 `+`。
3. 选择 **Add package from git URL...**。
4. 输入：

```text
https://github.com/OnClick9927/WooTween.git#upm
```

5. 等待 Unity 拉取并编译。

`#upm` 指向专门的 UPM 分支，该分支以包内容作为仓库根目录。不要把 `#main` 当作 UPM 包地址；`main` 是完整 Unity 工程结构。

### 固定到提交

生产项目建议固定到经过验证的提交，避免分支更新自动改变依赖：

```text
https://github.com/OnClick9927/WooTween.git#<commit-sha>
```

固定提交前请确认该提交的目录结构是有效 UPM 包。也可以在项目的 `Packages/manifest.json` 中维护依赖：

```json
{
  "dependencies": {
    "com.woo.tween": "https://github.com/OnClick9927/WooTween.git#upm"
  }
}
```

## 从完整仓库导入

如果需要调试源码、查看 Example 场景或参与开发，可以克隆完整仓库：

```bash
git clone https://github.com/OnClick9927/WooTween.git
```

用仓库记录的 Unity 版本打开工程。核心源码位于 `Assets/WooTween`，扩展源码位于 `Assets/WooTween.Extend`，示例位于 `Assets/Example`。

## 导入可选扩展

UPM 包中的 `Package Resources/extend.unitypackage` 包含常见 Unity 组件快捷 API。导入方式：

1. 在 Project 窗口定位 WooTween 包；
2. 展开 `Package Resources`；
3. 双击 `extend.unitypackage`；
4. 在导入窗口中选择需要的扩展文件；
5. 点击 Import。

导入后通常会出现在项目 `Assets/WooTween.Extend` 下。扩展文件直接引用 UGUI、TMP、物理等类型；如果项目未安装对应 Unity 包，可只导入需要的文件，或删除不适用的扩展文件。

## 目录说明

```text
Assets/WooTween/
├─ Runtime/
│  ├─ Component/       TweenComponent 与 Actor 基类
│  ├─ Context/         Tween 上下文、组合上下文、对象池
│  ├─ Curve/           Ease 与 IValueEvaluator
│  ├─ Scheduler/       Play Mode / Edit Mode 调度
│  ├─ ValueCalculator/ 内置值类型计算器
│  └─ Tween.cs         公共创建、配置与生命周期扩展
├─ Editor/             Inspector、Scene Handle、TweenWatcher
├─ Package Resources/  可选扩展 unitypackage
├─ package.json
└─ README.md

Assets/WooTween.Extend/
├─ TweenEx_Transform.cs
├─ TweenEx_RectTransform.cs
├─ TweenEx_UGUI.cs
├─ TweenEx_TMP.cs
├─ TweenEx_Camera.cs
├─ TweenEx_Audio.cs
├─ TweenEx_Rendering.cs
├─ TweenEx_Rigidbody.cs
└─ TweenEx_Rigidbody2D.cs
```

## asmdef 项目注意事项

WooTween 核心运行时程序集名为 `WooTween`。如果你的代码位于自定义 asmdef 中，需要在该 asmdef 的 Assembly Definition References 中添加 `WooTween`。

导入到 `Assets/WooTween.Extend` 的扩展目录没有独立 asmdef，默认进入 `Assembly-CSharp`。如果业务代码位于其他 asmdef 中而看不到扩展方法，可以：

- 为扩展目录创建自己的 asmdef，并引用 `WooTween`、UGUI、TMP 等所需程序集；或
- 把需要的扩展文件移动到业务程序集可引用的位置；或
- 直接使用不依赖扩展程序集的 `Tween.DoGoto`。

## 验证安装

创建脚本并编译：

```csharp
using UnityEngine;
using WooTween;

public sealed class WooTweenInstallCheck : MonoBehaviour
{
    private void Start()
    {
        Tween.DoGoto(
            transform,
            transform.position,
            Vector3.right,
            1f,
            static value => value.position,
            static (value, position) => value.position = position,
            false);
    }
}
```

进入 Play Mode 后，Hierarchy 中会在首次需要运行时出现一个名为 `Tween` 的常驻 GameObject。目标对象应在一秒内移动到 `(1, 0, 0)`。

## 升级建议

升级前：

1. 提交或备份项目；
2. 记录当前 WooTween 提交和 Unity 版本；
3. 搜索是否持有完成后上下文引用；
4. 检查是否依赖 `autoCycle`、回调时序或 Sequence/Parallel 的具体行为；
5. 在独立分支运行 Play Mode、Edit Mode 预览和打包测试。

UPM 缓存无法刷新时，可删除项目 `Library/PackageCache` 中对应缓存后重新打开 Unity。不要直接修改 PackageCache 作为长期方案，因为 Unity 可能覆盖其中内容。
