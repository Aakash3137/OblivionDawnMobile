#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Base Costs", menuName = "Data/Base Costs")]
public class BaseCosts : ScriptableObject
{
    public int maxLevel = GameData.GameMaxObjectLevel;
    public int baseValue;
    public int maxValue;

    [Space(20)]
    public List<int> fragmentCost;
    [Button("FragmentCosts : Default min = 4; max = 80; divisible by 2")]

    public void GenerateFragmentCosts(float multiplier = 1, int divisibleBy = 2)
    {
        fragmentCost = new List<int>(maxLevel);
        var lastValue = maxValue - baseValue;
        for (int i = 0; i < maxLevel; i++)
        {
            float cost = EaseInOutQuad((float)i / maxLevel) * lastValue + baseValue;
            cost *= multiplier;
            cost = Mathf.CeilToInt(cost);
            cost -= cost % divisibleBy;
            fragmentCost.Add((int)cost);
        }
    }

    [Space(20)]
    public List<int> upgradeCost;
    [Button("UpgradeCosts : Default min = 10; max = 2000; divisible by 10")]
    public void GenerateUpgradeCosts(float multiplier = 1, int divisibleBy = 10)
    {
        upgradeCost = new List<int>(maxLevel);
        var lastValue = maxValue - baseValue;
        for (int i = 0; i < maxLevel; i++)
        {
            float cost = EaseInOutQuad((float)i / maxLevel) * lastValue + baseValue;
            cost *= multiplier;
            cost = Mathf.CeilToInt(cost);
            cost -= cost % divisibleBy;
            upgradeCost.Add((int)cost);
        }
    }

    public static float EaseInOutQuad(float y)
    {
        y = Mathf.Clamp01(y);
        return y < 0.5f ? 2f * y * y : 4 * y - 2 * y * y - 1;
    }
}
#endif