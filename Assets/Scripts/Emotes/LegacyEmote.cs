using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestKinetix
{
    public class LegacyEmote : IEmote
    {
        private AnimationClip clip;
        private AnimationContext context;

        private Dictionary<Transform, Dictionary<string, object>> bonesTargetProperties;
        private Dictionary<Transform, Dictionary<string, object>> bonesOriginalProperties;

        private bool isPlaying;


        public LegacyEmote(AnimationClip clip, AnimationContext context)
        {
            this.clip = clip;
            this.context = context;


            isPlaying = false;
            bonesTargetProperties = LegacyAnimationClipReader.DetermineTargetBoneProperties(clip, context.AnimatedCharacter);
        }

        public void Play()
        {
            if (!isPlaying) {
                context.AnimationController.StartCoroutine(StartAnimation());
            }
        }

        public IEnumerator Cancel()
        {
            if (isPlaying) {
                context.AnimationController.StopAllCoroutines();

                yield return RevertToPreviousState();
            }

            yield return null;
        }

        #region Coroutines

        private IEnumerator StartAnimation()
        {
            float animTimer = 0f;
            context.Animator.enabled = false;

            // Back-up the bones props to revert to it after the animation is done
            // What would be ideal is to get the next animation's first keyframe 
            // but they are not in the same format as the legacy anims and i don't have time
            bonesOriginalProperties = LegacyAnimationClipReader.GetBonesPropertiesBackup(bonesTargetProperties, context.AnimatedCharacter);
            isPlaying = true;
            
            yield return LinearInterpolator.BlendTo(bonesTargetProperties);

            while (animTimer < clip.length)
            {
                animTimer += Time.deltaTime;

                clip.SampleAnimation(context.AnimatedCharacter.gameObject, animTimer);

                yield return new WaitForEndOfFrame();                
            }

            // The animation is done, go back to previous pos before the animator is enabled again
            yield return RevertToPreviousState();
        }

        public IEnumerator RevertToPreviousState()
        {
            yield return LinearInterpolator.BlendTo(bonesOriginalProperties);
            
            isPlaying = false;
            context.Animator.enabled = true;
        }

        #endregion
    }
}
