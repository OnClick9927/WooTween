﻿
using UnityEngine;
using WooTween;
using UnityEngine.UI;

namespace IFramework_Demo
{
    public class TweenTest : MonoBehaviour
    {
        public Transform cube;
        public Camera camera;
        public Light light;
        ITween tc;
        public AnimationCurve curve;


        public Image image;
        
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                tc.Rewind(1);
            }
            if (Input.GetKey(KeyCode.A))
            {
                tc.ReStart();
            }
            if (Input.GetKey(KeyCode.Q))
            {
                tc.Complete(false);
            }
        }




        public void Start()
        {
            // Debug.Log(camera.pixelRect);
            // camera.DoRect(new Rect(0,0,0.8f,0.8f), 4);

           
            
            //tc = cube.DoMove(new Vector3[] {
            //    Vector3.zero,
            //    Vector3.one,
            //    Vector3.one * 2,
            //    Vector3.one * 3,
            //    Vector3.one * -4,
            //    Vector3.one * 5,
            //    Vector3.one * 6,
            //}, 5, false)
            //.SetRecyle(false);

            Debug.Log(Time.time);
            tc = cube.DoMove(cube.transform.position + Vector3.right * 5, 2, false)
                  .SetLoop(4, LoopType.PingPong)
                  .SetAnimationCurve(curve)
                  .SetRecycle(false)
                  .OnComplete(() =>
                  {
                      Debug.Log(Time.time);
                  })
                  ;
            //cube.DoMove(cube.transform.position + Vector3.up * 2, 2)
            //         .SetLoop(-1, LoopType.PingPong)
            //         .SetRecyle(false);



            //cube.DoScale(Vector3.one * 10, 0.5f, true)
            //    .SetLoop(3, LoopType.PingPong);
            //cube.DoRota(new Vector3(0, 360, 0), 5f, false)
            //    .SetLoop(-1, LoopType.ReStart)
            //    .SetRecyle(false);
            //cube.GetComponent<Renderer>().material.DoColor(Color.cyan, 0.6f)
            //     .SetLoop(-1, LoopType.PingPong)
            //     .SetRecyle(false);


            //text.DoText(0, 10f, 2f).SetLoop(-1, LoopType.PingPong);
            //text.DoText("123456789", 2)
            //        .SetLoop(-1, LoopType.PingPong);


            // image.DoBlendableColor(Color.red, 2);

            tc.SetUpdateType(TweenUpdateType.Update).SetDeltaTime(0.1f).SetDelta(0.2f).SetTimeScale(2);

        }
    }
}
