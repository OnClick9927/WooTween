﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace WooTween
{
    [AddComponentMenu("IFramework/TweenComponent"), DisallowMultipleComponent]
    public class TweenComponent : MonoBehaviour
    {
        internal enum Mode
        {
            Sequence,
            Parallel
        }
        [SerializeField] internal Mode mode = Mode.Sequence;
        [SerializeField] private float timeScale = 1;
        [SerializeField] private int loops = 1;
        [SerializeField] private string id;

        [SerializeField] private bool PlayOnAwake;


        [SerializeReference]
        [HideInInspector]
        internal List<TweenComponentActor> actors = new List<TweenComponentActor>();

        [System.Serializable]
        public class TweenComponentEvent : UnityEvent<ITweenContext> { }
        [System.Serializable]
        public class TweenComponentTickEvent : UnityEvent<ITweenContext, float, float> { }

        [HideInInspector] public TweenComponentEvent onRewind = new TweenComponentEvent();

        [HideInInspector] public TweenComponentEvent onCancel = new TweenComponentEvent();
        [HideInInspector] public TweenComponentEvent onBegin = new TweenComponentEvent();
        [HideInInspector] public TweenComponentEvent onComplete = new TweenComponentEvent();
        [HideInInspector] public TweenComponentTickEvent onTick = new TweenComponentTickEvent();

        public T FindActor<T>() where T : TweenComponentActor
        {
            for (int i = 0; i < actors.Count; i++)
            {
                var actor = actors[i];
                if (actor is T)
                    return actor as T;
            }
            return null;
        }
        internal bool hasValue => context != null;
        public bool paused => !hasValue ? true : context.paused;
        private ITweenContext context;
        private void ResetActorsPercent()
        {
#if UNITY_EDITOR
            for (int i = 0; i < actors.Count; i++)
            {
                var actor = actors[i];
                actor.ResetPercent();
            }
#endif
        }
        private void Awake()
        {
            context = null;
            if (PlayOnAwake)
            {
                Play();
            }
        }
        private void RecyleContext()
        {
            context?.SetAutoCycle(true);
            context?.Cancel();
            context = null;
        }
        public void Play()
        {
            //ResetActorsPercent();
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                RecyleContext();
            }
#endif
            if (context == null)
            {
                ITweenGroup group = null;
                if (mode == Mode.Sequence)
                    context = group = Tween.Sequence();
                else
                    context = group = Tween.Parallel();



                for (int i = 0; i < actors.Count; i++)
                {

                    var actor = actors[i];
                    actor.transform = transform;
                    group.NewContext(actor.Create);
                }

#if UNITY_EDITOR
                if (!UnityEditor.EditorApplication.isPlaying)
                {
                    group.OnRewind((e) =>
                    {
                        ResetActorsPercent();
                    });
                }
#endif

                group.SetAutoCycle(false)
                     .SetTimeScale(timeScale)
                     .OnBegin(onBegin.Invoke)
                     .OnComplete(onComplete.Invoke)
                     .OnTick(onTick.Invoke)
                     .OnCancel(onCancel.Invoke)
                     .OnRewind(onRewind.Invoke)
                     .SetId(id).Run().SetLoops(loops);
            }
            else
            {
                ReStart();
            }
        }
        public void UnPause() => context?.UnPause();
        public void Pause() => context?.Pause();
        public void ReStart()
        {
            //ResetActorsPercent();
            context.SetTimeScale(timeScale);
            context?.ReStart();
        }

        public void Stop() => context?.Stop();
        public void Rewind() => context?.Rewind();

        public void Cancel()
        {
            context?.Cancel();
        }

        private void OnDisable()
        {
            RecyleContext();
        }


    }

}