using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingSceneController : MonoBehaviour
{
    [Header("UI")]
    public Slider loadingSlider;
    public TMP_Text loadingText;
    public TMP_Text percentageText;

    [Header("Scene")]
    public string sceneToLoad = "Home";

    [Header("Loading Settings")]
    public float minimumLoadingTime = 4f;   // 👈 Increase this for more delay
    public float fillSpeed = 0.5f;          // Slider speed

    private void Start()
    {
        StartCoroutine(LoadSceneAsync());
        StartCoroutine(AnimateLoadingText());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        float timer = 0f;
        float displayedProgress = 0f;

        while (!operation.isDone)
        {
            timer += Time.deltaTime;

            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);
            float fakeProgress = timer / minimumLoadingTime;

            float targetProgress = Mathf.Min(realProgress, fakeProgress);

            displayedProgress = Mathf.MoveTowards(displayedProgress, targetProgress, fillSpeed * Time.deltaTime);
            loadingSlider.value = displayedProgress;

            if (percentageText != null)
                percentageText.text = Mathf.RoundToInt(displayedProgress * 100f) + "%";

            if (displayedProgress >= 1f && timer >= minimumLoadingTime)
            {
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    IEnumerator AnimateLoadingText()
    {
        string baseText = "Loading";
        int dotCount = 0;

        while (true)
        {
            dotCount = (dotCount + 1) % 4;
            loadingText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(0.4f);
        }
    }
}