using UnityEngine;

public class CameraPanning : MonoBehaviour
{
    [Header("Movement Settings")]
    public float panSpeed = 0.1f;   // Adjust sensitivity for touch

    private Vector2 lastPanPosition;
    private int panFingerId;

    void Update()
    {
        // --- Keyboard input ---
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = new Vector3(moveX, 0f, moveZ) * panSpeed * 50f * Time.deltaTime;
        transform.Translate(move, Space.World);

        // --- Touch input ---
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                panFingerId = touch.fingerId;
                lastPanPosition = touch.position;
            }
            else if (touch.fingerId == panFingerId && touch.phase == TouchPhase.Moved)
            {
                Vector2 delta = touch.position - lastPanPosition;
                lastPanPosition = touch.position;

                // Map screen delta to world X/Z movement
                Vector3 moveTouch = new Vector3(-delta.x, 0f, -delta.y) * panSpeed;

                // Apply both axes together → diagonal works
                transform.Translate(moveTouch, Space.World);
            }
        }
    }
}
