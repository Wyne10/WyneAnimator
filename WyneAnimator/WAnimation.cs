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

        [SerializeField] private WTween[] _serializedValuesTweens;

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
            AnimationComponent.GetType().GetProperty("hierarchyCapacity").ExcludeMember(ref _componentValues);
/*            if (AnimationComponent.GetType().GetProperty("enabled") != null)
                _componentValues.Add(new ValueInfo(typeof(Behaviour).GetProperty("enabled")));
            if (AnimationComponent.GetType().GetProperty("hierarchyCapacity") != null)*/

            ValuesTweens.Clear();
            _serializedValuesTweens = new WTween[0];

            foreach (ValueInfo valueInfo in _componentValues)
            {
                if (!valueInfo.CheckType()) continue;
                ValuesTweens.Add(valueInfo, new WTween(AnimationComponent, valueInfo, 0, 1, DG.Tweening.Ease.Unset, 0, DG.Tweening.LoopType.Restart, false));
            }

            _initialized = true;
        }

        public void Load()
        {
            if (AnimationComponent == null) return;
            if (_serializedValuesTweens.Length == 0) return;
            if (Loaded) return;
            
            _componentValues = AnimationComponent.GetType().ExcludeType(typeof(MonoBehaviour));
            AnimationComponent.GetType().GetProperty("enabled").IncludeProperty(ref _componentValues);
            AnimationComponent.GetType().GetProperty("hierarchyCapacity").ExcludeMember(ref _componentValues);

            foreach (ValueInfo valueInfo in _componentValues)
            {
                if (!valueInfo.CheckType()) continue;

                bool tweenFound = false;
                bool tokenFound = false;

                foreach (WTween tween in _serializedValuesTweens)
                {
                    if (tween.ValueMetadataToken == valueInfo.MemberInfo.MetadataToken)
                    {
                        tweenFound = true;

                        foreach (ValueInfo valueInfoKey in ValuesTweens.Keys)
                        {
                            if (valueInfoKey.MemberInfo.MetadataToken == tween.ValueMetadataToken)
                            {
                                tokenFound = true;
                                break;
                            }
                        }

                        if (!tokenFound)
                            ValuesTweens.Add(valueInfo, tween);

                        break;
                    }
                }

                if (!tweenFound)
                    ValuesTweens.Add(valueInfo, new WTween(AnimationComponent, valueInfo, 0, 1, DG.Tweening.Ease.Unset, 0, DG.Tweening.LoopType.Restart, false));
            }

            foreach (WTween tween in ValuesTweens.Values)
            {
                tween.UpdateEndValue();
            }

            Loaded = true;
        }

        public void Serialize()
        {
            _serializedValuesTweens = ValuesTweens.Values.ToArray();
        }

        public void InitializeTweens()
        {
            foreach (WTween tween in _serializedValuesTweens)
            {
                if (tween.Animate)
                {
                    tween.UpdateValue();
                }
            }
        }

        public void StartAnimation(MonoBehaviour holder)
        {
            holder.StartCoroutine(AnimationCoroutine(holder));
        }

        internal void ForceStartAnimation(MonoBehaviour holder)
        {
            foreach (WTween tween in _serializedValuesTweens)
            {
                tween.StartTween(holder);
            }
        }

        private IEnumerator AnimationCoroutine(MonoBehaviour holder)
        {
            if (_animationCondition == null) yield break;
            if (_animationConditionObject == null) yield break;

            while (true)
            {
                if (_animationCondition.CheckCondition())
                {
                    foreach (WTween tween in _serializedValuesTweens)
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
