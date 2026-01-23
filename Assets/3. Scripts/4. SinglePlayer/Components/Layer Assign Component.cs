using UnityEngine;

public class LayerAssignComponent : MonoBehaviour
{
    MilitaryUnit myUnit;

    public void Initialize(Side side)
    {
        AssignLayers(side);
    }
    private void AssignLayers(Side side)
    {
        myUnit = TryGetComponent<MilitaryUnit>(out var unit) ? unit : null;

        switch (side)
        {
            case Side.Player:

                if (myUnit != null && myUnit.canFly)
                    gameObject.layer = LayerMask.NameToLayer("PlayerAir");
                else
                    gameObject.layer = LayerMask.NameToLayer("PlayerGround");
                break;
            case Side.Enemy:

                if (myUnit != null && myUnit.canFly)
                    gameObject.layer = LayerMask.NameToLayer("EnemyAir");
                else
                    gameObject.layer = LayerMask.NameToLayer("EnemyGround");
                break;
        }
    }

}
