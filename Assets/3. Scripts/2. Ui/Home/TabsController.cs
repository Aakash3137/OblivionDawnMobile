using System;
using UnityEngine;

public class TabsController : MonoBehaviour
{
    public TabButton01[] tabButtons;
    public GameObject[] panels;

    private int currentIndex = -1;

    void Start()
    {
        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].Init(this, i);
        }

        SelectTab(2); // Default tab
    }

    public void SelectTab(int index)
    {
        if (currentIndex == index)
            return;

        currentIndex = index;

        for (int i = 0; i < tabButtons.Length; i++)
        {
            bool isSelected = i == index;

            tabButtons[i].SetSelected(isSelected);
            panels[i].SetActive(isSelected);

            //get child of button named "Icon" and set it's scale to 1.2 using lerp animation
            
            Transform iconTransform = tabButtons[i].transform.Find("Icon");
            if (iconTransform != null)
            {
                //set recttransform scale to 1.2 using lerp animation
                Vector3 targetScale = isSelected ? Vector3.one * 1.2f : Vector3.one * 0.8f;
                StartCoroutine(LerpScale(iconTransform, targetScale, 0.15f));

            }


        }
    }

    private System.Collections.IEnumerator LerpScale(Transform iconTransform, Vector3 targetScale, float duration)
    {
        Vector3 startScale = iconTransform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            iconTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        iconTransform.localScale = targetScale;
    }
}