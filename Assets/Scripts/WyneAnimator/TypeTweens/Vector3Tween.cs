using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace WyneAnimator
{
    [Serializable]
    public class Vector3Tween : TypeTween<Vector3>
    {
        public Vector3Tween(object endValue) : base(endValue)
        { }

        public override IEnumerator TweenCoroutine(object obj, ValueInfo value, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale)
        {
            yield return new WaitForSeconds(delay);
            yield return DOVirtual.Vector3((Vector3)value.GetValue(obj), _endValue, duration, v => { value.SetValue(obj, v); }).SetEase(ease).SetLoops(loops, loopType).SetUpdate(ignoreTimeScale).WaitForCompletion();
        }
    }
}
