﻿using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WyneAnimator
{
    [Serializable]
    public abstract class TypeTween<TType> : ITypeTween
    {
        [SerializeField] protected TType _endValue;

        public TypeTween(object endValue) { _endValue = (TType)endValue; }

        public void StartTween(object obj, MonoBehaviour holder, ValueInfo value, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale)
        {
            if (!EqualityComparer<TType>.Default.Equals((TType)value.GetValue(obj), _endValue))
            {
                holder.StartCoroutine(TweenCoroutine(obj, value, delay, duration, ease, loops, loopType, ignoreTimeScale));
            }
        }

        public virtual IEnumerator TweenCoroutine(object obj, ValueInfo value, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale)
        {
            yield return null;
        }

        public object GetEndValue()
        {
            return _endValue;
        }
    }
}
