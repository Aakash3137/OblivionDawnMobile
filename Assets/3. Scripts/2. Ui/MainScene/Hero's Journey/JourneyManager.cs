using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class JourneyManager : MonoBehaviour
{
    [Header ("UI Element")]
    [SerializeField] internal Button cross_button;
    [SerializeField] private LevelBox levelBoxPrefab;
    [SerializeField] private Transform LevelParent;

    [Header ("Data")]
    [SerializeField] LevelData LevelsData;

#region LifeCycle
    void OnEnable()
    {
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
        StartCoroutine(InitJourney());
    }

    public void Start()
    {
        cross_button.onClick.AddListener(OnClickShowHomePage);
        CreateLevelBox();
    }

    private void OnDisable()
    {
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }

#endregion


#region Init

    IEnumerator InitJourney()
    {
        yield return null; // wait one frame

        // CreateLevelBox();

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(LevelParent.GetComponent<RectTransform>());
    }

#endregion

#region UI
    public void OnClickShowHomePage()
    {
        HomeUIManager.Instance.ShowPanel(PanelName.Home);
        //AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }
    
    private void CreateLevelBox()
    {
        LevelsData.GenrateLevel(levelBoxPrefab, LevelParent);
    }
#endregion
}
