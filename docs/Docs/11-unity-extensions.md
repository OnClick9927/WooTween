# Unity 扩展方法

扩展源码位于完整仓库 `Assets/WooTween.Extend`，UPM 用户可从 `Package Resources/extend.unitypackage` 按需导入。

大多数属性提供两个重载：

```csharp
DoX(start, end, duration, ...)
DoX(end, duration, ...) // 调用时读取当前属性作为 start
```

## Transform

### 普通插值

| 方法 | 属性 |
| --- | --- |
| `DoPosition` | `Transform.position` |
| `DoLocalPosition` | `Transform.localPosition` |
| `DoRotate` | `Transform.eulerAngles` |
| `DoLocalRotate` | `Transform.localEulerAngles` |
| `DoLocalScale` | `Transform.localScale` |

```csharp
transform.DoPosition(Vector3.right * 3f, 0.5f);
transform.DoLocalRotate(new Vector3(0f, 180f, 0f), 0.5f);
transform.DoLocalScale(Vector3.one * 1.2f, 0.2f);
```

旋转扩展按 Euler Vector3 插值，不是 Quaternion 最短弧球面插值。跨越 0/360 度或需要特定旋转方向时，应显式设计起终 Euler 值，或用自定义属性扩展。

### Shake

- `DoShakePosition`
- `DoShakeLocalPosition`
- `DoShakeRotate`
- `DoShakeLocalRotate`
- `DoShakeLocalScale`

### Punch

- `DoPunchPosition`
- `DoPunchLocalPosition`
- `DoPunchRotate`
- `DoPunchLocalRotate`
- `DoPunchLocalScale`

### Jump

- `DoJumpPosition`
- `DoJumpLocalPosition`
- `DoJumpRotate`
- `DoJumpLocalRotate`
- `DoJumpLocalScale`

### Array 路径

- `DoPositionArray`
- `DoLocalPositionArray`
- `DoRotateArray`
- `DoLocalRotateArray`
- `DoLocalScaleArray`

### Bézier 路径

- `DoPositionArrayBezier`
- `DoLocalPositionArrayBezier`
- `DoRotateArrayBezier`
- `DoLocalRotateArrayBezier`
- `DoLocalScaleArrayBezier`

路径数组至少三个点。

## RectTransform

| 方法 | 属性 |
| --- | --- |
| `DoSizeDelta` | `sizeDelta` |
| `DoPivot` | `pivot` |
| `DoAnchorMin` | `anchorMin` |
| `DoAnchorMax` | `anchorMax` |
| `DoAnchoredPosition` | `anchoredPosition` |

```csharp
rectTransform.DoAnchoredPosition(Vector2.zero, 0.25f).SetEase(Ease.OutQuad);
rectTransform.DoSizeDelta(new Vector2(600f, 300f), 0.3f);
```

布局系统、ContentSizeFitter 或 LayoutGroup 可能在同一帧覆盖这些属性。动画布局元素前应禁用冲突的布局驱动，或动画化不受布局控制的父容器。

## UGUI

| 目标 | 方法 | 属性/效果 |
| --- | --- | --- |
| `Graphic` | `DoColor` | color |
| `Graphic` | `DoGradientColor` | 按 Gradient colorKeys 创建 Sequence |
| `Image` | `DoFillAmount` | fillAmount |
| `Slider` | `DoValue` | value |
| `Text` | `DoFontSize` | fontSize，整数 |
| `Text` | `DoText` | 按字符数显示 end 的前缀 |
| `CanvasGroup` | `DoAlpha` | alpha |

```csharp
canvasGroup.DoAlpha(1f, 0.2f);
image.DoFillAmount(1f, 0.5f);
text.DoText("正在加载...", 0.8f);
```

`DoText(start, end, duration)` 使用 start.Length 和 end.Length 作为数值范围，但写回内容始终是 `end.Substring(0, count)`。它适合逐字显示最终文本，不执行 start 到 end 的字符串内容变形，并会在字符变化帧创建新字符串。

`DoGradientColor` 使用 Gradient 的 colorKeys 构造多个 DoColor 子 Tween。应至少提供一个 color key。

## TextMeshPro

