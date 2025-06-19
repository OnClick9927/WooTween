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
    public static partial class TweenEx_Rendering
    {
        public static ITweenContext<float, Material> DoFloat(this Material target, string name, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, (target) => target.GetFloat(name), (target, value) => target.SetFloat(name, value), snap);

        public static ITweenContext<Color, Material> DoColor(this Material target, string name, Color start, Color end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, (target) => target.GetColor(name), (target, value) => target.SetColor(name, value), snap);

        public static ITweenContext<Vector4, Material> DoVector(this Material target, string name, Vector4 start, Vector4 end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, (target) => target.GetVector(name), (target, value) => target.SetVector(name, value), snap);


        public static ITweenContext<float, Material> DoInt(this Material target, string name, float start, float end, float duration)
=> Tween.DoGoto(target, start, end, duration, (target) => target.GetInteger(name), (target, value) => target.SetInteger(name, (int)value), true);


        public static ITweenContext<float, Material> DoFloat(this Material target, string name, float end, float duration, bool snap = false)
=> target.DoFloat(name, target.GetFloat(name), end, duration, snap);

        public static ITweenContext<Color, Material> DoColor(this Material target, string name, Color end, float duration, bool snap = false)
=> target.DoColor(name, target.GetColor(name), end, duration, snap);

        public static ITweenContext<Vector4, Material> DoVector(this Material target, string name, Vector4 end, float duration, bool snap = false)
=> target.DoVector(name, target.GetVector(name), end, duration, snap);


        public static ITweenContext<float, Material> DoInt(this Material target, string name, float end, float duration)
=> target.DoInt(name, target.GetFloat(name), end, duration);


        public static ITweenContext<Color, SpriteRenderer> DoColor(this SpriteRenderer target, Color start, Color end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, (target) => target.color, (target, value) => target.color = value, snap);
        public static ITweenContext<Color, SpriteRenderer> DoColor(this SpriteRenderer target, Color end, float duration, bool snap = false)
=> target.DoColor(target.color, end, duration, snap);



        public class DoVectorActor : TweenComponentActor<Vector4, Material>
        {
            public StartValueType startType;
            public string name;
            public Vector4 start = Vector4.zero;
            public Vector4 end = Vector4.zero;
            protected override ITweenContext<Vector4, Material> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoVector(name, end, duration, snap);
                return target.DoVector(name, start, end, duration, snap);
            }
        }

        public class DoMaterialColorActor : TweenComponentActor<Color, Material>
        {
            public StartValueType startType;
            public string name;

            public Color start = Color.white;
            public Color end = Color.white;
            protected override ITweenContext<Color, Material> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoColor(name, end, duration, snap);
                return target.DoColor(name, start, end, duration, snap);
            }
        }
        public class DoFloatActor : TweenComponentActor<float, Material>
        {
            public StartValueType startType;
            public string name;
            public float start = 0;
            public float end = 0;
            protected override ITweenContext<float, Material> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoFloat(name, end, duration, snap);
                return target.DoFloat(name, start, end, duration, snap);
            }
        }
        public class DoIntActor : TweenComponentActor<float, Material>
        {
            public StartValueType startType;
            public string name;
            public float start = 0;
            public float end = 0;
            protected override ITweenContext<float, Material> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoInt(name, end, duration);
                return target.DoInt(name, start, end, duration);
            }
        }
        public class DoColorActor : TweenComponentActor<Color, SpriteRenderer>
        {
            public StartValueType startType;
            public Color start = Color.white;
            public Color end = Color.white;
            protected override ITweenContext<Color, SpriteRenderer> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoColor(end, duration, snap);
                return target.DoColor(start, end, duration, snap);
            }
        }


    }
}