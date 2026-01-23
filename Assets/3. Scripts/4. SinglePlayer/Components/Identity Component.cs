using UnityEngine;

public class IdentityComponent : MonoBehaviour
{
    public Identity identity;
    public Side side;

    public void Initialize(Identity identity, Side side)
    {
        this.identity = identity;
        this.side = side;
    }
}


