using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Progression")]
    [SerializeField] private List<XP_Required> xpRequirements;

    [System.Serializable]
    public class LevelTrack
    {
        public List<TrackEntry> entries;
    }
    

    [Header("References")]
    [SerializeField] private PlayerLevelData playerLevelData;
    [SerializeField] private Userdata _Data;

    [Header("Track UI")]
    [SerializeField] private TrackEntryUI trackEntryPrefab;
    [SerializeField] private List<LevelTrack> trackEntriesPerLevel;

    public int _XP;

    [Serializable]
    public class XP_Required
    {
        public int level;
        public int xpRequired;
    }

    public int PlayerXP
    {
        get => _XP;
        set
        {
            _XP = value;
            SetLevel();
        }
    }

    public void SetLevel()
    {
        int currentLevel = 0;

        for (int i = 0; i < xpRequirements.Count; i++)
        {
            if (xpRequirements[i].xpRequired <= PlayerXP)
                currentLevel++;
        }

        _Data.Level = currentLevel;
    }

    public void SetXP(int amount)
    {
        PlayerXP += amount;
    }

    public void GenrateLevel(LevelBox prefab, Transform parent)
    {
        if (xpRequirements.Count == 0)
            return;

        // 🔹 LEVEL 1
        CreateLevelBox(prefab, parent, 1);

        // 🔹 LOOP
        for (int i = 1; i < xpRequirements.Count; i++)
        {
            int level = i + 1;

            // 🔸 Entries BETWEEN levels (display only)
            if (trackEntriesPerLevel != null && i - 1 < trackEntriesPerLevel.Count)
            {
                var levelTrack = trackEntriesPerLevel[i - 1];
                var entries = levelTrack.entries;

                if (entries != null)
                {
                    foreach (var entry in entries)
                    {
                        TrackEntryUI ui = Instantiate(trackEntryPrefab, parent);

                        bool isLocked = level > _Data.Level;
                        ui.Init(entry, isLocked);
                    }
                }
            }

            // 🔸 Next level box
            CreateLevelBox(prefab, parent, level);
        }
    }

    private void CreateLevelBox(LevelBox prefab, Transform parent, int level)
    {
        LevelBox box = Instantiate(prefab, parent);

        bool isLocked = level > _Data.Level;
        var entry = playerLevelData.GetEntry(level);

        box.Init(level, isLocked, entry);
        box.gameObject.name = "Level_" + level;
    }
}