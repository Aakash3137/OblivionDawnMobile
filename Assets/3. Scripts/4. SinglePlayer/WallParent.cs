using System.Collections.Generic;
using UnityEngine;

public class WallParent : MonoBehaviour
{
    [SerializeField] private List<GameObject> _Walls;

    public void DisableWall(int index)
    {
        _Walls[index].SetActive(false);
    }
}
