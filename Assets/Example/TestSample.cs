using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TestSample : MonoBehaviour
{

    public Vector3 start = Vector3.one;
    public Vector3 end = Vector3.zero;
    public float dur = 1;
    public float progress = 0;
    // Update is called once per frame
    void Update()
    {
        progress += Time.deltaTime;
        progress = Mathf.Repeat(progress, 1f);
        WooTween.Tween.Sample(transform, start, end, dur, (target) =>
        {
            return target.position;
        }, (tar, value) => { tar.position = value; }, false, progress);
    }
}
