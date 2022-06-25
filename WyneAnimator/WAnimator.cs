using UnityEngine;

namespace WS.WyneAnimator
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

            foreach (WAnimation animation in Animations)
            {
                animation.StartAnimation(this);
            }
        }

        public void TriggerAnimation(string triggerName)
        {
            foreach (WAnimation animation in Animations)
            {
                if (animation.AnimationConditionType != WAnimationConditionType.OnTrigger) continue;
                if (!string.Equals(animation.TriggerName, triggerName, System.StringComparison.OrdinalIgnoreCase)) continue;

                animation.ForceStartAnimation(this);
            }
        }

        public void TriggerAllAnimations()
        {
            foreach (WAnimation animation in Animations)
            {
                if (animation.AnimationConditionType != WAnimationConditionType.OnTrigger) continue;
                animation.ForceStartAnimation(this);
            }
        }
    }
}

