using DG.Tweening;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace WS.WyneAnimator
{
    [Serializable]
    public abstract class TypeTween<TType> : ITypeTween
    {
        [SerializeField] protected TType _endValue;

        public TypeTween(object endValue) { _endValue = (TType)endValue; }

        public void StartTween(object obj, MonoBehaviour holder, PropertyInfo property, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale)
        {
            if (obj == null) return;
            if (property == null) return;
            holder.StartCoroutine(TweenCoroutine(obj, property, delay, duration, ease, loops, loopType, ignoreTimeScale));
        }

        public virtual IEnumerator TweenCoroutine(object obj, PropertyInfo value, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale)
        {
            yield return null;
        }

        public object GetEndValue()
        {
            return _endValue;
        }
    }
}
