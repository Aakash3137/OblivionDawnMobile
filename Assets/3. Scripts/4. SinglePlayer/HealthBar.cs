using UnityEngine;

public class SpriteHealthBar : MonoBehaviour
{
    public SpriteRenderer foregroundBar;
    public Stats targetHealth;
    Transform cam;
    float initialWidth;

    void Start()
    {
        cam = Camera.main.transform;
        targetHealth = GetComponentInParent<Stats>();

        if (foregroundBar != null)
            initialWidth = foregroundBar.transform.localScale.x;
       
    }
    void Update()
    {
        // Always face the camera
        transform.LookAt(transform.position + cam.forward);

        // Measure: max health should not be zero
        if (targetHealth != null && foregroundBar != null)
        {
            if (targetHealth.maxHealth != 0)
            {
                float ratio = (float)targetHealth.currentHealth / targetHealth.maxHealth;
                ratio = Mathf.Clamp01(ratio);
                
                // Scale only X
                foregroundBar.transform.localScale = new Vector3(initialWidth * ratio, foregroundBar.transform.localScale.y, foregroundBar.transform.localScale.z);
            }
        }
    }
}