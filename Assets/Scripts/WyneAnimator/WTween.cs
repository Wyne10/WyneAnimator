using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.UI;

namespace WyneAnimator
{
    [Serializable]
    public class WTween
    {
        [SerializeField] private Component _animationComponent;

        private FieldInfo _fieldValue;
        private PropertyInfo _propertyValue;

        [SerializeField] public int ValueMetadataToken;

        [SerializeField] public TweenValueType ValueType;

        public object NewValue;

        [SerializeField] private int _newIntComponentValue;
        [SerializeField] private float _newFloatComponentValue;
        [SerializeField] private Color _newColorComponentValue;
        [SerializeField] private Vector2 _newVector2ComponentValue;
        [SerializeField] private Vector3 _newVector3ComponentValue;
        [SerializeField] private Vector4 _newVector4ComponentValue;
        [SerializeField] private bool _newBoolComponentValue;

        [SerializeField] public bool IgnoreTimeScale;
        [SerializeField] public float Delay;
        [SerializeField] public float Duration;
        [SerializeField] public int Loops;

        [SerializeField] public Ease Ease;
        [SerializeField] public LoopType LoopType;

        [SerializeField] public bool IsExpanded = false;

        private WTween(Component animationComponent, bool ignoreTimeScale, float delay, float duration, int loops, Ease ease, LoopType loopType)
        {
            _animationComponent = animationComponent;
            IgnoreTimeScale = ignoreTimeScale;
            Delay = delay;
            Duration = duration;
            Loops = loops;
            Ease = ease;
            LoopType = loopType;
        }

        public WTween(Component animationComponent, FieldInfo fieldValue, bool ignoreTimeScale, float delay, float duration, int loops, Ease ease, LoopType loopType) : this(animationComponent, ignoreTimeScale, delay, duration, loops, ease, loopType)
        {
            _fieldValue = fieldValue;
            SetNewAnimationValue(fieldValue.GetValue(animationComponent));
            ValueMetadataToken = fieldValue.MetadataToken;
        }

        public WTween(Component animationComponent, PropertyInfo propertyValue, bool ignoreTimeScale, float delay, float duration, int loops, Ease ease, LoopType loopType) : this(animationComponent, ignoreTimeScale, delay, duration, loops, ease, loopType)
        {
            _propertyValue = propertyValue;
            SetNewAnimationValue(propertyValue.GetValue(animationComponent));
            ValueMetadataToken = propertyValue.MetadataToken;
        }

        public void StartTween(MonoBehaviour holder)
        {
            holder.StartCoroutine(TweenCoroutine());
        }

