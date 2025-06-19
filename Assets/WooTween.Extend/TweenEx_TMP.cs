/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using TMPro;
using UnityEngine;

namespace WooTween
{
    public static partial class TweenEx_TMP
    {
        public static ITweenContext<float, TMP_Text> DoFontSize(this TMP_Text target, float start, float end, float duration)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.fontSize, static (target, value) => target.fontSize = (int)value, true);
        public static ITweenContext<float, TMP_Text> DoText(this TMP_Text target, string start, string end, float duration)
=> Tween.DoGoto(target, (float)start.Length, (float)end.Length, duration, static (target) => target.text.Length, (target, value) => target.text = end.Substring(0, Mathf.Min((int)value, end.Length)), true);

        public static ITweenContext<float, TMP_Text> DoFontSize(this TMP_Text target, float end, float duration)
=> target.DoFontSize(target.fontSize, end, duration);
        public static ITweenContext<float, TMP_Text> DoText(this TMP_Text target, string end, float duration)
=> target.DoText(target.text, end, duration);

        public class DoFontSizeActor : TweenComponentActor<float, TMP_Text>
        {
            public StartValueType startType;
            public float start = 15;
            public float end = 16;
            protected override ITweenContext<float, TMP_Text> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoFontSize(end, duration);
                return target.DoFontSize(start, end, duration);
            }
        }
        public class DoTextActor : TweenComponentActor<float, TMP_Text>
        {
            public StartValueType startType;
            public string start = string.Empty;
            public string end = "Test";
            protected override ITweenContext<float, TMP_Text> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoText(end, duration);
                return target.DoText(start, end, duration);
            }
        }


        public static ITweenContext<float, TMP_Text> DoCharacterSpacing(this TMP_Text target, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.characterSpacing, static (target, value) => target.characterSpacing = (int)value, snap);

        public static ITweenContext<float, TMP_Text> DoWordSpacing(this TMP_Text target, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.wordSpacing, static (target, value) => target.wordSpacing = (int)value, snap);
        public static ITweenContext<float, TMP_Text> DoParagraphSpacing(this TMP_Text target, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.paragraphSpacing, static (target, value) => target.paragraphSpacing = (int)value, snap);
        public static ITweenContext<float, TMP_Text> DoLineSpacing(this TMP_Text target, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.lineSpacing, static (target, value) => target.lineSpacing = (int)value, snap);

        public static ITweenContext<float, TMP_Text> DoCharacterSpacing(this TMP_Text target, float end, float duration, bool snap = false)
=> target.DoCharacterSpacing(target.characterSpacing,  end, duration, snap);

        public static ITweenContext<float, TMP_Text> DoWordSpacing(this TMP_Text target,  float end, float duration, bool snap = false)
=> target.DoCharacterSpacing(target.wordSpacing, end, duration, snap);
        public static ITweenContext<float, TMP_Text> DoParagraphSpacing(this TMP_Text target,  float end, float duration, bool snap = false)
=> target.DoCharacterSpacing(target.paragraphSpacing, end, duration, snap);
        public static ITweenContext<float, TMP_Text> DoLineSpacing(this TMP_Text target,  float end, float duration, bool snap = false)
=> target.DoCharacterSpacing(target.lineSpacing, end, duration, snap);

        public class DoCharacterSpacingActor : TweenComponentActor<float, TMP_Text>
        {
            public StartValueType startType;
            public float start = 1;
            public float end = 0;
            protected override ITweenContext<float, TMP_Text> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoCharacterSpacing(end, duration);
                return target.DoCharacterSpacing(start, end, duration);
            }
        }
        public class DoWordSpacingActor : TweenComponentActor<float, TMP_Text>
        {
            public StartValueType startType;
            public float start = 1;
            public float end = 0;
            protected override ITweenContext<float, TMP_Text> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoWordSpacing(end, duration);
                return target.DoWordSpacing(start, end, duration);
            }
        }
        public class DoParagraphSpacingActor : TweenComponentActor<float, TMP_Text>
        {
            public StartValueType startType;
            public float start = 1;
            public float end = 0;
            protected override ITweenContext<float, TMP_Text> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoParagraphSpacing(end, duration);
                return target.DoParagraphSpacing(start, end, duration);
            }
        }
        public class DoLineSpacingActor : TweenComponentActor<float, TMP_Text>
        {
            public StartValueType startType;
            public float start = 1;
            public float end = 0;
            protected override ITweenContext<float, TMP_Text> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoLineSpacing(end, duration);
                return target.DoLineSpacing(start, end, duration);
            }
        }
    }
}