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
    public static partial class TweenEx_Audio
    {
        public static ITweenContext<float, AudioSource> DoVolume(this AudioSource target, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.volume, static (target, value) => target.volume = value, snap);

        public static ITweenContext<float, AudioSource> DoPitch(this AudioSource target, float start, float end, float duration, bool snap = false)
=> Tween.DoGoto(target, start, end, duration, static (target) => target.pitch, static (target, value) => target.pitch = value, snap);


        public static ITweenContext<float, AudioSource> DoVolume(this AudioSource target, float end, float duration, bool snap = false)
=> target.DoVolume(target.volume, end, duration, snap);

        public static ITweenContext<float, AudioSource> DoPitch(this AudioSource target, float end, float duration, bool snap = false)
=> target.DoPitch(target.volume, end, duration, snap);

        public class DoVolumeActor : TweenComponentActor<float, AudioSource>
        {
            public StartValueType startType;
            public float start = 0;
            public float end = 1;
            protected override ITweenContext<float, AudioSource> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoVolume(end, duration, snap);
                return target.DoVolume(start, end, duration, snap);
            }
        }

        public class DoPitchActor : TweenComponentActor<float, AudioSource>
        {
            public StartValueType startType;
            public float start = 0;
            public float end = 1;
            protected override ITweenContext<float, AudioSource> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoPitch(end, duration, snap);
                return target.DoPitch(start, end, duration, snap);
            }
        }
    }



}