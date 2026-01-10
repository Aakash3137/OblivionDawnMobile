using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    protected float progressAmount = 0f;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        transform.forward = mainCamera.transform.forward;
    }

    public virtual void UpdateFillAmount(float amount)
    {
        amount = Mathf.Clamp01(amount);
        fillImage.fillAmount = amount;
    }
}
