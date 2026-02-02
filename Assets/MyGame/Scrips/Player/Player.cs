using UnityEngine;

public class Player : MonoBehaviour
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
        if(currentHealth <= 0)
        {
            isDead = true;
            
        }
    }
}
