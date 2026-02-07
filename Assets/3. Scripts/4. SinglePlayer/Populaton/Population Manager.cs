using UnityEngine;

public class PopulationManager : MonoBehaviour
{
    public static PopulationManager Instance;

    private void Awake()
    {
        Instance = this;
    }

}
