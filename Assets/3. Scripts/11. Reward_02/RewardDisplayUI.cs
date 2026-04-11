using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Sirenix.Utilities;

public class RewardDisplayUI : MonoBehaviour
{

    public bool IsAnimating { get; private set; }

    [Header("UI")]
    public RectTransform cardRoot;
    public Image icon;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI amountText;

    [Header("Animation")]
    public float popDuration = 0.35f;
    public float moveUpDistance = 80f;
    public float textRiseDuration = 0.3f;

    private Vector2 startPos;

    private void Awake()
    {
        startPos = cardRoot.anchoredPosition;

        IsAnimating = true;
    }

    public void Show(RewardInstance reward)
    {
        StopAllCoroutines();
        gameObject.SetActive(true);

        // Set data
        icon.sprite = reward.rewardData.icon;
        nameText.text = reward.rewardData.rewardName;
        //set alpha of nameText to 0
        nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, 0f);
        amountText.color = new Color(amountText.color.r, amountText.color.g, amountText.color.b, 0f);
        
        amountText.text = "x" + reward.amount;

        // Start animation
        StartCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation()
    {
        // Final position (where card should end)
        Vector2 finalPos = startPos;

        // Start BELOW
        Vector2 startBelow = finalPos - new Vector2(0, moveUpDistance);

        // Reset
        cardRoot.localScale = Vector3.zero;
        cardRoot.anchoredPosition = startBelow;

        float time = 0;

        // POP + MOVE UP INTO PLACE
        while (time < popDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / popDuration);

            // Scale
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f); // nice pop
            cardRoot.localScale = Vector3.one * scale;

            // Move UP into final position
            cardRoot.anchoredPosition = Vector2.Lerp(startBelow, finalPos, t);

            yield return null;
        }

        // TEXT RISE
        yield return StartCoroutine(AnimateTextRise());

        IsAnimating = false;
    }

    private IEnumerator AnimateTextRise()
    {
        float time = 0;

        Vector2 nameStart = nameText.rectTransform.anchoredPosition - new Vector2(0,200);
        //set opacity of nameStart text to 0
        Color nameColor = nameText.color;
        Color amountColor = amountText.color;
        amountColor.a = 0f;
        nameColor.a = 0f;

        Vector2 nameEnd = nameText.rectTransform.anchoredPosition;

        // Vector2 amountStart = amountText.rectTransform.anchoredPosition - new Vector2(0, 40);
        // Vector2 amountEnd = amountText.rectTransform.anchoredPosition;

        // Reset below
        nameText.rectTransform.anchoredPosition = nameStart;
        //amountText.rectTransform.anchoredPosition = amountStart;

        while (time < textRiseDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / textRiseDuration);

            //set opacity of nameText to 1 with fade in
            nameColor.a = t;
            nameText.color = nameColor;

            amountColor.a = t;
            amountText.color = amountColor;

            nameText.color = new Color(nameText.color.r, nameText.color.g, nameText.color.b, t);
            amountText.color = new Color(amountText.color.r, amountText.color.g, amountText.color.b, t);

            nameText.rectTransform.anchoredPosition = Vector2.Lerp(nameStart, nameEnd, t);
            //amountText.rectTransform.anchoredPosition = Vector2.Lerp(amountStart, amountEnd, t);

            yield return null;
        }
    }
}