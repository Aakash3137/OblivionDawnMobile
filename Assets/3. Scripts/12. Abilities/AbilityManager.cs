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

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeAbilityButtons();
    }

    private void Update()
    {
        var keys = new List<AbilitySO>(cooldownTimers.Keys);

        foreach (var ability in keys)
        {
            if (cooldownTimers[ability] > 0)
            {
                cooldownTimers[ability] -= Time.deltaTime;
            }
        }
    }

    public void ActivateAbility(AbilitySO ability)
    {
        // Check cooldown
        if (cooldownTimers.ContainsKey(ability) && cooldownTimers[ability] > 0)
        {
            Debug.Log($"{ability.abilityName} is on cooldown!");
            return;
        }

        // Get all valid units
        List<AbilityController> targets = GetTargets(ability);

        // Apply ability on each unit
        foreach (var controller in targets)
        {
            controller.ActivateAbility(ability);
        }

        // Start cooldown (GLOBAL per ability)
        cooldownTimers[ability] = ability.cooldown;
    }

    private List<AbilityController> GetTargets(AbilitySO ability)
    {
        List<AbilityController> targets = new List<AbilityController>();

        foreach (var unit in BattleUnitRegistry.Units)
        {
            if (!unit.TryGetComponent<UnitStats>(out var unitStats))
                continue;

            if (!unit.TryGetComponent<AbilityController>(out var controller))
                continue;

            switch (ability.targetType)
            {
                case AbilityTargetType.UnitClass:
                    if (unitStats.unitType == ability.targetUnitType)
                        targets.Add(controller);
                    break;

                case AbilityTargetType.UnitName:
                    if (unitStats.gameUnitName == ability.targetUnitName)
                        targets.Add(controller);
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
        HashSet<GameUnitName> processedUnits = new HashSet<GameUnitName>();

        foreach (var unit in BattleUnitRegistry.PlayerUnits)
        {
            if (processedUnits.Contains(unit.gameUnitName))
                continue;

            processedUnits.Add(unit.gameUnitName);

            if (unit.unitProduceSO.abilities == null || unit.unitProduceSO.abilities.Count == 0)
                continue;

            foreach (var ability in unit.unitProduceSO.abilities)
            {
                if (ability.abilityType == AbilityType.Passive)
                    continue;

                CreateAbilityButton(ability);
            }
        }
    }

    private void CreateAbilityButton(AbilitySO ability)
    {
        GameObject buttonObj = Instantiate(abilityButtonPrefab, abilitiesContainer);
        Image buttonImage = buttonObj.GetComponent<Image>();
        Button button = buttonObj.GetComponent<Button>();

        if (buttonImage != null && ability.abilityIcon != null)
            buttonImage.sprite = ability.abilityIcon;

        if (button != null)
            button.onClick.AddListener(() => ActivateAbility(ability));
    }
}