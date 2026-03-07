using System;
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

#region UI
    public void OnClickShowHomePage()
    {
        HomeUIManager.Instance.ShowPanel(PanelName.Home);
        AudioManager.PlayAudioOnce(GameAudioType.ButtonClick);
    }
    
    private void CreateLevelBox()
    {
        LevelsData.GenrateLevel(levelBoxPrefab, LevelParent);
    }
#endregion
}
