using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TrackEntry
{
    public TrackEntryType entryType;

    [Header("UI")]
    public Sprite icon;
    public string displayText;
}

public enum TrackEntryType
{
    MapUnlock,
    FeatureUnlock,
    Info
}

public class TrackEntryUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;

    public void Init(TrackEntry entry, bool isLocked)
    {
        icon.sprite = entry.icon;
        label.text = entry.displayText;

        // Optional visual lock (dim)
        if (isLocked)
        {
            icon.color = Color.gray;
            label.color = Color.gray;
        }
        else
        {
            icon.color = Color.white;
            label.color = Color.white;
        }
    }
}