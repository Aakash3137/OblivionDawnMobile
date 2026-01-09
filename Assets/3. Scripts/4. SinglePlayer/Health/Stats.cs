using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header(" EDITOR VIEW ONLY ")]
    public float maxHealth;
    public float currentHealth;
    public int Level;
    public float armour;
    public Collider hitCollider;

    [Header("Assign in Inspector")]
    public FadeHealthBar healthBarFade;
    public HealthProgress healthBar; // assign in Inspector

    internal virtual void Start()
    {
        healthBar = GetComponentInChildren<HealthProgress>();
        hitCollider = GetComponent<Collider>();

        if (healthBar != null)
        {
            healthBar.UpdateFillAmount(currentHealth / maxHealth);
        }
        else
        {
            //Debug.Log($"<color=#FFC0CB>{name} missing HealthBar. Assign the script.</color>");
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
        if (healthBarFade != null)
        {
            healthBarFade.ShowOnHit();
            healthBarFade.Isvisible = true;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
