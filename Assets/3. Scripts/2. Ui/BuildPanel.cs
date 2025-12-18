using UnityEngine;

public class BuildPanel : MonoBehaviour
{  
    [Header("Offense Variables")]
    [SerializeField] private GameObject _offenseBG;
    [SerializeField] private GameObject _offenseBuildPanel;

    [Header("Defense Variables")]
    [SerializeField] private GameObject _defenseBG;
    [SerializeField] private GameObject _defenseBuildPanel;

    [Header("Coin Variables")]
    [SerializeField] private GameObject _coinBG;
    [SerializeField] private GameObject _coinBuildPanel;

    [Header("Offense Options")]
    [SerializeField] private GameObject[] _offenseOptionImages;

    [Header("Defense Options")]
    [SerializeField] private GameObject[] _defenseOptionImages;

    [Header("Coin Options")]
    [SerializeField] private GameObject[] _coinOptionImages;

    void Start()
    {
        StartSettings();
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
}

