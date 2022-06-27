using DG.Tweening;
using System.Collections;
using UnityEngine;

namespace WS.WyneAnimator
{
    public interface ITypeTween
    {
        void StartTween(object obj, MonoBehaviour holder, ValueInfo value, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale);
        IEnumerator TweenCoroutine(object obj, ValueInfo value, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale);
        object GetEndValue();
    }
}


