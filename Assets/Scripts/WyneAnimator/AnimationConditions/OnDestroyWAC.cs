using System;
using UnityEngine;

namespace WyneAnimator
{
    [Serializable]
    public class OnDestroyWAC : WAnimationCondition
    {
        private bool _activated = false;

        public OnDestroyWAC(GameObject gameObject, Component component) : base(gameObject, component)
        { }

        public override bool CheckCondition()
        {
            if (!_activated && _gameObject == null)
            {
                _activated = true;
                return true;
            }

            return false;
        }
    }
}
