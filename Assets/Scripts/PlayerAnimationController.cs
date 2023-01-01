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

        private BlendedLegacyAnimation legacyEmote;

        private void Awake()
        {
            legacyEmote = new BlendedLegacyAnimation(legAnim, new AnimationContext(transform, animator));
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R)) {
                animator.ResetTrigger("CancelEmotes");

                PlayHumanoidEmote();
            } else if (Input.GetKeyDown(KeyCode.T)) {
                CancelEmotes();

                StartCoroutine(legacyEmote.Play());
            }
        }

        #region Emotes

        public void OnMove(float magnitude)
        {
            animator.SetFloat("MoveMagnitude", magnitude);

            if (magnitude > 0) {
                CancelEmotes();
            }
        }

        public void CancelEmotes()
        {
            animator.SetTrigger("CancelEmotes");
            
            animator.enabled = true;
            StopCoroutine(legacyEmote.Play());
        }


        private void PlayHumanoidEmote()
        {
            animator.SetTrigger("PlayHumanoidEmote");
        }

        #endregion
    }
}
