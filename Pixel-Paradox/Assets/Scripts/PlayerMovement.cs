using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    [SerializeField] private BoxCollider2D playerCollider;

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
    public float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    public float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("Crouch Settings")]
    public float crouchSpeed = 2.5f;
    [SerializeField] private Vector2 crouchSize = new Vector2(1f, 0.8f);
    [SerializeField] private Vector2 crouchOffset = new Vector2(0f, -0.6f);
    private Vector2 originalSize;
    private Vector2 originalOffset;
    private bool isCrouching = false;

    void Start()
    {
        if (playerCollider != null)
        {
            originalSize = playerCollider.size;
            originalOffset = playerCollider.offset;
        }
    }

    void Update()
    {
        if (isDashing) return;

        bool grounded = isGrounded();

        if (grounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            ExecuteJump();
        }

        float speed = isCrouching ? crouchSpeed : moveSpeed;
        rb.linearVelocity = new Vector2(horizontalMovement * speed, rb.linearVelocity.y);

        Gravity();
        Flip();
        UpdateAnimations(grounded); 
    }

    private void StartCrouch()
    {
        if (isCrouching) return;
        isCrouching = true;
        animator.SetBool("isCrouching", true);

        float originalBottom = originalOffset.y - originalSize.y / 2f;

        playerCollider.size = crouchSize;

        float newOffsetY = originalBottom + crouchSize.y / 2f;
        playerCollider.offset = new Vector2(originalOffset.x, newOffsetY);
        Physics2D.SyncTransforms();
    }

    private void StopCrouch()
    {
        if (!isCrouching) return;
        isCrouching = false;
        animator.SetBool("isCrouching", false);

        playerCollider.size = originalSize;
        playerCollider.offset = originalOffset;
    }

    private void ExecuteJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed) jumpBufferCounter = jumpBufferTime;

        if (context.canceled && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
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

    private void UpdateAnimations(bool grounded)
    {
        if (isDashing) return;


        animator.SetBool("isJumping", !grounded);

        if (!grounded)
        {
            float yVel = rb.linearVelocity.y;
            if (yVel < 3f && yVel > -3f) animator.SetFloat("yVelocity", 0f);
            else animator.SetFloat("yVelocity", yVel);
        }

        animator.SetFloat("xVelocity", Mathf.Abs(horizontalMovement));
    }

    private void Flip()
    {
        if (horizontalMovement > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalMovement < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash) StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        animator.SetBool("isDashing", true);
        animator.SetTrigger("DashTrigger");

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravity;
        isDashing = false;
        animator.SetBool("isDashing", false);

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike")) Die();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spike")) Die();
    }

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPos == null) return;
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed) StartCrouch();
        else if (context.canceled) StopCrouch();
    }
}