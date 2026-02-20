using UnityEngine;
using UnityEngine.UI;

public class AIModeShift : MonoBehaviour
{
    [Header("Buttons")]
    public Button attackButton;
    public Button defenseButton;


    private void Start()
    {
        attackButton.onClick.AddListener(() => SelectMode(true));
        defenseButton.onClick.AddListener(() => SelectMode(false));

        // Default selection = Attack
        SelectMode(true);
    }

    void SelectMode(bool isAttack)
    {
        MenuManager.Instance.isAttackPersonality = isAttack;
        if (isAttack)
        {
            SetButtonAlpha(attackButton, 1f);
            SetButtonAlpha(defenseButton, 0.4f);
        }
        else
        {
            SetButtonAlpha(defenseButton, 1f);
            SetButtonAlpha(attackButton, 0.4f);
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