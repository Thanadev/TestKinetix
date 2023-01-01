using UnityEngine;


namespace TestKinetix
{
    public class AnimationContext
    {
        public Animator Animator { get { return animator; } }
        public Transform AnimatedCharacter { get { return animatedCharacter; } }
        public MonoBehaviour AnimationController { get { return animationController; } }

        private Animator animator;
        private Transform animatedCharacter;
        private MonoBehaviour animationController;

        public AnimationContext(Transform animatedCharacter, Animator animator, MonoBehaviour animationController)
        {
            this.animator = animator;
            this.animatedCharacter = animatedCharacter;
            this.animationController = animationController;
        }
    }
}