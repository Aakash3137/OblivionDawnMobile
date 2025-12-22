// using UnityEngine;
// using UnityEngine.EventSystems;
// using Unity.Cinemachine;

// public class CameraPanning : MonoBehaviour
// {
//     [Header("Movement")]
//     public float mouseSensitivity = 15f;

//     [Header("Zoom")]
//     public float mouseZoomSpeed = 0.1f;
//     public float touchZoomSpeed = 0.01f;
//     public float minZoomIn = 5f;
//     public float maxZoomOut = 15f;

//     [Header("Clamp Bounds")]
//     public Vector3 cameraMinBounds = new Vector3(-5, 10, -5);
//     public Vector3 cameraMaxBounds = new Vector3(25, 10, 25);

//     [Header("Idle Reset")]
//     public float maxIdleDuration = 3f;
//     public float smoothingSpeed = 5f;

//     private Vector3 initialCameraPosition;
//     private Vector3 lastGroundPoint;
//     private Vector3 lastTouchGroundPoint;

//     private float idleTimer;

//     private bool isPanning;
//     private bool isZooming;

//     private CinemachineCamera cam;

//     void Start()
//     {
//         initialCameraPosition = transform.position;
//         cam = GetComponentInChildren<CinemachineCamera>();
//     }

//     void Update()
//     {
//         PanCamera();
//         ZoomCamera();
//         ResetCamera();
//     }

//     // --------------------------------------------------
//     // PAN LOGIC (1 finger pan, mouse drag, keyboard)
//     // --------------------------------------------------
//     void PanCamera()
//     {
//         if (IsPointerOverUI())
//             return;

//         // ----- MOBILE (ONE FINGER PAN) -----
//         if (Input.touchCount == 1 && !isZooming)
//         {
//             Touch touch = Input.GetTouch(0);

//             if (touch.phase == TouchPhase.Began)
//             {
//                 isPanning = true;
//                 lastTouchGroundPoint = GetGroundPoint();
//             }
//             else if (touch.phase == TouchPhase.Moved && isPanning)
//             {
//                 Vector3 current = GetGroundPoint();
//                 Vector3 delta = lastTouchGroundPoint - current;
//                 MoveCamera(delta);
//             }
//             else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
//             {
//                 isPanning = false;
//             }

//             return;
//         }

//         // ----- DESKTOP (MOUSE + KEYBOARD) -----
//         if (Input.touchCount == 0)
//         {
//             float keyboardX = Input.GetAxis("Horizontal") * mouseSensitivity;
//             float keyboardY = Input.GetAxis("Vertical") * mouseSensitivity;

//             transform.position += transform.right * keyboardX * Time.deltaTime;
//             transform.position += transform.forward * keyboardY * Time.deltaTime;

//             if (Input.GetMouseButtonDown(0))
//             {
//                 lastGroundPoint = GetGroundPoint();
//             }

//             if (Input.GetMouseButton(0))
//             {
//                 Vector3 current = GetGroundPoint();
//                 Vector3 delta = lastGroundPoint - current;
//                 MoveCamera(delta);
//             }
//         }
//     }

//     // --------------------------------------------------
//     // ZOOM LOGIC (2 finger pinch, mouse wheel)
//     // --------------------------------------------------
//     void ZoomCamera()
//     {
//         if (cam == null)
//             return;

//         // ----- MOBILE PINCH -----
//         if (Input.touchCount == 2)
//         {
//             isZooming = true;
//             isPanning = false;

//             Touch t0 = Input.GetTouch(0);
//             Touch t1 = Input.GetTouch(1);

//             Vector2 t0Prev = t0.position - t0.deltaPosition;
//             Vector2 t1Prev = t1.position - t1.deltaPosition;

//             float prevDist = Vector2.Distance(t0Prev, t1Prev);
//             float currDist = Vector2.Distance(t0.position, t1.position);

//             float delta = currDist - prevDist;
//             cam.Lens.OrthographicSize -= delta * touchZoomSpeed;
//         }
//         else
//         {
//             isZooming = false;
//         }

