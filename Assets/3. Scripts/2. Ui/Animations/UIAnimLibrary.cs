using UnityEngine;

[CreateAssetMenu(fileName = "UIAnimLibrary", menuName = "UI/Animation Library")]
public class UIAnimLibrary : ScriptableObject
{
    public UIAnimPresetSO panelOpen;
    public UIAnimPresetSO panelClose;
    public UIAnimPresetSO buttonPress;
    public UIAnimPresetSO buttonHover;
}