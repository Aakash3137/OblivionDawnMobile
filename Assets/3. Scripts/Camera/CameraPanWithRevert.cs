using UnityEngine;

public class CameraPanWithRevert : MonoBehaviour
{
    [Header("Pan Limits (relative to starting position)")]
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -10f;
    public float maxY = 10f;

    [Header("Pan Settings")]
    public float panSpeed = 0.5f;
    public float panSmoothness = 10f;

    [Header("Auto Revert Settings")]
    public float revertDelay = 3f;
    public float revertSpeed = 1.5f;
    public float revertSmoothness = 5f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private float revertTimer = 0f;
    private bool isPanning = false;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos;
    }

    void Update()
    {
        HandleInput();
        HandleRevert();
        ApplySmoothMovement();
    }

    // -----------------------------
    // INPUT HANDLING
    // -----------------------------
    void HandleInput()
    {
        Vector2 delta = Vector2.zero;

#if UNITY_EDITOR || UNITY_STANDALONE
        // Mouse drag support for testing in editor
        if (Input.GetMouseButton(0))
        {
            delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            isPanning = true;
        }
        else
        {
            isPanning = false;
        }
#else
        // Touch screen support
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);

            if (t.phase == TouchPhase.Moved)
            {
                delta = t.deltaPosition / Screen.dpi; 
                isPanning = true;
            }
            else
            {
                isPanning = false;
            }
        }
#endif

        if (isPanning)
        {
            revertTimer = 0f; // reset revert timer

            // Calculate target position based on drag
            Vector3 move = new Vector3(-delta.x, -delta.y, 0f) * panSpeed;

            targetPos += move;

            // Clamp movement relative to starting position
            targetPos.x = Mathf.Clamp(targetPos.x, startPos.x + minX, startPos.x + maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, startPos.y + minY, startPos.y + maxY);
        }
    }

    // -----------------------------
    // AUTO REVERT LOGIC
    // -----------------------------
    void HandleRevert()
    {
        if (!isPanning)
        {
            revertTimer += Time.deltaTime;

            if (revertTimer >= revertDelay)
            {
                // Gradually move target back to start
                targetPos = Vector3.Lerp(
                    targetPos,
                    startPos,
                    Time.deltaTime * revertSpeed
                );
            }
        }
    }

    // -----------------------------
    // APPLY CAMERA MOVEMENT
    // -----------------------------
    void ApplySmoothMovement()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * (isPanning ? panSmoothness : revertSmoothness)
        );
    }
}
