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
    [UnityEngine.AddComponentMenu("")]
    class TweenScheduler_Runtime : MonoBehaviour
    {
        public TweenScheduler scheduler;
        public static TweenScheduler_Runtime Instance
        {
            get
            {
                if (!Application.isPlaying) return null;
                if (ins == null)
                {
                    ins = new GameObject("Tween").AddComponent<TweenScheduler_Runtime>();
                    DontDestroyOnLoad(ins.gameObject);
                }
                return ins;
            }
        }

        private static TweenScheduler_Runtime ins;
        private void Awake()
        {
            scheduler = new TweenScheduler();

        }

        private void Update()
        {
            scheduler.Update();
        }
        protected void OnDestroy()
        {
            scheduler.KillTweens();
        }


    }





}