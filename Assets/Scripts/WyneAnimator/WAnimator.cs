using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WyneAnimator
{
    public class WAnimator : MonoBehaviour
    {
        [Header("OnAwake")]
        [SerializeField] public WAnimation[] OnAwakeAnimations;

        [Header("OnEnable")]
        [SerializeField] public WAnimation[] OnEnableAnimations;

        [Header("OnStart")]
        [SerializeField] public WAnimation[] OnStartAnimations;

        private List<WAnimation> _allAnimations = new List<WAnimation>();

        private void Awake()
        {
            _allAnimations.AddArray(OnEnableAnimations);
            _allAnimations.AddArray(OnAwakeAnimations);
            _allAnimations.AddArray(OnStartAnimations);

            foreach (WAnimation animation in _allAnimations)
            {
                foreach (WTween tween in animation.SerializedComponentFieldsTweens)
                {
                    tween.UpdateAnimationValue();
                }

                foreach (WTween tween in animation.SerializedComponentPropertiesTweens)
                {
                    tween.UpdateAnimationValue();
                }
            }

            foreach (WAnimation animation in OnAwakeAnimations)
            {
                foreach (WTween tween in animation.SerializedComponentFieldsTweens)
                {
                    tween.StartTween(this);
                }

                foreach (WTween tween in animation.SerializedComponentPropertiesTweens)
                {
                    tween.StartTween(this);
                }
            }
        }

        private void OnEnable()
        {
            foreach (WAnimation animation in OnEnableAnimations)
            {
                foreach (WTween tween in animation.SerializedComponentFieldsTweens)
                {
                    tween.StartTween(this);
                }

                foreach (WTween tween in animation.SerializedComponentPropertiesTweens)
                {
                    tween.StartTween(this);
                }
            }
        }

        private void Start()
        {
            foreach (WAnimation animation in OnStartAnimations)
            {
                foreach (WTween tween in animation.SerializedComponentFieldsTweens)
                {
                    tween.StartTween(this);
                }

                foreach (WTween tween in animation.SerializedComponentPropertiesTweens)
                {
                    tween.StartTween(this);
                }
            }
        }
    }
}

