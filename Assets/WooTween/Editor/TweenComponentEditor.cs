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
using System.Linq;
using UnityEditor;
using UnityEngine;
using static WooTween.EditorTools;
namespace WooTween
{

    [CustomEditor(typeof(TweenComponent))]
    class TweenComponentEditor : Editor
    {
        enum ActorType
        {
            Group,
            Context,
            None = 999999,
        }
        TweenComponent comp;
        private void OnEnable()
        {
            var editortypes = typeof(TweenActorEditor<>).GetSubTypesInAssemblies().Where(x => !x.IsGenericType && !x.IsAbstract).ToList();


            comp = target as TweenComponent;
            var types = typeof(TweenComponentActor).GetSubTypesInAssemblies()
           .Where(x => !x.IsAbstract).ToList();
            Dictionary<string, int> map = new Dictionary<string, int>();
            foreach (var type in types)
            {
                var _baseType = type;
                ActorType case_index = ActorType.None;
                while (true)
                {
                    if (_baseType == typeof(TweenComponentActor))
                    {
                        case_index = ActorType.None;
                        break;
                    }
                    if (_baseType.IsGenericType && _baseType.GetGenericTypeDefinition() == typeof(TweenGroupComponentActor<>))
                    {
                        case_index = ActorType.Group;
                        break;
                    }
                    if (_baseType.IsGenericType && _baseType.GetGenericTypeDefinition() == typeof(TweenComponentActor<,>))
                    {
                        case_index = ActorType.Context;
                        break;
                    }
                    else if (_baseType == typeof(object)) break;
                    _baseType = _baseType.BaseType;
                }
                string target = "Common";
                if (case_index != ActorType.None)
                {

                    var arg_index = (int)case_index;


                    var args = _baseType.GetGenericArguments();
                    target = args[arg_index].Name;
                }

                options.Add($"{target}/{type.Name.Replace("Actor", "")}");
                if (!map.ContainsKey(target))
                    map[target] = 0;
                map[target]++;
                options_type.Add(type);


                var find_editor = editortypes.Find(x =>
                {
                    var _type = x;
                    while (true)
                    {
                        if (_type.IsGenericType && _type.GetGenericTypeDefinition() == typeof(TweenActorEditor<>)
                        && _type.GetGenericArguments()[0] == type)
                        {
                            return true;
                        }
                        _type = _type.BaseType;
                        if (_type == null || _type == typeof(object))
                        {
                            return false;
                        }
                    }
                });

                if (find_editor != null)
                    editor_actor.Add(type, Activator.CreateInstance(find_editor) as ITweenActorEditor);
                else
                    editor_actor.Add(type, Activator.CreateInstance(typeof(TweenActorEditor<>).MakeGenericType(type)) as ITweenActorEditor);
                //}
                //else
                //{
                //    Console.WriteLine();
                //}
            }
            int count = map.Count;

            foreach (var item in map.Values)
            {
                count = Mathf.Max(count, item);
            }
            max_count = count;
        }
        int max_count;
        List<string> options = new List<string>();
        List<Type> options_type = new List<Type>();
        Dictionary<Type, ITweenActorEditor> editor_actor = new Dictionary<Type, ITweenActorEditor>();

        private void Tools()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(nameof(TweenComponent.Play)))
            {
                comp.Play();
            }
            using (new EditorGUI.DisabledGroupScope(!comp.hasValue))
            {
                GUILayout.Space(10);
                if (comp.paused)
                {
                    if (GUILayout.Button(nameof(TweenComponent.UnPause)))
                        comp.UnPause();
                }
                else
                {
                    if (GUILayout.Button(nameof(TweenComponent.Pause)))
                        comp.Pause();
                }
                GUILayout.Space(10);

                if (GUILayout.Button(nameof(TweenComponent.Stop)))
                    comp.Stop();

                if (GUILayout.Button(nameof(TweenComponent.Rewind)))
                    comp.Rewind();

            }

