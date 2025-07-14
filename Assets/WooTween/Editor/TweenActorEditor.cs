/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static WooTween.EditorTools;
using static WooTween.TweenComponentContextActor;
namespace WooTween
{
    interface ITweenActorEditor
    {
        void OnInspectorGUI(TweenComponentActor actor);
        void OnSceneGUI(TweenComponentActor actor);
        float GetActorLength(TweenComponentActor actor);
    }

    public class TweenActorEditor<T> : ITweenActorEditor where T : TweenComponentActor
    {
        protected virtual void OnSceneGUI(T actor) { }
        protected void DrawBase(T actor)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            FieldDefaultInspector(actor.GetType().GetField("target"), actor);

            actor.id = EditorGUILayout.TextField("ID", actor.id);
            actor.duration = EditorGUILayout.FloatField("Duration", actor.duration);
            if (actor.duration < 0)
                actor.duration = 0.1f;
            //actor.snap = EditorGUILayout.Toggle("Snap", actor.snap);
            if (actor is TweenGroupComponentActor _group)
            {
                GUILayout.Space(5);
                _group.snap = EditorGUILayout.Toggle("Snap", _group.snap);

                _group.loops = EditorGUILayout.IntField("Loops", _group.loops);
                if (_group.loops < 0)
                    _group.loops = -1;

                GUILayout.Space(10);
            }
            else if (actor is TweenComponentContextActor _actor)
            {
                _actor.snap = EditorGUILayout.Toggle("Snap", _actor.snap);

                _actor.sourceDelta = EditorGUILayout.FloatField("Source Delta", _actor.sourceDelta);
                _actor.delay = EditorGUILayout.FloatField("Delay", _actor.delay);
                if (_actor.delay < 0)
                    _actor.delay = 0;
                GUILayout.Space(10);

                _actor.loopType = (LoopType)EditorGUILayout.EnumPopup(nameof(LoopType), _actor.loopType);
                _actor.loops = EditorGUILayout.IntField("Loops", _actor.loops);
                if (_actor.loops < 0)
                    _actor.loops = -1;

                GUILayout.Space(10);

                _actor.curveType = (CurveType)EditorGUILayout.EnumPopup(nameof(CurveType), _actor.curveType);
                if (_actor.curveType == CurveType.Ease)
                {
                    _actor.ease = (Ease)EditorGUILayout.EnumPopup(nameof(Ease), _actor.ease);
                }
                else
                {
                    AnimationCurve curve = _actor.curve;
                    if (curve == null)
                    {
                        curve = new AnimationCurve();
                    }
                    _actor.curve = EditorGUILayout.CurveField(nameof(AnimationCurve), curve);
                }
            }

            GUILayout.EndVertical();
        }
        protected void DrawSelf(T actor)
        {
            List<Type> types = new List<Type>();


            var _baseType = actor.GetType();
            while (true)
            {
                if (_baseType == typeof(TweenComponentActor))
                {
                    break;
                }
                if (_baseType.IsGenericType && _baseType.GetGenericTypeDefinition() == typeof(TweenGroupComponentActor<>))
                {
                    break;
                }

                if (_baseType.IsGenericType && _baseType.GetGenericTypeDefinition() == typeof(TweenComponentActor<,>))
                {
                    break;
                }
                types.Insert(0, _baseType);
                _baseType = _baseType.BaseType;
            }


            GUILayout.BeginVertical(EditorStyles.helpBox);

            foreach (var type in types)
            {
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldDefaultInspector(fields[i], actor);
                }
            }
            GUILayout.EndVertical();
        }

        protected virtual void OnInspectorGUI(T actor)
        {
            DrawBase(actor);
            GUILayout.Space(5);
            DrawSelf(actor);
        }

        void ITweenActorEditor.OnInspectorGUI(TweenComponentActor actor)
        {
            OnInspectorGUI(actor as T);
        }

        void ITweenActorEditor.OnSceneGUI(TweenComponentActor actor)
        {
            OnSceneGUI(actor as T);
        }

        float ITweenActorEditor.GetActorLength(TweenComponentActor actor)
        {


            if (actor is TweenGroupComponentActor _group)
            {
                if (_group.loops == -1)
                    return float.MaxValue;
                return _group.loops * _group.duration;
            }
            else if (actor is TweenComponentContextActor _actor)
            {
                if (_actor.loops == -1)
                    return float.MaxValue;
                return _actor.loops * _actor.duration + _actor.delay;
            }
            return actor.duration;
        }
    }
}