        public IEnumerator TweenCoroutine()
        {
            object startValue;

            yield return new WaitForSeconds(Delay);

            if (_fieldValue != null)
            {
                startValue = _fieldValue.GetValue(_animationComponent);

                if (ValueType == TweenValueType.Int)
                {
                    if ((int)startValue == _newIntComponentValue) yield break;
                    yield return DOVirtual.Int((int)startValue, _newIntComponentValue, Duration, v => { _fieldValue.SetValue(_animationComponent, v); }).SetEase(Ease).SetLoops(Loops, LoopType).SetUpdate(IgnoreTimeScale).WaitForCompletion();
                }
                else if (ValueType == TweenValueType.Float)
                {
                    if ((float)startValue == _newFloatComponentValue) yield break;
                    yield return DOVirtual.Float((float)startValue, _newFloatComponentValue, Duration, v => { _fieldValue.SetValue(_animationComponent, v); }).SetEase(Ease).SetLoops(Loops, LoopType).SetUpdate(IgnoreTimeScale).WaitForCompletion();
                }
                else if (ValueType == TweenValueType.Color)
                {
                    if ((Color)startValue == _newColorComponentValue) yield break;
                    yield return DOVirtual.Color((Color)startValue, _newColorComponentValue, Duration, v => { _fieldValue.SetValue(_animationComponent, v); }).SetEase(Ease).SetLoops(Loops, LoopType).SetUpdate(IgnoreTimeScale).WaitForCompletion();
                }
                else if (ValueType == TweenValueType.Vector2)
                {
                    if ((Vector2)startValue == _newVector2ComponentValue) yield break;
                    yield return DOTween.To(() => (Vector2)startValue, v => { _fieldValue.SetValue(_animationComponent, v); }, _newVector2ComponentValue, Duration).SetEase(Ease).SetLoops(Loops, LoopType).SetUpdate(IgnoreTimeScale).WaitForCompletion();
                }
                else if (ValueType == TweenValueType.Vector3)
                {
                    if (startValue.GetType() == typeof(Quaternion))
                    {
                        startValue = ((Quaternion)startValue).eulerAngles;
                        _propertyValue = _animationComponent.GetType().GetProperty("eulerAngles");
                        if ((Vector3)startValue == _newVector3ComponentValue) yield break;
                        yield return DOVirtual.Vector3((Vector3)startValue, _newVector3ComponentValue, Duration, v => { _propertyValue.SetValue(_animationComponent, v); }).SetEase(Ease).SetLoops(Loops, LoopType).SetUpdate(IgnoreTimeScale).WaitForCompletion();
                        yield break;
                    }

                    if ((Vector3)startValue == _newVector3ComponentValue) yield break;
                    yield return DOVirtual.Vector3((Vector3)startValue, _newVector3ComponentValue, Duration, v => { _fieldValue.SetValue(_animationComponent, v); }).SetEase(Ease).SetLoops(Loops, LoopType).SetUpdate(IgnoreTimeScale).WaitForCompletion();
                }
                else if (ValueType == TweenValueType.Vector4)
                {
                    if ((Vector4)startValue == _newVector4ComponentValue) yield break;
                    yield return DOTween.To(() => (Vector4)startValue, v => { _fieldValue.SetValue(_animationComponent, v); }, _newVector4ComponentValue, Duration).SetEase(Ease).SetLoops(Loops, LoopType).SetUpdate(IgnoreTimeScale).WaitForCompletion();
                }
                else if (ValueType == TweenValueType.Bool)
                {
                    if ((bool)startValue == _newBoolComponentValue) yield break;
                    _fieldValue.SetValue(_animationComponent, _newBoolComponentValue);
                }
            }
            else if (_propertyValue != null)
            {
                startValue = _propertyValue.GetValue(_animationComponent);

                if (ValueType == TweenValueType.Int)
                {
                    if ((int)startValue == _newIntComponentValue) yield break;
                    yield return DOVirtual.Int((int)startValue, _newIntComponentValue, Duration, v => { _propertyValue.SetValue(_animationComponent, v); }).SetEase(Ease).SetLoops(Loops, LoopType).SetUpdate(IgnoreTimeScale).WaitForCompletion();
                }
                else if (ValueType == TweenValueType.Float)
                {
                    if ((float)startValue == _newFloatComponentValue) yield break;
                    yield return DOVirtual.Float((float)startValue, _newFloatComponentValue, Duration, v => { _propertyValue.SetValue(_animationComponent, v); }).SetEase(Ease).SetLoops(Loops, LoopType).SetUpdate(IgnoreTimeScale).WaitForCompletion();
                }
                else if (ValueType == TweenValueType.Color)
                {
                    if ((Color)startValue == _newColorComponentValue) yield break;
                    yield return DOVirtual.Color((Color)startValue, _newColorComponentValue, Duration, v => { _propertyValue.SetValue(_animationComponent, v); }).SetEase(Ease).SetLoops(Loops, LoopType).SetUpdate(IgnoreTimeScale).WaitForCompletion();
                }
                else if (ValueType == TweenValueType.Vector2)
                {
                    if ((Vector2)startValue == _newVector2ComponentValue) yield break;
                    yield return DOTween.To(() => (Vector2)startValue, v => { _propertyValue.SetValue(_animationComponent, v); }, _newVector2ComponentValue, Duration).SetEase(Ease).SetLoops(Loops, LoopType).SetUpdate(IgnoreTimeScale).WaitForCompletion();
                }
                else if (ValueType == TweenValueType.Vector3)
                {
                    if (startValue.GetType() == typeof(Quaternion))
                    {
                        startValue = ((Quaternion)startValue).eulerAngles;
                        _propertyValue = _animationComponent.GetType().GetProperty("eulerAngles");
                        if ((Vector3)startValue == _newVector3ComponentValue) yield break;
                        yield return DOVirtual.Vector3((Vector3)startValue, _newVector3ComponentValue, Duration, v => { _propertyValue.SetValue(_animationComponent, v); }).SetEase(Ease).SetLoops(Loops, LoopType).SetUpdate(IgnoreTimeScale).WaitForCompletion();
                        yield break;
                    }

                    if ((Vector3)startValue == _newVector3ComponentValue) yield break;
                    yield return DOVirtual.Vector3((Vector3)startValue, _newVector3ComponentValue, Duration, v => { _propertyValue.SetValue(_animationComponent, v); }).SetEase(Ease).SetLoops(Loops, LoopType).SetUpdate(IgnoreTimeScale).WaitForCompletion();
                }
                else if (ValueType == TweenValueType.Vector4)
                {
                    if ((Vector4)startValue == _newVector4ComponentValue) yield break;
                    yield return DOTween.To(() => (Vector4)startValue, v => { _propertyValue.SetValue(_animationComponent, v); }, _newVector4ComponentValue, Duration).SetEase(Ease).SetLoops(Loops, LoopType).SetUpdate(IgnoreTimeScale).WaitForCompletion();
                }
                else if (ValueType == TweenValueType.Bool)
                {
                    if ((bool)startValue == _newBoolComponentValue) yield break;
                    _propertyValue.SetValue(_animationComponent, _newBoolComponentValue);
                }
            }
        }

