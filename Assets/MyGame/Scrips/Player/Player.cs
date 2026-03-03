using UnityEngine;

public class Player : MonoBehaviour, IcanTakeDamage
{
    [Header("status")]
    public int health = 100;
    public int currentHealth;
    public bool isDead = false;



    void  Start()
    {
        currentHealth = health;
    }

    private void Dead()
    {
        isDead = true;

        if(currentHealth <= 0)
        {
            isDead = true;
            
        }
    }
    

    public void TakeDamage(int damageAmount, Vector2 hitPoint, GameObject hitDirection)
    {
        if(isDead) return;
        currentHealth -= damageAmount;
        if(currentHealth <= 0)
        {
            Dead();
        }
    }

    public bool  IsDead()
    {
        return isDead;
    }       
}
