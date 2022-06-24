using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WS.WyneAnimator
{
    [Serializable]
    public class WTween
    {
        [SerializeField] private Component _animationComponent;

        public ValueInfo Value;
        [SerializeField] private int _valueMetadataToken;
        public int ValueMetadataToken { get => _valueMetadataToken; }

        private object _endValue;

        [SerializeReference] private ITypeTween _typeTween;

        [SerializeField] public float Delay;
        [SerializeField] public float Duration;
        [SerializeField] public Ease Ease;
        [SerializeField] public int Loops;
        [SerializeField] public LoopType LoopType;
        [SerializeField] public bool IgnoreTimeScale;

        [SerializeField] public bool IsExpanded = false;

        public object EndValue 
        { get => _endValue;
          set
            {
                _endValue = value;
                SetTypeTween(value);
            }
        }

        private WTween(Component animationComponent, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale)
        {
            _animationComponent = animationComponent;
            Delay = delay;
            Duration = duration;
            Ease = ease;
            Loops = loops;
            LoopType = loopType;
            IgnoreTimeScale = ignoreTimeScale;
        }

        public WTween(Component animationComponent, ValueInfo value, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale) : this(animationComponent, delay, duration, ease, loops, loopType, ignoreTimeScale)
        {
            Value = value;
            _valueMetadataToken = value.MemberInfo.MetadataToken;
            EndValue = value.GetValue(animationComponent);
        }

        public void StartTween(MonoBehaviour holder)
        {
            _typeTween.StartTween(_animationComponent, holder, Value, Delay, Duration, Ease, Loops, LoopType, IgnoreTimeScale);
        }

        private void SetTypeTween(object value)
        {
            if (value.GetType() == typeof(int) || value.GetType() == typeof(long))
            {
                _typeTween = new IntTween(_endValue);
            }
            else if (value.GetType() == typeof(float) || value.GetType() == typeof(double))
            {
                _typeTween = new FloatTween(_endValue);
            }
            else if (value.GetType() == typeof(Color))
            {
                _typeTween = new ColorTween(_endValue);
            }
            else if (value.GetType() == typeof(Vector2))
            {
                _typeTween = new Vector2Tween(_endValue);
            }
            else if (value.GetType() == typeof(Vector3))
            {
                _typeTween = new Vector3Tween(_endValue);
            }
            else if (value.GetType() == typeof(bool))
            {
                _typeTween = new BoolTween(_endValue);
            }
        }

        public void UpdateValue()
        {
            List<ValueInfo> componentValues = _animationComponent.GetType().ExcludeType(typeof(MonoBehaviour));
            if (_animationComponent.GetType().GetProperty("enabled") != null)
                componentValues.Add(new ValueInfo(typeof(Behaviour).GetProperty("enabled")));

            foreach (ValueInfo valueInfo in componentValues)
            {
                if (!valueInfo.CheckType()) continue;

                if (valueInfo.MemberInfo.MetadataToken == _valueMetadataToken)
                {
                    Value = valueInfo;
                    return;
                }
            }
        }

        public void UpdateEndValue()
        {
            EndValue = _typeTween.GetEndValue();
        }
    }
}
