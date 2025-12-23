using UnityEngine;
using System.Collections;

public class BuildPanel : MonoBehaviour
{
    [Header("Panel Sections")]
    [SerializeField] private GameObject _offenseBG;
    [SerializeField] private GameObject _offenseBuildPanel;
    [SerializeField] private GameObject _defenseBG;
    [SerializeField] private GameObject _defenseBuildPanel;
    [SerializeField] private GameObject _coinBG;
    [SerializeField] private GameObject _coinBuildPanel;

    [Header("Option Images")]
    [SerializeField] private GameObject[] _offenseOptionImages;
    [SerializeField] private GameObject[] _defenseOptionImages;
    [SerializeField] private GameObject[] _coinOptionImages;

    [Header("Fade Settings")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;

    private Coroutine fadeRoutine;

    void Start()
    {
        StartSettings();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void OnClickOffense()
    {
        _offenseBG.SetActive(true);
        _offenseBuildPanel.SetActive(true);
        _defenseBG.SetActive(false);
        _defenseBuildPanel.SetActive(false);
        _coinBG.SetActive(false);
        _coinBuildPanel.SetActive(false);
    }

    public void OnClickDefense()
    {
        _offenseBG.SetActive(false);
        _offenseBuildPanel.SetActive(false);
        _defenseBG.SetActive(true);
        _defenseBuildPanel.SetActive(true);
        _coinBG.SetActive(false);
        _coinBuildPanel.SetActive(false);
    }

    public void OnClickCoin()
    {
        _offenseBG.SetActive(false);
        _offenseBuildPanel.SetActive(false);
        _defenseBG.SetActive(false);
        _defenseBuildPanel.SetActive(false);
        _coinBG.SetActive(true);
        _coinBuildPanel.SetActive(true);
    }

    public void StartSettings()
    {
        _offenseBG.SetActive(true);
        _offenseBuildPanel.SetActive(true);
        _defenseBG.SetActive(false);
        _defenseBuildPanel.SetActive(false);
        _coinBG.SetActive(false);
        _coinBuildPanel.SetActive(false);
    }

    // --- Fade Methods ---
    public void FadeOut()
    {
        if (canvasGroup == null) { gameObject.SetActive(false); return; }
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeCanvas(1f, 0f, false));
    }

    public void FadeIn()
    {
        if (canvasGroup == null) { gameObject.SetActive(true); return; }
        gameObject.SetActive(true);
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeCanvas(0f, 1f, true));
    }

    private IEnumerator FadeCanvas(float from, float to, bool enableAtEnd)
    {
        float elapsed = 0f;
        canvasGroup.alpha = from;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = to;
        canvasGroup.interactable = to > 0.9f;
        canvasGroup.blocksRaycasts = to > 0.9f;

        if (!enableAtEnd && to == 0f)
            gameObject.SetActive(false);

        fadeRoutine = null;
    }
}
