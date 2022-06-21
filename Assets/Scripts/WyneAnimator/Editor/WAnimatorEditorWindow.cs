using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;
using DG.Tweening;

namespace WyneAnimator
{
    public class WAnimatorEditorWindow : EditorWindow
    {
        private WAnimator _animator;
        private SerializedObject _animatorSerializedObject;

        private SerializedProperty _WAnimations;

        private Vector2 _scrollPos = Vector2.zero;

        private Texture2D _WAnimationTexture;
        private Color _WAnimationColor = new Color(56f / 255f, 56f / 255f, 56f / 255f);
        private GUIStyle _WAnimationStyle = new GUIStyle();

        private GUIStyle _headerTextStyle = new GUIStyle();

        public static void Open(WAnimator animator)
        {
            WAnimatorEditorWindow window = GetWindow<WAnimatorEditorWindow>("WAnimations Editor");
            window._animator = animator;
            window._animatorSerializedObject = new SerializedObject(animator);
            window._WAnimations = window._animatorSerializedObject.FindProperty("Animations");
            window.InitializeTextures();
            window.InitializeStyles();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));

            DrawAnimationInspector();

            EditorGUILayout.EndVertical();
        }

        private void OnDestroy()
        {
            _animatorSerializedObject.ApplyModifiedProperties();

            foreach (WAnimation animation in _animator.Animations)
            {
                animation.Loaded = false;
            }
        }

        private void DrawAnimationInspector()
        {
            EditorGUILayout.BeginHorizontal();
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            if (_animator.Animations.Length > 0)
            {
                int i = 0;

                foreach (SerializedProperty WAnimation in _WAnimations)
                {
                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.BeginHorizontal();

                    string foldoutName = WAnimation.displayName + " " + _animator.Animations[i].AnimationComponent;
                    WAnimation.isExpanded = EditorGUILayout.Foldout(WAnimation.isExpanded, foldoutName, true);

                    if (GUILayout.Button("-", GUILayout.Width(25f)))
                    {
                        List<WAnimation> newWAnimations = _animator.Animations.ToList();
                        newWAnimations.RemoveAt(i);
                        WAnimation[] newWAnimationsArray = newWAnimations.ToArray();
                        _animator.Animations = newWAnimationsArray;
                        _animatorSerializedObject.Update();
                        EditorGUILayout.EndHorizontal();
                        break;
                    }

                    EditorGUILayout.EndHorizontal();

                    if (WAnimation.isExpanded)
                    {
                        EditorGUILayout.Space(10);

                        _animator.Animations[i].AnimationConditionType = (WAnimationConditionType)EditorGUILayout.EnumPopup("Condition", _animator.Animations[i].AnimationConditionType);
                        _animator.Animations[i].AnimationConditionObject = (GameObject)EditorGUILayout.ObjectField("Condition Object", _animator.Animations[i].AnimationConditionObject, typeof(GameObject), true);

                        _animator.Animations[i].AnimationComponent = (Component)EditorGUILayout.ObjectField("Animated Component", _animator.Animations[i].AnimationComponent, typeof(Component), true);
                        _animator.Animations[i].Initialize();
                        _animator.Animations[i].Load();

                        EditorGUILayout.Space(10);

                        if (_animator.Animations[i].AnimationComponent != null)
                        {
                            foreach (ValueInfo value in _animator.Animations[i].ValuesTweens.Keys)
                            {
                                _animator.Animations[i].ValuesTweens[value].IsExpanded = EditorGUILayout.Foldout(_animator.Animations[i].ValuesTweens[value].IsExpanded, value.Name, true);

                                if (_animator.Animations[i].ValuesTweens[value].IsExpanded)
                                {
                                    GUILayout.BeginVertical(_WAnimationStyle);

                                    GUILayout.BeginHorizontal();
                                    _animator.Animations[i].ValuesTweens[value].EndValue = VisualizeObject(_animator.Animations[i].ValuesTweens[value].EndValue, "To");
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    _animator.Animations[i].ValuesTweens[value].IgnoreTimeScale = EditorGUILayout.Toggle("Ignore Time Scale", _animator.Animations[i].ValuesTweens[value].IgnoreTimeScale);
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    _animator.Animations[i].ValuesTweens[value].Duration = EditorGUILayout.FloatField("Duration", _animator.Animations[i].ValuesTweens[value].Duration);
                                    _animator.Animations[i].ValuesTweens[value].Delay = EditorGUILayout.FloatField("Delay", _animator.Animations[i].ValuesTweens[value].Delay);
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    _animator.Animations[i].ValuesTweens[value].Loops = EditorGUILayout.IntField("Loops", _animator.Animations[i].ValuesTweens[value].Loops);
                                    _animator.Animations[i].ValuesTweens[value].LoopType = (LoopType)EditorGUILayout.EnumPopup("Loop Type", _animator.Animations[i].ValuesTweens[value].LoopType);
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    _animator.Animations[i].ValuesTweens[value].Ease = (Ease)EditorGUILayout.EnumPopup("Ease", _animator.Animations[i].ValuesTweens[value].Ease);
                                    GUILayout.EndHorizontal();

                                    GUILayout.EndVertical();
                                }

                                EditorGUILayout.Space(10);
                            }
                        }
                    }

                    _animator.Animations[i].Serialize();
                    i++;
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(10);
                }
            }

            if (GUILayout.Button("Add Animation"))
            {
                List<WAnimation> newWAnimations = _animator.Animations.ToList();
                newWAnimations.Add(new WAnimation());
                WAnimation[] newWAnimationsArray = newWAnimations.ToArray();
                _animator.Animations = newWAnimationsArray;
                _animatorSerializedObject.Update();
            }

            _animatorSerializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndHorizontal();
        }

        private object VisualizeObject(object obj, string label)
        {
            if (obj.GetType() == typeof(int) || obj.GetType() == typeof(long))
            {
                return EditorGUILayout.IntField(label, (int)obj);
            }
            else if (obj.GetType() == typeof(float) || obj.GetType() == typeof(double))
            {
                return EditorGUILayout.FloatField(label, (float)obj);
            }
            else if (obj.GetType() == typeof(Color))
            {
                return EditorGUILayout.ColorField(label, (Color)obj);
            }
            else if (obj.GetType() == typeof(Vector2))
            {
                return EditorGUILayout.Vector2Field(label, (Vector2)obj);
            }
            else if (obj.GetType() == typeof(Vector3))
            {
                return EditorGUILayout.Vector3Field(label, (Vector3)obj);
            }
            else if (obj.GetType() == typeof(bool))
            {
                return EditorGUILayout.Toggle(label, (bool)obj);
            }
            else
            {
                return obj;
            }
        }

        private void InitializeStyles()
        {
            _WAnimationStyle.normal.background = _WAnimationTexture;

            _headerTextStyle.fontStyle = FontStyle.Bold;
            _headerTextStyle.normal.textColor = new Color(193f / 255f, 193f / 255f, 193f / 255f);
            _headerTextStyle.alignment = TextAnchor.MiddleCenter;
            _headerTextStyle.fontSize = 22;
        }

        private void InitializeTextures()
        {
            _WAnimationTexture = new Texture2D(1, 1);
            _WAnimationTexture.SetPixel(0, 0, _WAnimationColor);
            _WAnimationTexture.Apply();
        }

    }
}


