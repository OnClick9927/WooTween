using UnityEditor;
using UnityEngine;
namespace WooTween
{
        [CustomEditor(typeof(DoPositionComponent))]
        public class DoPositionComponentEditor : TweenComponentEditor<Vector3, Transform> { }
}