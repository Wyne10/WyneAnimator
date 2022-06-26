using DG.Tweening;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace WS.WyneAnimator
{
    [Serializable]
    public class Vector3Tween : TypeTween<Vector3>
    {
        public Vector3Tween(object endValue) : base(endValue)
        { }

        public override IEnumerator TweenCoroutine(object obj, PropertyInfo property, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale)
        {
            yield return new WaitForSeconds(delay);
            yield return DOVirtual.Vector3((Vector3)property.GetValue(obj), _endValue, duration, v => { property.SetValue(obj, v); }).SetEase(ease).SetLoops(loops, loopType).SetUpdate(ignoreTimeScale).WaitForCompletion();
        }
    }
}
