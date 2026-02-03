using LitMotion;
using LitMotion.Extensions;
using UnityEngine;

public class RewardCanvas : MonoBehaviour
{
    public static RewardCanvas Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}
