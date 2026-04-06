#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class UpgradeDataEditorWindow : EditorWindow
{
    public VisualTreeAsset visualTree;
    public VisualTreeAsset spacerVisualTree;
    public VisualTreeAsset unitVisualTree;
    public VisualTreeAsset defenseVisualTree;
    public VisualTreeAsset resourceVisualTree;
    public VisualTreeAsset offenseVisualTree;
    public VisualTreeAsset cityCenterVisualTree;

    public VisualTreeAsset unitLevelDataVisualTree;
    public VisualTreeAsset defenseLevelDataVisualTree;
    public VisualTreeAsset resourceLevelDataVisualTree;
    public VisualTreeAsset offenseLevelDataVisualTree;
    public VisualTreeAsset cityCenterLevelDataVisualTree;

    public AllBuildingData allBuildingData;
    public AllUnitData allUnitData;

    private ScrollView selectScroll;
    private ScrollView levelDataScroll;

    private VisualElement unitLevelPanel;
    private VisualElement defenseLevelPanel;
    private VisualElement offenseLevelPanel;
    private VisualElement resourceLevelPanel;
    private VisualElement mainLevelPanel;

    private VisualElement unitFieldHeader;
    private VisualElement defenseFieldHeader;
    private VisualElement resourceFieldHeader;
    private VisualElement offenseFieldHeader;
    private VisualElement mainFieldHeader;

    private IntegerField maxLevelField;
    private ObjectField currentEditingObjectField;

    private Button generateLevelsButton;

    [MenuItem("Custom Editor Tools/Upgrade Data")]
    public static void ShowWindow()
    {
        GetWindow<UpgradeDataEditorWindow>("Upgrade Data");
    }
    public void CreateGUI()
    {
        var root = rootVisualElement;
        root.Clear();

        visualTree.CloneTree(root);

        maxLevelField = root.Q<IntegerField>("MaxLevel");
        maxLevelField.value = GameData.GameMaxObjectLevel;
        currentEditingObjectField = root.Q<ObjectField>("CurrentEditingObject");

        selectScroll = root.Q<ScrollView>("ScrollPanel");
        levelDataScroll = root.Q<ScrollView>("LevelScroll");

        unitLevelPanel = root.Q<VisualElement>("UnitLevelPanel");
        defenseLevelPanel = root.Q<VisualElement>("DefenseLevelPanel");
        mainLevelPanel = root.Q<VisualElement>("MainLevelPanel");
        resourceLevelPanel = root.Q<VisualElement>("ResourceLevelPanel");
        offenseLevelPanel = root.Q<VisualElement>("OffenseLevelPanel");

        unitFieldHeader = root.Q<VisualElement>("UnitHeader");
        defenseFieldHeader = root.Q<VisualElement>("DefenseHeader");
        resourceFieldHeader = root.Q<VisualElement>("ResourceHeader");
        offenseFieldHeader = root.Q<VisualElement>("OffenseHeader");
        mainFieldHeader = root.Q<VisualElement>("MainHeader");

        RefreshUI();

        RegisterButtons(root);
    }

    private void OnClickSelect(ScriptableObject scriptable)
    {
        RefreshUI();

        switch (scriptable)
        {
            case UnitProduceStatsSO unit:
                unitLevelPanel.style.display = DisplayStyle.Flex;
                generateLevelsButton = unitLevelPanel.Q<Button>("GenerateLevels");
                GenerateUnitLevelData(unit);
                break;
            case MainBuildingDataSO main:
                mainLevelPanel.style.display = DisplayStyle.Flex;
                generateLevelsButton = mainLevelPanel.Q<Button>("GenerateLevels");
                GenerateMainLevelData(main);
                break;
            case DefenseBuildingDataSO defense:
                defenseLevelPanel.style.display = DisplayStyle.Flex;
                generateLevelsButton = defenseLevelPanel.Q<Button>("GenerateLevels");
                GenerateDefenseLevelData(defense);
                break;
            case ResourceBuildingDataSO resource:
                resourceLevelPanel.style.display = DisplayStyle.Flex;
                generateLevelsButton = resourceLevelPanel.Q<Button>("GenerateLevels");
                GenerateResourceLevelData(resource);
                break;
            case OffenseBuildingDataSO offense:
                offenseLevelPanel.style.display = DisplayStyle.Flex;
                generateLevelsButton = offenseLevelPanel.Q<Button>("GenerateLevels");
                GenerateOffenseLevelData(offense);
                break;
        }

        currentEditingObjectField.value = scriptable;

        if (generateLevelsButton != null)
            generateLevelsButton.clicked += OnClickGenerateLevels;

    }

    #region Units
    private void CreateUnitFields()
    {
        RefreshUI();

        if (unitFieldHeader != null)
            unitFieldHeader.style.display = DisplayStyle.Flex;

        var unitSO = allUnitData.allUnitsSO;
        FactionName prevFactionName = 0;

        foreach (var unit in unitSO)
        {
            var instance = unitVisualTree.CloneTree();
            var serializedObject = new SerializedObject(unit);

            instance.Q<ObjectField>("Scriptable").value = unit;

            // Binding Scriptable obj to the instance
            instance.Q<EnumField>("Faction").BindProperty(serializedObject.FindProperty($"unitIdentity.faction"));
            instance.Q<EnumField>("Name").BindProperty(serializedObject.FindProperty($"gameUnitName"));
            instance.Q<EnumField>("Type").BindProperty(serializedObject.FindProperty($"unitType"));
            instance.Q<ObjectField>("Icon").BindProperty(serializedObject.FindProperty($"unitIcon"));
            instance.Q<ObjectField>("Spawner").BindProperty(serializedObject.FindProperty($"spawnerBuilding"));
            instance.Q<IntegerField>("Population").BindProperty(serializedObject.FindProperty($"populationCost"));
            instance.Q<EnumField>("CardState").BindProperty(serializedObject.FindProperty($"cardDetails.cardState"));
            instance.Q<IntegerField>("minBuildingLevel").BindProperty(serializedObject.FindProperty($"cardDetails.minBuildingLevel"));
            instance.Q<IntegerField>("purchaseCost").BindProperty(serializedObject.FindProperty($"cardDetails.purchaseCost"));
            instance.Q<FloatField>("upgradeCostMultiplier").BindProperty(serializedObject.FindProperty($"cardDetails.upgradeCostMultiplier"));
            instance.Q<FloatField>("fragmentCostMultiplier").BindProperty(serializedObject.FindProperty($"cardDetails.fragmentCostMultiplier"));

            instance.Q<Button>("select").clicked += () => OnClickSelect(unit);

            if (prevFactionName != unit.unitIdentity.faction)
            {
                selectScroll.Add(spacerVisualTree.CloneTree());
                prevFactionName = unit.unitIdentity.faction;
            }
            selectScroll.Add(instance);
        }
    }

    private void GenerateUnitLevelData(UnitProduceStatsSO unitSO)
    {
        var serializedObject = new SerializedObject(unitSO);

        for (int i = 0; i < unitSO.unitUpgradeData.Length; i++)
        {
            var instance = unitLevelDataVisualTree.CloneTree();
            // Get the serialized property for this specific array element            
            SerializedProperty serializedProperty = serializedObject.FindProperty($"unitUpgradeData.Array.data[{i}]");

            instance.Q<EnumField>("Name").value = unitSO.gameUnitName;

            // Bind the to the specific array element property
            instance.Q<IntegerField>("Level").BindProperty(serializedProperty.FindPropertyRelative("unitLevel"));
            instance.Q<FloatField>("Health").BindProperty(serializedProperty.FindPropertyRelative("unitBasicStats.maxHealth"));
            instance.Q<FloatField>("Armor").BindProperty(serializedProperty.FindPropertyRelative("unitBasicStats.armor"));
            instance.Q<FloatField>("BuildTime").BindProperty(serializedProperty.FindPropertyRelative("unitSpawnTime"));
            instance.Q<FloatField>("UnitDMG").BindProperty(serializedProperty.FindPropertyRelative("unitAttackStats.damage"));
            instance.Q<FloatField>("BuildingDMG").BindProperty(serializedProperty.FindPropertyRelative("unitAttackStats.buildingDamage"));
            instance.Q<FloatField>("FireRate").BindProperty(serializedProperty.FindPropertyRelative("unitAttackStats.fireRate"));
            instance.Q<FloatField>("AtkRng").BindProperty(serializedProperty.FindPropertyRelative("unitRangeStats.attackRange"));
            instance.Q<FloatField>("Speed").BindProperty(serializedProperty.FindPropertyRelative("unitMobilityStats.moveSpeed"));

            levelDataScroll.Add(instance);
        }
    }
    #endregion

    #region Defense
    private void CreateDefenseFields()
    {
        RefreshUI();
        defenseFieldHeader.style.display = DisplayStyle.Flex;

        var defenseSO = allBuildingData.defenseBuildingsSO;
        FactionName prevFactionName = 0;
        foreach (var defense in defenseSO)
        {
            var instance = defenseVisualTree.CloneTree();
            var serializedObject = new SerializedObject(defense);
            List<IntegerField> buildingCosts = GetBuildingCostsFields(instance);
            List<IntegerField> upKeepCosts = GetUpKeepCostsFields(instance);

            instance.Q<ObjectField>("Scriptable").value = defense;

            // Binding Scriptable obj to the instance
            instance.Q<EnumField>("Faction").BindProperty(serializedObject.FindProperty($"buildingIdentity.faction"));
            instance.Q<EnumField>("Name").BindProperty(serializedObject.FindProperty($"gameBuildingName"));
            instance.Q<EnumField>("Type").BindProperty(serializedObject.FindProperty($"defenseType"));
            instance.Q<ObjectField>("Icon").BindProperty(serializedObject.FindProperty($"buildingIcon"));
            instance.Q<IntegerField>("Population").BindProperty(serializedObject.FindProperty($"populationCost"));
            instance.Q<EnumField>("CardState").BindProperty(serializedObject.FindProperty($"cardDetails.cardState"));
            instance.Q<IntegerField>("minBuildingLevel").BindProperty(serializedObject.FindProperty($"cardDetails.minBuildingLevel"));
            instance.Q<IntegerField>("purchaseCost").BindProperty(serializedObject.FindProperty($"cardDetails.purchaseCost"));
            instance.Q<FloatField>("upgradeCostMultiplier").BindProperty(serializedObject.FindProperty($"cardDetails.upgradeCostMultiplier"));
            instance.Q<FloatField>("fragmentCostMultiplier").BindProperty(serializedObject.FindProperty($"cardDetails.fragmentCostMultiplier"));
            for (int i = 0; i < buildingCosts.Count; i++)
                buildingCosts[i].BindProperty(serializedObject.FindProperty($"buildingBuildCost.Array.data[{i}].resourceAmount"));
            for (int i = 0; i < upKeepCosts.Count; i++)
                upKeepCosts[i].BindProperty(serializedObject.FindProperty($"upKeepCost.Array.data[{i}].resourceAmount"));

            var hasResourceToggle = instance.Q<Toggle>("hasUpkeep");
            hasResourceToggle.BindProperty(serializedObject.FindProperty($"hasUpkeep"));
            hasResourceToggle.RegisterValueChangedCallback(evt => OnToggleUpkeep(evt.newValue, upKeepCosts));

            OnToggleUpkeep(hasResourceToggle.value, upKeepCosts);

            instance.Q<Button>("select").clicked += () => OnClickSelect(defense);

            if (prevFactionName != defense.buildingIdentity.faction)
            {
                selectScroll.Add(spacerVisualTree.CloneTree());
                prevFactionName = defense.buildingIdentity.faction;
            }
            selectScroll.Add(instance);
        }
    }
    private void GenerateDefenseLevelData(DefenseBuildingDataSO defenseSO)
    {
        var serializedObject = new SerializedObject(defenseSO);

        for (int i = 0; i < defenseSO.defenseBuildingUpgradeData.Count; i++)
        {
            var instance = defenseLevelDataVisualTree.CloneTree();
            SerializedProperty serializedProperty = serializedObject.FindProperty($"defenseBuildingUpgradeData.Array.data[{i}]");

            instance.Q<EnumField>("Name").value = defenseSO.gameBuildingName;

            // Bind the to the specific array element property
            instance.Q<IntegerField>("Level").BindProperty(serializedProperty.FindPropertyRelative("buildingLevel"));
            instance.Q<FloatField>("Health").BindProperty(serializedProperty.FindPropertyRelative("buildingBasicStats.maxHealth"));
            instance.Q<FloatField>("Armor").BindProperty(serializedProperty.FindPropertyRelative("buildingBasicStats.armor"));
            instance.Q<FloatField>("BuildTime").BindProperty(serializedProperty.FindPropertyRelative("buildingBuildTime"));
            instance.Q<FloatField>("UnitDMG").BindProperty(serializedProperty.FindPropertyRelative("defenseAttackStats.damage"));
            instance.Q<FloatField>("BuildingDMG").BindProperty(serializedProperty.FindPropertyRelative("defenseAttackStats.buildingDamage"));
            instance.Q<FloatField>("FireRate").BindProperty(serializedProperty.FindPropertyRelative("defenseAttackStats.fireRate"));
            instance.Q<FloatField>("AtkRng").BindProperty(serializedProperty.FindPropertyRelative("defenseRangeStats.attackRange"));

            levelDataScroll.Add(instance);
        }
    }
    #endregion

    #region Offense
    private void CreateOffenseFields()
    {
        RefreshUI();
        offenseFieldHeader.style.display = DisplayStyle.Flex;

        var offenseSO = allBuildingData.offenseBuildingsSO;
        FactionName prevFactionName = 0;

        foreach (var offense in offenseSO)
        {
            var instance = offenseVisualTree.CloneTree();
            var serializedObject = new SerializedObject(offense);
            List<IntegerField> buildingCosts = GetBuildingCostsFields(instance);
            List<IntegerField> upKeepCosts = GetUpKeepCostsFields(instance);

            instance.Q<ObjectField>("Scriptable").value = offense;

            // Binding Scriptable obj to the instance
            instance.Q<EnumField>("Faction").BindProperty(serializedObject.FindProperty($"buildingIdentity.faction"));
            instance.Q<EnumField>("Name").BindProperty(serializedObject.FindProperty($"gameBuildingName"));
            instance.Q<EnumField>("Type").BindProperty(serializedObject.FindProperty($"offenseType"));
            instance.Q<ObjectField>("Icon").BindProperty(serializedObject.FindProperty($"buildingIcon"));
            instance.Q<EnumField>("CardState").BindProperty(serializedObject.FindProperty($"cardDetails.cardState"));
            instance.Q<IntegerField>("minBuildingLevel").BindProperty(serializedObject.FindProperty($"cardDetails.minBuildingLevel"));
            instance.Q<IntegerField>("purchaseCost").BindProperty(serializedObject.FindProperty($"cardDetails.purchaseCost"));
            instance.Q<FloatField>("upgradeCostMultiplier").BindProperty(serializedObject.FindProperty($"cardDetails.upgradeCostMultiplier"));
            instance.Q<FloatField>("fragmentCostMultiplier").BindProperty(serializedObject.FindProperty($"cardDetails.fragmentCostMultiplier"));
            for (int i = 0; i < buildingCosts.Count; i++)
                buildingCosts[i].BindProperty(serializedObject.FindProperty($"buildingBuildCost.Array.data[{i}].resourceAmount"));
            for (int i = 0; i < upKeepCosts.Count; i++)
                upKeepCosts[i].BindProperty(serializedObject.FindProperty($"upKeepCost.Array.data[{i}].resourceAmount"));

            var hasResourceToggle = instance.Q<Toggle>("hasUpkeep");
            hasResourceToggle.BindProperty(serializedObject.FindProperty($"hasUpkeep"));
            hasResourceToggle.RegisterValueChangedCallback(evt => OnToggleUpkeep(evt.newValue, upKeepCosts));

            OnToggleUpkeep(hasResourceToggle.value, upKeepCosts);

            instance.Q<Button>("select").clicked += () => OnClickSelect(offense);

            if (prevFactionName != offense.buildingIdentity.faction)
            {
                selectScroll.Add(spacerVisualTree.CloneTree());
                prevFactionName = offense.buildingIdentity.faction;
            }
            selectScroll.Add(instance);
        }
    }
    private void GenerateOffenseLevelData(OffenseBuildingDataSO offenseSO)
    {
        var serializedObject = new SerializedObject(offenseSO);

        for (int i = 0; i < offenseSO.offenseBuildingUpgradeData.Count; i++)
        {
            var instance = offenseLevelDataVisualTree.CloneTree();
            SerializedProperty serializedProperty = serializedObject.FindProperty($"offenseBuildingUpgradeData.Array.data[{i}]");

            instance.Q<EnumField>("Name").value = offenseSO.gameBuildingName;

            // Bind the to the specific array element property
            instance.Q<IntegerField>("Level").BindProperty(serializedProperty.FindPropertyRelative("buildingLevel"));
            instance.Q<FloatField>("Health").BindProperty(serializedProperty.FindPropertyRelative("buildingBasicStats.maxHealth"));
            instance.Q<FloatField>("Armor").BindProperty(serializedProperty.FindPropertyRelative("buildingBasicStats.armor"));
            instance.Q<FloatField>("BuildTime").BindProperty(serializedProperty.FindPropertyRelative("buildingBuildTime"));
            instance.Q<IntegerField>("maxSpawnableUnits").BindProperty(serializedProperty.FindPropertyRelative("maxSpawnableUnits"));

            levelDataScroll.Add(instance);
        }
    }
    #endregion

    #region Resource
    private void CreateResourceFields()
    {
        RefreshUI();
        resourceFieldHeader.style.display = DisplayStyle.Flex;

        var resourceSO = allBuildingData.resourceBuildingsSO;
        FactionName prevFactionName = 0;

        foreach (var resource in resourceSO)
        {
            var instance = resourceVisualTree.CloneTree();
            var serializedObject = new SerializedObject(resource);
            List<IntegerField> buildingCosts = GetBuildingCostsFields(instance);
            List<IntegerField> upKeepCosts = GetUpKeepCostsFields(instance);

            instance.Q<ObjectField>("Scriptable").value = resource;

            // Binding Scriptable obj to the instance
            instance.Q<EnumField>("Faction").BindProperty(serializedObject.FindProperty($"buildingIdentity.faction"));
            instance.Q<EnumField>("Name").BindProperty(serializedObject.FindProperty($"gameBuildingName"));
            instance.Q<EnumField>("Type").BindProperty(serializedObject.FindProperty($"resourceType"));
            instance.Q<ObjectField>("Icon").BindProperty(serializedObject.FindProperty($"buildingIcon"));
            instance.Q<EnumField>("CardState").BindProperty(serializedObject.FindProperty($"cardDetails.cardState"));
            instance.Q<IntegerField>("minBuildingLevel").BindProperty(serializedObject.FindProperty($"cardDetails.minBuildingLevel"));
            instance.Q<IntegerField>("purchaseCost").BindProperty(serializedObject.FindProperty($"cardDetails.purchaseCost"));
            instance.Q<FloatField>("upgradeCostMultiplier").BindProperty(serializedObject.FindProperty($"cardDetails.upgradeCostMultiplier"));
            instance.Q<FloatField>("fragmentCostMultiplier").BindProperty(serializedObject.FindProperty($"cardDetails.fragmentCostMultiplier"));
            for (int i = 0; i < buildingCosts.Count; i++)
                buildingCosts[i].BindProperty(serializedObject.FindProperty($"buildingBuildCost.Array.data[{i}].resourceAmount"));
            for (int i = 0; i < upKeepCosts.Count; i++)
                upKeepCosts[i].BindProperty(serializedObject.FindProperty($"upKeepCost.Array.data[{i}].resourceAmount"));

            var hasResourceToggle = instance.Q<Toggle>("hasUpkeep");
            hasResourceToggle.BindProperty(serializedObject.FindProperty($"hasUpkeep"));
            hasResourceToggle.RegisterValueChangedCallback(evt => OnToggleUpkeep(evt.newValue, upKeepCosts));

            OnToggleUpkeep(hasResourceToggle.value, upKeepCosts);

            instance.Q<Button>("select").clicked += () => OnClickSelect(resource);

            if (prevFactionName != resource.buildingIdentity.faction)
            {
                selectScroll.Add(spacerVisualTree.CloneTree());
                prevFactionName = resource.buildingIdentity.faction;
            }
            selectScroll.Add(instance);
        }
    }

    private void GenerateResourceLevelData(ResourceBuildingDataSO resourceSO)
    {
        var serializedObject = new SerializedObject(resourceSO);

        for (int i = 0; i < resourceSO.resourceBuildingUpgradeData.Count; i++)
        {
            var instance = resourceLevelDataVisualTree.CloneTree();
            SerializedProperty serializedProperty = serializedObject.FindProperty($"resourceBuildingUpgradeData.Array.data[{i}]");

            instance.Q<EnumField>("Name").value = resourceSO.gameBuildingName;

            // Bind the to the specific array element property
            instance.Q<IntegerField>("Level").BindProperty(serializedProperty.FindPropertyRelative("buildingLevel"));
            instance.Q<FloatField>("Health").BindProperty(serializedProperty.FindPropertyRelative("buildingBasicStats.maxHealth"));
            instance.Q<FloatField>("Armor").BindProperty(serializedProperty.FindPropertyRelative("buildingBasicStats.armor"));
            instance.Q<FloatField>("BuildTime").BindProperty(serializedProperty.FindPropertyRelative("buildingBuildTime"));
            instance.Q<IntegerField>("resourceAmountPerBatch").BindProperty(serializedProperty.FindPropertyRelative("resourceAmountPerBatch"));
            instance.Q<IntegerField>("resourceAmountCapacity").BindProperty(serializedProperty.FindPropertyRelative("resourceAmountCapacity"));

            levelDataScroll.Add(instance);
        }
    }
    #endregion

    #region CityCenter
    private void CreateCityCenterFields()
    {
        RefreshUI();
        mainFieldHeader.style.display = DisplayStyle.Flex;

        var mainSO = allBuildingData.mainBuildingSO;
        FactionName prevFactionName = 0;

        foreach (var main in mainSO)
        {
            var instance = cityCenterVisualTree.CloneTree();
            var serializedObject = new SerializedObject(main);
            List<IntegerField> buildingCosts = GetBuildingCostsFields(instance);
            List<IntegerField> upKeepCosts = GetUpKeepCostsFields(instance);

            instance.Q<ObjectField>("Scriptable").value = main;

            // Binding Scriptable obj to the instance
            instance.Q<EnumField>("Faction").BindProperty(serializedObject.FindProperty($"buildingIdentity.faction"));
            instance.Q<EnumField>("Name").BindProperty(serializedObject.FindProperty($"gameBuildingName"));
            instance.Q<ObjectField>("Icon").BindProperty(serializedObject.FindProperty($"buildingIcon"));
            instance.Q<Toggle>("FactionUnlocked").BindProperty(serializedObject.FindProperty($"factionUnlocked"));
            instance.Q<EnumField>("CardState").BindProperty(serializedObject.FindProperty($"cardDetails.cardState"));
            instance.Q<IntegerField>("minBuildingLevel").BindProperty(serializedObject.FindProperty($"cardDetails.minBuildingLevel"));
            instance.Q<IntegerField>("purchaseCost").BindProperty(serializedObject.FindProperty($"cardDetails.purchaseCost"));
            instance.Q<FloatField>("upgradeCostMultiplier").BindProperty(serializedObject.FindProperty($"cardDetails.upgradeCostMultiplier"));
            instance.Q<FloatField>("fragmentCostMultiplier").BindProperty(serializedObject.FindProperty($"cardDetails.fragmentCostMultiplier"));

            instance.Q<Button>("select").clicked += () => OnClickSelect(main);

            if (prevFactionName != main.buildingIdentity.faction)
            {
                selectScroll.Add(spacerVisualTree.CloneTree());
                prevFactionName = main.buildingIdentity.faction;
            }
            selectScroll.Add(instance);
        }
    }

    private void GenerateMainLevelData(MainBuildingDataSO mainSO)
    {
        var serializedObject = new SerializedObject(mainSO);

        for (int i = 0; i < mainSO.mainBuildingUpgradeData.Count; i++)
        {
            var instance = cityCenterLevelDataVisualTree.CloneTree();
            SerializedProperty serializedProperty = serializedObject.FindProperty($"mainBuildingUpgradeData.Array.data[{i}]");

            instance.Q<EnumField>("Name").value = mainSO.gameBuildingName;

            // Bind the to the specific array element property
            instance.Q<IntegerField>("Level").BindProperty(serializedProperty.FindPropertyRelative("buildingLevel"));
            instance.Q<FloatField>("Health").BindProperty(serializedProperty.FindPropertyRelative("buildingBasicStats.maxHealth"));
            instance.Q<FloatField>("Armor").BindProperty(serializedProperty.FindPropertyRelative("buildingBasicStats.armor"));
            instance.Q<FloatField>("BuildTime").BindProperty(serializedProperty.FindPropertyRelative("buildingBuildTime"));
            instance.Q<IntegerField>("maxDeckEquipCount").BindProperty(serializedProperty.FindPropertyRelative("maxDeckEquipCount"));
            instance.Q<IntegerField>("maxPopulation").BindProperty(serializedProperty.FindPropertyRelative("maxPopulation"));
            instance.Q<IntegerField>("starterResources").BindProperty(serializedProperty.FindPropertyRelative("starterResources"));

            levelDataScroll.Add(instance);
        }
    }
    #endregion

    #region Helper Functions
    private void OnClickGenerateLevels()
    {
        var currentEditingObject = currentEditingObjectField.value as ScriptableObject;
        StatUpgrade.GenerateUpgradeData(currentEditingObject);
        OnClickSelect(currentEditingObject);
    }

    private void OnToggleUpkeep(bool value, List<IntegerField> fields)
    {
        foreach (var field in fields)
        {
            field.style.visibility = value ? Visibility.Visible : Visibility.Hidden;
        }
    }
    private void RegisterButtons(VisualElement root)
    {
        var unitButton = root.Q<Button>("Unit");
        var defenseButton = root.Q<Button>("Defense");
        var resourceButton = root.Q<Button>("Resource");
        var offenseButton = root.Q<Button>("Offense");
        var cityCenterButton = root.Q<Button>("CityCenter");
        var setMaxLevelButton = root.Q<Button>("SetMaxLevel");
        var resetButton = root.Q<Button>("Reset");

        unitButton.clicked += CreateUnitFields;
        defenseButton.clicked += CreateDefenseFields;
        resourceButton.clicked += CreateResourceFields;
        offenseButton.clicked += CreateOffenseFields;
        cityCenterButton.clicked += CreateCityCenterFields;
        setMaxLevelButton.clicked += SetMaxLevel;
        resetButton.clicked += RestoreDefaultValues;
    }

    private void RestoreDefaultValues()
    {
        foreach (var building in allBuildingData.allBuildingsSO)
            building.buildingIdentity.spawnLevel = 0;

        foreach (var main in allBuildingData.mainBuildingSO)
            main.buildingIdentity.spawnLevel = 0;

        foreach (var unit in allUnitData.allUnitsSO)
            unit.unitIdentity.spawnLevel = 0;

        RefreshUI();
    }

    private void SetMaxLevel()
    {
        GameData.GameMaxObjectLevel = maxLevelField.value;
    }
    private void RefreshUI()
    {
        selectScroll.Clear();
        levelDataScroll.Clear();

        currentEditingObjectField.value = null;

        unitLevelPanel.style.display = DisplayStyle.None;
        defenseLevelPanel.style.display = DisplayStyle.None;
        mainLevelPanel.style.display = DisplayStyle.None;
        resourceLevelPanel.style.display = DisplayStyle.None;
        offenseLevelPanel.style.display = DisplayStyle.None;

        unitFieldHeader.style.display = DisplayStyle.None;
        mainFieldHeader.style.display = DisplayStyle.None;
        defenseFieldHeader.style.display = DisplayStyle.None;
        resourceFieldHeader.style.display = DisplayStyle.None;
        offenseFieldHeader.style.display = DisplayStyle.None;

        if (generateLevelsButton != null)
            generateLevelsButton.clicked -= OnClickGenerateLevels;

        generateLevelsButton = null;
    }
    private List<IntegerField> GetBuildingCostsFields(VisualElement instance)
    {
        List<IntegerField> fields = new()
        {
            instance.Q<IntegerField>("Food"),
            instance.Q<IntegerField>("Gold"),
            instance.Q<IntegerField>("Metal"),
            instance.Q<IntegerField>("Power")
        };
        return fields;
    }
    private List<IntegerField> GetUpKeepCostsFields(VisualElement instance)
    {
        List<IntegerField> fields = new()
        {
            instance.Q<IntegerField>("FoodUpkeep"),
            instance.Q<IntegerField>("GoldUpkeep"),
            instance.Q<IntegerField>("MetalUpkeep"),
            instance.Q<IntegerField>("PowerUpkeep")
        };
        return fields;
    }
    #endregion

}
#endif