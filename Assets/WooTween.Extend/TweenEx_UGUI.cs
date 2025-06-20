/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace WooTween
{
    public static partial class TweenEx_UGUI
    {


        public static ITweenGroup DoGradientColor(this Graphic target, Gradient gradient, float duration, bool snap = false)
        {
            var colors = gradient.colorKeys;
            int len = colors.Length;
            var s = Tween.Sequence();
            for (int i = 0; i < len; ++i)
            {
                GradientColorKey c = colors[i];
                if (i == 0)
                    target.color = c.color;

                float colorDuration = duration * (i == 0 ? c.time : c.time - colors[i - 1].time);
                s.NewContext(() => target.DoColor(c.color, colorDuration, snap));
            }
            return s.Run();
        }



        public static ITweenContext<Color, Graphic> DoColor(this Graphic target, Color start, Color end, float duration, bool snap = false)
   => Tween.DoGoto(target, start, end, duration, static (target) => target.color, static (target, value) => target.color = value, snap);

        public static ITweenContext<float, Image> DoFillAmount(this Image target, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.fillAmount, static (target, value) => target.fillAmount = value, snap);

        public static ITweenContext<float, Slider> DoValue(this Slider target, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.value, static (target, value) => target.value = value, snap);

        public static ITweenContext<float, Text> DoFontSize(this Text target, float start, float end, float duration)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.fontSize, static (target, value) => target.fontSize = (int)value, true);
        public static ITweenContext<float, Text> DoText(this Text target, string start, string end, float duration)
=> Tween.DoGoto(target, (float)start.Length, (float)end.Length, duration, static (target) => target.text.Length, (target, value) => target.text = end.Substring(0, Mathf.Min((int)value, end.Length)), true);

        public static ITweenContext<float, CanvasGroup> DoAlpha(this CanvasGroup target, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.alpha, static (target, value) => target.alpha = value, snap);


        public static ITweenContext<Color, Graphic> DoColor(this Graphic target, Color end, float duration, bool snap = false)
=> target.DoColor(target.color, end, duration, snap);

        public static ITweenContext<float, Image> DoFillAmount(this Image target, float end, float duration, bool snap = false)
=> target.DoFillAmount(target.fillAmount, end, duration, snap);

        public static ITweenContext<float, Slider> DoValue(this Slider target, float end, float duration, bool snap = false)
=> target.DoValue(target.value, end, duration, snap);

        public static ITweenContext<float, Text> DoFontSize(this Text target, float end, float duration)
=> target.DoFontSize(target.fontSize, end, duration);
        public static ITweenContext<float, Text> DoText(this Text target, string end, float duration)
=> target.DoText(target.text, end, duration);

        public static ITweenContext<float, CanvasGroup> DoAlpha(this CanvasGroup target, float end, float duration, bool snap = false)
=> target.DoAlpha(target.alpha, end, duration, snap);

        public class DoGradientColorActor : TweenGroupComponentActor<Graphic>
        {
            public Gradient gradient;
            protected override ITweenGroup OnCreate()
            {
                return target.DoGradientColor(gradient, duration, snap);
            }
        }
        public class DoColorActor : TweenComponentActor<Color, Graphic>
        {
            public StartValueType startType;
            public Color start = Color.white;
            public Color end = Color.white;
            protected override ITweenContext<Color, Graphic> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoColor(end, duration, snap);
                return target.DoColor(start, end, duration, snap);
            }
        }
        public class DoFillAmountActor : TweenComponentActor<float, Image>
        {
            public StartValueType startType;
            public float start = 0;
            public float end = 1;
            protected override ITweenContext<float, Image> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoFillAmount(end, duration, snap);
                return target.DoFillAmount(start, end, duration, snap);
            }
        }

        public class DoValueActor : TweenComponentActor<float, Slider>
        {
            public StartValueType startType;
            public float start = 0;
            public float end = 1;
            protected override ITweenContext<float, Slider> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoValue(end, duration, snap);
                return target.DoValue(start, end, duration, snap);
            }
        }
        public class DoAlphaActor : TweenComponentActor<float, CanvasGroup>
        {
            public StartValueType startType;
            public float start = 1;
            public float end = 0;
            protected override ITweenContext<float, CanvasGroup> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoAlpha(end, duration, snap);
                return target.DoAlpha(start, end, duration, snap);
            }
        }
        public class DoFontSizeActor : TweenComponentActor<float, Text>
        {
            public StartValueType startType;
            public float start = 15;
            public float end = 16;
            protected override ITweenContext<float, Text> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoFontSize(end, duration);
                return target.DoFontSize(start, end, duration);
            }
        }
        public class DoTextActor : TweenComponentActor<float, Text>
        {
            public StartValueType startType;
            public string start = string.Empty;
            public string end = "Test";
            protected override ITweenContext<float, Text> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoText(end, duration);
                return target.DoText(start, end, duration);
            }
        }
    }
}