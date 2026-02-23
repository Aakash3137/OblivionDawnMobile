using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class PersonalityButton
{
    public AIPersonalityEnum personality;
    public Button button;
}

public class AIModeShift : MonoBehaviour
{
    [Header("Personality Buttons")]
    public List<PersonalityButton> personalityButtons; // each button tagged with its enum

    private void Start()
    {
        foreach (var pb in personalityButtons)
        {
            AIPersonalityEnum captured = pb.personality;
            pb.button.onClick.AddListener(() => SelectPersonality(captured));
        }

        // Default = Dragon_Flare
        SelectPersonality(AIPersonalityEnum.Dragon_Flare);
    }

    void SelectPersonality(AIPersonalityEnum selected)
    {
        MenuManager.Instance.SetPersonality(selected);

        foreach (var pb in personalityButtons)
        {
            SetButtonAlpha(pb.button, pb.personality == selected ? 1f : 0.4f);
        }
    }

    void SetButtonAlpha(Button button, float alpha)
    {
        ColorBlock colors = button.colors;
        Color normal = colors.normalColor;
        normal.a = alpha;
        colors.normalColor = normal;
        button.colors = colors;
    }
}