using UnityEditor;
using UnityEngine;
namespace WooTween
{
        [CustomEditor(typeof(DoRotationComponent))]
        public class DoRotationComponentEditor : TweenComponentEditor<Vector3, Transform> { }
}