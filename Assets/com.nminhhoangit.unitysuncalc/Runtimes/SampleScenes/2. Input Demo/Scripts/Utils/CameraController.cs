using UnityEngine;

namespace nminhhoangit.SunCalculator
{
    public class CameraController : MonoBehaviour
    {
        public Transform Target;
        public float ZoomSpeed = 100f;
        public float RotateSpeed = 1f;
        public float MinFieldOfView = 30f;
        public float MaxFieldOfView = 100f;

        private Camera m_MainCamera;
        private float m_InitialDistance;
        private Vector3 m_InitialRotation;
        private bool m_IsRotating = false;
        private Vector3 m_PreviousMousePosition;

        private void Start()
        {
            m_MainCamera = GetComponent<Camera>();
            m_InitialDistance = Vector3.Distance(transform.position, Target?.position ?? Vector3.zero);
            m_InitialRotation = transform.eulerAngles;
        }

        private void Update()
        {
            // Update the camera's field of view
            HandleFieldOfView();

            // Rotate the camera around the target
            HandleRotation();
        }

        private void HandleFieldOfView()
        {
            // Zoom using mouse scroll
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scroll) > 0f)
            {
                float zoomAmount = scroll * ZoomSpeed * Time.deltaTime * 100f;
                float newFieldOfView = Mathf.Clamp(m_MainCamera.fieldOfView - zoomAmount, MinFieldOfView, MaxFieldOfView);
                m_MainCamera.fieldOfView = newFieldOfView;
            }

            // Zoom using touch pinch gesture
            else if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
                Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

                float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
                float touchDeltaMag = (touch1.position - touch2.position).magnitude;

                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
                float zoomAmount = deltaMagnitudeDiff * ZoomSpeed * Time.deltaTime;
                float newFieldOfView = Mathf.Clamp(m_MainCamera.fieldOfView + zoomAmount, MinFieldOfView, MaxFieldOfView);
                m_MainCamera.fieldOfView = newFieldOfView;
            }
        }

        private void HandleRotation()
        {
            if (Input.touchCount == 2)
                return;

            // Rotate using mouse drag
            if (Input.GetMouseButtonDown(0))
            {
                m_IsRotating = true;
                m_PreviousMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                m_IsRotating = false;
            }

            if (m_IsRotating)
            {
                Vector3 currentMousePosition = Input.mousePosition;
                Vector3 mouseDelta = currentMousePosition - m_PreviousMousePosition;

                float rotationX = mouseDelta.x * RotateSpeed;
                float rotationY = mouseDelta.y * RotateSpeed;

                transform.RotateAround(Target?.position ?? Vector3.zero, Vector3.up, rotationX);
                transform.RotateAround(Target?.position ?? Vector3.zero, transform.right, -rotationY);

                m_PreviousMousePosition = currentMousePosition;
            }
        }
    }
}
