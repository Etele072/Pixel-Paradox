using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header("Dashing")]
    public bool canDash = true;
    public bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;

    [Header("Jumping")]
    public float jumpPower = 10f;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    void Update()
    {
        if (isDashing)
        {
            return;
        }

        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);

        Gravity();
        Flip();
        UpdateAnimations();
    }

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void UpdateAnimations()
    {
        bool grounded = isGrounded();
        animator.SetBool("isJumping", !grounded);

        if (!grounded)
        {
            float yVel = rb.linearVelocity.y;
            if (yVel < 3f && yVel > -3f)
            {
                animator.SetFloat("yVelocity", 0f);
            }
            else
            {
                animator.SetFloat("yVelocity", yVel);
            }
        }

        animator.SetFloat("xVelocity", Mathf.Abs(horizontalMovement));
    }

    private void Flip()
    {
        if (horizontalMovement > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalMovement < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        
        rb.gravityScale = 0f;
        
        // A jelenlegi velocity helyett a karakter irányát (localScale.x) használjuk.
        // Így álló helyzetbõl is kilõ a karakter.
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }

        if (context.canceled && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPos == null) return;
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}