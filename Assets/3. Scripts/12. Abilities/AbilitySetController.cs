using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySetController : MonoBehaviour
{
    public static AbilitySetController Instance { get; private set; }

    [Header("Slots (Assign in Inspector)")]
    [SerializeField] private Transform slotCost1;
    [SerializeField] private Transform slotCost2;
    [SerializeField] private Transform slotCost3;
    [SerializeField] private Transform slotCost4;

    [Header("UI")]
    [SerializeField] private Image fillBar;
    [SerializeField] private Button specialAbilityButton;

    private List<Button> allButtons = new List<Button>();

    private List<Button> abilitiesCost1 = new List<Button>();
    private List<Button> abilitiesCost2 = new List<Button>();
    private List<Button> abilitiesCost3 = new List<Button>();
    private List<Button> abilitiesCost4 = new List<Button>();

    private Button currentCost1;
    private Button currentCost2;
    private Button currentCost3;
    private Button currentCost4;

    private float currentFill = 0f;
    
    
    private void Awake()
    {
        Instance = this;
        
        if (specialAbilityButton != null)
            specialAbilityButton.interactable = false;
    }

    private void OnEnable()
    {
        KillCounterManager.OnUnitKilled += HandleUnitKilled;
    }

    private void OnDisable()
    {
        KillCounterManager.OnUnitKilled -= HandleUnitKilled;
    }

    private void Start()
    {
        StartCoroutine(InitRoutine());
    }

    private IEnumerator InitRoutine()
    {
        while (AbilityManager.Instance == null)
            yield return null;

        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            RefreshPools();

            if (abilitiesCost1.Count > 0 || abilitiesCost2.Count > 0 ||
                abilitiesCost3.Count > 0 || abilitiesCost4.Count > 0)
                break;
        }

        specialAbilityButton.interactable = false;

        SetupInitialButtons();
        
        if (specialAbilityButton != null)
            specialAbilityButton.interactable = false;
    }

    private void Update()
    {
        if (AbilityManager.Instance == null) return;

        if (AbilityManager.Instance.AbilityButtons.Count != allButtons.Count)
        {
            RefreshPools();
            TryFillEmptySlots();
        }
    }

    
    private void RefreshPools()
    {
        allButtons = new List<Button>(AbilityManager.Instance.AbilityButtons);

        abilitiesCost1.Clear();
        abilitiesCost2.Clear();
        abilitiesCost3.Clear();
        abilitiesCost4.Clear();

        foreach (var btn in allButtons)
        {
            if (btn == null) continue;

            AbilityButtonLink link = btn.GetComponent<AbilityButtonLink>();
            if (link == null || link.ability == null) continue;

            switch (link.ability.abilityCost)
            {
                case 1: abilitiesCost1.Add(btn); break;
                case 2: abilitiesCost2.Add(btn); break;
                case 3: abilitiesCost3.Add(btn); break;
                case 4: abilitiesCost4.Add(btn); break;
            }
        }

        HideAllExceptCurrent();
    }

    private void HideAllExceptCurrent()
    {
        foreach (var btn in allButtons)
        {
            if (btn == null) continue;
            
            if (btn != currentCost1 && btn != currentCost2 &&
                btn != currentCost3 && btn != currentCost4)
            {
                btn.gameObject.SetActive(false);
            }
        }
    }

    
    private void SetupInitialButtons()
    {
        currentCost1 = PickAndShow(abilitiesCost1, slotCost1, null);
        currentCost2 = PickAndShow(abilitiesCost2, slotCost2, null);
        currentCost3 = PickAndShow(abilitiesCost3, slotCost3, null);
        currentCost4 = PickAndShow(abilitiesCost4, slotCost4, null);

        SubscribeAllPoolListeners();

        currentFill = 0f;
        UpdateFillUI();
        UpdateUnlocks();
    }

    private void TryFillEmptySlots()
    {
        SubscribeAllPoolListeners();

        if (currentCost1 == null && abilitiesCost1.Count > 0)
            currentCost1 = PickAndShow(abilitiesCost1, slotCost1, null);
        else
            TrySwapCooledDownSlot(abilitiesCost1, slotCost1, ref currentCost1);

        if (currentCost2 == null && abilitiesCost2.Count > 0)
            currentCost2 = PickAndShow(abilitiesCost2, slotCost2, null);
        else
            TrySwapCooledDownSlot(abilitiesCost2, slotCost2, ref currentCost2);

        if (currentCost3 == null && abilitiesCost3.Count > 0)
            currentCost3 = PickAndShow(abilitiesCost3, slotCost3, null);
        else
            TrySwapCooledDownSlot(abilitiesCost3, slotCost3, ref currentCost3);

        if (currentCost4 == null && abilitiesCost4.Count > 0)
            currentCost4 = PickAndShow(abilitiesCost4, slotCost4, null);
        else
            TrySwapCooledDownSlot(abilitiesCost4, slotCost4, ref currentCost4);

        UpdateUnlocks();
    }

    public void OnAbilityCooldownEnded()
    {
        TrySwapCooledDownSlot(abilitiesCost1, slotCost1, ref currentCost1);
        TrySwapCooledDownSlot(abilitiesCost2, slotCost2, ref currentCost2);
        TrySwapCooledDownSlot(abilitiesCost3, slotCost3, ref currentCost3);
        TrySwapCooledDownSlot(abilitiesCost4, slotCost4, ref currentCost4);

        UpdateUnlocks();
    }

    private void TrySwapCooledDownSlot(List<Button> list, Transform slot, ref Button current)
    {
        if (current == null) return;

        AbilitySO currentAbility = AbilityManager.Instance.GetAbility(current);
        if (currentAbility == null) return;

        Button ready = FindReadyButton(list, current);
        if (ready == null) return;

        foreach (var btn in list)
        {
            if (btn != null && btn != ready)
                btn.gameObject.SetActive(false);
        }

        current.gameObject.SetActive(false);
        current = ready;
        ready.transform.SetParent(slot, false);
        ready.gameObject.SetActive(true);
        RestoreButtonVisuals(ready);
        SubscribeClickListener(ready);
    }

    private Button PickAndShow(List<Button> list, Transform slot, Button exclude)
    {
        if (list.Count == 0) return exclude;

        Button chosen = FindReadyButton(list, exclude);

        if (chosen == null)
        {
            List<Button> fallback = list.Where(b => b != null && b != exclude).ToList();
            if (fallback.Count == 0)
                fallback = list.Where(b => b != null).ToList();

            if (fallback.Count == 0) return exclude;
            chosen = fallback[Random.Range(0, fallback.Count)];
        }

        foreach (var btn in list)
        {
            if (btn != null && btn != chosen)
                btn.gameObject.SetActive(false);
        }

        if (exclude != null && exclude != chosen)
            exclude.gameObject.SetActive(false);

        chosen.transform.SetParent(slot, false);
        chosen.gameObject.SetActive(true);
        RestoreButtonVisuals(chosen);

        return chosen;
    }

    private Button FindReadyButton(List<Button> list, Button exclude)
    {
        List<Button> ready = list.Where(b =>
        {
            if (b == null || b == exclude) return false;
            AbilitySO ab = AbilityManager.Instance.GetAbility(b);
            if (ab == null) return false;
            return !AbilityManager.Instance.IsOnCooldown(ab);
        }).ToList();

        if (ready.Count == 0) return null;
        return ready[Random.Range(0, ready.Count)];
    }

    private HashSet<Button> subscribedButtons = new HashSet<Button>();

    private void SubscribeClickListener(Button btn)
    {
        if (btn == null || subscribedButtons.Contains(btn)) return;
        subscribedButtons.Add(btn);
        
        Button captured = btn;
        btn.onClick.AddListener(() => OnAbilityClicked(captured));
    }

    private void OnAbilityClicked(Button btn)
    {
        AbilityButtonLink link = btn.GetComponent<AbilityButtonLink>();
        if (link == null) return;

        int cost = link.ability.abilityCost;
        ReduceFill(cost);

        List<Button> list = null;
        Transform slot = null;

        switch (cost)
        {
            case 1: list = abilitiesCost1; slot = slotCost1; break;
            case 2: list = abilitiesCost2; slot = slotCost2; break;
            case 3: list = abilitiesCost3; slot = slotCost3; break;
            case 4: list = abilitiesCost4; slot = slotCost4; break;
        }

        if (list != null && slot != null)
        {
            Button ready = FindReadyButton(list, btn);
            
            if (ready != null)
            {
                foreach (var b in list)
                {
                    if (b != null && b != ready)
                        b.gameObject.SetActive(false);
                }

                btn.gameObject.SetActive(false);
                ready.transform.SetParent(slot, false);
                ready.gameObject.SetActive(true);
                RestoreButtonVisuals(ready);
                
                switch (cost)
                {
                    case 1: currentCost1 = ready; break;
                    case 2: currentCost2 = ready; break;
                    case 3: currentCost3 = ready; break;
                    case 4: currentCost4 = ready; break;
                }
                
                SubscribeClickListener(ready);
            }
        }

        UpdateUnlocks();
    }

    
    private void SubscribeAllPoolListeners()
    {
        foreach (var btn in allButtons)
            SubscribeClickListener(btn);
    }

    private void RestoreButtonVisuals(Button btn)
    {
        if (btn == null) return;
        
        Image img = btn.GetComponent<Image>();
        if (img != null)
        {
            Color color = img.color;
            color.a = 1f;
            img.color = color;
        }
    }

  
    private void HandleUnitKilled(UnitProduceStatsSO unitStats, Side deadUnitSide)
    {
        if (deadUnitSide != Side.Enemy) return;

        int amount = unitStats.populationCost * 3;
        AddFill(amount);
    }

    private void AddFill(float amount)
    {
        currentFill = Mathf.Clamp(currentFill + amount, 0f, 100f);
        UpdateFillUI();
        UpdateUnlocks();
    }

    private void ReduceFill(int cost)
    {
        float reduce = cost * 20f;
        currentFill = Mathf.Clamp(currentFill - reduce, 0f, 100f);
        UpdateFillUI();
        UpdateUnlocks();
    }

    private void UpdateFillUI()
    {
        if (fillBar != null)
            fillBar.fillAmount = currentFill / 100f;
    }

   
    private void UpdateUnlocks()
    {
        SetSlotInteractable(currentCost1, currentFill >= 20);
        SetSlotInteractable(currentCost2, currentFill >= 40);
        SetSlotInteractable(currentCost3, currentFill >= 60);
        SetSlotInteractable(currentCost4, currentFill >= 80);

        if (specialAbilityButton != null)
            specialAbilityButton.interactable = currentFill >= 100;
    }

    private void SetSlotInteractable(Button btn, bool fillOk)
    {
        if (btn == null) return;

        if (!fillOk)
        {
            btn.interactable = false;
            return;
        }

        AbilitySO ability = AbilityManager.Instance != null
            ? AbilityManager.Instance.GetAbility(btn)
            : null;

        bool onCooldown = ability != null && AbilityManager.Instance.IsOnCooldown(ability);
        btn.interactable = !onCooldown;
    }

    public void OnSpecialAbilityUsed()
    {
        currentFill = 0f;
        UpdateFillUI();
        UpdateUnlocks();
    }
}