using UnityEngine;
using UnityEngine.UI;

public class AbilityTargetingSystem : MonoBehaviour
{
    public GameObject targetImageObj;
    public RectTransform targetImage;

    public Button setTargetButton;
    public GameObject launchButtonPanel;
    public Button launchButton;

    public Camera mainCamera;
    public LayerMask groundLayer;

    public TargetDragUI dragScript;

    private bool hasValidTarget;
    private Vector3 targetWorldPos;

    public static bool IsTargetingActive;

    void Start()
    {
        targetImageObj.SetActive(false);
        launchButtonPanel.gameObject.SetActive(false);

        setTargetButton.onClick.AddListener(OnSetTarget);
        launchButton.onClick.AddListener(OnLaunch);
    }

    void Update()
    {
        if (!IsTargetingActive) return;

        UpdateTarget(targetImage.position);
    }

    void OnSetTarget()
    {
        IsTargetingActive = true;

        setTargetButton.gameObject.SetActive(false);
        launchButton.gameObject.SetActive(true);
        targetImageObj.SetActive(true);

        targetImage.anchoredPosition = Vector2.zero;

        dragScript.canDrag = true;
        hasValidTarget = false;
    }

    void OnLaunch()
    {
        if (hasValidTarget)
        {
            Debug.Log("Launch at: " + targetWorldPos);
        }

        ResetAll();
    }

    void ResetAll()
    {
        IsTargetingActive = false;

        targetImageObj.SetActive(false);
        launchButton.gameObject.SetActive(false);
        setTargetButton.gameObject.SetActive(true);

        dragScript.canDrag = false;
        hasValidTarget = false;
    }

    void UpdateTarget(Vector2 screenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            targetWorldPos = hit.point;
            hasValidTarget = true;
        }
        else
        {
            hasValidTarget = false;
        }
    }
}