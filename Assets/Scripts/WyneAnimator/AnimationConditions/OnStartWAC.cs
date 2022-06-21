using System;
using UnityEngine;

namespace WyneAnimator
{
    [Serializable]
    public class OnStartWAC : WAnimationCondition
    {
        private bool _activated = false;

        public OnStartWAC(GameObject gameObject, Component component) : base (gameObject, component)
        {  }

        public override bool CheckCondition()
        {
            if (!_activated && _gameObject.activeSelf)
            {
                _activated = true;
                return true;
            }

            return false;
        }
    }
}
