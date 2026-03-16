using UnityEngine;
using System;
using System.Collections.Generic;

public class MapLevelController : MonoBehaviour
{
    [SerializeField] private Userdata _Data;

    [SerializeField] List<MapData> mapDatas= new List<MapData>();

    [SerializeField] private LevelBox levelBoxPrefab;
}

[Serializable]
public class MapData
{
    public int LevelNo;
    public bool RewardStatus = false;
    public RewardData rewardData;
}
