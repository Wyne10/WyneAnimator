using DG.Tweening;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace WS.WyneAnimator
{
    [Serializable]
    public class IntTween : TypeTween<int>
    {
        public IntTween(object endValue) : base(endValue)
        { }

        public override IEnumerator TweenCoroutine(object obj, PropertyInfo property, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale)
        {
            yield return new WaitForSeconds(delay);
            yield return DOVirtual.Int((int)property.GetValue(obj), _endValue, duration, v => { property.SetValue(obj, v); }).SetEase(ease).SetLoops(loops, loopType).SetUpdate(ignoreTimeScale).WaitForCompletion();
        }
    }
}
