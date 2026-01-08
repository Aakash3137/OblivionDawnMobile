using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header(" EDITOR VIEW ONLY ")]
    public float maxHealth;
    public float currentHealth;
    public int Level;
    public float armour;

    [Header("Assign in Inspector")]
    public FadeHealthBar healthBarFade;
    public HealthProgress healthBar; // assign in Inspector

    private void Start()
    {
        healthBar = GetComponentInChildren<HealthProgress>();

        if (healthBar != null)
        {
            healthBar.UpdateFillAmount(currentHealth / maxHealth);
        }
        else
        {
            Debug.Log($"<color=#FFC0CB>{name} missing HealthBar. Assign the script.</color>");
        }
    }
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (healthBar != null)
        {
            healthBar.FadeInHealthBar();
            healthBar.UpdateFillAmount(currentHealth / maxHealth);
        }
        if (currentHealth <= 0)
        {
            Die();
        }
        if (healthBarFade != null)
        {
            healthBarFade.ShowOnHit();
            healthBarFade.Isvisible = true;
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
