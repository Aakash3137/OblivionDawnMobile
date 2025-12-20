using UnityEngine;

public class CameraPanning : MonoBehaviour
{
    [Header("Movement Settings")]
    public float mouseSensitivity;   // Adjust sensitivity for touch
    public float touchSensitivity;   // Adjust sensitivity for touch
    public float zoomSpeed;     // Adjust sensitivity for mouse wheel
    public float minZoomIn;     // Adjust min Zoom level
    public float maxZoomOut;     // Adjust max Zoom level

    void Update()
    {
        PanCamera();
        ZoomCamera();
    }
    void PanCamera()
    {
        float keyboardX = Input.GetAxis("Horizontal") * mouseSensitivity;
        float keyboardY = Input.GetAxis("Vertical") * mouseSensitivity;

        transform.position -= transform.right * keyboardX * Time.deltaTime;
        transform.position -= transform.up * keyboardY * Time.deltaTime;
        
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.position -= transform.right * mouseX * Time.deltaTime;
            transform.position -= transform.forward * mouseY * Time.deltaTime;
        }
        if(Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPosition = touch.deltaPosition;
                float touchX = touchDeltaPosition.x * touchSensitivity;
                float touchY = touchDeltaPosition.y * touchSensitivity;
                transform.position -= transform.right * touchX * Time.deltaTime;
                transform.position -= transform.up * touchY * Time.deltaTime;
            }
        }
    }

    void ZoomCamera()
    {
        //get mouse scroll wheel input
        float scroll = Input.mouseScrollDelta.y;

        Camera camera = Camera.main; // or a reference to your specific camera

        if (camera != null)
        {
            // Pinching out (distance increases) decreases orthographic size (zooms in)
            // Pinching in (distance decreases) increases orthographic size (zooms out)
            camera.orthographicSize -= scroll * zoomSpeed;

            // Clamp the size within min/max limits
            camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minZoomIn, maxZoomOut);
        }

        // Check for two touches on the screen
        if (Input.touchCount == 2)
        {
            // Store the touches
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Find the position in the previous frame of each touch
            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            // Calculate the previous distance between touches
            float prevTouchDistance = Vector2.Distance(touch0PrevPos, touch1PrevPos);
            // Calculate the current distance between touches
            float currentTouchDistance = Vector2.Distance(touch0.position, touch1.position);

            // Calculate the difference in distances
            float distanceDelta = currentTouchDistance - prevTouchDistance;

            // Adjust the camera's orthographic size based on the distance change
            if (camera != null)
            {
                // Pinching out (distance increases) decreases orthographic size (zooms in)
                // Pinching in (distance decreases) increases orthographic size (zooms out)
                camera.orthographicSize -= distanceDelta * zoomSpeed;

                // Clamp the size within min/max limits
                camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minZoomIn, maxZoomOut);
            }
        }
    }
}
