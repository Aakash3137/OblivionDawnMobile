using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UintStanceButton : MonoBehaviour
{
    private Button button;
    private TMP_Text buttonText;

    void Start()
    {
        button = GetComponent<Button>();
        buttonText = GetComponentInChildren<TMP_Text>();
        button.onClick.AddListener(ToggleStance);
    }

    private void ToggleStance()
    {
        var currentStance = GameManager.Instance.unitStance;
        var color = new Color(1f, 1f, 1f, 1f);

        switch (GameManager.Instance.unitStance)
        {
            case UnitStance.Attacking:
                currentStance = UnitStance.Defending;
                color = new Color(0f, 0f, 1f, 1f);
                break;
            case UnitStance.Defending:
                currentStance = UnitStance.Attacking;
                color = new Color(1f, 0f, 0f, 1f);
                break;
        }

        GameManager.Instance.unitStance = currentStance;

        buttonText.SetText($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{currentStance}");
    }
}
