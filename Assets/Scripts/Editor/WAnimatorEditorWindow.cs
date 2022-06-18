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

        // Array of arrays of all the animations
        private SerializedProperty[] _WAnimationsProperties;

        // Selected array of animations
        private SerializedProperty _selectedWAnimationArray;

        private Vector2 _scrollPos = Vector2.zero;

        private Texture2D _animationPropertyTexture;
        private Color _animationPropertyColor = new Color(56f / 255f, 56f / 255f, 56f / 255f);
        private GUIStyle _animationPropertyStyle = new GUIStyle();

        private GUIStyle _headerTextStyle = new GUIStyle();

        public static void Open(WAnimator animator)
        {
            WAnimatorEditorWindow window = GetWindow<WAnimatorEditorWindow>("WAnimations Editor");
            window._animator = animator;
            window._animatorSerializedObject = new SerializedObject(animator);
            window._WAnimationsProperties = new SerializedProperty[]
            {
                window._animatorSerializedObject.FindProperty("OnEnableAnimations"),
                window._animatorSerializedObject.FindProperty("OnAwakeAnimations"),
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

            if (_selectedWAnimationArray != null)
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

            FieldInfo animationArrayInfo = typeof(WAnimator).GetField(_selectedWAnimationArray.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            WAnimation[] animationArray = (WAnimation[])animationArrayInfo.GetValue(_animator);

            foreach (WAnimation animation in animationArray)
            {
                animation.Loaded = false;
            }
        }

        private void DrawAnimationSelection()
        {
            foreach (SerializedProperty WAnimationArray in _WAnimationsProperties)
            {
                if (GUILayout.Button(WAnimationArray.displayName))
                {
                    _selectedWAnimationArray = _animatorSerializedObject.FindProperty(WAnimationArray.name);
                }
            }
        }

        private void DrawAnimationInspector()
        {
            // Selected animations header
            GUILayout.Label(_selectedWAnimationArray.displayName, _headerTextStyle);

            EditorGUILayout.Separator();

            // Scroll view
            EditorGUILayout.BeginHorizontal();
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            // Selected animations array
            FieldInfo animationArrayInfo = typeof(WAnimator).GetField(_selectedWAnimationArray.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            WAnimation[] animationArray = (WAnimation[])animationArrayInfo.GetValue(_animator);

            if (_selectedWAnimationArray.arraySize > 0)
            {
                int i = 0;
                // Foreach WAnimation in selected animations array
                foreach (SerializedProperty WAnimationProperty in _selectedWAnimationArray)
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
                        _selectedWAnimationArray = _animatorSerializedObject.FindProperty(_selectedWAnimationArray.name);
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
                            // Foreach field in selected component
                            foreach (FieldInfo field in animationArray[i].ComponentFields)
                            {
                                // If field can be handled by animator
                                if (!field.CheckReflectedValue()) continue;

                                // Create foldout for animation properties of field
                                animationArray[i].ComponentFieldsTweens[field].IsExpanded = EditorGUILayout.Foldout(animationArray[i].ComponentFieldsTweens[field].IsExpanded, field.Name, true);

                                // Animation properties of field
                                if (animationArray[i].ComponentFieldsTweens[field].IsExpanded)
                                {
                                    GUILayout.BeginVertical(_animationPropertyStyle);

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ComponentFieldsTweens[field].SetNewAnimationValue(VisualizeObject(animationArray[i].ComponentFieldsTweens[field].NewValue, "To"));
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ComponentFieldsTweens[field].IgnoreTimeScale = EditorGUILayout.Toggle("Ignore Time Scale", animationArray[i].ComponentFieldsTweens[field].IgnoreTimeScale);
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ComponentFieldsTweens[field].Duration = EditorGUILayout.FloatField("Duration", animationArray[i].ComponentFieldsTweens[field].Duration);
                                    animationArray[i].ComponentFieldsTweens[field].Delay = EditorGUILayout.FloatField("Delay", animationArray[i].ComponentFieldsTweens[field].Delay);
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ComponentFieldsTweens[field].Loops = EditorGUILayout.IntField("Loops", animationArray[i].ComponentFieldsTweens[field].Loops);
                                    animationArray[i].ComponentFieldsTweens[field].LoopType = (LoopType)EditorGUILayout.EnumPopup("Loop Type", animationArray[i].ComponentFieldsTweens[field].LoopType);
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ComponentFieldsTweens[field].Ease = (Ease)EditorGUILayout.EnumPopup("Ease", animationArray[i].ComponentFieldsTweens[field].Ease);
                                    GUILayout.EndHorizontal();

                                    GUILayout.EndVertical();
                                }

                                EditorGUILayout.Space(10);
                            }

                            // Foreach property in selected component
                            foreach (PropertyInfo property in animationArray[i].ComponentProperties)
                            {
                                // If property can be handled by animator
                                if (!property.CheckReflectedValue()) continue;

                                // Create foldout for animation properties of field
                                animationArray[i].ComponentPropertiesTweens[property].IsExpanded = EditorGUILayout.Foldout(animationArray[i].ComponentPropertiesTweens[property].IsExpanded, property.Name, true);

                                // Animation properties of field
                                if (animationArray[i].ComponentPropertiesTweens[property].IsExpanded)
                                {
                                    GUILayout.BeginVertical(_animationPropertyStyle);

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ComponentPropertiesTweens[property].SetNewAnimationValue(VisualizeObject(animationArray[i].ComponentPropertiesTweens[property].NewValue, "To"));
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ComponentPropertiesTweens[property].IgnoreTimeScale = EditorGUILayout.Toggle("Ignore Time Scale", animationArray[i].ComponentPropertiesTweens[property].IgnoreTimeScale);
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ComponentPropertiesTweens[property].Duration = EditorGUILayout.FloatField("Duration", animationArray[i].ComponentPropertiesTweens[property].Duration);
                                    animationArray[i].ComponentPropertiesTweens[property].Delay = EditorGUILayout.FloatField("Delay", animationArray[i].ComponentPropertiesTweens[property].Delay);
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ComponentPropertiesTweens[property].Loops = EditorGUILayout.IntField("Loops", animationArray[i].ComponentPropertiesTweens[property].Loops);
                                    animationArray[i].ComponentPropertiesTweens[property].LoopType = (LoopType)EditorGUILayout.EnumPopup("Loop Type", animationArray[i].ComponentPropertiesTweens[property].LoopType);
                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();
                                    animationArray[i].ComponentPropertiesTweens[property].Ease = (Ease)EditorGUILayout.EnumPopup("Ease", animationArray[i].ComponentPropertiesTweens[property].Ease);
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
                _selectedWAnimationArray = _animatorSerializedObject.FindProperty(_selectedWAnimationArray.name);
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
            _animationPropertyStyle.normal.background = _animationPropertyTexture;

            _headerTextStyle.fontStyle = FontStyle.Bold;
            _headerTextStyle.normal.textColor = new Color(193f / 255f, 193f / 255f, 193f / 255f);
            _headerTextStyle.alignment = TextAnchor.MiddleCenter;
            _headerTextStyle.fontSize = 22;
        }

        private void InitializeTextures()
        {
            _animationPropertyTexture = new Texture2D(1, 1);
            _animationPropertyTexture.SetPixel(0, 0, _animationPropertyColor);
            _animationPropertyTexture.Apply();
        }

    }
}


