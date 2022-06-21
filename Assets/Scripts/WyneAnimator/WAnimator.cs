using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WyneAnimator
{
    public class WAnimator : MonoBehaviour
    {
        [SerializeField] public WAnimation[] Animations;

        private void Awake()
        {
            foreach (WAnimation animation in Animations)
            {
                animation.InitializeTweens();
            }
        }

        private void Start()
        {
            foreach (WAnimation animation in Animations)
            {
                animation.StartAnimation(this);
            }
        }

    }
}

