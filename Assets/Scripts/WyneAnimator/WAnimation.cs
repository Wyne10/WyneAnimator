using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WyneAnimator
{
    [Serializable]
    public class WAnimation
    {
        [SerializeField] public Component AnimationComponent;
        [SerializeField] private Component _previousComponent;

        private List<FieldInfo> _componentFields;
        private List<PropertyInfo> _componentProperties;
        public List<FieldInfo> ComponentFields { get => _componentFields; }
        public List<PropertyInfo> ComponentProperties { get => _componentProperties; }

        public Dictionary<FieldInfo, WTween> ComponentFieldsTweens = new Dictionary<FieldInfo, WTween>();
        public Dictionary<PropertyInfo, WTween> ComponentPropertiesTweens = new Dictionary<PropertyInfo, WTween>();

        [SerializeField] public WTween[] SerializedComponentFieldsTweens;
        [SerializeField] public WTween[] SerializedComponentPropertiesTweens;

        [SerializeField] private bool _initialized = false;
        public bool Loaded = false;

        public void Initialize()
        {
            if (AnimationComponent != _previousComponent)
            {
                _initialized = false;
                _previousComponent = AnimationComponent;
            }

            if (AnimationComponent != null)
            {
                if (!_initialized)
                {
                    AnimationComponent.GetType().ExcludeTypes(typeof(MonoBehaviour), out _componentFields, out _componentProperties);

                    ComponentFieldsTweens.Clear();
                    ComponentPropertiesTweens.Clear();

                    SerializedComponentFieldsTweens = new WTween[0];
                    SerializedComponentPropertiesTweens = new WTween[0];

                    foreach (FieldInfo field in _componentFields)
                    {
                        if (!field.CheckReflectedValue()) continue;
                        ComponentFieldsTweens.Add(field, new WTween(AnimationComponent, field, true, 0, 1, 0, DG.Tweening.Ease.Unset, DG.Tweening.LoopType.Restart));
                    }

                    foreach (PropertyInfo property in _componentProperties)
                    {
                        if (!property.CheckReflectedValue()) continue;
                        ComponentPropertiesTweens.Add(property, new WTween(AnimationComponent, property, true, 0, 1, 0, DG.Tweening.Ease.Unset, DG.Tweening.LoopType.Restart));
                    }

                    _initialized = true;
                }
            }
        }
        
        public void Load()
        {
            if (AnimationComponent != null)
            {
                if (!Loaded)
                {
                    AnimationComponent.GetType().ExcludeTypes(typeof(MonoBehaviour), out _componentFields, out _componentProperties);

                    foreach (FieldInfo field in _componentFields)
                    {
                        if (!field.CheckReflectedValue()) continue;

                        bool found = false;

                        foreach (WTween tween in SerializedComponentFieldsTweens)
                        {
                            if (tween.ValueMetadataToken == field.MetadataToken)
                            {
                                found = true;
                                ComponentFieldsTweens.AddOrReplace(field, tween);
                                tween.UpdateNewAnimationValue();
                                break;
                            }
                        }

                        if (!found)
                            ComponentFieldsTweens.AddOrReplace(field, new WTween(AnimationComponent, field, true, 0, 1, 0, DG.Tweening.Ease.Unset, DG.Tweening.LoopType.Restart));
                    }

                    foreach (PropertyInfo property in _componentProperties)
                    {
                        if (!property.CheckReflectedValue()) continue;

                        bool found = false;

                        foreach (WTween tween in SerializedComponentPropertiesTweens)
                        {
                            if (tween.ValueMetadataToken == property.MetadataToken)
                            {
                                found = true;
                                ComponentPropertiesTweens.AddOrReplace(property, tween);
                                tween.UpdateNewAnimationValue();
                                break;
                            }
                        }

                        if (!found)
                            ComponentPropertiesTweens.AddOrReplace(property, new WTween(AnimationComponent, property, true, 0, 1, 0, DG.Tweening.Ease.Unset, DG.Tweening.LoopType.Restart));
                    }

                    Loaded = true;
                }
            }
        }

        public void Serialize()
        {
            SerializedComponentFieldsTweens = ComponentFieldsTweens.Values.ToArray();
            SerializedComponentPropertiesTweens = ComponentPropertiesTweens.Values.ToArray();
        }
    }
}

