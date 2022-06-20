using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WyneAnimator
{
    public class WAnimator : MonoBehaviour
    {
        [SerializeField] public WAnimation[] OnStartAnimations;

        private void Awake()
        {
            foreach (WAnimation animation in OnStartAnimations)
            {
                animation.InitializeTweens();
            }
        }

        private void Start()
        {
            foreach (WAnimation animation in OnStartAnimations)
            {
                animation.StartTweens(this);
            }
        }

    }
}

