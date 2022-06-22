using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace WS.WyneAnimator
{
    [Serializable]
    public class BoolTween : TypeTween<bool>
    {
        public BoolTween(object endValue) : base(endValue)
        { }

        public override IEnumerator TweenCoroutine(object obj, ValueInfo value, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale)
        {
            yield return new WaitForSeconds(delay);
            value.SetValue(obj, _endValue);
        }
    }
}
