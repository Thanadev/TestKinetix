using UnityEngine;

namespace TestKinetix
{

    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementController : MonoBehaviour
    {
        [Header("Movement")]

        [SerializeField]
        private Transform cameraOrbiter; // Used to get the camera forward and move relative to that direction

        [SerializeField]
        private float walkSpeed = 1;

        [Tooltip("Turn speed for the character. The less speed the longer the character will take to turn")]
        [SerializeField]
        private float faceDirectionRatio = 0.01f;
        
        private Vector3 movementDirection;

        private CharacterController characterController;
        private PlayerAnimationController animController;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animController = GetComponent<PlayerAnimationController>();
        }

        private void Update()
        {
            Move();            
        }

        private void Move()
        {
            movementDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

            // Delegate animation to animController
            animController.OnMove(movementDirection.magnitude);

            if (movementDirection.magnitude > 0) {
                // Determine the direction of movement
                movementDirection = cameraOrbiter.TransformDirection(movementDirection) * -1;
                
                // Rotate to face direction
                Quaternion wantedRotation = Quaternion.LookRotation(movementDirection, transform.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, faceDirectionRatio);

                characterController.Move(movementDirection * walkSpeed * Time.deltaTime);   
            }            
        }
    }
}