//         // ----- MOUSE WHEEL -----
//         float scroll = Input.mouseScrollDelta.y;
//         if (scroll != 0f)
//         {
//             cam.Lens.OrthographicSize -= scroll * mouseZoomSpeed;
//         }

//         cam.Lens.OrthographicSize = Mathf.Clamp(
//             cam.Lens.OrthographicSize,
//             minZoomIn,
//             maxZoomOut
//         );
//     }

//     // --------------------------------------------------
//     // CAMERA MOVEMENT + FIXED CLAMP
//     // --------------------------------------------------
//     void MoveCamera(Vector3 delta)
//     {
//         transform.position += new Vector3(delta.x, 0f, delta.z);
//         // Clamp the camera position within the bounds
//         transform.position = new Vector3(
//             Mathf.Clamp(transform.position.x, cameraMinBounds.x, cameraMaxBounds.x),
//             Mathf.Clamp(transform.position.y, cameraMinBounds.y, cameraMaxBounds.y),
//             Mathf.Clamp(transform.position.z, cameraMinBounds.z, cameraMaxBounds.z)
//         );
//     }

//     // --------------------------------------------------
//     // GROUND PROJECTION
//     // --------------------------------------------------
//     Vector3 GetGroundPoint()
//     {
//         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//         Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

//         if (groundPlane.Raycast(ray, out float distance))
//         {
//             return ray.GetPoint(distance);
//         }

//         return Vector3.zero;
//     }

//     // --------------------------------------------------
//     // UI BLOCK
//     // --------------------------------------------------
//     bool IsPointerOverUI()
//     {
//         if (EventSystem.current == null)
//             return false;

//         if (EventSystem.current.IsPointerOverGameObject())
//             return true;

//         if (Input.touchCount > 0)
//             return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

//         return false;
//     }

//     // --------------------------------------------------
//     // IDLE RESET
//     // --------------------------------------------------
//     void ResetCamera()
//     {
//         if (Input.anyKey || Input.touchCount > 0)
//         {
//             idleTimer = 0f;
//             return;
//         }

//         idleTimer += Time.deltaTime;

//         if (idleTimer >= maxIdleDuration)
//         {
//             transform.position = Vector3.Lerp(
//                 transform.position,
//                 initialCameraPosition,
//                 smoothingSpeed * Time.deltaTime
//             );
//         }
//     }
// }






using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Cinemachine;

public class CameraPanning : MonoBehaviour
{
    [Header("Movement")]
    public float mouseSensitivity = 15f;

    [Header("Zoom")]
    public float mouseZoomSpeed = 0.1f;
    public float touchZoomSpeed = 0.01f;
    public float minZoomIn = 5f;
    public float maxZoomOut = 12.5f;

    [Header("Clamp Bounds")]
    public Vector3 cameraMinBounds = new Vector3(-5, 10, -5);
    public Vector3 cameraMaxBounds = new Vector3(25, 10, 25);

    [Header("Idle Reset")]
    public float maxIdleDuration = 3f;
    public float smoothingSpeed = 5f;

    private Vector3 initialCameraPosition;
    private Vector3 lastGroundPoint;
    private Vector3 lastTouchGroundPoint;

    private float idleTimer;

    private bool isPanning;
    private bool isZooming;

    private CinemachineCamera cam;

    void Start()
    {
        initialCameraPosition = transform.position;
        cam = GetComponentInChildren<CinemachineCamera>();
    }

    void Update()
    {
        PanCamera();
        ZoomCamera();
        ResetCamera();
    }

    // -------------------------
    // PAN LOGIC
    // -------------------------
    void PanCamera()
    {
        if (IsPointerOverUI())
            return;

        // Mobile: one-finger pan
        if (Input.touchCount == 1 && !isZooming)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isPanning = true;
                lastTouchGroundPoint = GetGroundPointFromScreen(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved && isPanning)
            {
                Vector3 current = GetGroundPointFromScreen(touch.position);
                Vector3 delta = lastTouchGroundPoint - current;
                MoveCamera(delta);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isPanning = false;
            }

            return;
        }

