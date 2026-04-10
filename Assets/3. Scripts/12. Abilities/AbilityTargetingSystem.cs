using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class AbilityTargetingSystem : MonoBehaviour
{
    [Header("UI")]
    public GameObject targetImageObj;
    public RectTransform targetImage;
    public Canvas canvas;

    public Button setTargetButton;
    public GameObject launchButtonPanel;
    public Button launchButton;

    [Header("World")]
    public Camera mainCamera;
    public LayerMask groundLayer;
    public GameObject vfxPrefab;

    [Header("Refs")]
    public TargetDragUI dragScript;
    
    [Header("Damage")]
    public float damage = 1000f;

    public float damageArea = 10f;
    

    [HideInInspector] public Vector3 lastRayOrigin;
    [HideInInspector] public Vector3 lastRayHitPoint;
    
    [HideInInspector] public Vector3 defaultRayOrigin;
    [HideInInspector] public Vector3 defaultRayHitPoint;
    
    private bool hasValidTarget;
    private Vector3 targetWorldPos;

    private bool justActivated;
    
    public static bool IsTargetingActive;
    internal bool onlyonce = true;
    void Start()
    {
        targetImageObj.SetActive(false);
        launchButtonPanel.SetActive(false);

        setTargetButton.onClick.AddListener(OnSetTarget);
        launchButton.onClick.AddListener(OnLaunch);
    }

    void Update()
    {
        if (!IsTargetingActive) return;
        if (TargetDragUI.IsDraggingUI) return;

        Vector2 screenPos = GetScreenPositionFromUI();
        
        UpdateTarget(screenPos);
        HandleOutsideClick();
    }
    
    Vector2 GetScreenPositionFromUI()
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            
            // Get the target's position relative to canvas center
            Vector2 localPos = targetImage.anchoredPosition;
            
            // Canvas pivot is at center (0.5, 0.5), convert to screen space
            // Screen space origin is bottom-left
            Vector2 screenPos = new Vector2(
                localPos.x + Screen.width / 2f,
                localPos.y + Screen.height / 2f
            );
            return screenPos;
        }
        else
        {
            // For Camera canvas
            return RectTransformUtility.WorldToScreenPoint(canvas.worldCamera ?? mainCamera, targetImage.position);
        }
    }

    void OnSetTarget()
    {
        IsTargetingActive = true;
        justActivated = true;
        
        setTargetButton.gameObject.SetActive(false);
        launchButtonPanel.SetActive(true);
        targetImageObj.SetActive(true);

        targetImage.anchoredPosition = Vector2.zero;

        dragScript.canDrag = true;

        InitializeDefaultRaycast();
        
        Vector2 screenPos = GetScreenPositionFromUI();
        UpdateTarget(screenPos);
        
        Invoke(nameof(ClearActivationFlag), 0.1f);
    }
    
    void ClearActivationFlag()
    {
        justActivated = false;
    }

    void OnLaunch()
    {
        if (hasValidTarget)
        {
            targetWorldPos += Vector3.up * 0.5f;
            Debug.Log("Spawning VFX at: " + targetWorldPos);
        
            GameObject vfx = Instantiate(vfxPrefab, targetWorldPos, Quaternion.identity);
            Destroy(vfx, 3f);

            // Lightning AoE Damage on Launch 
            Collider[] hitColliders = Physics.OverlapSphere(targetWorldPos, damageArea);

            foreach (Collider col in hitColliders)
            {
                Stats stats = col.GetComponent<Stats>();
                if (stats != null && stats.side == Side.Enemy)
                {
                    float distance = Vector3.Distance(targetWorldPos, col.transform.position);
                    float finalDamage = 0f;

                    if (distance <= 4f)
                    {
                        finalDamage = damage; // 100%
                    }
                    else if (distance <= 6f)
                    {
                        finalDamage = damage * 0.8f; // 80%
                    }
                    else if (distance <= 8f)
                    {
                        finalDamage = damage * 0.4f; // 40%
                    }
                    else if (distance <= 10f)
                    {
                        finalDamage = damage * 0.2f; // 20%
                    }
                    else
                    {
                        continue; // outside range
                    }

                    stats.TakeDamage(finalDamage);

                    Debug.Log($"Hit {col.name} | Distance: {distance:F2} | Damage: {finalDamage}");
                }
            }
        }
        else
        {
            Debug.LogWarning("No valid target to launch!");
        }

        ResetAll();
    }

    void ResetAll()
    {
        IsTargetingActive = false;

        targetImageObj.SetActive(false);
        launchButtonPanel.SetActive(false);
        setTargetButton.gameObject.SetActive(true);

        dragScript.canDrag = false;
        hasValidTarget = false;
        
        lastRayOrigin =  defaultRayOrigin;
        lastRayHitPoint = defaultRayHitPoint;
        

        targetImage.anchoredPosition = Vector2.zero;
    }

    void InitializeDefaultRaycast()
    {
        Vector2 screenPos = GetScreenPositionFromUI();

        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {

            lastRayOrigin = ray.origin;
            lastRayHitPoint = hit.point;

            defaultRayOrigin = ray.origin;
            defaultRayHitPoint = hit.point;

            targetWorldPos = hit.point;
            hasValidTarget = true;
        }
    }
    
    void UpdateTarget(Vector2 screenPos)
    {
        if (lastRayHitPoint != Vector3.zero)
        {
            targetWorldPos = lastRayHitPoint;
            hasValidTarget = true;

            Debug.DrawLine(lastRayOrigin, lastRayHitPoint, Color.green, 0.5f);
            Debug.DrawRay(lastRayHitPoint, Vector3.up * 2f, Color.blue, 0.5f);
        }
        else
        {
            hasValidTarget = false;
        }
    }

    public void SetTargetPosition(Vector3 worldPos)
    {
        targetWorldPos = worldPos;
        hasValidTarget = true;
    }

    void HandleOutsideClick()
    {
        if (justActivated) return;
        if (CameraPanningPerspective.IsDragging) return;
        if (TargetDragUI.IsDraggingUI) return;
        
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOnTarget())
                ResetAll();
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (!IsPointerOnTarget())
                ResetAll();
        }
    }

    bool IsPointerOnTarget()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var r in results)
        {
            // Target Image
            if (r.gameObject == targetImageObj ||
                r.gameObject.transform.IsChildOf(targetImageObj.transform))
                return true;

            // Launch Button Panel (IMPORTANT FIX)
            if (r.gameObject == launchButtonPanel ||
                r.gameObject.transform.IsChildOf(launchButtonPanel.transform))
                return true;
        }

        return false;
    }
}