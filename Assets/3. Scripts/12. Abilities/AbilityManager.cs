using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;

    private Dictionary<AbilitySO, float> cooldownTimers = new Dictionary<AbilitySO, float>();
    
    public GameObject abilityButtonPrefab;
    public Transform abilitiesContainer;
    public List<Button> AbilityButtons = new List<Button>();
    
    // Track button-ability pairs for cooldown visuals
    private Dictionary<AbilitySO, Button> abilityButtonMap = new Dictionary<AbilitySO, Button>();
    private Dictionary<AbilitySO, Image> abilityImageMap = new Dictionary<AbilitySO, Image>();
    
    private HashSet<GameUnitName> processedUnits = new HashSet<GameUnitName>();

    private void Awake()
    {
        Instance = this;
        Debug.Log("[AbilityManager] Awake - Instance created");
    }

    private void Start()
    {
        Debug.Log("[AbilityManager] Start called");
        Debug.Log($"[AbilityManager] abilityButtonPrefab assigned: {(abilityButtonPrefab != null)}");
        Debug.Log($"[AbilityManager] abilitiesContainer assigned: {(abilitiesContainer != null)}");
        
        // Don't initialize here - units haven't spawned yet
        // InitializeAbilityButtons();
        
        // Instead, start checking for units
        StartCoroutine(WaitForUnitsAndInitialize());
    }

    private IEnumerator WaitForUnitsAndInitialize()
    {
        int attempts = 0;
        while (true)
        {
            attempts++;
            int currentCount = BattleUnitRegistry.PlayerUnits.Count;
            
            if (currentCount == 0)
            {
            }
            else
            {
                Debug.Log($"[AbilityManager] Units detected → playerUnits={currentCount} → init UI");
                InitializeAbilityButtons();
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Update()
    {
        var keys = new List<AbilitySO>(cooldownTimers.Keys);

        foreach (var ability in keys)
        {
            if (cooldownTimers[ability] > 0)
            {
                cooldownTimers[ability] -= Time.deltaTime;
                
                // Update button visual during cooldown
                UpdateButtonCooldown(ability);
            }
            else
            {
                // Cooldown finished, restore button
                RestoreButton(ability);
            }
        }
    }

    public void ActivateAbility(AbilitySO ability)
    {
        Debug.Log($"[AbilityManager] ActivateAbility called for: {ability.abilityName}");

        // Check cooldown
        if (cooldownTimers.ContainsKey(ability) && cooldownTimers[ability] > 0)
        {
            Debug.Log($"[AbilityManager] {ability.abilityName} is on cooldown! Remaining: {cooldownTimers[ability]:F2}s");
            return;
        }

        // Get all valid units
        List<AbilityController> targets = GetTargets(ability);
        Debug.Log($"[AbilityManager] Found {targets.Count} targets for {ability.abilityName}");

        // Apply ability on each unit
        foreach (var controller in targets)
        {
            Debug.Log($"[AbilityManager] Applying {ability.abilityName} to {controller.gameObject.name}");
            controller.ActivateAbility(ability);
        }

        // Start cooldown (GLOBAL per ability)
        cooldownTimers[ability] = ability.cooldown;
        Debug.Log($"[AbilityManager] {ability.abilityName} cooldown started: {ability.cooldown}s");
        
        // Disable button and start fade
        DisableButton(ability);
    }

    private List<AbilityController> GetTargets(AbilitySO ability)
    {
        List<AbilityController> targets = new List<AbilityController>();

        Debug.Log($"[AbilityManager] GetTargets - Total units in registry: {BattleUnitRegistry.Units.Count}");
        Debug.Log($"[AbilityManager] Target Type: {ability.targetType}, Scope: {ability.abilityScope}, Caster Side: {ability.casterSide}");

        // Determine which units to check based on scope
        List<UnitStats> unitsToCheck = new List<UnitStats>();
        
        if (ability.abilityScope == AbilityScope.Personal)
        {
            // Only affect units on the caster's side
            unitsToCheck = ability.casterSide == Side.Player 
                ? BattleUnitRegistry.PlayerUnits 
                : BattleUnitRegistry.EnemyUnits;
            Debug.Log($"[AbilityManager] Personal scope - checking {unitsToCheck.Count} {ability.casterSide} units");
        }
        else // Universal
        {
            // Affect all units regardless of side
            unitsToCheck = BattleUnitRegistry.Units;
            Debug.Log($"[AbilityManager] Universal scope - checking all {unitsToCheck.Count} units");
        }

        foreach (var unit in unitsToCheck)
        {
            if (unit == null)
            {
                Debug.LogWarning($"[AbilityManager] Null unit in registry");
                continue;
            }

            if (!unit.TryGetComponent<UnitStats>(out var unitStats))
            {
                Debug.LogWarning($"[AbilityManager] Unit {unit.gameObject.name} has no UnitStats");
                continue;
            }

            if (!unit.TryGetComponent<AbilityController>(out var controller))
            {
                Debug.LogWarning($"[AbilityManager] Unit {unit.gameObject.name} has no AbilityController");
                continue;
            }

            switch (ability.targetType)
            {
                case AbilityTargetType.All:
                    // Target all units (already filtered by scope above)
                    Debug.Log($"[AbilityManager] Target matched by All: {unit.gameObject.name}");
                    targets.Add(controller);
                    break;

                case AbilityTargetType.UnitClass:
                    if (unitStats.unitType == ability.targetUnitType)
                    {
                        Debug.Log($"[AbilityManager] Target matched by UnitClass: {unit.gameObject.name}");
                        targets.Add(controller);
                    }
                    break;

                case AbilityTargetType.UnitName:
                    if (unitStats.gameUnitName == ability.targetUnitName)
                    {
                        Debug.Log($"[AbilityManager] Target matched by UnitName: {unit.gameObject.name}");
                        targets.Add(controller);
                    }
                    break;

                case AbilityTargetType.Self:
                    // handled separately if needed
                    break;

                case AbilityTargetType.Area:
                    // later
                    break;
            }
        }

        return targets;
    }

    // (Optional) UI helpers
    public bool IsOnCooldown(AbilitySO ability)
    {
        return cooldownTimers.ContainsKey(ability) && cooldownTimers[ability] > 0;
    }

    public float GetRemainingCooldown(AbilitySO ability)
    {
        if (!cooldownTimers.ContainsKey(ability))
            return 0;

        return cooldownTimers[ability];
    }

    private void InitializeAbilityButtons()
    {
        Debug.Log($"[AbilityManager] InitializeAbilityButtons - Player units count: {BattleUnitRegistry.PlayerUnits.Count}");

        foreach (var unit in BattleUnitRegistry.PlayerUnits)
        {
            Debug.Log($"[AbilityManager] Checking unit: {unit.gameUnitName}");
            
            if (processedUnits.Contains(unit.gameUnitName))
            {
                Debug.Log($"[AbilityManager] Already processed {unit.gameUnitName}, skipping");
                continue;
            }

            processedUnits.Add(unit.gameUnitName);

            if (unit.unitProduceSO.abilities == null || unit.unitProduceSO.abilities.Count == 0)
            {
                Debug.Log($"[AbilityManager] Unit {unit.gameUnitName} has no abilities");
                continue;
            }

            Debug.Log($"[AbilityManager] Processing {unit.unitProduceSO.abilities.Count} abilities for {unit.gameUnitName}");

            foreach (var ability in unit.unitProduceSO.abilities)
            {
                Debug.Log($"[AbilityManager] Ability: {ability.abilityName}, Type: {ability.abilityType}");
                
                if (ability.abilityType == AbilityType.Passive)
                {
                    Debug.Log($"[AbilityManager] Skipping passive ability: {ability.abilityName}");
                    continue;
                }

                Debug.Log($"[AbilityManager] Creating button for ability: {ability.abilityName}");
                CreateAbilityButton(ability);
            }
        }
        
        Debug.Log($"[AbilityManager] Button initialization complete. Total buttons created: {AbilityButtons.Count}");
    }

    private void CreateAbilityButton(AbilitySO ability)
    {
        Debug.Log($"[AbilityManager] CreateAbilityButton called for: {ability.abilityName}");
        Debug.Log($"[AbilityManager] abilityButtonPrefab: {(abilityButtonPrefab != null ? "Assigned" : "NULL")}");
        Debug.Log($"[AbilityManager] abilitiesContainer: {(abilitiesContainer != null ? "Assigned" : "NULL")}");
        
        if (abilityButtonPrefab == null)
        {
            Debug.LogError("[AbilityManager] abilityButtonPrefab is not assigned in inspector!");
            return;
        }
        
        if (abilitiesContainer == null)
        {
            Debug.LogError("[AbilityManager] abilitiesContainer is not assigned in inspector!");
            return;
        }
        
        GameObject buttonObj = Instantiate(abilityButtonPrefab, abilitiesContainer);
        Debug.Log($"[AbilityManager] Button instantiated: {buttonObj.name}");
        
        Image buttonImage = buttonObj.GetComponent<Image>();
        Button button = buttonObj.GetComponent<Button>();

        if (buttonImage != null && ability.abilityIcon != null)
        {
            buttonImage.sprite = ability.abilityIcon;
            Debug.Log($"[AbilityManager] Icon set for {ability.abilityName}");
        }
        else
        {
            Debug.LogWarning($"[AbilityManager] ButtonImage or Icon missing for {ability.abilityName}");
        }

        if (button != null)
        {
            button.onClick.AddListener(() => ActivateAbility(ability));
            
            // Store references for cooldown management
            abilityButtonMap[ability] = button;
            abilityImageMap[ability] = buttonImage;
            AbilityButtons.Add(button);
            
            Debug.Log($"[AbilityManager] Button fully configured for {ability.abilityName}");
        }
        else
        {
            Debug.LogError($"[AbilityManager] Button component not found on prefab for {ability.abilityName}");
        }
    }

    // Public method to refresh buttons if units spawn later
    public void RefreshAbilityButtons()
    {
        Debug.Log("[AbilityManager] Refreshing ability buttons");
        
        // Clear existing buttons
        foreach (Transform child in abilitiesContainer)
        {
            Destroy(child.gameObject);
        }
        AbilityButtons.Clear();
        abilityButtonMap.Clear();
        abilityImageMap.Clear();
        
        // Reinitialize
        InitializeAbilityButtons();
    }
    
    // Add a special/universal ability dynamically
    public void AddSpecialAbility(AbilitySO ability)
    {
        if (ability == null)
        {
            Debug.LogWarning("[AbilityManager] Cannot add null ability");
            return;
        }
        
        if (ability.abilityType != AbilityType.Active)
        {
            Debug.LogWarning($"[AbilityManager] Special abilities must be Active type. {ability.abilityName} is {ability.abilityType}");
            return;
        }
        
        Debug.Log($"[AbilityManager] Adding special ability: {ability.abilityName}");
        CreateAbilityButton(ability);
    }
    
    private void DisableButton(AbilitySO ability)
    {
        if (abilityButtonMap.TryGetValue(ability, out Button button))
        {
            button.interactable = false;
        }
        
        if (abilityImageMap.TryGetValue(ability, out Image image))
        {
            // Fade the button to 50% opacity
            Color color = image.color;
            color.a = 0.5f;
            image.color = color;
        }
    }
    
    private void UpdateButtonCooldown(AbilitySO ability)
    {
        // Optional: You can add a fill image or text to show cooldown progress here
        // For now, just keep it faded and disabled
    }
    
    private void RestoreButton(AbilitySO ability)
    {
        if (abilityButtonMap.TryGetValue(ability, out Button button))
        {
            button.interactable = true;
        }
        
        if (abilityImageMap.TryGetValue(ability, out Image image))
        {
            // Restore full opacity
            Color color = image.color;
            color.a = 1f;
            image.color = color;
        }
    }
}