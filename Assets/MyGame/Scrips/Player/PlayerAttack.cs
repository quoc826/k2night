using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack")]

    public int attackDamage = 10;
    public float nextAttackTime = 0.2f;
    public float attackDuration = 4f;  // Thời gian tấn công
    private float attackEndTime = 0f;    // Thời điểm kết thúc tấn công
    [Header("Transform")]
    public Transform attackPoint;
    public LayerMask enemyLayers;
    public float attackRange = 0.5f;

    [Header("Animation")]
    public  Animator anim;
    private int attackID;
    private int attack2ID;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        attackID = Animator.StringToHash("isAttack");
        attack2ID = Animator.StringToHash("isAttack2");
    }

    // Update is called once per frame
    void Update()
    {
        AttackController();
    }


    public void AttackController()
    {   
        if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0) && !isAttacking())
        {
            anim.SetTrigger(attackID);
            GetKey();
        }
        if( Input.GetMouseButtonDown(1) && !isAttacking())
        {
            anim.SetTrigger(attack2ID);
            GetKey();
        }

    }

 

    public bool isAttacking()
    {
        return Time.time < attackEndTime;
    }


    IEnumerator Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
        for (int i = 0; i < hitEnemies.Length; i++)
        {

            hitEnemies[i].GetComponent<IcanTakeDamage>()?.TakeDamage(attackDamage, attackPoint.position, gameObject);

        }
        yield return new WaitForSeconds(0.1f);
    }

    private bool GetKey()
    {
        if (Time.time > nextAttackTime)
        {
            StartCoroutine(Attack());
            nextAttackTime = Time.time + 1f / 1f;
            attackEndTime = Time.time + attackDuration;  // Đặt thời gian kết thúc tấn công
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
