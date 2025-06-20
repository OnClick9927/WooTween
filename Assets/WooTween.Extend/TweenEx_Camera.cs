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
    public static partial class TweenEx_Camera
    {
        public static ITweenContext<float, Camera> DoAspect(this Camera target, float start, float end, float duration, bool snap = false)
  => Tween.DoGoto(target, start, end, duration, static (target) => target.aspect, static (target, value) => target.aspect = value, snap);

        public static ITweenContext<float, Camera> DoNearClipPlane(this Camera target, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.nearClipPlane, static (target, value) => target.nearClipPlane = value, snap);
        public static ITweenContext<float, Camera> DoFarClipPlane(this Camera target, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.farClipPlane, static (target, value) => target.farClipPlane = value, snap);


        public static ITweenContext<float, Camera> DoFieldOfView(this Camera target, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.fieldOfView, static (target, value) => target.fieldOfView = value, snap);
        public static ITweenContext<float, Camera> DoOrthographicSize(this Camera target, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.orthographicSize, static (target, value) => target.orthographicSize = value, snap);

        public static ITweenContext<Color, Camera> DoBackgroundColor(this Camera target, Color start, Color end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.backgroundColor, static (target, value) => target.backgroundColor = value, snap);

        public static ITweenContext<Rect, Camera> DoRect(this Camera target, Rect start, Rect end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.rect, static (target, value) => target.rect = value, snap);
        public static ITweenContext<Rect, Camera> DoPixelRect(this Camera target, Rect start, Rect end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.pixelRect, static (target, value) => target.pixelRect = value, snap);

        public static ITweenContext<float, Camera> DoAspect(this Camera target, float end, float duration, bool snap = false)
=> target.DoAspect(target.aspect, end, duration, snap);

        public static ITweenContext<float, Camera> DoNearClipPlane(this Camera target, float end, float duration, bool snap = false)
=> target.DoNearClipPlane(target.nearClipPlane, end, duration, snap);
        public static ITweenContext<float, Camera> DoFarClipPlane(this Camera target, float end, float duration, bool snap = false)
=> target.DoFarClipPlane(target.farClipPlane, end, duration, snap);


        public static ITweenContext<float, Camera> DoFieldOfView(this Camera target, float end, float duration, bool snap = false)
=> target.DoFieldOfView(target.fieldOfView, end, duration, snap);
        public static ITweenContext<float, Camera> DoOrthographicSize(this Camera target, float end, float duration, bool snap = false)
=> target.DoOrthographicSize(target.orthographicSize, end, duration, snap);

        public static ITweenContext<Color, Camera> DoBackgroundColor(this Camera target, Color end, float duration, bool snap = false)
=> target.DoBackgroundColor(target.backgroundColor, end, duration, snap);
        public static ITweenContext<Rect, Camera> DoRect(this Camera target, Rect end, float duration, bool snap = false)
=> target.DoRect(target.rect, end, duration, snap);
        public static ITweenContext<Rect, Camera> DoPixelRect(this Camera target, Rect end, float duration, bool snap = false)
=> target.DoPixelRect(target.pixelRect, end, duration, snap);


        public class DoAspectActor : TweenComponentActor<float, Camera>
        {
            public StartValueType startType;
            public float start = 0;
            public float end = 1;
            protected override ITweenContext<float, Camera> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoAspect(end, duration, snap);
                return target.DoAspect(start, end, duration, snap);
            }
        }
        public class DoNearClipPlaneActor : TweenComponentActor<float, Camera>
        {
            public StartValueType startType;
            public float start = 0;
            public float end = 1;
            protected override ITweenContext<float, Camera> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoNearClipPlane(end, duration, snap);
                return target.DoNearClipPlane(start, end, duration, snap);
            }
        }
        public class DoFarClipPlaneActor : TweenComponentActor<float, Camera>
        {
            public StartValueType startType;
            public float start = 0;
            public float end = 1;
            protected override ITweenContext<float, Camera> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoFarClipPlane(end, duration, snap);
                return target.DoFarClipPlane(start, end, duration, snap);
            }
        }
        public class DoFieldOfViewActor : TweenComponentActor<float, Camera>
        {
            public StartValueType startType;
            public float start = 0;
            public float end = 1;
            protected override ITweenContext<float, Camera> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoFieldOfView(end, duration, snap);
                return target.DoFieldOfView(start, end, duration, snap);
            }
        }
        public class DoOrthographicSizeActor : TweenComponentActor<float, Camera>
        {
            public StartValueType startType;
            public float start = 0;
            public float end = 1;
            protected override ITweenContext<float, Camera> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoOrthographicSize(end, duration, snap);
                return target.DoOrthographicSize(start, end, duration, snap);
            }
        }
        public class DoBackgroundColorActor : TweenComponentActor<Color, Camera>
        {
            public StartValueType startType;
            public Color start = Color.white;
            public Color end = Color.white;
            protected override ITweenContext<Color, Camera> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoBackgroundColor(end, duration, snap);
                return target.DoBackgroundColor(start, end, duration, snap);
            }
        }

        public class DoRectActor : TweenComponentActor<Rect, Camera>
        {
            public StartValueType startType;
            public Rect start = Rect.zero;
            public Rect end = Rect.zero;
            protected override ITweenContext<Rect, Camera> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoRect(end, duration, snap);
                return target.DoRect(start, end, duration, snap);
            }
        }

        public class DoPixelRectActor : TweenComponentActor<Rect, Camera>
        {
            public StartValueType startType;
            public Rect start = Rect.zero;
            public Rect end = Rect.zero;
            protected override ITweenContext<Rect, Camera> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoPixelRect(end, duration, snap);
                return target.DoPixelRect(start, end, duration, snap);
            }
        }
    }

}