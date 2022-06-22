using System;
using UnityEngine;
using UnityEngine.UI;

namespace WS.WyneAnimator
{
    [Serializable]
    public class OnClickWAC : WAnimationCondition
    {
        [SerializeField] private Button _button;
        private bool _isClicked = false;
        private bool _isEventAdded = false;

        public OnClickWAC(GameObject gameObject, Component component) : base(gameObject, component)
        {
            _button = gameObject.GetComponent<Button>();
        }

        public override bool CheckCondition()
        {
            if (!_isEventAdded && _button != null)
            {
                _button.onClick.AddListener(delegate { _isClicked = true; });
                _isEventAdded = true;
            }

            if (_isClicked)
            {
                _isClicked = false;
                return true;
            }

            return false;
        }
    }
}
