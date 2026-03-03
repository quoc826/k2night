using UnityEngine;

public class SatusEnemy : MonoBehaviour, IcanTakeDamage
{
    [Header("Enemy Status")]
    public int maxHealth = 100;
    public int currentHealth;
    public bool isDead = false; 

    [Header("Enemy Attack")]
    public int attackDamage = 10;
    public float timeDestroy = 2f;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    public Transform attackPoint;
    public LayerMask PlayerLayer;

    [Header("time attack")]
    public float nextAttackTime = 0f;
    
    public bool isAttacking = false; 

    [Header("Enemy Animation")]
    public Animator anim;
    private int isAttackHash;
    private int isDieHash;
    private int isWalkHash;
    private int isHitHash;
    private int isIdleHash;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        isAttackHash = Animator.StringToHash("isAttack");
        isDieHash = Animator.StringToHash("isDie");
        isWalkHash = Animator.StringToHash("isWalk");
        isHitHash = Animator.StringToHash("isHit");
        isIdleHash = Animator.StringToHash("isIdle");
        currentHealth = maxHealth;
    }

    public void Die()
    {
        isDead = true;
        anim.SetTrigger(isDieHash);
        Destroy(gameObject, timeDestroy);
    }

    public void TakeDamage(int damageAmount, Vector2 hitPoint, GameObject hitDirection)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        anim.SetTrigger(isHitHash);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Player"))
        {
            isAttacking = true; 

            if (collision.GetComponent<Player>().IsDead() == false)
            {
                if (Time.time > nextAttackTime)
                {
                    nextAttackTime = Time.time + attackCooldown; 

                    IcanTakeDamage damageable = collision.GetComponent<IcanTakeDamage>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(attackDamage, transform.position, gameObject);
                        anim.SetBool(isAttackHash, true);
                    }
                }
            }
            else
            {
                isAttacking = false;
                anim.SetBool(isIdleHash, true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            isAttacking = false; 
            anim.SetBool(isAttackHash, false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}