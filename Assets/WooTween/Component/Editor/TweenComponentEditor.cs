
using UnityEditor;
using UnityEngine;
namespace WooTween
{
    public class TweenComponentEditor<T, Target> : Editor where T : struct where Target : Object
    {
        public virtual bool drawTargets { get { return false; } }
        public TweenComponent<T, Target> component { get { return this.target as TweenComponent<T, Target>; } }
        public override void OnInspectorGUI()
        {
            GUILayout.Space(5);
            using (new EditorGUI.DisabledGroupScope(Application.isPlaying))
            {
                EditorGUI.BeginChangeCheck();
                GUILayout.BeginHorizontal();
                {
                    component.autoPlay = GUILayout.Toggle(component.autoPlay, "autoPlay", "toolbarButton");
                    component.autoRecyle = GUILayout.Toggle(component.autoRecyle, "autoRecycle", "toolbarButton");
                    GUILayout.EndHorizontal();
                }
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("duration"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("snap"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("curve"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("LoopType"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("loop"));
                if (drawTargets)
                {
                    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("targets"), true);
                }
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("type"));
                switch (component.type)
                {
                    case global::WooTween.TweenComponent<T, Target>.TweenType.Single:
                        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("start"));
                        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("end"));

                        break;
                    case global::WooTween.TweenComponent<T, Target>.TweenType.Array:
                        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("array"), true);

                        break;
                    default:
                        break;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    this.serializedObject.ApplyModifiedProperties();
                }
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Play"))
                    {
                        component.Play();
                    }
                    if (GUILayout.Button("Rewind"))
                    {
                        component.Rewind(1);
                    }
                    if (GUILayout.Button("Complete"))
                    {
                        component.Complete(false);
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
        private void OnDestroy()
        {
            component.Complete(false);
        }
    }
}
