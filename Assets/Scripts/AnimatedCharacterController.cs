using UnityEngine;

namespace TestKinetix {

    [RequireComponent(typeof(CharacterController))]
    public class AnimatedCharacterController : MonoBehaviour
    {
        [Header("Emotes")]
        [SerializeField] private Animator animator;

        
        [Header("Movement")]
        [SerializeField] private Transform cameraOrbiter; // Used to get the camera forward and move relative to that direction
        [SerializeField] private float walkSpeed = 1;
        [Tooltip("Turn speed for the character. The less speed the longer the character will take to turn")]
        [SerializeField] private float faceDirectionRatio = 0.01f;
        private Vector3 movementDirection;

        private CharacterController characterController;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Move();

            if (Input.GetKeyDown(KeyCode.R)) {
                animator.ResetTrigger("CancelEmotes");

                PlayHumanoidEmote();
            }
        }

        private void Move()
        {
            movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

            animator.SetFloat("MoveMagnitude", movementDirection.magnitude);

            if (movementDirection.magnitude > 0) {
                movementDirection = cameraOrbiter.TransformDirection(movementDirection) * -1;
                
                Quaternion wantedRotation = Quaternion.LookRotation(movementDirection, transform.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, faceDirectionRatio);

                CancelEmote();

                characterController.Move(movementDirection * walkSpeed * Time.deltaTime);   
            }            
        }



        #region Emotes

        private void PlayHumanoidEmote()
        {
            animator.SetTrigger("PlayHumanoidEmote");
        }

        private void CancelEmote()
        {
            animator.SetTrigger("CancelEmotes");
        }

        #endregion
    }
}