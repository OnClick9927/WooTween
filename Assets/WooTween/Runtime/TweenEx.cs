﻿
using System;
using UnityEngine;
using UnityEngine.UI;
using WooPool;

namespace WooTween
{
    public static partial class TweenEx
    {
        public static ITween SetRecycle(this ITween tween, bool rec)
        {
            if (tween.recycled)
                throw new Exception("The Tween Has Been Recyled,Do not Do anything On it");
            tween.autoRecycle = rec;
            return tween;
        }
        public static ITween OnComplete(this ITween tween, Action onComplete)
        {
            if (tween.recycled)
                throw new Exception("The Tween Has Been Recyled,Do not Do anything On it");
            (tween as Tween).onComplete += onComplete;
            return tween;
        }
        public static ITween SetLoop(this ITween tween, int loop, LoopType loopType)
        {
            if (tween.recycled)
                throw new Exception("The Tween Has Been Recyled,Do not Do anything On it");
            tween.loop = loop;
            tween.loopType = loopType;
            return tween;
        }



        public static ITween SetAnimationCurve(this ITween tween, AnimationCurve curve)
        {
            var converter = PoolEx.GlobalAllocate<AnimationCurveCoverter>().Config(curve);
            return tween.SetConverter(converter);
        }
        public static ITween SetEase(this ITween tween, Ease ease)
        {
            var converter = PoolEx.GlobalAllocate<EaseCoverter>().Config(ease);
            return tween.SetConverter(converter);
        }





        public static ITween SetUpdateType(this ITween tween, TweenUpdateType type)
        {
            TweenSingleton.updateType = type;
            return tween;
        }
        public static ITween SetDeltaTime(this ITween tween, float delta)
        {
            TweenValue.deltaTime = delta;
            return tween;
        }
        public static ITween SetDelta(this ITween tween, float delta)
        {
            TweenValue.delta = delta;
            return tween;
        }
        public static ITween SetTimeScale(this ITween tween, float speed)
        {
            TweenValue.timeScale = speed;
            return tween;
        }




        private static IArrayTween<T> AllocateArrayTween<T>(EnvironmentType env) where T : struct
        {
            if (env != EnvironmentType.Editor)
                TweenSingleton.Initialized();

            return TweenObject.Allocate<ArrayTween<T>>(env);
        }
        private static ISingleTween<T> AllocateSingleTween<T>(EnvironmentType env) where T : struct
        {
            if (env != EnvironmentType.Editor)
                TweenSingleton.Initialized();
            return TweenObject.Allocate<SingleTween<T>>(env);
        }
        public static ITween SetAutoRecyle(this ITween tween,bool auto)
        {
            tween.autoRecycle = auto;
            return tween;
        }

        public static ITween<T> DoGoto<T>(T start, T end, float duration, Func<T> getter, Action<T> setter, bool snap, EnvironmentType env= EnvironmentType.RT) where T : struct
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying && !Application.isPlaying)
            {
                env = EnvironmentType.Editor;
            }
#endif
            var tween = AllocateSingleTween<T>(env);
            tween.Config(start, end, duration, getter, setter, snap);
            
            (tween as Tween).WaitToRun();
            return tween;
        }
        public static ITween<T> DoGoto<T>(T[] array, float duration, Func<T> getter, Action<T> setter, bool snap, EnvironmentType env = EnvironmentType.RT) where T : struct
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying && !Application.isPlaying)
            {
                env = EnvironmentType.Editor;
            }
