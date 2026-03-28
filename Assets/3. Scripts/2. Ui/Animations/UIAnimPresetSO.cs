using UnityEngine;

[CreateAssetMenu(fileName = "UIAnimPreset", menuName = "UI/Animation Preset")]
public class UIAnimPresetSO : ScriptableObject
{
    public float duration = 0.2f;

    [Header("Scale")]
    public bool useScale = true;
    public Vector3 startScale = new Vector3(0.9f, 0.9f, 1f);
    public Vector3 endScale = Vector3.one;

    [Header("Fade")]
    public bool useFade = false;
    public float startAlpha = 0f;
    public float endAlpha = 1f;

    [Header("Bounce")]
    public bool useBounce = true;
    public float bounceStrength = 1.1f;

    [Header("Curve")]
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
}