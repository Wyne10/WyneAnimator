using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WyneAnimator
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

        [SerializeField] public GameObject AnimationConditionObject;

        [SerializeField] public Component AnimationComponent;
        [SerializeField] private Component _previousComponent;

        private List<ValueInfo> _componentValues;
        public Dictionary<ValueInfo, WTween> ValuesTweens = new Dictionary<ValueInfo, WTween>();

        [SerializeField] private WTween[] _serializedValuesTweens;

        [SerializeField] private bool _initialized = false;
        public bool Loaded = false;

        public void Initialize()
        {
            if (AnimationComponent != _previousComponent)
            {
                _initialized = false;
                _previousComponent = AnimationComponent;
            }

            if (AnimationComponent == null) return;
            if (_initialized) return;

            _componentValues = AnimationComponent.GetType().ExcludeType(typeof(MonoBehaviour));

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
                tween.UpdateValue();
            }
        }

        public void StartAnimation(MonoBehaviour holder)
        {
            holder.StartCoroutine(AnimationCoroutine(holder));
        }

        private IEnumerator AnimationCoroutine(MonoBehaviour holder)
        {
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
            if (conditionType == WAnimationConditionType.OnStart)
                _animationCondition = new OnStartWAC(AnimationConditionObject, AnimationComponent);
        }
    }

    public enum WAnimationConditionType
    {
        OnStart
    }

}
