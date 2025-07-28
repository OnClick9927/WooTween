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
    public static partial class TweenEx_Rigidbody
    {
        public static ITweenContext<Vector3, Rigidbody> DoPosition(this Rigidbody target, Vector3 start, Vector3 end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.position, static (target, value) => target.position = value, snap);
        public static ITweenContext<Vector3, Rigidbody> DoPunchPosition(this Rigidbody target, Vector3 start, Vector3 end, Vector3 strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> Tween.DoPunch(target, start, end, duration, static (target) => target.position, static (target, value) => target.position = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Rigidbody> DoShakePosition(this Rigidbody target, Vector3 start, Vector3 end, Vector3 strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> Tween.DoShake(target, start, end, duration, static (target) => target.position, static (target, value) => target.position = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Rigidbody> DoRotation(this Rigidbody target, Vector3 start, Vector3 end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.rotation.eulerAngles, static (target, value) => target.rotation = Quaternion.Euler(value), snap);
        public static ITweenContext<Vector3, Rigidbody> DoPunchRotation(this Rigidbody target, Vector3 start, Vector3 end, Vector3 strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> Tween.DoPunch(target, start, end, duration, static (target) => target.rotation.eulerAngles, static (target, value) => target.rotation = Quaternion.Euler(value), strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Rigidbody> DoShakeRotation(this Rigidbody target, Vector3 start, Vector3 end, Vector3 strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> Tween.DoShake(target, start, end, duration, static (target) => target.rotation.eulerAngles, static (target, value) => target.rotation = Quaternion.Euler(value), strength, frequency, dampingRatio, snap);


        public static ITweenContext<Vector3, Rigidbody> DoPosition(this Rigidbody target, Vector3 end, float duration, bool snap = false)
=> target.DoPosition(target.position, end, duration, snap);
        public static ITweenContext<Vector3, Rigidbody> DoPunchPosition(this Rigidbody target, Vector3 end, Vector3 strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> target.DoPunchPosition(target.position, end, strength, duration, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Rigidbody> DoShakePosition(this Rigidbody target, Vector3 end, Vector3 strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> target.DoShakePosition(target.position, end, strength, duration, frequency, dampingRatio, snap);

        public static ITweenContext<Vector3, Rigidbody> DoRotation(this Rigidbody target, Vector3 end, float duration, bool snap = false)
=> target.DoRotation(target.rotation.eulerAngles, end, duration, snap);
        public static ITweenContext<Vector3, Rigidbody> DoPunchRotation(this Rigidbody target, Vector3 end, Vector3 strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> target.DoPunchRotation(target.rotation.eulerAngles, end, strength, duration, frequency, dampingRatio, snap);

        public static ITweenContext<Vector3, Rigidbody> DoShakeRotation(this Rigidbody target, Vector3 end, Vector3 strength, float duration, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> target.DoShakeRotation(target.rotation.eulerAngles, end, strength, duration, frequency, dampingRatio, snap);

        public class DoShakePositionActor_Rigidbody : DoPositionActor_Rigidbody
        {
            public Vector3 strength = Vector3.one;
            public int frequency = 10;
            public int dampingRatio = 1;
            protected override ITweenContext<Vector3, Rigidbody> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoShakePosition(end, strength, duration, frequency, dampingRatio, snap);
                return target.DoShakePosition(start, end, strength, duration, frequency, dampingRatio, snap);
            }
        }
        public class DoPunchPositionActor_Rigidbody : DoPositionActor_Rigidbody
        {
            public Vector3 strength = Vector3.one;
            public int frequency = 10;
            public int dampingRatio = 1;
            protected override ITweenContext<Vector3, Rigidbody> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoPunchPosition(end, strength, duration, frequency, dampingRatio, snap);
                return target.DoPunchPosition(start, end, strength, duration, frequency, dampingRatio, snap);
            }
        }
        public class DoPositionActor_Rigidbody : TweenComponentActor<Vector3, Rigidbody>
        {
            public StartValueType startType;
            public Vector3 start = Vector3.zero;
            public Vector3 end = Vector3.one;
            protected override ITweenContext<Vector3, Rigidbody> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoPosition(end, duration, snap);
                return target.DoPosition(start, end, duration, snap);
            }
        }

        public class DoShakeRotationActor_Rigidbody : DoRotationActor_Rigidbody
        {

            public Vector3 strength = Vector3.one;
            public int frequency = 10;
            public int dampingRatio = 1;
            protected override ITweenContext<Vector3, Rigidbody> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoShakeRotation(end, strength, duration, frequency, dampingRatio, snap);
                return target.DoShakeRotation(start, end, strength, duration, frequency, dampingRatio, snap);
            }
        }
        public class DoPunchRotationActor_Rigidbody : DoRotationActor_Rigidbody
        {

            public Vector3 strength = Vector3.one;
            public int frequency = 10;
            public int dampingRatio = 1;
            protected override ITweenContext<Vector3, Rigidbody> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoPunchRotation(end, strength, duration, frequency, dampingRatio, snap);
                return target.DoPunchRotation(start, end, strength, duration, frequency, dampingRatio, snap);
            }
        }
        public class DoRotationActor_Rigidbody : TweenComponentActor<Vector3, Rigidbody>
        {
            public StartValueType startType;
            public Vector3 start = Vector3.zero;
            public Vector3 end = Vector3.one;
            protected override ITweenContext<Vector3, Rigidbody> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoRotation(end, duration, snap);
                return target.DoRotation(start, end, duration, snap);
            }
        }


    }

}