using UnityEngine;

namespace nminhhoangit.SunCalculator
{
    public class FaceCamera : MonoBehaviour
    {
        private void LateUpdate()
        {
            // Get the direction from the child object to the camera
            Vector3 cameraDirection = Camera.main.transform.position - transform.position;

            // Rotate the child object to face the camera direction
            transform.rotation = Quaternion.LookRotation(cameraDirection, Vector3.up);
        }
    }
}