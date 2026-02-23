using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public AIPersonalityEnum selectedPersonality = AIPersonalityEnum.Dragon_Flare;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void SetPersonality(AIPersonalityEnum personality)
    {
        selectedPersonality = personality;
        GameDebug.Log($"[MenuManager] Selected: {personality}");
    }

    public AIPersonalityEnum SelectedPersonalityFromMenu()
    {
        return selectedPersonality;
    }
}