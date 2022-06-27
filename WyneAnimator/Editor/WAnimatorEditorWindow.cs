using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WS.WyneAnimator
{
    public class WAnimatorEditorWindow : EditorWindow
    {
        private WAnimator _animator;

        // EditorWindow stuff
        private SerializedObject _animatorSerializedObject;
        private SerializedProperty _WAnimations;
        private Vector2 _scrollPos = Vector2.zero;
        // EditorWidnow stuff

        // Colors
        private Color _WTweenColor = new Color(56f / 255f, 56f / 255f, 56f / 255f);
        private Color _blueWTweenColor = new Color(58f / 255f, 154f / 255f, 200f / 255f, 155f / 255f);
        private Color _redWAnimationColor = new Color(255f / 255f, 0f / 255f, 0f / 255f, 155f / 255f);
        // Colors

        // Textures
        private Texture2D _WTweenTexture;
        private Texture2D _blueWTweenTexture;
        private Texture2D _redWAnimationTexture;
        // Textures

        // Styles
        private GUIStyle _WTweenStyle = new GUIStyle();
        private GUIStyle _blueWTweenStyle = new GUIStyle(EditorStyles.foldout);
        private GUIStyle _redWAnimationStyle = new GUIStyle(EditorStyles.foldout);
        private GUIStyle _headerTextStyle = new GUIStyle();
        // Styles

        public static void Open(WAnimator animator)
        {
            WAnimatorEditorWindow window = GetWindow<WAnimatorEditorWindow>("WAnimations Editor");
            window._animator = animator;
            window._animatorSerializedObject = new SerializedObject(animator);
            window._WAnimations = window._animatorSerializedObject.FindProperty("Animations");

            if (window._animator.Animations == null)
                window._animator.Animations = new WAnimation[0];

            window.InitializeTextures();
            window.InitializeStyles();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }


        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));

            DrawAnimationInspector();

            EditorGUILayout.EndVertical();
        }

        private void Update()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.LogWarning("WARNING! Please close WAnimations Editor window before entering playmode, this may cause errors!");
                this.Close();
            }
        }

        private void OnDestroy()
        {
            foreach (WAnimation animation in _animator.Animations)
            {
                animation.Loaded = false;
            }

            if (PrefabUtility.IsPartOfPrefabInstance(_animator.gameObject))
                PrefabUtility.ApplyPrefabInstance(_animator.gameObject, InteractionMode.UserAction);
            _animatorSerializedObject.ApplyModifiedProperties();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
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
                    // Draw animation foldout

                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.BeginHorizontal();

                    string foldoutName = WAnimation.displayName + " " + _animator.Animations[i].AnimationComponent;

                    if (_animator.Animations[i].AnimationConditionType != WAnimationConditionType.OnTrigger && _animator.Animations[i].AnimationConditionObject == null)
                    {
                        WAnimation.isExpanded = EditorGUILayout.Foldout(WAnimation.isExpanded, foldoutName, true, _redWAnimationStyle);
                    }
                    else if (_animator.Animations[i].AnimationComponent == null)
                    {
                        WAnimation.isExpanded = EditorGUILayout.Foldout(WAnimation.isExpanded, foldoutName, true, _redWAnimationStyle);
                    }
                    else
                    {
                        WAnimation.isExpanded = EditorGUILayout.Foldout(WAnimation.isExpanded, foldoutName, true);
                    }

                    if (GUILayout.Button("-", GUILayout.Width(25f)))
                    {
                        List<WAnimation> newWAnimations = _animator.Animations.ToList();
                        newWAnimations.RemoveAt(i);
                        _animator.Animations = newWAnimations.ToArray();
                        _animatorSerializedObject.Update();
                        EditorGUILayout.EndHorizontal();
                        break;
                    }

                    if (GUILayout.Button("Reset values", GUILayout.Width(100f)))
                    {
                        _animator.Animations[i].Initialize(true);
                    }

                    EditorGUILayout.EndHorizontal();

                    if (WAnimation.isExpanded)
                    {
                        // Draw animation tweens

                        EditorGUILayout.Space(10);

                        _animator.Animations[i].AnimationConditionType = (WAnimationConditionType)EditorGUILayout.EnumPopup("Condition", _animator.Animations[i].AnimationConditionType);

                        // Change Condition object/Trigger name
                        if (_animator.Animations[i].AnimationConditionType == WAnimationConditionType.OnTrigger)
                        {
                            _animator.Animations[i].TriggerName = EditorGUILayout.TextField("Trigger Name", _animator.Animations[i].TriggerName);
                            _animator.Animations[i].AnimationConditionObject = null;
                        }
                        else
                        {
                            _animator.Animations[i].AnimationConditionObject = (GameObject)EditorGUILayout.ObjectField("Condition Object", _animator.Animations[i].AnimationConditionObject, typeof(GameObject), true);
                            _animator.Animations[i].TriggerName = "";
                        }

                        // Check if condition object have "Button" component (OnClick condition)
                        if (_animator.Animations[i].AnimationConditionType == WAnimationConditionType.OnClick)
                        {
                            if (_animator.Animations[i].AnimationConditionObject != null)
                            {
                                if (_animator.Animations[i].AnimationConditionObject.GetComponent<Button>() == null)
                                {
                                    Debug.LogWarning("No 'Button' Component was found on " + _animator.Animations[i].AnimationConditionObject.name + "!");
                                    _animator.Animations[i].AnimationConditionObject = null;
                                }
                            }
                        }

                        _animator.Animations[i].AnimationComponent = (Component)EditorGUILayout.ObjectField("Animation Component", _animator.Animations[i].AnimationComponent, typeof(Component), true);
                        _animator.Animations[i].Initialize(false);
                        _animator.Animations[i].Load();

                        EditorGUILayout.Space(10);

                        // Draw tweens values
                        if (_animator.Animations[i].AnimationComponent != null)
                        {
                            _animator.Animations[i].IsExpanded = EditorGUILayout.Foldout(_animator.Animations[i].IsExpanded, "Values", true);

                            if (_animator.Animations[i].IsExpanded)
                            {
                                EditorGUILayout.Space(10);

                                foreach (ValueInfo value in _animator.Animations[i].ValuesTweens.Keys)
                                {
                                    _animator.Animations[i].ValuesTweens[value].UpdateProperty();

                                    if (!EqualityComparer<object>.Default.Equals(_animator.Animations[i].ValuesTweens[value].PreviousValueInfoValue, _animator.Animations[i].ValuesTweens[value].Value.GetValue(_animator.Animations[i].AnimationComponent)) && !_animator.Animations[i].ValuesTweens[value].Animate)
                                    {
                                        _animator.Animations[i].ValuesTweens[value].EndValue = _animator.Animations[i].ValuesTweens[value].Value.GetValue(_animator.Animations[i].AnimationComponent);
                                        _animator.Animations[i].ValuesTweens[value].PreviousValueInfoValue = _animator.Animations[i].ValuesTweens[value].Value.GetValue(_animator.Animations[i].AnimationComponent);
                                    }

                                    if (!EqualityComparer<object>.Default.Equals(_animator.Animations[i].ValuesTweens[value].EndValue, _animator.Animations[i].ValuesTweens[value].Value.GetValue(_animator.Animations[i].AnimationComponent))
                                    || _animator.Animations[i].ValuesTweens[value].IgnoreTimeScale != false
                                    || _animator.Animations[i].ValuesTweens[value].Duration != 0
                                    || _animator.Animations[i].ValuesTweens[value].Delay != 0
                                    || _animator.Animations[i].ValuesTweens[value].Loops != 0
                                    || _animator.Animations[i].ValuesTweens[value].LoopType != LoopType.Restart
                                    || _animator.Animations[i].ValuesTweens[value].Ease != Ease.Unset)
                                    {
                                        _animator.Animations[i].ValuesTweens[value].Animate = true;
                                        _animator.Animations[i].ValuesTweens[value].IsExpanded = EditorGUILayout.Foldout(_animator.Animations[i].ValuesTweens[value].IsExpanded, value.Name, true, _blueWTweenStyle);
                                    }
                                    else
                                    {
                                        _animator.Animations[i].ValuesTweens[value].Animate = false;
                                        _animator.Animations[i].ValuesTweens[value].IsExpanded = EditorGUILayout.Foldout(_animator.Animations[i].ValuesTweens[value].IsExpanded, value.Name, true);
                                    }

                                    if (_animator.Animations[i].ValuesTweens[value].IsExpanded)
                                    {
                                        GUILayout.BeginVertical(_WTweenStyle);

                                        GUILayout.BeginHorizontal();
                                        _animator.Animations[i].ValuesTweens[value].EndValue = VisualizeObject(_animator.Animations[i].ValuesTweens[value].EndValue, "To");

                                        if (GUILayout.Button("Reset", GUILayout.Width(75f)))
                                        {
                                            _animator.Animations[i].ValuesTweens[value].EndValue = _animator.Animations[i].ValuesTweens[value].Value.GetValue(_animator.Animations[i].AnimationComponent);
                                        }
                                        GUILayout.EndHorizontal();

                                        if (_animator.Animations[i].ValuesTweens[value].EndValue.GetType() != typeof(bool))
                                        {
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
                                        }
                                        else
                                        {
                                            GUILayout.BeginHorizontal();
                                            _animator.Animations[i].ValuesTweens[value].Delay = EditorGUILayout.FloatField("Delay", _animator.Animations[i].ValuesTweens[value].Delay);
                                            GUILayout.EndHorizontal();
                                        }

                                        GUILayout.EndVertical();
                                    }

                                    EditorGUILayout.Space(10);
                                }
                            }
                            _animator.Animations[i].Serialize();
                        }
                    }
                    i++;
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(10);
                }
            }

            if (GUILayout.Button("Add Animation"))
            {
                List<WAnimation> newWAnimations = _animator.Animations.ToList();
                newWAnimations.Add(new WAnimation());
                _animator.Animations = newWAnimations.ToArray();
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
            _WTweenStyle.normal.background = _WTweenTexture;

            _blueWTweenStyle.normal.background = _blueWTweenTexture;

            _redWAnimationStyle.normal.background = _redWAnimationTexture;

            _headerTextStyle.fontStyle = FontStyle.Bold;
            _headerTextStyle.normal.textColor = new Color(193f / 255f, 193f / 255f, 193f / 255f);
            _headerTextStyle.alignment = TextAnchor.MiddleCenter;
            _headerTextStyle.fontSize = 22;
        }

        private void InitializeTextures()
        {
            _WTweenTexture = new Texture2D(1, 1);
            _WTweenTexture.SetPixel(0, 0, _WTweenColor);
            _WTweenTexture.Apply();

            _blueWTweenTexture = new Texture2D(1, 1);
            _blueWTweenTexture.SetPixel(0, 0, _blueWTweenColor);
            _blueWTweenTexture.Apply();

            _redWAnimationTexture = new Texture2D(1, 1);
            _redWAnimationTexture.SetPixel(0, 0, _redWAnimationColor);
            _redWAnimationTexture.Apply();
        }
    }

}
