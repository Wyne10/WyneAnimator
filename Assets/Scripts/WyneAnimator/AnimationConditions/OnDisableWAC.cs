using System;
using UnityEngine;

namespace WS.WyneAnimator
{
    [Serializable]
    public class OnDisableWAC : WAnimationCondition
    {
        private bool _previousGameObjectState = false;

        public OnDisableWAC(GameObject gameObject, Component component) : base(gameObject, component)
        { }

        public override bool CheckCondition()
        {
            if (!_gameObject.activeSelf && _previousGameObjectState)
            {
                _previousGameObjectState = false;
                return true;
            }

            if (_gameObject.activeSelf)
            {
                _previousGameObjectState = true;
            }

            return false;
        }
    }
}
