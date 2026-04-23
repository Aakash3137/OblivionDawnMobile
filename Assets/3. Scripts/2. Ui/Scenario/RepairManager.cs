using UnityEngine;

public class RepairManager : MonoBehaviour
{
    public static RepairManager Instance;

    [SerializeField] private RepairButtonHandler RepairPrefab;
    [SerializeField] private GameObject EffectPrefab;
    [SerializeField] internal RepairButtonHandler CurrentRepairBtn;
    [SerializeField] private GameObject CurrentEffect;
    public Camera cam;

    internal bool IsRepairing = false;
    
    private void Awake()
    {
        cam = Camera.main;
        Instance = this;
    }

private void Update()
    {
        if (GameStateManager.Instance.IsGameOver) 
            return;
        
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    CheckClick(touch.position);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                CheckClick(Input.mousePosition);
            }
        }
    }

    void CheckClick(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        bool hasBuilding = false;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.GetComponent<BuildingPlacementHelper>() != null)
            {
                hasBuilding = true;
                break;
            }
        }

        // ❗ If no building hit → close panel
        if (!hasBuilding)
        {
            Debug.Log("Repair Close! No building hit. Closing Repair Button if open.");
            OnClickRepairBtnClose();
        }
    }

    public RepairButtonHandler OnClickRepairBtnOpen(Transform RepairParent, Transform GlowParent, Stats StatsData, bool forRepair)
    {
        if(CurrentRepairBtn == null)
            IsRepairing = false;
        
        if(IsRepairing)
        {
            return null;
        }

        if(CurrentRepairBtn != null && RepairParent == CurrentRepairBtn.transform.parent)
        {
            if(CurrentRepairBtn != null)
            {
                Destroy(CurrentRepairBtn.gameObject);
                IsRepairing = false;
            }
            return null;
        }

        if(CurrentRepairBtn != null)
        {
            Destroy(CurrentRepairBtn.gameObject);
        }


        CurrentRepairBtn = Instantiate(RepairPrefab, RepairParent);
        CurrentRepairBtn.StatsData = StatsData;
        if(StatsData.gameObject.name.Contains("MainBuilding"))
        {
            CurrentRepairBtn.KillBtn.interactable = false;
        }
        CurrentRepairBtn.gameObject.SetActive(true);
        CurrentRepairBtn.EffectPlace = GlowParent;
        CurrentRepairBtn.PlayShow();
        IsRepairing = forRepair;

        return CurrentRepairBtn;
    }

    public void OnClickRepairBtnClose()
    {
        Debug.Log("Closing Repair Button");
        if(CurrentRepairBtn != null)
        {
            CurrentRepairBtn.PlayHide();
            Debug.Log("Repair button closed: "+ CurrentRepairBtn);
            Destroy(CurrentRepairBtn.gameObject, 1f);
            CurrentRepairBtn = null;
        }
    }
}