        public void SetNewAnimationValue(object newValue)
        {
            NewValue = newValue;

            if (newValue.GetType() == typeof(int) || newValue.GetType() == typeof(long))
            {
                ValueType = TweenValueType.Int;
                _newIntComponentValue = (int)newValue;
            }
            else if (newValue.GetType() == typeof(float) || newValue.GetType() == typeof(double))
            {
                ValueType = TweenValueType.Float;
                _newFloatComponentValue = (float)newValue;
            }
            else if (newValue.GetType() == typeof(Color))
            {
                ValueType = TweenValueType.Color;
                _newColorComponentValue = (Color)newValue;
            }
            else if (newValue.GetType() == typeof(Vector2))
            {
                ValueType = TweenValueType.Vector2;
                _newVector2ComponentValue = (Vector2)newValue;
            }
            else if (newValue.GetType() == typeof(Vector3))
            {
                ValueType = TweenValueType.Vector3;
                _newVector3ComponentValue = (Vector3)newValue;
            }
            else if (newValue.GetType() == typeof(Vector4))
            {
                ValueType = TweenValueType.Vector4;
                _newVector4ComponentValue = (Vector4)newValue;
            }
            else if (newValue.GetType() == typeof(Quaternion))
            {
                ValueType = TweenValueType.Vector3;
                _newVector3ComponentValue = ((Quaternion)newValue).eulerAngles;
            }
            else if (newValue.GetType() == typeof(bool))
            {
                ValueType = TweenValueType.Bool;
                _newBoolComponentValue = (bool)newValue;
            }
        }

        public object GetNewAnimationValue()
        {
            if (ValueType == TweenValueType.Int)
            {
                return _newIntComponentValue;
            }
            else if (ValueType == TweenValueType.Float)
            {
                return _newFloatComponentValue;
            }
            else if (ValueType == TweenValueType.Color)
            {
                return _newColorComponentValue;
            }
            else if (ValueType == TweenValueType.Vector2)
            {
                return _newVector2ComponentValue;
            }
            else if (ValueType == TweenValueType.Vector3)
            {
                return _newVector3ComponentValue;
            }
            else if (ValueType == TweenValueType.Vector4)
            {
                return _newVector4ComponentValue;
            }
            else if (ValueType == TweenValueType.Bool)
            {
                return _newBoolComponentValue;
            }

            return null;
        }

        public void UpdateNewAnimationValue()
        {
            NewValue = GetNewAnimationValue();
        }

        public void UpdateAnimationValue()
        {
            List<FieldInfo> componentFields;
            List<PropertyInfo> componentProperties;

            _animationComponent.GetType().ExcludeTypes(typeof(MonoBehaviour), out componentFields, out componentProperties);

            foreach (FieldInfo field in componentFields)
            {
                if (!field.CheckReflectedValue()) continue;

                if (field.MetadataToken == ValueMetadataToken)
                {
                    _fieldValue = field;
                }
            }

            foreach (PropertyInfo property in componentProperties)
            {
                if (!property.CheckReflectedValue()) continue;
                
                if (property.MetadataToken == ValueMetadataToken)
                {
                    _propertyValue = property;
                }
            }
        }
    }

    public enum TweenValueType
    {
        Int,
        Float,
        Color,
        Vector2,
        Vector3,
        Vector4,
        Bool
    }
}