        // Desktop: mouse + keyboard
        if (Input.touchCount == 0)
        {
            float keyboardX = Input.GetAxis("Horizontal") * mouseSensitivity;
            float keyboardY = Input.GetAxis("Vertical") * mouseSensitivity;

            transform.position += transform.right * keyboardX * Time.deltaTime;
            transform.position += transform.forward * keyboardY * Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            {
                lastGroundPoint = GetGroundPointFromScreen(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 current = GetGroundPointFromScreen(Input.mousePosition);
                Vector3 delta = lastGroundPoint - current;
                MoveCamera(delta);
            }
        }
    }

    // --------------------------------------------------
    // ZOOM LOGIC (2 finger pinch, mouse wheel)
    // --------------------------------------------------
    void ZoomCamera()
    {
        if (cam == null)
            return;

        float oldSize = cam.Lens.OrthographicSize;
        Vector3 zoomFocusWorldPos = Vector3.zero;
        bool zooming = false;

        // Mobile: two-finger pinch
        if (Input.touchCount == 2)
        {
            isZooming = true;
            isPanning = false;

            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 midPoint = (t0.position + t1.position) * 0.5f;
            zoomFocusWorldPos = GetGroundPointFromScreen(midPoint);

            Vector2 t0Prev = t0.position - t0.deltaPosition;
            Vector2 t1Prev = t1.position - t1.deltaPosition;

            float prevDist = Vector2.Distance(t0Prev, t1Prev);
            float currDist = Vector2.Distance(t0.position, t1.position);

            float delta = currDist - prevDist;
            cam.Lens.OrthographicSize -= delta * touchZoomSpeed;
            zooming = true;
        }
        else
        {
            isZooming = false;
        }

        // Desktop: mouse wheel
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0f)
        {
            zoomFocusWorldPos = GetGroundPointFromScreen(Input.mousePosition);
            cam.Lens.OrthographicSize -= scroll * mouseZoomSpeed;
            zooming = true;
        }

        if (!zooming)
            return;

        // Clamp zoom
        cam.Lens.OrthographicSize = Mathf.Clamp(cam.Lens.OrthographicSize, minZoomIn, maxZoomOut);

        // Move camera to keep focus under finger / cursor
        float newSize = cam.Lens.OrthographicSize;
        float zoomRatio = newSize / oldSize;

        Vector3 camToFocus = transform.position - zoomFocusWorldPos;
        Vector3 newCamPos = zoomFocusWorldPos + camToFocus * zoomRatio;

        // Apply fixed bounds
        newCamPos.x = Mathf.Clamp(newCamPos.x, cameraMinBounds.x, cameraMaxBounds.x);
        newCamPos.y = Mathf.Clamp(newCamPos.y, cameraMinBounds.y, cameraMaxBounds.y);
        newCamPos.z = Mathf.Clamp(newCamPos.z, cameraMinBounds.z, cameraMaxBounds.z);

        transform.position = newCamPos;
    }

    // -------------------------
    // CAMERA MOVEMENT + FIXED CLAMP
    // -------------------------
    void MoveCamera(Vector3 delta)
    {
        transform.position += delta;
        // Apply fixed bounds
        transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, cameraMinBounds.x, cameraMaxBounds.x), 
                Mathf.Clamp(transform.position.y, cameraMinBounds.y, cameraMaxBounds.y), 
                Mathf.Clamp(transform.position.z, cameraMinBounds.z, cameraMaxBounds.z));
    }

    // -------------------------
    // PROJECT SCREEN POINT TO GROUND
    // -------------------------
    Vector3 GetGroundPointFromScreen(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return Vector3.zero;
    }

    // -------------------------
    // UI BLOCK
    // -------------------------
    bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        if (EventSystem.current.IsPointerOverGameObject())
            return true;

        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        return false;
    }

    // -------------------------
    // IDLE RESET
    // -------------------------
    void ResetCamera()
    {
        if (Input.anyKey || Input.touchCount > 0)
        {
            idleTimer = 0f;
            return;
        }

        idleTimer += Time.deltaTime;

        if (idleTimer >= maxIdleDuration)
        {
            transform.position = Vector3.Lerp(transform.position, initialCameraPosition, smoothingSpeed * Time.deltaTime);
        }
    }
}
