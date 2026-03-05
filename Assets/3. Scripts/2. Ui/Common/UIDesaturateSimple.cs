using UnityEngine;
using UnityEngine.UI;

public class UIDesaturateSimple : MonoBehaviour
{
    [SerializeField] private Image targetImage;

    public void Desaturate()
    {
        Color c = targetImage.color;

        float gray = (c.r + c.g + c.b) / 3f;
        targetImage.color = new Color(gray, gray, gray, c.a);
    }

    public void Restore()
    {
        targetImage.color = Color.white;
    }
}