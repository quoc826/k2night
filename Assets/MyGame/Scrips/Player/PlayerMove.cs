using Unity.Mathematics;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 15f;


    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.5f;
    public LayerMask groundLayer;

    [Header("Components")]
    private Rigidbody2D rb;
    private bool IsFacingRight = true;
    private PlayerAttack playerAttack;

    [Header("Animation")]
    public Animator anim;
    private int isRunID;
    private int isJumpID;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        playerAttack = GetComponent<PlayerAttack>();
        isRunID = Animator.StringToHash("isRun");
        isJumpID = Animator.StringToHash("isJump");
    }

    void Update()
    {
        Move();
    }


    void Move()
    {
 
        if (playerAttack.isAttacking())
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            anim.SetBool(isRunID, false);
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        if (math.abs(rb.linearVelocity.x) > 0.1f && isGroundCheck())
        {

            anim.SetBool(isRunID, true);
        }
        else
        {

            anim.SetBool(isRunID, false);
        }

        if (horizontal > 0 && !IsFacingRight)
        {
            Flip();
        }
        else if (horizontal < 0 && IsFacingRight)
        {
            Flip();
        }

        if (Input.GetButtonDown("Jump") && isGroundCheck())
        {

            anim.SetTrigger(isJumpID);
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    public bool facingRight()
    {
        return IsFacingRight;
    }

    void Flip()
    {
        IsFacingRight = !IsFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }



    private bool isGroundCheck()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}
