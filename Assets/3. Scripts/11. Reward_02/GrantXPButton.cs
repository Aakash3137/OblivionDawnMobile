using UnityEngine;
using UnityEngine.UI;

public class GrantXPButton : MonoBehaviour
{
    [Header("XP Settings")]
    public int xpAmount = 100;   // 👈 Set from Inspector

    [Header("References")]
    public LevelData levelData;  // 👈 Assign your LevelData asset

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickGrantXP);
    }

    private void OnClickGrantXP()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData not assigned!");
            return;
        }

        levelData.SetXP(xpAmount);

        Debug.Log($"Granted {xpAmount} XP. Total XP: {levelData.PlayerXP}");
    }
}