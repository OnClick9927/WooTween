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
    public static partial class TweenEx_Rigidbody2D
    {
        public static ITweenContext<Vector2, Rigidbody2D> DoPosition(this Rigidbody2D target, Vector2 start, Vector2 end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.position, static (target, value) => target.position = value, snap);
        public static ITweenContext<Vector2, Rigidbody2D> DoPunchPosition(this Rigidbody2D target, Vector2 start, Vector2 end, Vector2 strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> Tween.DoPunch(target, start, end, duration, static (target) => target.position, static (target, value) => target.position = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector2, Rigidbody2D> DoShakePosition(this Rigidbody2D target, Vector2 start, Vector2 end, Vector2 strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> Tween.DoShake(target, start, end, duration, static (target) => target.position, static (target, value) => target.position = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<float, Rigidbody2D> DoRotation(this Rigidbody2D target, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.rotation, static (target, value) => target.rotation = value, snap);
        public static ITweenContext<float, Rigidbody2D> DoPunchRotation(this Rigidbody2D target, float start, float end, float strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> Tween.DoPunch(target, start, end, duration, static (target) => target.rotation, static (target, value) => target.rotation = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<float, Rigidbody2D> DoShakeRotation(this Rigidbody2D target, float start, float end, float strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> Tween.DoShake(target, start, end, duration, static (target) => target.rotation, static (target, value) => target.rotation = value, strength, frequency, dampingRatio, snap);


        public static ITweenContext<Vector2, Rigidbody2D> DoPosition(this Rigidbody2D target, Vector2 end, float duration, bool snap = false)
=> target.DoPosition(target.position, end, duration, snap);
        public static ITweenContext<Vector2, Rigidbody2D> DoPunchPosition(this Rigidbody2D target, Vector2 end, Vector2 strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> target.DoPunchPosition(target.position, end, strength, duration, frequency, dampingRatio, snap);
        public static ITweenContext<Vector2, Rigidbody2D> DoShakePosition(this Rigidbody2D target, Vector2 end, Vector2 strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> target.DoShakePosition(target.position, end, strength, duration, frequency, dampingRatio, snap);

        public static ITweenContext<float, Rigidbody2D> DoRotation(this Rigidbody2D target, float end, float duration, bool snap = false)
=> target.DoRotation(target.rotation, end, duration, snap);
        public static ITweenContext<float, Rigidbody2D> DoPunchRotation(this Rigidbody2D target, float end, float strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> target.DoPunchRotation(target.rotation, end, strength, duration, frequency, dampingRatio, snap);

        public static ITweenContext<float, Rigidbody2D> DoShakeRotation(this Rigidbody2D target, float end, float strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> target.DoShakeRotation(target.rotation, end, strength, duration, frequency, dampingRatio, snap);

        public class DoShakePositionActor : DoPositionActor
        {
            public Vector2 strength = Vector2.one;
            public int frequency = 10;
            public int dampingRatio = 1;
            protected override ITweenContext<Vector2, Rigidbody2D> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoShakePosition(end, strength, duration, frequency, dampingRatio, snap);
                return target.DoShakePosition(start, end, strength, duration, frequency, dampingRatio, snap);
            }
        }
        public class DoPunchPositionActor : DoPositionActor
        {
            public Vector2 strength = Vector2.one;
            public int frequency = 10;
            public int dampingRatio = 1;
            protected override ITweenContext<Vector2, Rigidbody2D> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoPunchPosition(end, strength, duration, frequency, dampingRatio, snap);
                return target.DoPunchPosition(start, end, strength, duration, frequency, dampingRatio, snap);
            }
        }
        public class DoPositionActor : TweenComponentActor<Vector2, Rigidbody2D>
        {
            public StartValueType startType;
            public Vector2 start = Vector2.zero;
            public Vector2 end = Vector2.one;
            protected override ITweenContext<Vector2, Rigidbody2D> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoPosition(end, duration, snap);
                return target.DoPosition(start, end, duration, snap);
            }
        }

        public class DoShakeRotationActor : DoRotationActor
        {

            public float strength = 1;
            public int frequency = 10;
            public int dampingRatio = 1;
            protected override ITweenContext<float, Rigidbody2D> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoShakeRotation(end, strength, duration, frequency, dampingRatio, snap);
                return target.DoShakeRotation(start, end, strength, duration, frequency, dampingRatio, snap);
            }
        }
        public class DoPunchRotationActor : DoRotationActor
        {

            public float strength = 1;
            public int frequency = 10;
            public int dampingRatio = 1;
            protected override ITweenContext<float, Rigidbody2D> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoPunchRotation(end, strength, duration, frequency, dampingRatio, snap);
                return target.DoPunchRotation(start, end, strength, duration, frequency, dampingRatio, snap);
            }
        }
        public class DoRotationActor : TweenComponentActor<float, Rigidbody2D>
        {
            public StartValueType startType;
            public float start = 0;
            public float end = 1;
            protected override ITweenContext<float, Rigidbody2D> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoRotation(end, duration, snap);
                return target.DoRotation(start, end, duration, snap);
            }
        }


    }

}