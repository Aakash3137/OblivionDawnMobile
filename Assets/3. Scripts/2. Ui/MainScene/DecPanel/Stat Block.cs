using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBlock : MonoBehaviour
{
    [SerializeField] internal Image IconImage;
    [SerializeField] internal TMP_Text TitleText;
    [SerializeField] internal TMP_Text CurrentValueText;
    [SerializeField] internal TMP_Text IncreaseByText;
    [SerializeField] internal bool Increasable;
    [SerializeField] private CanvasGroup canvasGroup;


    public void SetValues(string title, string currentValue, string increaseBy)
    {
        TitleText.text = title;
        CurrentValueText.text = currentValue;
        IncreaseByText.text = increaseBy;
    }

    public void EnableBlock()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void DisableBlock()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
    public void OverrideIcon(Sprite icon)
    {
        IconImage.sprite = icon;
    }
}

