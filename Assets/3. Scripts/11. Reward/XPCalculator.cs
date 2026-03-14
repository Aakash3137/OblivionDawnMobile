using UnityEngine;

public class XPCalculator : MonoBehaviour
{
    public static XPCalculator Instance;

    private float base_XP;
    private float performance_XP;
    private float bonus_XP;
    private float duration_Multiplier = 1f;
    private float balance_Multiplier = 1f;
    public int total_XP { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddBonusXP(float value) => bonus_XP += value;
    public void SetDurationMultiplier(float value) => duration_Multiplier = value;
    public void SetBalanceMultiplier(float value) => balance_Multiplier = value;

    public void UpdateXP(GameStateEnum state)
    {
        switch (state)
        {
            case GameStateEnum.VICTORY: base_XP = 100; break;
            case GameStateEnum.DEFEAT: base_XP = 35; break;
            case GameStateEnum.DRAW: base_XP = 60; break;
            default: base_XP = 0; break;
        }

        CalculateXP();
    }

    private float PerformanceXP()
    {
        float unitKills = KillCounterManager.Instance.playerTotalUnitKills;
        float resourceKills = KillCounterManager.Instance.playerTotalResourceKills;
        float defenseKills = KillCounterManager.Instance.playerTotalDefenseKills;
        float offenseKills = KillCounterManager.Instance.playerTotalOffenseKills;

        performance_XP = (unitKills * 0.5f +
                          resourceKills * 1.5f +
                          defenseKills * 1.2f +
                          offenseKills * 1.2f);
        return performance_XP;
    }

    private float BonusXP()
    {
        /*
         float difficultyMultiplier = 1f;
        float winStreakBonus = 1f;

        // Difficulty multiplier
        switch (GameController.Instance.difficulty)
        {
            case Difficulty.Easy: difficultyMultiplier = 0.75f; break;
            case Difficulty.Normal: difficultyMultiplier = 1f; break;
            case Difficulty.Hard: difficultyMultiplier = 1.25f; break;
            case Difficulty.Expert: difficultyMultiplier = 1.5f; break;
        }

        // Win streak bonus
        if (GameController.Instance.winStreak >= 3)
            winStreakBonus = 1.1f;
        else if (GameController.Instance.winStreak >= 5)
            winStreakBonus = 1.2f;
        else if (GameController.Instance.winStreak >= 10)
            winStreakBonus = 1.5f;

        bonus_XP = difficultyMultiplier * winStreakBonus;
        */

        return bonus_XP;
    }

    private void CalculateXP()
    {
        total_XP = (int)((base_XP + PerformanceXP() + BonusXP()) * duration_Multiplier * balance_Multiplier);
    }
}