            GUILayout.EndHorizontal();
        }

        List<float> lens = new List<float>();

        public override void OnInspectorGUI()
        {
            GUI.enabled = !EditorApplication.isPlaying;
            base.OnInspectorGUI();
            var _style = new GUIStyle(EditorStyles.miniPullDown)
            {
                fixedHeight = 25
            };

            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(25));
            var index = EditorTools.AdvancedPopup(rect, -1, options.ToArray(), Mathf.Min(max_count * 18 + 40, 400), _style);
            EditorGUI.LabelField(RectEx.Zoom(rect,
                TextAnchor.MiddleCenter, new Vector2(0, -5)),
                new GUIContent("Actors", EditorGUIUtility.TrIconContent("d_Toolbar Plus").image), EditorStyles.boldLabel);
            if (index != -1)
            {
                var type = options_type[index];
                comp.actors.Add(Activator.CreateInstance(type) as TweenComponentActor);
            }
            lens.Clear();
            string actor_name = "";
            for (int i = 0; i < comp.actors.Count; i++)
            {
                var actor = comp.actors[i];
                var length = editor_actor[actor.GetType()].GetActorLength(actor);

                lens.Add(length);
                var _name = actor.GetType().Name;
                if (_name.Length > actor_name.Length)
                    actor_name = _name;
            }
            var size = EditorStyles.foldout.CalcSize(new GUIContent(actor_name));

            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < comp.actors.Count; i++)
            {
                var actor = comp.actors[i];
                var mode = DrawActor(actor, i, size);
                switch (mode)
                {
                    case Mode.Remove:
                        comp.actors.RemoveAt(i);
                        break;
                    case Mode.MoveDown:
                        {
                            if (i != comp.actors.Count - 1)
                            {
                                comp.actors[i] = comp.actors[i + 1];
                                comp.actors[i + 1] = actor;
                            }
                        }
                        break;
                    case Mode.MoveUp:
                        {
                            if (i != 0)
                            {
                                comp.actors[i] = comp.actors[i - 1];
                                comp.actors[i - 1] = actor;
                            }
                        }
                        break;

                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(comp);
                //if (comp.hasValue && !comp.paused)
            }
            Repaint();
            GUILayout.Space(10);

            var _fold = GetFoldout(this);
            if (EditorGUILayout.DropdownButton(new GUIContent("Events"), FocusType.Passive, _style))
            {
                _fold = !_fold;
                SetFoldout(this, _fold);
            }
            if (_fold)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(comp.onBegin)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(comp.onCancel)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(comp.onRewind)));

                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(comp.onComplete)));
                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(comp.onTick)));

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(comp);
                    serializedObject.ApplyModifiedProperties();
                }
            }
            GUILayout.Space(10);

            Tools();
        }

        private enum Mode
        {
            Remove, MoveDown, MoveUp, None
        }
        private Mode DrawActor(TweenComponentActor actor, int index, Vector2 size)
        {
            var length = lens[index];
            var start = 0f;
            var total_length = 0f;
            if (comp.mode == TweenComponent.Mode.Sequence)
            {
                for (var i = 0; i < index; i++)
                {
                    start += lens[i];
                }
                total_length = start;
                for (var i = index; i < lens.Count; i++)
                {
                    total_length += lens[i];
                }
            }
            else
            {
                start = 0;
                for (var i = 0; i < lens.Count; i++)
                {
                    total_length = Mathf.Max(total_length, lens[i]);
                }
            }
            var end = start + length;

            Mode mode = Mode.None;
            EditorGUILayout.LabelField("", GUI.skin.textField, GUILayout.Height(25));
            var rect = EditorTools.RectEx.Zoom(GUILayoutUtility.GetLastRect(),
                TextAnchor.MiddleRight, new Vector2(-20, 0));
            var rs = EditorTools.RectEx.VerticalSplit(rect, rect.width - 80, 4);



            EditorGUI.ProgressBar(rs[0], actor.percent, "");
            {
                var last_rect = rs[0];
                last_rect.x += size.x;
                last_rect.width -= size.x;
                //EditorGUI.MinMaxSlider(last_rect, ref start, ref end, 0, total_length);

                var width = last_rect.width;
                var x_min = width * (start / total_length) + last_rect.x;

                var x_max = width * (end / total_length) + last_rect.x;
                var _rect = new Rect(x_min, last_rect.y, x_max - x_min, last_rect.height);
                GUI.Box(_rect, string.Empty, EditorStyles.selectionRect);

                float num_width = 100;
                var __rect = new Rect(_rect.x, _rect.y, num_width, 18);
                //__rect.center = new Vector2(_rect.x, _rect.y);
                GUI.Label(_rect, $"{start}~{end}", new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                });
                //__rect = new Rect(_rect.xMax - num_width, _rect.y, num_width, 18);
                ////__rect.center = new Vector2(_rect.x, _rect.y);
                //GUI.Label(__rect, end.ToString("0.00"), new GUIStyle(EditorStyles.boldLabel)
                //{
                //    alignment = TextAnchor.MiddleRight
                //});

            }

            var fold = EditorGUI.Foldout(rs[0], GetFoldout(actor), $"{actor.GetType().Name}", true);
            SetFoldout(actor, fold);
            //GUI.Box(last_rect, "");
            var rss = RectEx.VerticalSplit(rs[1], rect.height, 0);
            if (GUI.Button(rss[0], EditorGUIUtility.TrIconContent("d_Toolbar Minus")))
                mode = Mode.Remove;
            rss = RectEx.VerticalSplit(rss[1], rect.height, 0);
            using (new EditorGUI.DisabledGroupScope(index == 0))
                if (GUI.Button(rss[0], EditorGUIUtility.TrIconContent("d_scrollup")))
                    mode = Mode.MoveUp;
            rss = RectEx.VerticalSplit(rss[1], rect.height, 0);
            using (new EditorGUI.DisabledGroupScope(index == comp.actors.Count - 1))
                if (GUI.Button(rss[0], EditorGUIUtility.TrIconContent("d_scrolldown")))
                    mode = Mode.MoveDown;





            GUILayout.BeginHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginVertical();
            if (mode == Mode.None && fold)
            {
                editor_actor[actor.GetType()].OnInspectorGUI(actor);

            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            return mode;
        }


        private void OnSceneGUI()
        {
            for (int i = 0; i < comp.actors.Count; i++)
            {
                var actor = comp.actors[i];
                if (GetFoldout(actor))
                    editor_actor[actor.GetType()].OnSceneGUI(actor);

            }
        }





    }
}
