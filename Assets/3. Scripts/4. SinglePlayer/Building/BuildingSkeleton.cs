using System.Collections.Generic;
using UnityEngine;

public class BuildingSkeleton : MonoBehaviour
{
    [SerializeField] private List<MonoBehaviour> GenericComponents;

    private void Awake()
    {
        foreach (var component in GenericComponents)
        {
            component.enabled = false;
        }
    }
}
