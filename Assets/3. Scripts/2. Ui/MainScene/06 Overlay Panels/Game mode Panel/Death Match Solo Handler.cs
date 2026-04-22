using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathMatchSoloHandler : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private TMP_Dropdown playStyleDropdown;
    [SerializeField] private TMP_Dropdown factionDropdown;

    private void Start()
    {
        difficultyDropdown.ClearOptions();

        string[] difficultyNames = Enum.GetNames(typeof(Difficulty));
        difficultyDropdown.AddOptions(new List<string>(difficultyNames));

        playStyleDropdown.ClearOptions();

        string[] playStyleNames = Enum.GetNames(typeof(PlayStyle));
        playStyleDropdown.AddOptions(new List<string>(playStyleNames));

        factionDropdown.ClearOptions();

        string[] enemyFactionNames = Enum.GetNames(typeof(FactionName));
        factionDropdown.AddOptions(new List<string>(enemyFactionNames));
    }

    public void Initialize()
    {
        GameData.difficulty = (Difficulty)difficultyDropdown.value;
        GameData.playStyle = (PlayStyle)playStyleDropdown.value;
        GameData.enemyFaction = (FactionName)factionDropdown.value;

        Debug.Log($"<color=green>[DeathMatchSoloHandler] Set Difficulty: {GameData.difficulty}, PlayStyle: {GameData.playStyle}, EnemyFaction: {GameData.enemyFaction}</color>");
    }
}
