using UnityEngine;

namespace TestKinetix {

    /**
    *   Should be on a parent object containing the actual camera
    */
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform targetCamera;
        [SerializeField] private Transform lookAtTarget;
        [SerializeField] private float cameraRotationSpeed = 2f;


        private void Update()
        {
            UpdateTargetCameraRotation();

            UpdateOrbiterPosition();

            UpdateOrbiterAngleFromUserInput();
        }


        private void UpdateTargetCameraRotation()
        {
            targetCamera.LookAt(lookAtTarget);
        }

        
        private void UpdateOrbiterPosition()
        {
            transform.position = lookAtTarget.position;
        }


        private void UpdateOrbiterAngleFromUserInput()
        {
            float wantedYAngle = transform.eulerAngles.y + cameraRotationSpeed * Input.GetAxis("Mouse X");

            Vector3 wantedRotationEuler = new Vector3(0, wantedYAngle, 0);

            transform.localRotation = Quaternion.Euler(wantedRotationEuler); 
        }
    }
}