/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using UnityEngine;

namespace WooTween
{
    public static partial class TweenEx_RectTransform
    {
        public static ITweenContext<Vector2, RectTransform> DoSizeDelta(this RectTransform target, Vector2 start, Vector2 end, float duration, bool snap = false)
          => Tween.DoGoto(target, start, end, duration, static (target) => target.sizeDelta, static (target, value) => target.sizeDelta = value, snap);

        public static ITweenContext<Vector2, RectTransform> DoPivot(this RectTransform target, Vector2 start, Vector2 end, float duration, bool snap = false)
           => Tween.DoGoto(target, start, end, duration, static (target) => target.pivot, static (target, value) => target.pivot = value, snap);


        public static ITweenContext<Vector2, RectTransform> DoAnchorMin(this RectTransform target, Vector2 start, Vector2 end, float duration, bool snap = false)
        => Tween.DoGoto(target, start, end, duration, static (target) => target.anchorMin, static (target, value) => target.anchorMin = value, snap);

        public static ITweenContext<Vector2, RectTransform> DoAnchorMax(this RectTransform target, Vector2 start, Vector2 end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.anchorMax, static (target, value) => target.anchorMax = value, snap);

        public static ITweenContext<Vector2, RectTransform> DoAnchoredPosition(this RectTransform target, Vector2 start, Vector2 end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.anchoredPosition, static (target, value) => target.anchoredPosition = value, snap);



        public static ITweenContext<Vector2, RectTransform> DoSizeDelta(this RectTransform target, Vector2 end, float duration, bool snap = false)
          => target.DoSizeDelta(target.sizeDelta, end, duration, snap);

        public static ITweenContext<Vector2, RectTransform> DoPivot(this RectTransform target, Vector2 end, float duration, bool snap = false)
          => target.DoPivot(target.pivot, end, duration, snap);


        public static ITweenContext<Vector2, RectTransform> DoAnchorMin(this RectTransform target, Vector2 end, float duration, bool snap = false)
          => target.DoAnchorMin(target.anchorMin, end, duration, snap);

        public static ITweenContext<Vector2, RectTransform> DoAnchorMax(this RectTransform target, Vector2 end, float duration, bool snap = false)
          => target.DoAnchorMax(target.anchorMax, end, duration, snap);

        public static ITweenContext<Vector2, RectTransform> DoAnchoredPosition(this RectTransform target, Vector2 end, float duration, bool snap = false)
          => target.DoAnchoredPosition(target.anchoredPosition, end, duration, snap);


        public class DoSizeDeltaActor : TweenComponentActor<Vector2, RectTransform>
        {
            public StartValueType startType;
            public Vector2 start = Vector3.zero;
            public Vector2 end = Vector3.one;
            protected override ITweenContext<Vector2, RectTransform> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoSizeDelta(end, duration, snap);
                return target.DoSizeDelta(start, end, duration, snap);
            }
        }
        public class DoPivotActor : TweenComponentActor<Vector2, RectTransform>
        {
            public StartValueType startType;
            public Vector2 start = Vector3.zero;
            public Vector2 end = Vector3.one;
            protected override ITweenContext<Vector2, RectTransform> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoPivot(end, duration, snap);
                return target.DoPivot(start, end, duration, snap);
            }
        }
        public class DoAnchorMinActor : TweenComponentActor<Vector2, RectTransform>
        {
            public StartValueType startType;
            public Vector2 start = Vector3.zero;
            public Vector2 end = Vector3.one;
            protected override ITweenContext<Vector2, RectTransform> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoAnchorMin(end, duration, snap);
                return target.DoAnchorMin(start, end, duration, snap);
            }
        }
        public class DoAnchorMaxActor : TweenComponentActor<Vector2, RectTransform>
        {
            public StartValueType startType;
            public Vector2 start = Vector3.zero;
            public Vector2 end = Vector3.one;
            protected override ITweenContext<Vector2, RectTransform> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoAnchorMax(end, duration, snap);
                return target.DoAnchorMax(start, end, duration, snap);
            }
        }
        public class DoAnchoredPositionActor : TweenComponentActor<Vector2, RectTransform>
        {
            public StartValueType startType;
            public Vector2 start = Vector3.zero;
            public Vector2 end = Vector3.one;
            protected override ITweenContext<Vector2, RectTransform> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoAnchoredPosition(end, duration, snap);
                return target.DoAnchoredPosition(start, end, duration, snap);
            }
        }
    }

}