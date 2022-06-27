using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WS.WyneAnimator
{
    [Serializable]
    public class WAnimation
    {
        [SerializeField] private WAnimationConditionType _animationConditionType;
        public WAnimationConditionType AnimationConditionType
        {
            get => _animationConditionType;
            set
            {
                _animationConditionType = value;
                SetAnimationCondition(value);
            }
        }
        [SerializeReference] private WAnimationCondition _animationCondition;

        [SerializeField] private GameObject _animationConditionObject;
        public GameObject AnimationConditionObject
        {
            get => _animationConditionObject;
            set
            {
                _animationConditionObject = value;
                SetAnimationCondition(_animationConditionType);
            }
        }

        [SerializeField] public string TriggerName;

        [SerializeField] public Component AnimationComponent;
        [SerializeField] private Component _previousComponent;

        private List<ValueInfo> _componentValues;
        public Dictionary<ValueInfo, WTween> ValuesTweens = new Dictionary<ValueInfo, WTween>();

        [SerializeField] private WTween[] _serializedPropertiesTweens;

        [SerializeField] private bool _initialized = false;
        public bool Loaded = false;

        [SerializeField] public bool IsExpanded = false;

        public void Initialize(bool force)
        {
            if (AnimationComponent != _previousComponent)
            {
                _initialized = false;
                _previousComponent = AnimationComponent;
            }

            if (AnimationComponent == null) return;
            if (!force)
                if (_initialized) return;

            _componentValues = AnimationComponent.GetType().ExcludeType(typeof(MonoBehaviour));
            AnimationComponent.GetType().GetProperty("enabled").IncludeProperty(ref _componentValues);
            AnimationComponent.GetType().GetProperty("hierarchyCapacity").ExcludeProperty(ref _componentValues);

            ValuesTweens.Clear();
            _serializedPropertiesTweens = new WTween[0];

            foreach (ValueInfo value in _componentValues)
            {
                if (!value.CheckType()) continue;
                ValuesTweens.Add(value, new WTween(AnimationComponent, value, 0, 0, DG.Tweening.Ease.Unset, 0, DG.Tweening.LoopType.Restart, false));
            }

            _initialized = true;
        }

        public void Load()
        {
            if (AnimationComponent == null) return;
            if (Loaded) return;
            
            _componentValues = AnimationComponent.GetType().ExcludeType(typeof(MonoBehaviour));
            AnimationComponent.GetType().GetProperty("enabled").IncludeProperty(ref _componentValues);
            AnimationComponent.GetType().GetProperty("hierarchyCapacity").ExcludeProperty(ref _componentValues);

            foreach (ValueInfo value in _componentValues)
            {
                if (!value.CheckType()) continue;

                bool tweenFound = false;

                foreach (WTween tween in _serializedPropertiesTweens)
                {
                    if (value.MemberInfo.MetadataToken == tween.ValueMetadataToken)
                    {
                        if (!ValuesTweens.ContainsValue(tween))
                        {
                            ValuesTweens.Add(value, tween);
                            tweenFound = true;
                            break;
                        }
                    }
                }

                if (!tweenFound)
                {
                    bool found = false;

                    foreach (ValueInfo tweenKey in ValuesTweens.Keys)
                    {
                        if (value.MemberInfo.MetadataToken == tweenKey.MemberInfo.MetadataToken)
                            found = true;
                    }

                    if (!found)
                        ValuesTweens.Add(value, new WTween(AnimationComponent, value, 0, 0, DG.Tweening.Ease.Unset, 0, DG.Tweening.LoopType.Restart, false));
                }
            }

            foreach (WTween tween in ValuesTweens.Values)
            {
                tween.UpdateEndValue();
            }

            Loaded = true;
        }

        public void Serialize()
        {
            _serializedPropertiesTweens = ValuesTweens.Values.Where(v => v.Animate == true).ToArray();
        }

        public void InitializeTweens()
        {
            foreach (WTween tween in _serializedPropertiesTweens)
            {
                if (tween.Animate)
                {
                    tween.UpdateProperty();
                }
            }
        }

        public void StartAnimation(MonoBehaviour holder)
        {
            holder.StartCoroutine(AnimationCoroutine(holder));
        }

        internal void ForceStartAnimation(MonoBehaviour holder)
        {
            if (AnimationComponent == null) return;

            foreach (WTween tween in _serializedPropertiesTweens)
            {
                tween.StartTween(holder);
            }
        }

        private IEnumerator AnimationCoroutine(MonoBehaviour holder)
        {
            if (AnimationComponent == null) yield break;
            if (_animationCondition == null) yield break;
            if (_animationConditionObject == null) yield break;

            while (true)
            {
                if (_animationCondition.CheckCondition())
                {
                    foreach (WTween tween in _serializedPropertiesTweens)
                    {
                        tween.StartTween(holder);
                    }
                }

                yield return null;
            }
        }

        private void SetAnimationCondition(WAnimationConditionType conditionType)
        {
            if (_animationConditionObject == null) return;

            if (conditionType == WAnimationConditionType.OnStart)
                _animationCondition = new OnStartWAC(_animationConditionObject, AnimationComponent);
            else if (conditionType == WAnimationConditionType.OnEnable)
                _animationCondition = new OnEnableWAC(_animationConditionObject, AnimationComponent);
            else if (conditionType == WAnimationConditionType.OnDisable)
                _animationCondition = new OnDisableWAC(_animationConditionObject, AnimationComponent);
            else if (conditionType == WAnimationConditionType.OnDestroy)
                _animationCondition = new OnDestroyWAC(_animationConditionObject, AnimationComponent);
            else if (conditionType == WAnimationConditionType.OnClick)
                _animationCondition = new OnClickWAC(_animationConditionObject, AnimationComponent);
            else if (conditionType == WAnimationConditionType.OnUIHover)
                _animationCondition = new OnUIHoverWAC(_animationConditionObject, AnimationComponent);
            else if (conditionType == WAnimationConditionType.OnUIUnHover)
                _animationCondition = new OnUIUnHoverWAC(_animationConditionObject, AnimationComponent);
        }
    }

    public enum WAnimationConditionType
    {
        OnStart,
        OnEnable,
        OnDisable,
        OnDestroy,
        OnClick,
        OnTrigger,
        OnUIHover,
        OnUIUnHover
    }

}
