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

    private Dictionary<AbilitySO, Button> abilityButtonMap = new Dictionary<AbilitySO, Button>();
    private Dictionary<AbilitySO, Image> abilityImageMap = new Dictionary<AbilitySO, Image>();

    private HashSet<GameUnitName> processedUnits = new HashSet<GameUnitName>();

    private Dictionary<AbilitySO, GameUnitName> abilityOwnerMap = new Dictionary<AbilitySO, GameUnitName>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(WaitForUnitsAndInitialize());
    }

    private IEnumerator WaitForUnitsAndInitialize()
    {
        while (true)
        {
            InitializeAbilityButtons();  
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
                UpdateButtonCooldown(ability);
            }
            else
            {
                cooldownTimers[ability] = 0f;
                RestoreButton(ability);
                
                if (AbilitySetController.Instance != null)
                    AbilitySetController.Instance.OnAbilityCooldownEnded();
            }
        }


        var finished = keys.Where(a => cooldownTimers.ContainsKey(a) && cooldownTimers[a] <= 0f).ToList();
        foreach (var a in finished)
            cooldownTimers.Remove(a);
    }
    
    public bool IsOnCooldown(AbilitySO ability)
    {
        return cooldownTimers.TryGetValue(ability, out float t) && t > 0f;
    }

    public AbilitySO GetAbility(Button btn)
    {
        if (btn == null) return null;
        AbilityButtonLink link = btn.GetComponent<AbilityButtonLink>();
        return link != null ? link.ability : null;
    }

    public void OnUnitSpawned(UnitStats unit)
    {
        if (unit.side != Side.Player)
            return;
        
        if (unit.unitProduceSO.abilities == null) return;

        foreach (var ability in unit.unitProduceSO.abilities)
        {
            if (ability.abilityType != AbilityType.Active)
                continue;

            if (abilityButtonMap.ContainsKey(ability))
            {
                if (!abilityButtonMap[ability].gameObject.activeSelf)
                    EnableButton(ability);
            }
            else
            {
                CreateAbilityButton(ability);
                abilityOwnerMap[ability] = unit.gameUnitName;
            }
        }
    }

    public void OnUnitDied(UnitStats unit)
    {
        if (unit.side != Side.Player)
            return;
        
        foreach (var pair in abilityOwnerMap)
        {
            if (pair.Value == unit.gameUnitName)
            {
                AbilitySO ability = pair.Key;
                
                bool exists = GameplayRegistry.UnitsDictionary[Side.Player]
                    .Any(u => u != null && u.gameUnitName == unit.gameUnitName);

                if (!exists && abilityButtonMap[ability].gameObject.activeSelf)
                {
                    DisableButtonCompletely(ability);
                }
            }
        }

        processedUnits.Remove(unit.gameUnitName);
    }

    public void ActivateAbility(AbilitySO ability)
    {
        if (cooldownTimers.ContainsKey(ability) && cooldownTimers[ability] > 0)
            return;

        List<AbilityController> targets = GetTargets(ability);

        foreach (var controller in targets)
        {
            controller.ActivateAbility(ability);
        }

        cooldownTimers[ability] = ability.cooldown;
        DisableButton(ability);
    }

    private List<AbilityController> GetTargets(AbilitySO ability)
    {
        List<AbilityController> targets = new List<AbilityController>();

        List<UnitStats> unitsToCheck = ability.abilityScope == AbilityScope.Personal
            ? GameplayRegistry.UnitsDictionary[ability.casterSide] 
            // get unit stats of all sides from dictionary 
            : GameplayRegistry.UnitsDictionary.Values.SelectMany(x=>x).ToList();

        foreach (var unit in unitsToCheck)
        {
            if (unit == null) continue;
            if (!unit.TryGetComponent(out AbilityController controller)) continue;

            switch (ability.targetType)
            {
                case AbilityTargetType.All:
                    targets.Add(controller);
                    break;

                case AbilityTargetType.UnitClass:
                    if (unit.unitType == ability.targetUnitType)
                        targets.Add(controller);
                    break;

                case AbilityTargetType.UnitName:
                    if (unit.gameUnitName == ability.targetUnitName)
                        targets.Add(controller);
                    break;
            }
        }

        return targets;
    }

    private void InitializeAbilityButtons()
    {
        foreach (var unit in GameplayRegistry.UnitsDictionary[Side.Player])
        {
            if (unit == null) continue;

            if (processedUnits.Contains(unit.gameUnitName))
                continue;

            processedUnits.Add(unit.gameUnitName);

            if (unit.unitProduceSO.abilities == null) continue;

            foreach (var ability in unit.unitProduceSO.abilities)
            {
                if (ability.abilityType == AbilityType.Passive)
                    continue;

                if (abilityButtonMap.ContainsKey(ability))
                    continue;

                CreateAbilityButton(ability);
                abilityOwnerMap[ability] = unit.gameUnitName;
            }
        }
    }

    private void CreateAbilityButton(AbilitySO ability)
    {
        if (abilityButtonPrefab == null || abilitiesContainer == null)
            return;

        GameObject buttonObj = Instantiate(abilityButtonPrefab, abilitiesContainer);

        Image buttonImage = buttonObj.GetComponent<Image>();
        Button button = buttonObj.GetComponent<Button>();

        if (buttonImage != null && ability.abilityIcon != null)
            buttonImage.sprite = ability.abilityIcon;

        AbilityButtonLink link = buttonObj.GetComponent<AbilityButtonLink>();
        if (link != null)
            link.ability = ability;
        
        if (button != null)
        {
            button.onClick.AddListener(() => ActivateAbility(ability));

            abilityButtonMap[ability] = button;
            abilityImageMap[ability] = buttonImage;
            AbilityButtons.Add(button);
        }
    }
    
    private void DisableButton(AbilitySO ability)
    {
        if (abilityButtonMap.TryGetValue(ability, out Button button))
            button.interactable = false;

        if (abilityImageMap.TryGetValue(ability, out Image image))
        {
            Color color = image.color;
            color.a = 0.5f;
            image.color = color;
        }
    }

    private void RestoreButton(AbilitySO ability)
    {
        if (abilityButtonMap.TryGetValue(ability, out Button button))
            button.interactable = true;

        if (abilityImageMap.TryGetValue(ability, out Image image))
        {
            Color color = image.color;
            color.a = 1f;
            image.color = color;
        }
    }

    private void DisableButtonCompletely(AbilitySO ability)
    {
        if (abilityButtonMap.TryGetValue(ability, out Button button))
        {
            button.gameObject.SetActive(false);
        }
    }

    private void EnableButton(AbilitySO ability)
    {
        if (abilityButtonMap.TryGetValue(ability, out Button button))
        {
            button.gameObject.SetActive(true);
            button.interactable = true;
        }

        if (abilityImageMap.TryGetValue(ability, out Image image))
        {
            Color color = image.color;
            color.a = 1f;
            image.color = color;
        }
    }

    private void UpdateButtonCooldown(AbilitySO ability)
    {
    }
}