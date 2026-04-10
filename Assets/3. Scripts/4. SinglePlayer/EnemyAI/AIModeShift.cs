using UnityEngine;
using TMPro;
using System.Collections.Generic;

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}

public enum PlayStyle
{
    Attack,
    Defence,
    Mix
}

public class AIModeShift : MonoBehaviour
{
    [Header("TMP Dropdowns")]
    public TMP_Dropdown difficultyDropdown; // Easy, Medium, Hard
    public TMP_Dropdown styleDropdown;      // Attack, Defence, Mix

    private Dictionary<(PlayStyle, Difficulty), AIPersonalityEnum> personalityMap;

    private void Awake()
    {
        // Mapping setup
        personalityMap = new Dictionary<(PlayStyle, Difficulty), AIPersonalityEnum>
        {
            {(PlayStyle.Attack, Difficulty.Easy), AIPersonalityEnum.Tiger_Spark},
            {(PlayStyle.Attack, Difficulty.Medium), AIPersonalityEnum.Lion_Heart},
            {(PlayStyle.Attack, Difficulty.Hard), AIPersonalityEnum.Dragon_Flare},

            {(PlayStyle.Defence, Difficulty.Easy), AIPersonalityEnum.Baffalo_Defense},
            {(PlayStyle.Defence, Difficulty.Medium), AIPersonalityEnum.Rhino_Charge},
            {(PlayStyle.Defence, Difficulty.Hard), AIPersonalityEnum.Elephant_Stance},

            {(PlayStyle.Mix, Difficulty.Easy), AIPersonalityEnum.Rabbit},
            {(PlayStyle.Mix, Difficulty.Medium), AIPersonalityEnum.Goat},
            {(PlayStyle.Mix, Difficulty.Hard), AIPersonalityEnum.Wolf_Pack},
        };
    }

    private void Start()
    {
        difficultyDropdown.onValueChanged.AddListener(OnSelectionChanged);
        styleDropdown.onValueChanged.AddListener(OnSelectionChanged);

        UpdatePersonality();
    }

    void OnSelectionChanged(int index)
    {
        UpdatePersonality();
    }

    void UpdatePersonality()
    {
        Difficulty difficulty = (Difficulty)difficultyDropdown.value;
        PlayStyle style = (PlayStyle)styleDropdown.value;

        AIPersonalityEnum selected = personalityMap[(style, difficulty)];

        MenuManager.Instance.SetPersonality(selected);
    }
}