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

        private SerializedProperty[] _WAnimations;
        private SerializedProperty _selectedWAnimations;

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
            window._WAnimations = new SerializedProperty[]
            {
                window._animatorSerializedObject.FindProperty("OnStartAnimations"),
            };

            window.InitializeTextures();
            window.InitializeStyles();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));

            DrawAnimationSelection();

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));

            if (_selectedWAnimations != null)
            {
                DrawAnimationInspector();
            }
            else
            {
                GUILayout.Label("Select animation to edit!");
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void OnDestroy()
        {
            _animatorSerializedObject.ApplyModifiedProperties();

            if (_selectedWAnimations != null)
            {
                FieldInfo animationArrayInfo = typeof(WAnimator).GetField(_selectedWAnimations.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                WAnimation[] animationArray = (WAnimation[])animationArrayInfo.GetValue(_animator);

                foreach (WAnimation animation in animationArray)
                {
                    animation.Loaded = false;
                }
            }
        }

        private void DrawAnimationSelection()
        {
            foreach (SerializedProperty WAnimationArray in _WAnimations)
            {
                if (GUILayout.Button(WAnimationArray.displayName))
                {
                    _selectedWAnimations = _animatorSerializedObject.FindProperty(WAnimationArray.name);
                }
            }
        }

        private void DrawAnimationInspector()
        {
            // Selected animations header
            GUILayout.Label(_selectedWAnimations.displayName, _headerTextStyle);

            EditorGUILayout.Separator();

            // Scroll view
            EditorGUILayout.BeginHorizontal();
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            // Selected animations array
            FieldInfo animationArrayInfo = typeof(WAnimator).GetField(_selectedWAnimations.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            WAnimation[] animationArray = (WAnimation[])animationArrayInfo.GetValue(_animator);

            if (_selectedWAnimations.arraySize > 0)
            {
                int i = 0;
                // Foreach WAnimation in selected animations array
                foreach (SerializedProperty WAnimationProperty in _selectedWAnimations)
                {
                    // Create nice border
                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.BeginHorizontal();

                    // Create animation foldout
                    string foldoutName = WAnimationProperty.displayName + " " + animationArray[i].AnimationComponent;
                    WAnimationProperty.isExpanded = EditorGUILayout.Foldout(WAnimationProperty.isExpanded, foldoutName, true);

                    // Delete animation button
                    if (GUILayout.Button("-", GUILayout.Width(25f)))
                    {
                        List<WAnimation> newAnimationList = animationArray.ToList();
                        newAnimationList.RemoveAt(i);
                        WAnimation[] newAnimationArray = newAnimationList.ToArray();
                        animationArrayInfo.SetValue(_animator, newAnimationArray);
                        _animatorSerializedObject.Update();
                        _selectedWAnimations = _animatorSerializedObject.FindProperty(_selectedWAnimations.name);
                        EditorGUILayout.EndHorizontal();
                        break;
                    }

                    EditorGUILayout.EndHorizontal();

                    // If animation is expanded
                    if (WAnimationProperty.isExpanded)
                    {
                        EditorGUILayout.Space(10);

                        // Set animation component of expanded animation
                        animationArray[i].AnimationComponent = (Component)EditorGUILayout.ObjectField(animationArray[i].AnimationComponent, typeof(Component), true);
                        animationArray[i].Initialize();
                        animationArray[i].Load();

                        EditorGUILayout.Space(10);

                        if (animationArray[i].AnimationComponent != null)
                        {
                            foreach (ValueInfo value in animationArray[i].ValuesTweens.Keys)
                            {
                                animationArray[i].ValuesTweens[value].IsExpanded = EditorGUILayout.Foldout(animationArray[i].ValuesTweens[value].IsExpanded, value.Name, true);

                                if (animationArray[i].ValuesTweens[value].IsExpanded)
                                {
                                    GUILayout.BeginVertical(_WAnimationStyle);

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ValuesTweens[value].EndValue = VisualizeObject(animationArray[i].ValuesTweens[value].EndValue, "To");
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ValuesTweens[value].IgnoreTimeScale = EditorGUILayout.Toggle("Ignore Time Scale", animationArray[i].ValuesTweens[value].IgnoreTimeScale);
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ValuesTweens[value].Duration = EditorGUILayout.FloatField("Duration", animationArray[i].ValuesTweens[value].Duration);
                                    animationArray[i].ValuesTweens[value].Delay = EditorGUILayout.FloatField("Delay", animationArray[i].ValuesTweens[value].Delay);
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ValuesTweens[value].Loops = EditorGUILayout.IntField("Loops", animationArray[i].ValuesTweens[value].Loops);
                                    animationArray[i].ValuesTweens[value].LoopType = (LoopType)EditorGUILayout.EnumPopup("Loop Type", animationArray[i].ValuesTweens[value].LoopType);
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ValuesTweens[value].Ease = (Ease)EditorGUILayout.EnumPopup("Ease", animationArray[i].ValuesTweens[value].Ease);
                                    GUILayout.EndHorizontal();

                                    GUILayout.EndVertical();
                                }

                                EditorGUILayout.Space(10);
                            }
                        }
                    }

                    animationArray[i].Serialize();
                    i++;
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(10);
                }
            }

            if (GUILayout.Button("Add Animation"))
            {
                List<WAnimation> newAnimationList = animationArray.ToList();
                newAnimationList.Add(new WAnimation());
                WAnimation[] newAnimationArray = newAnimationList.ToArray();
                animationArrayInfo.SetValue(_animator, newAnimationArray);
                _animatorSerializedObject.Update();
                _selectedWAnimations = _animatorSerializedObject.FindProperty(_selectedWAnimations.name);
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
            else if (obj.GetType() == typeof(Vector4))
            {
                return EditorGUILayout.Vector3Field(label, (Vector4)obj);
            }
            else if (obj.GetType() == typeof(Quaternion))
            {
                return EditorGUILayout.Vector3Field(label, ((Quaternion)obj).eulerAngles);
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


