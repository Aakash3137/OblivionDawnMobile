using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBlock : MonoBehaviour
{
    [SerializeField] internal Image icon;
    [SerializeField] internal TMP_Text title;
    [SerializeField] internal TMP_Text currentValueText;
    [SerializeField] internal TMP_Text nextValueText;
    [SerializeField] private CanvasGroup canvasGroup;


    public void SetValues(string title, float currentValue, float nextValue)
    {
        this.title.SetText(title);

        currentValueText.SetText(currentValue % 1f == 0f ? "{0:0}" : "{0:0.00}", currentValue);

        if (nextValue == 0f || nextValue == currentValue)
            nextValueText.SetText("");
        else if (nextValue < 0f)
            nextValueText.SetText(nextValue % 1f == 0f ? "-{0:0}" : "-{0:0.00}", nextValue);
        else if (nextValue > 0f)
            nextValueText.SetText(nextValue % 1f == 0f ? "+{0:0}" : "+{0:0.00}", nextValue);
    }

    public void EnableBlock(bool showNextValue)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        nextValueText.gameObject.SetActive(showNextValue);
    }
    public void DisableBlock()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        nextValueText.gameObject.SetActive(false);
    }
    public void SetIcon(Sprite icon)
    {
        if (this.icon == null || this.icon.sprite == icon)
            return;

        this.icon.sprite = icon;
    }
}

