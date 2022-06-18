using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WyneAnimator
{
    [CustomEditor(typeof(WAnimator))]
    public class WAnimatorCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Editor"))
            {
                WAnimatorEditorWindow.Open((WAnimator)target);
            }
        }
    }
}

