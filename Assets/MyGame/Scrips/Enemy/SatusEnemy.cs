using UnityEngine; 

public class SatusEnemy : MonoBehaviour, IcanTakeDamage
{
    [Header("Enemy Status")]
    public int maxHealth = 100;
    public int currentHealth;
    bool isDead = false;

    [Header("Enemy Attack")]
    public int attackDamage = 10;
    public float attackRange = 1.5f; 
    public float timeDestroy = 2f;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log(gameObject.name + " sinh ra với máu: " + currentHealth);
    }

    public void Die()
    {
        isDead = true;
        Debug.Log(gameObject.name + " đã CHẾT!");
        Destroy(gameObject, timeDestroy);
    }

    public void TakeDamage(int damageAmount, Vector2 hitPoint, GameObject hitDirection)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        
        Debug.Log(gameObject.name + " bị chém! Trừ " + damageAmount + " máu. Máu hiện tại: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }
}