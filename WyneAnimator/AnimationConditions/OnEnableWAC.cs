using System;
using UnityEngine;

namespace WS.WyneAnimator
{
    [Serializable]
    public class OnEnableWAC : WAnimationCondition
    {
        private bool _previousGameObjectState = false;

        public OnEnableWAC(GameObject gameObject, Component component) : base(gameObject, component)
        { }

        public override bool CheckCondition()
        {
            if (_gameObject.activeSelf && !_previousGameObjectState)
            {
                _previousGameObjectState = true;
                return true;
            }

            if (!_gameObject.activeSelf)
            {
                _previousGameObjectState = false;
            }

            return false;
        }
    }
}
