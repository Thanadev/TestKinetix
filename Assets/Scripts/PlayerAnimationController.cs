using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestKinetix
{
    public class PlayerAnimationController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationClip legAnim;

        private LegacyEmote legacyEmote;
        private AnimatorEmote animatorEmote;

        private PlayerMovementController movementController;

        private void Awake()
        {
            legacyEmote = new LegacyEmote(legAnim, new AnimationContext(transform, animator, this));
            animatorEmote = new AnimatorEmote("PlayHumanoidEmote", "CancelEmotes", new AnimationContext(transform, animator, this));
            movementController = GetComponent<PlayerMovementController>();

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R)) {
                PlayEmote(animatorEmote);
            } else if (Input.GetKeyDown(KeyCode.T)) {
                PlayEmote(legacyEmote);
            }
        }


        #region Animation callback

        public void PlayEmote(IEmote emote)
        {
            if (animator.GetFloat("MoveMagnitude") == 0) {
                CancelEmotes();
                movementController.DisableMovement();

                emote.Play();
            }
        }

        public IEnumerator OnMove(float magnitude, Action onTransitionFinishedCallback)
        {
            if (magnitude > 0) {
                yield return CancelEmotes();
                onTransitionFinishedCallback();
            }

            animator.SetFloat("MoveMagnitude", magnitude);
        }

        public IEnumerator CancelEmotes()
        {
            yield return animatorEmote.Cancel();            
            yield return legacyEmote.Cancel();
        }

        #endregion
    }
}
