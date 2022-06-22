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
                return;
                
            }

            Debug.LogWarning("Animation with trigger '" + triggerName + "' couldn't be found!");
        }
    }
}

