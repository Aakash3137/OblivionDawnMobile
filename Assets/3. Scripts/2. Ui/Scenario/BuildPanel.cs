using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuildPanel : MonoBehaviour
{
    [Header("Panel Sections")]
    [SerializeField] private GameObject _offenseBG;
    [SerializeField] private CanvasGroup _offenseBuildPanel;
    [SerializeField] private GameObject _defenseBG;
    [SerializeField] private CanvasGroup _defenseBuildPanel;
    [SerializeField] private GameObject _resourceBG;
    [SerializeField] private CanvasGroup _resourceBuildPanel;

    // [Header("Option Images")]
    // [SerializeField] private GameObject[] _offenseOptionImages;
    // [SerializeField] private GameObject[] _defenseOptionImages;
    // [SerializeField] private GameObject[] _resourceOptionImages;

    [Header("Buttons")]
    [SerializeField] private Button _offenseButton;
    [SerializeField] private Button _defenseButton;
    [SerializeField] private Button _resourceButton;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.3f;

    private Coroutine fadeRoutine;

    void Start()
    {
        //OnClickOffense();
        ActivatePanels(_offenseBG, _offenseBuildPanel);
        ActivatePanels(_defenseBG, _defenseBuildPanel);
        ActivatePanels(_resourceBG, _resourceBuildPanel);
        AddListeners();
    }

    public void ShowBuildPanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void HideBuildPanel(CanvasGroup canvasGroup)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
    void AddListeners()
    {
        _offenseButton.onClick.AddListener(OnClickOffense);
        _defenseButton.onClick.AddListener(OnClickDefense);
        _resourceButton.onClick.AddListener(OnClickResource);
    }

    private void OnClickOffense()
    {
        ActivatePanels(_offenseBG, _offenseBuildPanel);
    }

    private void OnClickDefense()
    {
        ActivatePanels(_defenseBG, _defenseBuildPanel);
    }

    private void OnClickResource()
    {
        ActivatePanels(_resourceBG, _resourceBuildPanel);
    }

    private void ActivatePanels(GameObject Panel1, CanvasGroup Panel2)
    {
        //DisableAllPanels();
        Panel1.SetActive(true);
        ShowBuildPanel(Panel2);
    }
    
    private void DisableAllPanels()
    {
        _offenseBG.SetActive(false);
        HideBuildPanel(_offenseBuildPanel);
        _defenseBG.SetActive(false);
        HideBuildPanel(_defenseBuildPanel);
        _resourceBG.SetActive(false);
        HideBuildPanel(_resourceBuildPanel);
    }
    
    // // --- Fade Methods ---
    // private void FadeOut()
    // {
    //     if (canvasGroup == null) { gameObject.SetActive(false); return; }
    //     if (fadeRoutine != null) StopCoroutine(fadeRoutine);
    //     fadeRoutine = StartCoroutine(FadeCanvas(1f, 0f, false));
    // }

    // private void FadeIn()
    // {
    //     if (canvasGroup == null) { gameObject.SetActive(true); return; }
    //     gameObject.SetActive(true);
    //     if (fadeRoutine != null) StopCoroutine(fadeRoutine);
    //     fadeRoutine = StartCoroutine(FadeCanvas(0f, 1f, true));
    // }

    // private IEnumerator FadeCanvas(float from, float to, bool enableAtEnd)
    // {
    //     float elapsed = 0f;
    //     canvasGroup.alpha = from;
    //     canvasGroup.interactable = false;
    //     canvasGroup.blocksRaycasts = false;

    //     while (elapsed < fadeDuration)
    //     {
    //         elapsed += Time.deltaTime;
    //         canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
    //         yield return null;
    //     }

    //     canvasGroup.alpha = to;
    //     canvasGroup.interactable = to > 0.9f;
    //     canvasGroup.blocksRaycasts = to > 0.9f;

    //     if (!enableAtEnd && to == 0f)
    //         gameObject.SetActive(false);

    //     fadeRoutine = null;
    // }


}
