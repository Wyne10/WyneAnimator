using DG.Tweening;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace WS.WyneAnimator
{
    public interface ITypeTween
    {
        void StartTween(object obj, MonoBehaviour holder, PropertyInfo property, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale);
        IEnumerator TweenCoroutine(object obj, PropertyInfo property, float delay, float duration, Ease ease, int loops, LoopType loopType, bool ignoreTimeScale);
        object GetEndValue();
    }
}


