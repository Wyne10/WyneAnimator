using UnityEngine;
using UnityEditor;

namespace WS.WyneAnimator
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

