using UnityEngine;

public class UnitSide : MonoBehaviour
{
    public Side side; // set in Inspector or at runtime

    void Start()
    {
        SideManager manager = FindAnyObjectByType<SideManager>();
        if (manager != null)
        {
            manager.SetSide(gameObject, side);
        }
    }
}
