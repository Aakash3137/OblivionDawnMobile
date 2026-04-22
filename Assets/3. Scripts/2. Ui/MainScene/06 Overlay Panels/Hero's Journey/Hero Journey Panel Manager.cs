using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeroJourneyPanelManager : MonoBehaviour
{
    public static HeroJourneyPanelManager Instance { get; private set; }

    [Header("UI Element")]
    [SerializeField] private Button closeButton;
    [SerializeField] private LevelBox levelBoxPrefab;
    [SerializeField] private Transform LevelParent;

    [Header("Data")]
    [SerializeField] LevelData LevelsData;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        HidePanel();
    }
    public void Start()
    {
        CreateLevelBox();
        closeButton.onClick.AddListener(OnClickClosePanel);
    }

    IEnumerator InitJourney()
    {
        yield return null; // wait one frame

        // CreateLevelBox();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(LevelParent.GetComponent<RectTransform>());
    }
    private void CreateLevelBox()
    {
        LevelsData.GenerateLevel(levelBoxPrefab, LevelParent);
    }
    public void OpenJourneyPanel()
    {
        StartCoroutine(InitJourney());
        ShowPanel();
    }
    private void OnClickClosePanel()
    {
        AudioManager.PlayOneShot(GameAudioType.ButtonClick);
        HidePanel();
    }

    private void ShowPanel()
    {
        if (canvasGroup == null)
            TryGetComponent(out canvasGroup);

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HidePanel()
    {
        if (canvasGroup == null)
            TryGetComponent(out canvasGroup);

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    private void OnDestroy()
    {
        closeButton.onClick.RemoveListener(OnClickClosePanel);
    }
}
