using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[System.Serializable]
public class PlayerFactionSpecialAbilitySet
{
    public FactionName faction;
    public List<SpecialAbilityData> abilities;
}

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

    [Header("Abilities")]
    public List<PlayerFactionSpecialAbilitySet> factionAbilities;
    public SpecialAbilityType currentAbilityType;

    private List<SpecialAbilityData> currentFactionAbilities = new List<SpecialAbilityData>();
    private int abilityIndex = 0;

    private SpecialAbilityData currentAbility;

    [Header("Refs")]
    public TargetDragUI dragScript;

    [HideInInspector] public Vector3 lastRayOrigin;
    [HideInInspector] public Vector3 lastRayHitPoint;

    private Vector3 defaultRayOrigin;
    private Vector3 defaultRayHitPoint;

    private bool hasValidTarget;
    private Vector3 targetWorldPos;
    private bool justActivated;

    public static bool IsTargetingActive;

    void Start()
    {
        targetImageObj.SetActive(false);
        launchButtonPanel.SetActive(false);

        setTargetButton.onClick.AddListener(OnSetTarget);
        launchButton.onClick.AddListener(OnLaunch);

        LoadFactionAbilities();
        SetNextAbility();
    }

    void Update()
    {
        if (!IsTargetingActive) return;
        if (TargetDragUI.IsDraggingUI) return;

        Vector2 screenPos = GetScreenPositionFromUI();
        UpdateTarget(screenPos);

        HandleOutsideClick();
    }

    void LoadFactionAbilities()
    {
        currentFactionAbilities.Clear();

        foreach (var set in factionAbilities)
        {
            if (set.faction == GameData.playerFaction)
            {
                currentFactionAbilities = set.abilities;
                break;
            }
        }
    }

    void SetNextAbility()
    {
        if (currentFactionAbilities == null || currentFactionAbilities.Count == 0)
        {
            currentAbility = null;
            return;
        }

        abilityIndex = Mathf.Clamp(abilityIndex, 0, currentFactionAbilities.Count - 1);

        currentAbility = currentFactionAbilities[abilityIndex];
        currentAbilityType = currentAbility.type;

        if (currentFactionAbilities.Count > 1)
            abilityIndex = (abilityIndex + 1) % currentFactionAbilities.Count;

        Image img = setTargetButton.GetComponent<Image>();
        if (img != null && currentAbility.targetSprite != null)
            img.sprite = currentAbility.targetSprite;
    }

    void OnSetTarget()
    {
        if (currentAbility == null)
        {
            Debug.LogWarning("No ability selected!");
            return;
        }

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
        if (!hasValidTarget || currentAbility == null)
        {
            Debug.LogWarning("No valid target or ability!");
            ResetAll();
            return;
        }

        targetWorldPos += Vector3.up * 0.5f;

        GameObject vfx = Instantiate(currentAbility.vfxPrefab, targetWorldPos, Quaternion.identity);
        Destroy(vfx, 5f);

        AbilitySetController.Instance.OnSpecialAbilityUsed();

        ApplyDamage();

        SetNextAbility();

        ResetAll();
    }

    void ApplyDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(targetWorldPos, currentAbility.damageArea);

        foreach (Collider col in hitColliders)
        {
            Stats stats = col.GetComponent<Stats>();
            if (stats == null || stats.side != Side.Enemy)
                continue;

            float distance = Vector3.Distance(targetWorldPos, col.transform.position);
            float finalDamage = 0f;

            float radius = currentAbility.damageArea;
            float baseDamage = currentAbility.damage;

            if (distance <= radius * 0.4f)
                finalDamage = baseDamage;
            else if (distance <= radius * 0.6f)
                finalDamage = baseDamage * 0.8f;
            else if (distance <= radius * 0.8f)
                finalDamage = baseDamage * 0.4f;
            else if (distance <= radius)
                finalDamage = baseDamage * 0.2f;
            else
                continue;

            stats.TakeDamage(finalDamage);

            Debug.Log($"[{currentAbility.type}] Hit {col.name} | Damage: {finalDamage}");
        }
    }

    void ResetAll()
    {
        IsTargetingActive = false;

        targetImageObj.SetActive(false);
        launchButtonPanel.SetActive(false);
        setTargetButton.gameObject.SetActive(true);

        dragScript.canDrag = false;
        hasValidTarget = false;

        lastRayOrigin = defaultRayOrigin;
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

    Vector2 GetScreenPositionFromUI()
    {
        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            Vector2 localPos = targetImage.anchoredPosition;

            return new Vector2(
                localPos.x + Screen.width / 2f,
                localPos.y + Screen.height / 2f
            );
        }
        else
        {
            return RectTransformUtility.WorldToScreenPoint(
                canvas.worldCamera ?? mainCamera,
                targetImage.position
            );
        }
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
            if (r.gameObject == targetImageObj ||
                r.gameObject.transform.IsChildOf(targetImageObj.transform))
                return true;

            if (r.gameObject == launchButtonPanel ||
                r.gameObject.transform.IsChildOf(launchButtonPanel.transform))
                return true;
        }

        return false;
    }
}