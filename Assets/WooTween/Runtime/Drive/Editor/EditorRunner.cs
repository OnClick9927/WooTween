using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace WooTween
{
    public class EditorRunner
    {
        [InitializeOnLoadMethod]
        static void Do()
        {
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            var container = TweenDrive.GetDrive(EnvironmentType.Editor);
            container.Update();
        }
    }
}

