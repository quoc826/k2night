using System.Collections;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement Settings")]
    public float speed = 2f;
    public float waitTime = 2f;

    [Header("Animation")]
    public Animator anim;

    private int isIdleHash;
    private Transform currentTarget;
    private bool isWaiting = false;

    private SatusEnemy satusEnemy; 

    void Start()
    {
        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
        }

        satusEnemy = GetComponent<SatusEnemy>(); 

        isIdleHash = Animator.StringToHash("isIdle");
        currentTarget = pointA;
    }

    void Update()
    {
        if (satusEnemy != null && satusEnemy.isDead) return;

        if (satusEnemy != null && satusEnemy.isAttacking) return;

        if (isWaiting) return;

        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);
        
        if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            StartCoroutine(WaitAndSwitchTarget());
        }
        else
        {
            anim.SetBool(isIdleHash, false);
        }
    }

    private IEnumerator WaitAndSwitchTarget()
    {
        isWaiting = true;
        anim.SetBool(isIdleHash, true);
        
        yield return new WaitForSeconds(waitTime);
        
        currentTarget = currentTarget == pointA ? pointB : pointA;
        FlipSprite();

        isWaiting = false;
    }

    private void FlipSprite()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}