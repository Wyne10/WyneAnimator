using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WS.WyneAnimator
{
    [Serializable]
    public class OnUIUnHoverWAC : WAnimationCondition
    {
        private bool _initialized = false;
        private bool _activated = false;
        private GraphicRaycaster _raycaster;
        private EventSystem _eventSystem;
        private PointerEventData _pointerEventData;

        public OnUIUnHoverWAC(GameObject gameObject, Component component) : base(gameObject, component)
        { }

        public override bool CheckCondition()
        {
            if (!_initialized)
            {
                _raycaster = GameObject.FindObjectOfType<Canvas>().GetComponent<GraphicRaycaster>();
                _eventSystem = GameObject.FindObjectOfType<EventSystem>();
                _pointerEventData = new PointerEventData(_eventSystem);
                _initialized = true;
            }

            if (_raycaster == null) { Debug.LogWarning("Couldn't find 'Canvas' object to handle OnUIUnHover condition!"); return false; }
            if (_eventSystem == null) { Debug.LogWarning("Couldn't find 'EventSystem' object to handle OnUIUnHover condition!"); return false; }

            if (!_activated && EventSystem.current.IsPointerOverGameObject())
            {
                _pointerEventData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                _raycaster.Raycast(_pointerEventData, results);

                foreach (RaycastResult result in results)
                {
                    if (result.gameObject == _gameObject)
                    {
                        _activated = true;
                        break;
                    }
                }
            }

            if (_activated && !EventSystem.current.IsPointerOverGameObject())
            {
                _activated = false;
                return true;
            }

            return false;
        }
    }
}
