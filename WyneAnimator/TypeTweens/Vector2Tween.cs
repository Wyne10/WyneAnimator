using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace WS.WyneAnimator
{
    [Serializable]
    public class Vector2Tween : TypeTween<Vector2>
    {
        public Vector2Tween(object endValue) : base(endValue)
        { }

        public override IEnumerator TweenCoroutine(object obj, ValueInfo value, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale)
        {
            yield return new WaitForSeconds(delay);
            yield return DOTween.To(() => (Vector2)value.GetValue(obj), v => { value.SetValue(obj, v); }, _endValue, duration).SetEase(ease).SetLoops(loops, loopType).SetUpdate(ignoreTimeScale).WaitForCompletion();
        }
    }
}
