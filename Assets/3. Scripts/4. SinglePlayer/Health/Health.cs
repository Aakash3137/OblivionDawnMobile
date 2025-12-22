using UnityEngine; // V.2

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public FadeHealthBar healthBarFade; // assign in Inspector
    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount; currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        // Show health bar when hit. 
        if (healthBarFade != null) healthBarFade.ShowOnHit();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