| 方法 | 属性/效果 |
| --- | --- |
| `DoFontSize` | fontSize，写回整数值 |
| `DoText` | 按字符数显示 end 前缀 |
| `DoCharacterSpacing` | characterSpacing |
| `DoWordSpacing` | wordSpacing |
| `DoParagraphSpacing` | paragraphSpacing |
| `DoLineSpacing` | lineSpacing |

```csharp
tmpText.DoText("WooTween", 0.5f);
tmpText.DoCharacterSpacing(0f, 12f, 0.3f);
```

当前扩展中，只传 end 的 `DoWordSpacing`、`DoParagraphSpacing` 和 `DoLineSpacing` 重载会转调 `DoCharacterSpacing`。需要这些属性时请使用显式 start/end 重载，直到对应实现修正。

## Camera

| 方法 | Camera 属性 |
| --- | --- |
| `DoAspect` | aspect |
| `DoNearClipPlane` | nearClipPlane |
| `DoFarClipPlane` | farClipPlane |
| `DoFieldOfView` | fieldOfView |
| `DoOrthographicSize` | orthographicSize |
| `DoBackgroundColor` | backgroundColor |
| `DoRect` | rect |
| `DoPixelRect` | pixelRect |

根据 Camera 模式选择 FieldOfView 或 OrthographicSize。不要同时让 Cinemachine 或其他相机控制器写同一属性。

## AudioSource

| 方法 | 属性 |
| --- | --- |
| `DoVolume` | volume |
| `DoPitch` | pitch |

```csharp
audioSource.DoVolume(0f, 1f, 0.5f);
audioSource.DoPitch(1f, 1.2f, 0.25f);
```

当前只传 end 的 `DoPitch(end, duration)` 重载使用 `target.volume` 作为 start。为避免错误起点，请使用显式 `DoPitch(target.pitch, end, duration)` 重载。

## Material 与 SpriteRenderer

Material：

- `DoFloat(propertyName, ...)`
- `DoInt(propertyName, ...)`
- `DoColor(propertyName, ...)`
- `DoVector(propertyName, ...)`

SpriteRenderer：

- `DoColor(...)`
- `DoGradientColor(gradient, duration)`

```csharp
material.DoFloat("_Cutoff", 0f, 1f, 0.5f);
material.DoColor("_BaseColor", Color.white, Color.red, 0.5f);
spriteRenderer.DoColor(Color.clear, 0.3f);
```

Material 方法捕获字符串属性名，会为 getter/setter 创建闭包。大量创建时可在项目中用固定 Shader Property ID 编写专用扩展。

当前只传 end 的 `DoInt` 重载通过 `GetFloat` 读取起点；使用整数 Shader 属性时建议显式传入 start。

直接修改 Renderer.material 会实例化材质。扩展接收的是 Material，调用者应明确使用共享材质、实例材质还是 MaterialPropertyBlock。WooTween 不自动管理材质实例生命周期。

## Rigidbody

3D：

- `DoPosition`
- `DoRotation`
- `DoShakePosition` / `DoPunchPosition`
- `DoShakeRotation` / `DoPunchRotation`

2D：

- `DoPosition`
- `DoRotation`
- `DoShakePosition` / `DoPunchPosition`
- `DoShakeRotation` / `DoPunchRotation`

这些扩展直接写 `Rigidbody.position`、`Rigidbody.rotation`、`Rigidbody2D.position` 或 `Rigidbody2D.rotation`，调度时机是普通 Update，不是 FixedUpdate。动态刚体可能与物理解算竞争；适合运动学刚体或明确接受 Update 驱动的场景。需要物理一致性时，应实现 FixedUpdate 专用调度或使用 MovePosition/MoveRotation。

## Actor 类型

扩展文件同时定义对应 TweenComponentActor。导入后会自动出现在 TweenComponent 的 Actors 菜单。

Actor 常见字段：

- `StartValueType startType`
- `start` / `end`
- `strength`
- `frequency` / `dampingRatio`
- `jumpCount` / `jumpDamping`
- `points`
- `TransformActSpace`
- `ArrayTweenType`

## 按需导入建议

- 不使用旧版 UGUI Text 时，可以不导入 `TweenEx_UGUI.cs`；
- 不安装 TextMeshPro 时，不导入 `TweenEx_TMP.cs`；
- 不需要物理扩展时，不导入 Rigidbody 文件；
- 核心泛型 API 始终可用；
- 自定义 asmdef 项目应为扩展目录补充正确程序集引用。
