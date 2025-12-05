using UnityEngine; // V.2

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
