using UnityEngine;

[CreateAssetMenu(menuName = "Rewards/Attack Unit Reward")]
public class UnitRewardData : RewardData02
{
    public UnitProduceStatsSO unitData;

    public override Sprite GetIcon(FactionName faction = default)
    {
        return unitData != null ? unitData.unitIcon : null;
    }

    public void UnlockUnit()
        {
            if (unitData == null)
            {
                Debug.LogError("UnitProduceStatsSO is NULL!");
                return;
            }

            if (unitData.cardDetails == null)
            {
                Debug.LogError($"CardDetails missing for {unitData.name}");
                return;
            }

            if (unitData.cardDetails.cardState == CardState.Unlocked)
            {
                Debug.Log($"{unitData.name} already unlocked");
                return;
            }

            unitData.cardDetails.cardState = CardState.Unlocked;

            Debug.Log($"✅ Unlocked Unit: {unitData.gameUnitName}");
        }

    public override void Grant(int amount)
    {
        //
    }
}