using UnityEngine;

public class AnimationContext
{
    public Animator Animator { get { return animator; } }
    public Transform AnimatedCharacter { get { return animatedCharacter; } }

    private Animator animator;
    private Transform animatedCharacter;

    public AnimationContext(Transform animatedCharacter, Animator animator)
    {
        this.animator = animator;
        this.animatedCharacter = animatedCharacter;
    }
}
