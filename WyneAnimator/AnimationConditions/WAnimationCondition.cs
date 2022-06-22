using System;
using UnityEngine;

namespace WS.WyneAnimator
{
    [Serializable]
    public abstract class WAnimationCondition
    {
        [SerializeField] protected GameObject _gameObject;
        [SerializeField] protected Component _component;

        protected WAnimationCondition(GameObject gameObject, Component component) { _gameObject = gameObject; _component = component; }

        public abstract bool CheckCondition();
    }
}