#endif
            var tween = AllocateArrayTween<T>(env);
            tween.Config(array, duration, getter, setter, snap);
            (tween as Tween).WaitToRun();
            return tween;
        }
    }
    public static partial class TweenEx
    {
        public static ITween<Vector3> DoMove(this Transform target, Vector3 end, float duration, bool snap = false)
        {
            return DoGoto(target.position, end, duration, () => { return target.position; },
                    (value) => {
                        target.position = value;
                    }, snap
                );
        }
        public static ITween<float> DoMoveX(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.position.x, end, duration, () => { return target.position.x; }, (value) => {
                target.position = new Vector3(value, target.position.y, target.position.z);
            }, snap);
        }
        public static ITween<float> DoMoveY(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.position.y, end, duration, () => { return target.position.y; },
                             (value) => {
                                 target.position = new Vector3(target.position.x, value, target.position.z);
                             }, snap);
        }
        public static ITween<float> DoMoveZ(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.position.z, end, duration, () => { return target.position.z; }, (value) => {
                target.position = new Vector3(target.position.x, target.position.y, value);
            }, snap);
        }
        public static ITween<Vector3> DoLocalMove(this Transform target, Vector3 end, float duration, bool snap = false)
        {
            return DoGoto(target.localPosition, end, duration, () => { return target.localPosition; }, (value) => {
                target.localPosition = value;
            }, snap);
        }
        public static ITween<float> DoLocalMoveX(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.localPosition.x, end, duration, () => { return target.localPosition.x; }, (value) => {
                target.localPosition = new Vector3(value, target.localPosition.y, target.localPosition.z);
            }, snap);
        }
        public static ITween<float> DoLocalMoveY(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.localPosition.y, end, duration, () => { return target.localPosition.y; }, (value) => {
                target.localPosition = new Vector3(target.localPosition.x, value, target.localPosition.z);
            }, snap);
        }
        public static ITween<float> DoLocalMoveZ(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.localPosition.z, end, duration, () => { return target.localPosition.z; }, (value) => {
                target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, value);
            }, snap);
        }


        public static ITween<Vector3> DoScale(this Transform target, Vector3 end, float duration, bool snap = false)
        {
            return DoGoto(target.localScale, end, duration, () => { return target.localScale; }, (value) => {
                target.localScale = value;
            }, snap);
        }
        public static ITween<float> DoScaleX(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.localScale.x, end, duration, () => { return target.localScale.x; }, (value) => {
                target.localScale = new Vector3(value, target.localScale.y, target.localScale.z);
            }, snap);
        }
        public static ITween<float> DoScaleY(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.localScale.y, end, duration, () => { return target.localScale.y; }, (value) => {
                target.localScale = new Vector3(target.localScale.x, value, target.localScale.z);
            }, snap);
        }
        public static ITween<float> DoScaleZ(this Transform target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.localScale.z, end, duration, () => { return target.localScale.z; }, (value) => {
                target.localScale = new Vector3(target.localScale.x, target.localScale.y, value);
            }, snap);
        }


        public static ITween<Quaternion> DoRota(this Transform target, Quaternion end, float duration, bool snap = false)
        {
            return DoGoto(target.rotation, end, duration, () => { return target.rotation; }, (value) => {
                target.rotation = value;
            }, snap);
        }
        public static ITween<Vector3> DoRota(this Transform target, Vector3 end, float duration, bool snap = false)
        {
            return DoGoto(target.rotation.eulerAngles, end, duration, () => { return target.rotation.eulerAngles; }, (value) => {
                target.rotation = Quaternion.Euler(value);
            }, snap);
        }
        public static ITween<Quaternion> DoRotaFast(this Transform target, Vector3 end, float duration, bool snap = false)
        {
            return DoGoto(target.rotation, Quaternion.Euler(end), duration, () => { return target.rotation; }, (value) => {
                target.rotation = value;
            }, snap);
        }

        public static ITween<Quaternion> DoLocalRota(this Transform target, Quaternion end, float duration, bool snap = false)
        {
            return DoGoto(target.localRotation, end, duration, () => { return target.localRotation; }, (value) => {
                target.localRotation = value;
            }, snap);
        }
        public static ITween<Quaternion> DoLocalRota(this Transform target, Vector3 end, float duration, bool snap = false)
        {
            return DoGoto(target.localRotation, Quaternion.Euler(end), duration, () => { return target.localRotation; }, (value) => {
                target.localRotation = value;
            }, snap);
        }


        public static ITween<Color> DoColor(this Material target, Color end, float duration, bool snap = false)
        {
            return DoGoto(target.color, end, duration, () => { return target.color; }, (value) => {
                target.color = value;
            }, snap);
        }
        public static ITween<Color> DoColor(this Graphic target, Color end, float duration, bool snap = false)
        {
            return DoGoto(target.color, end, duration, () => { return target.color; }, (value) => {
                target.color = value;
            }, snap);
        }
        public static ITween<Color> DoColor(this Light target, Color end, float duration, bool snap = false)
        {
            return DoGoto(target.color, end, duration, () => { return target.color; }, (value) => {
                target.color = value;
            }, snap);
        }
        public static ITween<Color> DoColor(this Camera target, Color end, float duration, bool snap = false)
        {
            return DoGoto(target.backgroundColor, end, duration, () => { return target.backgroundColor; }, (value) => {
                target.backgroundColor = value;
            }, snap);
        }


        public static ITween<float> DoAlpha(this Material target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.color.a, end, duration, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this Graphic target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.color.a, end, duration, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this Light target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.color.a, end, duration, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this Camera target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.backgroundColor.a, end, duration, () => { return target.backgroundColor.a; }, (value) => {
                target.backgroundColor = new Color(target.backgroundColor.r, target.backgroundColor.g, target.backgroundColor.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this CanvasGroup target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.alpha, end, duration, () => { return target.alpha; }, (value) => {
                target.alpha = value;
            }, snap);
        }



        public static ITween<int> DoText(this Text target, int start, int end, float duration, bool snap = false)
        {
            return DoGoto(start, end, duration, () => {
                int value;
                if (int.TryParse(target.text, out value))
                    return value;
                return 0;
            }, (value) => {
                target.text = value.ToString();
            }, snap);
        }
        public static ITween<int> DoText(this Text target, string end, float duration)
        {
            return DoGoto(target.text.Length, end.Length, duration, () => { return target.text.Length; }, (value) => {
                target.text = end.Substring(0, value>=target.text.Length? target.text.Length:value);
            }, false);
        }
        public static ITween<float> DoText(this Text target, float start, float end, float duration, bool snap = false)
        {
            return DoGoto(start, end, duration, () => {
                float value;
                if (float.TryParse(target.text, out value))
                    return value;
                return 0;
            }, (value) => {
                target.text = value.ToString();
            }, snap);
        }



        public static ITween<float> DoFillAmount(this Image target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.fillAmount, end, duration, () => { return target.fillAmount; }, (value) => {
                target.fillAmount = value;
            }, snap);
        }
        public static ITween<float> DoNormalizedPositionX(this ScrollRect target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.normalizedPosition.x, end, duration, () => { return target.normalizedPosition.x; }, (value) => {
                target.normalizedPosition = new Vector2(value, target.normalizedPosition.y);
            }, snap);
        }
        public static ITween<float> DoNormalizedPositionY(this ScrollRect target, float end, float duration, bool snap = false)
        {
            return DoGoto(target.normalizedPosition.y, end, duration, () => { return target.normalizedPosition.y; }, (value) => {
                target.normalizedPosition = new Vector2(target.normalizedPosition.x, value);
            }, snap);
        }




        public static ITween<bool> DoActive(this GameObject target, bool end, float duration)
        {
            return DoGoto(target.activeSelf, end, duration, () => { return target.activeSelf; }, (value) => {
                target.SetActive(value);
            }, false);
        }
        public static ITween<bool> DoEnable(this Behaviour target, bool end, float duration)
        {
            return DoGoto(target.enabled, end, duration, () => { return target.enabled; }, (value) => {
                target.enabled = value;
            }, false);
        }
        public static ITween<bool> DoToggle(this Toggle target, bool end, float duration)
        {
            return DoGoto(target.isOn, end, duration, () => { return target.isOn; }, (value) => {
                target.isOn = value;
            }, false);
        }
    }
    public static partial class TweenEx
    {
        public static ITween<Vector3> DoMove(this Transform self, Vector3[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.position; }, (value) => { self.position = value; }, snap);
        }
        public static ITween<float> DoMoveX(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.position.x; }, (value) => { self.position = new Vector3(value, self.position.y, self.position.z); }, snap);
        }
        public static ITween<float> DoMoveY(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.position.y; }, (value) => { self.position = new Vector3(self.position.x, value, self.position.z); }, snap);
        }
        public static ITween<float> DoMoveZ(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.position.z; }, (value) => { self.position = new Vector3(self.position.x, self.position.y, value); }, snap);
        }


        public static ITween<Vector3> DoLocalMove(this Transform self, Vector3[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localPosition; }, (value) => { self.localPosition = value; }, snap);
        }
        public static ITween<float> DoLocalMoveX(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localPosition.x; }, (value) => { self.localPosition = new Vector3(value, self.localPosition.y, self.localPosition.z); }, snap);
        }
        public static ITween<float> DoLocalMoveY(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localPosition.y; }, (value) => { self.localPosition = new Vector3(self.localPosition.x, value, self.localPosition.z); }, snap);
        }
        public static ITween<float> DoLocalMoveZ(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localPosition.z; }, (value) => { self.localPosition = new Vector3(self.localPosition.x, self.localPosition.y, value); }, snap);
        }


        public static ITween<Vector3> DoScale(this Transform self, Vector3[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localScale; }, (value) => { self.localScale = value; }, snap);
        }
        public static ITween<float> DoScaleX(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localScale.x; }, (value) => { self.localScale = new Vector3(value, self.localScale.y, self.localScale.z); }, snap);
        }
        public static ITween<float> DoScaleY(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localScale.y; }, (value) => { self.localScale = new Vector3(self.localScale.x, value, self.localScale.z); }, snap);
        }
        public static ITween<float> DoScaleZ(this Transform self, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return self.localScale.z; }, (value) => { self.localScale = new Vector3(self.localScale.x, self.localScale.y, value); }, snap);
        }

        public static ITween<Quaternion> DoRota(this Transform target, Quaternion[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.rotation; }, (value) => {
                target.rotation = value;
            }, snap);
        }
        public static ITween<Vector3> DoRota(this Transform target, Vector3[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.rotation.eulerAngles; }, (value) => {
                target.rotation = Quaternion.Euler(value);
            }, snap);
        }
        public static ITween<Quaternion> DoLocalRota(this Transform target, Quaternion[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.localRotation; }, (value) => {
                target.localRotation = value;
            }, snap);
        }


        public static ITween<Color> DoColor(this Material target, Color[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.color; }, (value) => {
                target.color = value;
            }, snap);
        }
        public static ITween<Color> DoColor(this Graphic target, Color[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.color; }, (value) => {
                target.color = value;
            }, snap);
        }
        public static ITween<Color> DoColor(this Light target, Color[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.color; }, (value) => {
                target.color = value;
            }, snap);
        }
        public static ITween<Color> DoColor(this Camera target, Color[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.backgroundColor; }, (value) => {
                target.backgroundColor = value;
            }, snap);
        }

        public static ITween<float> DoAlpha(this Material target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this Graphic target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this Light target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.color.a; }, (value) => {
                target.color = new Color(target.color.a, target.color.g, target.color.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this Camera target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.backgroundColor.a; }, (value) => {
                target.backgroundColor = new Color(target.backgroundColor.r, target.backgroundColor.g, target.backgroundColor.b, value);
            }, snap);
        }
        public static ITween<float> DoAlpha(this CanvasGroup target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.alpha; }, (value) => {
                target.alpha = value;
            }, snap);
        }


        public static ITween<float> DoFillAmount(this Image target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.fillAmount; }, (value) => {
                target.fillAmount = value;
            }, snap);
        }
        public static ITween<float> DoNormalizedPositionX(this ScrollRect target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.normalizedPosition.x; }, (value) => {
                target.normalizedPosition = new Vector2(value, target.normalizedPosition.y);
            }, snap);
        }
        public static ITween<float> DoNormalizedPositionY(this ScrollRect target, float[] values, float duration, bool snap = false)
        {
            return DoGoto(values, duration, () => { return target.normalizedPosition.y; }, (value) => {
                target.normalizedPosition = new Vector2(target.normalizedPosition.x, value);
            }, snap);
        }

        
        //相机  后期添加晃动
        public static ITween<float> DoFieldOfView(this Camera target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.fieldOfView, value, duration, () => { return target.fieldOfView; },
                (value) => { target.fieldOfView = value; }, snap);
        }

        public static ITween<float> DoFarClipPlane(this Camera target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.farClipPlane, value, duration, () => { return target.farClipPlane; },
                (value) => { target.farClipPlane = value; }, snap);
        }

        public static ITween<float> DoNearClipPlane(this Camera target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.nearClipPlane, value, duration, () => { return target.nearClipPlane; },
                (value) => { target.nearClipPlane = value; }, snap);
        }

        public static ITween<float> DoOrthoSize(this Camera target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.orthographicSize, value, duration, () => { return target.orthographicSize; },
                (value) => { target.orthographicSize = value; }, snap);
        }

        public static ITween<Rect> DoPixelRect(this Camera target, Rect value, float duration, bool snap = false)
        {

            return DoGoto(target.pixelRect, value, duration, () => { return target.pixelRect; },
                (value) => { target.pixelRect = value; }, snap);
        }
        public static ITween<Rect> DoRect(this Camera target, Rect value, float duration, bool snap = false)
        {

            return DoGoto(target.rect, value, duration, () => { return target.rect; },
                (value) => { target.rect = value; }, snap);
        }
        
        
        
        
        //灯光 后期添加DOBlendableColor 
        public static ITween<float> DoIntensity(this Light target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.intensity, value, duration, () => { return target.intensity; },
                (value) => { target.intensity = value; }, snap);
        }

        public static ITween<float> DoShowStrength(this Light target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.shadowStrength, value, duration, () => { return target.shadowStrength; },
                (value) => { target.shadowStrength = value; }, snap);
        }
        

        
        //材质
        public static ITween<Vector2> DoOffset(this Material target, Vector2 value, float duration, bool snap = false)
        {

            return DoGoto(target.mainTextureOffset, value, duration, () => { return target.mainTextureOffset; },
                (value) => { target.mainTextureOffset = value; }, snap);
        }
        public static ITween<Vector2> DoTiling(this Material target, Vector2 value, float duration, bool snap = false)
        {

            return DoGoto(target.mainTextureScale, value, duration, () => { return target.mainTextureScale; },
                (value) => { target.mainTextureScale = value; }, snap);
        }
  
        
        
        
        //UI
        
      
        public static ITween<Color> DoColor(this Image target, Color value, float duration, bool snap = false)
        {

            return DoGoto(target.color, value, duration, () => { return target.color; },
                (value) => { target.color = value; }, snap);
        }
        
        public static ITween<float> DoAlpha(this Image target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.color.a, value, duration, 
                () =>
                {
                    return target.color.a;
                },
                (value) =>
                {
                    target.color = new Color(target.color.r,target.color.g,target.color.b,value);
                }, snap);
        }

       
        
        //Audio
        
        public static ITween<float> DoFade(this AudioSource target, float value, float duration, bool snap = false)
        {

            if (value < 0) value = 0;
            else if (value > 1) value = 1;
            return DoGoto(target.volume, value, duration, () => target.volume,
                (value) => { target.volume = value; }, snap);
        }
        public static ITween<float> DoPitch(this AudioSource target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.pitch, value, duration, () => target.pitch,
                (value) => { target.pitch = value; }, snap);
        }
        
        
        //刚体
        public static ITween<Vector3> DoMove(this Rigidbody target, Vector3 value, float duration, bool snap = false)
        {

            return DoGoto(target.position, value, duration, () => target.position,
                (value) => { target.position = value; }, snap);
        }

        public static ITween<float> DoMoveX(this Rigidbody target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.position.x, value, duration, () => target.position.x,
                (value) => { target.position = new Vector3(value,target.position.y,target.position.z); }, snap);
        }
        public static ITween<float> DoMoveY(this Rigidbody target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.position.y, value, duration, () => target.position.y,
                (value) => { target.position = new Vector3(target.position.x,value,target.position.z); }, snap);
        }

        public static ITween<float> DoMoveZ(this Rigidbody target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.position.z, value, duration, () => target.position.z,
                (value) => { target.position = new Vector3(target.position.x, target.position.y, value); }, snap);
        }

        public static ITween<Quaternion> DoRotate(this Rigidbody target, Quaternion value, float duration, bool snap = false)
        {

            return DoGoto(target.rotation, value, duration, () => target.rotation,
                (value) => { target.rotation = value; }, snap);
        }
        
        public static ITween<Vector3> DoRotate(this Rigidbody target, Vector3 value, float duration, bool snap = false)
        {
            return DoGoto(target.rotation.eulerAngles, value, duration, () => { return target.rotation.eulerAngles; }, (value) => {
                target.rotation = Quaternion.Euler(value);
            }, snap);
        }
        
        //刚体2D
        
        public static ITween<Vector2> DoMove(this Rigidbody2D target, Vector2 value, float duration, bool snap = false)
        {

            return DoGoto(target.position, value, duration, () => target.position,
                (value) => { target.position = value; }, snap);
        }

        public static ITween<float> DoMoveX(this Rigidbody2D target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.position.x, value, duration, () => target.position.x,
                (value) => { target.position = new Vector3(value,target.position.y); }, snap);
        }
        public static ITween<float> DoMoveY(this Rigidbody2D target, float value, float duration, bool snap = false)
        {
            return DoGoto(target.position.y, value, duration, () => target.position.y,
                (value) => { target.position = new Vector3(target.position.x,value); }, snap);
        }


        public static ITween<float> DoRotate(this Rigidbody2D target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.rotation, value, duration, () => target.rotation,
                (value) => { target.rotation = value; }, snap);
        }
        
        //SpriteRenderer
        public static ITween<Color> DoColor(this SpriteRenderer target, Color value, float duration, bool snap = false)
        {

            return DoGoto(target.color, value, duration, () => target.color,
                (value) => { target.color = value; }, snap);
        }
        public static ITween<float> DoFade(this SpriteRenderer target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.color.a, value, duration, () => target.color.a,
                (value) => { target.color = new Color(target.color.r, target.color.g, target.color.b, value); }, snap);
        }
        
        
        //LayoutElement
        public static ITween<Vector2> DoFlexibleSize(this LayoutElement target, Vector2 value, float duration, bool snap = false)
        {

            return DoGoto(new Vector2(target.flexibleWidth, target.flexibleHeight), value, duration,
                () => new Vector2(target.flexibleWidth, target.flexibleHeight),
                (value) =>
                {
                    target.flexibleWidth = value.x;

                    target.flexibleHeight = value.y;
                }, snap);
        }
        
        public static ITween<Vector2> DoMinSize(this LayoutElement target, Vector2 value, float duration, bool snap = false)
        {

            return DoGoto(new Vector2(target.minWidth, target.minHeight), value, duration,
                () => new Vector2(target.minWidth, target.minHeight),
                (value) =>
                {
                    target.minWidth = value.x;

                    target.minHeight = value.y;
                }, snap);
        }
        public static ITween<Vector2> DoPreferredSize(this LayoutElement target, Vector2 value, float duration, bool snap = false)
        {

            return DoGoto(new Vector2(target.preferredWidth, target.preferredHeight), value, duration,
                () => new Vector2(target.preferredWidth, target.preferredHeight),
                (value) =>
                {
                    target.preferredWidth = value.x;

                    target.preferredHeight = value.y;
                }, snap);
        }
        
        
        //Outline
        
        
        public static ITween<Color> DoColor(this Outline target, Color value, float duration, bool snap = false)
        {

            return DoGoto(target.effectColor, value, duration, () => target.effectColor,
                (value) => { target.effectColor = value; }, snap);
        }
        public static ITween<float> DoFade(this Outline target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.effectColor.a, value, duration, () => target.effectColor.a,
                (value) => { target.effectColor = new Color(target.effectColor.r,target.effectColor.g,target.effectColor.b,value); }, snap);
        }
        
        //RectTransform
        
        public static ITween<Vector2> DoAnchorMax(this RectTransform target, Vector2 value, float duration, bool snap = false)
        {

            return DoGoto(target.anchorMax, value, duration, () => target.anchorMax,
                (value) => { target.anchorMax = value; }, snap);
        }
        
        public static ITween<Vector2> DoAnchorMin(this RectTransform target, Vector2 value, float duration, bool snap = false)
        {

            return DoGoto(target.anchorMin, value, duration, () => target.anchorMin,
                (value) => { target.anchorMin = value; }, snap);
        }
        public static ITween<Vector2> DoAnchorPos(this RectTransform target, Vector2 value, float duration, bool snap = false)
        {

            return DoGoto(target.anchoredPosition, value, duration, () => target.anchoredPosition,
                (value) => { target.anchoredPosition = value; }, snap);
        }
        
        public static ITween<float> DoAnchorPosX(this RectTransform target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.anchoredPosition.x, value, duration, () => target.anchoredPosition.x,
                (value) => { target.anchoredPosition =new Vector2(value,target.anchoredPosition.y); }, snap);
        }

        public static ITween<float> DoAnchorPosY(this RectTransform target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.anchoredPosition.y, value, duration, () => target.anchoredPosition.y,
                (value) => { target.anchoredPosition =new Vector2(target.anchoredPosition.x,value); }, snap);
        }
        
        public static ITween<Vector3> DoAnchorPos3D(this RectTransform target, Vector3 value, float duration, bool snap = false)
        {

            return DoGoto(target.anchoredPosition3D, value, duration, () => target.anchoredPosition3D,
                (value) => { target.anchoredPosition3D = value; }, snap);
        }

        public static ITween<float> DoAnchorPos3DX(this RectTransform target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.anchoredPosition3D.x, value, duration, () => target.anchoredPosition3D.x,
                (value) => { target.anchoredPosition3D = new Vector3(value,target.anchoredPosition3D.y,target.anchoredPosition3D.z); }, snap);
        }
        public static ITween<float> DoAnchorPos3DY(this RectTransform target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.anchoredPosition3D.y, value, duration, () => target.anchoredPosition3D.y,
                (value) => { target.anchoredPosition3D = new Vector3(target.anchoredPosition3D.x,value,target.anchoredPosition3D.z); }, snap);
        }

        public static ITween<float> DoAnchorPos3DZ(this RectTransform target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.anchoredPosition3D.z, value, duration, () => target.anchoredPosition3D.z,
                (value) => { target.anchoredPosition3D = new Vector3(target.anchoredPosition3D.x,target.anchoredPosition3D.y,value); }, snap);
        }

        public static ITween<Vector2> DOPivot(this RectTransform target, Vector2 value, float duration, bool snap = false)
        {

            return DoGoto(target.pivot, value, duration, () => target.pivot,
                (value) => { target.pivot = value; }, snap);
        }

        public static ITween<float> DOPivotX(this RectTransform target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.pivot.x, value, duration, () => target.pivot.x,
                (value) => { target.pivot =new Vector2(value,target.pivot.y) ; }, snap);
        }
        
        public static ITween<float> DOPivotY(this RectTransform target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.pivot.y, value, duration, () => target.pivot.y,
                (value) => { target.pivot =new Vector2(target.pivot.x,value) ; }, snap);
        }
        
        public static ITween<Vector2> DOSizeDelta(this RectTransform target, Vector2 value, float duration, bool snap = false)
        {

            return DoGoto(target.sizeDelta, value, duration, () => target.sizeDelta,
                (value) => { target.pivot =value ; }, snap);
        }
        
        //ScrollRect
        
        public static ITween<Vector2> DoNormalizedPos(this ScrollRect target, Vector2 value, float duration, bool snap = false)
        {

            return DoGoto(target.normalizedPosition, value, duration, () => target.normalizedPosition,
                (value) => { target.normalizedPosition =value ; }, snap);
        }
        public static ITween<float> DoHorizontalNormalizedPos(this ScrollRect target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.horizontalNormalizedPosition, value, duration, () => target.horizontalNormalizedPosition,
                (value) => { target.horizontalNormalizedPosition =value ; }, snap);
        }
        public static ITween<float> DoVerticalNormalizedPos(this ScrollRect target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.verticalNormalizedPosition, value, duration, () => target.verticalNormalizedPosition,
                (value) => { target.verticalNormalizedPosition =value ; }, snap);
        }
        
        //Slider
        public static ITween<float> DoValue(this Slider target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.value, value, duration, () => target.value,
                (value) => { target.value =value ; }, snap);
        }
        
        //text
        
        public static ITween<Color> DoValue(this Text target, Color value, float duration, bool snap = false)
        {

            return DoGoto(target.color, value, duration, () => target.color,
                (value) => { target.color =value ; }, snap);
        }
        
        public static ITween<float> DoFade(this Text target, float value, float duration, bool snap = false)
        {

            return DoGoto(target.color.a, value, duration, () => target.color.a,
                (value) => { target.color =new Color(target.color.r,target.color.g,target.color.b,value) ; }, snap);
        }
        
        
        
        
    }

}












