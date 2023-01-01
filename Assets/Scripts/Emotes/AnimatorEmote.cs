using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestKinetix
{
    public class AnimatorEmote : IEmote
    {
        private string playTriggerName;
        private string cancelTriggerName;
        private AnimationContext context;

        public AnimatorEmote(string playTriggerName, string cancelTriggerName, AnimationContext context)
        {
            this.playTriggerName = playTriggerName;
            this.cancelTriggerName = cancelTriggerName;
            this.context = context;
        }

        public void Play()
        {
            context.Animator.SetTrigger(playTriggerName);
            context.Animator.ResetTrigger(cancelTriggerName);   
        }

        public IEnumerator Cancel()
        {
            context.Animator.ResetTrigger(playTriggerName);
            context.Animator.SetTrigger(cancelTriggerName);

            yield return null;
        }
    }
}