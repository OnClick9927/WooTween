/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

namespace WooTween
{
    public static partial class TweenEx_Common
    {
        public class DoWaitActor : TweenComponentActor
        {

            protected override ITweenContext _Create()
            {
                return Tween.DoWait(duration);
            }

        }
    }

}