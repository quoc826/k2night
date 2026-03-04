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

    [Header("Drop Item")]
    public GameObject dropItem;

    public int minDropAmount = 1;
    public int maxDropAmount = 3;
    public float dropForce = 5f;
    public Transform dropPoint;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        isAttackHash = Animator.StringToHash("isAttack");
        isDieHash = Animator.StringToHash("isDead");
        isWalkHash = Animator.StringToHash("isWalk");
        isHitHash = Animator.StringToHash("isHit");
        isIdleHash = Animator.StringToHash("isIdle");
        currentHealth = maxHealth;
    }

    // Enemy take damage

    public void Die()
    {
        isDead = true;
        anim.SetTrigger(isDieHash);

        Destroy(gameObject, timeDestroy);
        DropItem();
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


    // Attack player
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

    // drop Item

    public void DropItem()
    {
        if (dropItem != null)
        {
            int amountToDrop = Random.Range(minDropAmount, maxDropAmount + 1);
            Vector3 spawmPos = dropPoint != null ? dropPoint.position : transform.position;
            for (int i = 0; i < amountToDrop; i++)
            {
                GameObject spawnedItem = Instantiate(dropItem, spawmPos, Quaternion.identity);
                Rigidbody2D rb = spawnedItem.GetComponent<Rigidbody2D>();

                if (rb != null)
                {
                    Vector2 dropDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1.5f));
                    rb.AddForce(dropDirection.normalized * dropForce, ForceMode2D.Impulse);
                }
                else
                {
                    Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                    spawnedItem.transform.position += randomOffset;
                }
            }
        }
    }
}