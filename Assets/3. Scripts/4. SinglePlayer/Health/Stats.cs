using UnityEngine;

public class Stats : MonoBehaviour
{
    [Header("For Editor view only")]
    public float maxHealth;
    public float currentHealth;
    public int Level;
    public float armour;
    [Header("Assign in Inspector")]
    public FadeHealthBar healthBarFade; // assign in Inspector

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
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
