using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Cinemachine;
using LitMotion;
using LitMotion.Extensions;

public class CameraPanningPerspective : MonoBehaviour
{
    [Header("Movement")]
    public float mouseSensitivity = 15f;

    [Header("Zoom")]
    public float mouseZoomSpeed = 1.2f;
    public float touchZoomSpeed = 0.02f;

    [Header("Clamp Bounds")]
    public Vector3 cameraMinBounds = new Vector3(-8, 5, -8);
    public Vector3 cameraMaxBounds = new Vector3(40, 25, 40);

    [Header("Idle Reset")]
    public float maxIdleDuration = 10f;
    public float smoothingSpeed = 2f;

    private Vector3 initialCameraPosition;
    private Vector3 lastGroundPoint;
    private Vector3 lastTouchGroundPoint;

    private float idleTimer;

    private bool isPanning;
    private bool isZooming;

    private CinemachineCamera cam;
    MotionHandle cameraResetHandle;


    // Adding this functionality
    // If user drags → NEVER open build panel
    // If user taps and releases without dragging → open build panel 

    public static bool IsDragging;

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

        if (Input.touchCount == 0 && !Input.GetMouseButton(0))
        {
            IsDragging = false;
        }

    }

    // -------------------------
    // PAN LOGIC
    // -------------------------
    void PanCamera()
    {
        if (IsPointerOverUI())
            return;
        
        if (AbilityTargetingSystem.IsTargetingActive)
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

            // transform.position = new Vector3(
            //     Mathf.Clamp(transform.position.x, cameraMinBounds.x, cameraMaxBounds.x),
            //     Mathf.Clamp(transform.position.y, cameraMinBounds.y, cameraMaxBounds.y),
            //     Mathf.Clamp(transform.position.z, cameraMinBounds.z, cameraMaxBounds.z));

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
    private void ZoomCamera()
    {
        
        if (cam == null)
            return;

        // float oldSize = cam.Lens.OrthographicSize;
        // Vector3 zoomFocusWorldPos = Vector3.zero;

        // Mobile: two-finger pinch
        if (Input.touchCount == 2)
        {
            isZooming = true;
            isPanning = false;

            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            // Vector2 midPoint = (t0.position + t1.position) * 0.5f;
            // zoomFocusWorldPos = GetGroundPointFromScreen(midPoint);

            Vector2 t0Prev = t0.position - t0.deltaPosition;
            Vector2 t1Prev = t1.position - t1.deltaPosition;

            float prevDist = Vector2.Distance(t0Prev, t1Prev);
            float currDist = Vector2.Distance(t0.position, t1.position);

            float delta = currDist - prevDist;

            Vector3 oldPosition = transform.position;

            transform.position += transform.forward * delta * touchZoomSpeed;

            // Clamp
            Vector3 clampedPos = new Vector3(
                Mathf.Clamp(transform.position.x, cameraMinBounds.x, cameraMaxBounds.x),
                Mathf.Clamp(transform.position.y, cameraMinBounds.y, cameraMaxBounds.y),
                Mathf.Clamp(transform.position.z, cameraMinBounds.z, cameraMaxBounds.z)
            );

            if (clampedPos != transform.position)
            {
                transform.position = oldPosition;
                return;
            }

            transform.position = clampedPos;
        }
        else
        {
            isZooming = false;
        }

        // Desktop: mouse wheel
        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0f)
        {
            Vector3 oldPosition = transform.position;

            transform.position += transform.forward * scroll * mouseZoomSpeed;

            // Clamp
            Vector3 clampedPos = new Vector3(
                Mathf.Clamp(transform.position.x, cameraMinBounds.x, cameraMaxBounds.x),
                Mathf.Clamp(transform.position.y, cameraMinBounds.y, cameraMaxBounds.y),
                Mathf.Clamp(transform.position.z, cameraMinBounds.z, cameraMaxBounds.z)
            );

            if (clampedPos != transform.position)
            {
                transform.position = oldPosition;
                return;
            }

            transform.position = clampedPos;
        }

        // Clamp zoom
        // cam.Lens.OrthographicSize = Mathf.Clamp(cam.Lens.OrthographicSize, minZoomIn, maxZoomOut);
    }

    // -------------------------
    // CAMERA MOVEMENT + FIXED CLAMP
    // -------------------------
    void MoveCamera(Vector3 delta)
    {
        if (delta.sqrMagnitude > 0.0001f)
            IsDragging = true;

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
        if (Vector3.Distance(transform.position, initialCameraPosition) < 0.05f)
            return;

        bool hasInput = Input.anyKey || Input.touchCount > 0 || Input.mouseScrollDelta.y != 0f;

        if (hasInput)
        {
            idleTimer = 0f;
            cameraResetHandle.TryCancel();
            return;
        }

        idleTimer += Time.deltaTime;

        if (idleTimer >= maxIdleDuration && !cameraResetHandle.IsPlaying())
        {
            StartCameraReset();

            // transform.position = Vector3.Lerp(transform.position, initialCameraPosition, smoothingSpeed * Time.deltaTime);
            // cam.Lens.OrthographicSize = Mathf.Lerp(cam.Lens.OrthographicSize, defaultZoom, smoothingSpeed * Time.deltaTime);
        }
    }

    void StartCameraReset()
    {
        cameraResetHandle.TryCancel();

        cameraResetHandle = LMotion.Create(transform.position, initialCameraPosition, smoothingSpeed)
            .WithEase(Ease.InOutCubic)
            .BindToPosition(transform)
            .AddTo(this);
    }

}
