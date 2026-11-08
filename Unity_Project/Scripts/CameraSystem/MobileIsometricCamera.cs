using UnityEngine;

namespace GolbaharTycoon.CameraSystem
{
    /// <summary>
    /// کنترلر دوربین ارتوگرافیک ایزومتریک موبایل برای یونیتی
    /// پشتیبانی از درگ روان ۳۶۰ درجه، زوم دو انگشتی (Pinch to Zoom) و اینرسی حرکتی
    /// </summary>
    public class MobileIsometricCamera : MonoBehaviour
    {
        [Header("تنظیمات دید")]
        public Camera targetCamera;
        public Vector3 cameraTarget = Vector3.zero;
        public float cameraAngle = 45f; // زاویه ۴۵ درجه ایزومتریک
        public float elevation = 125f;
        public float distance = 160f;

        [Header("تنظیمات زوم")]
        public float zoom = 1.05f;
        public float minZoom = 0.55f;
        public float maxZoom = 2.4f;

        [Header("حرکت و اینرسی")]
        public float panSpeed = 0.52f;
        public float inertiaDamping = 0.88f;

        private Vector2 lastPointerPos;
        private Vector2 dragVelocity;
        private bool isDragging;

        private void Start()
        {
            if (targetCamera == null) targetCamera = GetComponent<Camera>();
            UpdateCameraTransform();
        }

        private void Update()
        {
            HandleTouchInput();
            ApplyInertia();
            UpdateCameraTransform();
        }

        private void HandleTouchInput()
        {
            // ۱ لمس: درگ ۳۶۰ درجه روی نقشه
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    lastPointerPos = touch.position;
                    dragVelocity = Vector2.zero;
                    isDragging = true;
                }
                else if (touch.phase == TouchPhase.Moved && isDragging)
                {
                    Vector2 delta = touch.position - lastPointerPos;
                    lastPointerPos = touch.position;

                    // دوران بردار حرکت متناسب با زاویه دوربین
                    float rad = cameraAngle * Mathf.Deg2Rad;
                    float cosA = Mathf.Cos(rad);
                    float sinA = Mathf.Sin(rad);

                    float factor = panSpeed / zoom;
                    float worldDeltaX = -(delta.x * -sinA + delta.y * cosA) * factor;
                    float worldDeltaZ = -(delta.x *  cosA + delta.y * sinA) * factor;

                    cameraTarget.x += worldDeltaX;
                    cameraTarget.z += worldDeltaZ;
                    dragVelocity = new Vector2(worldDeltaX, worldDeltaZ);
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isDragging = false;
                }
            }
            // ۲ لمس: زوم دو انگشتی (Pinch)
            else if (Input.touchCount == 2)
            {
                isDragging = false;
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                Vector2 prevPos0 = touch0.position - touch0.deltaPosition;
                Vector2 prevPos1 = touch1.position - touch1.deltaPosition;

                float prevDist = (prevPos0 - prevPos1).magnitude;
                float currentDist = (touch0.position - touch1.position).magnitude;
                float diff = currentDist - prevDist;

                zoom = Mathf.Clamp(zoom + diff * 0.002f, minZoom, maxZoom);
            }
        }

        private void ApplyInertia()
        {
            if (!isDragging && dragVelocity.sqrMagnitude > 0.0001f)
            {
                cameraTarget.x += dragVelocity.x;
                cameraTarget.z += dragVelocity.y;
                dragVelocity *= inertiaDamping;
            }
        }

        public void UpdateCameraTransform()
        {
            if (targetCamera == null) return;
            float rad = cameraAngle * Mathf.Deg2Rad;
            Vector3 camPos = new Vector3(
                cameraTarget.x + Mathf.Cos(rad) * distance,
                elevation,
                cameraTarget.z + Mathf.Sin(rad) * distance
            );
            targetCamera.transform.position = camPos;
            targetCamera.transform.LookAt(cameraTarget);
            if (targetCamera.orthographic)
            {
                targetCamera.orthographicSize = 60f / zoom;
            }
        }

        public void FocusOn(Vector3 targetPos, float targetZoom = 1.15f)
        {
            cameraTarget = targetPos;
            zoom = targetZoom;
        }

        public void Rotate90Degrees()
        {
            cameraAngle = (cameraAngle + 90f) % 360f;
        }
    }
}
