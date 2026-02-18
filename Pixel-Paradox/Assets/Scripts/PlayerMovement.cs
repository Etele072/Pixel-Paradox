using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Health & Checkpoint")]
    private bool isDead = false;
    private Vector2 checkpointPos; 

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

    [Header("Crouch & Crawl Settings")]
    public float crouchSpeed = 2.5f;
    public float crawlSpeed = 1.5f;
    [SerializeField] private Vector2 crouchSize = new Vector2(1f, 0.8f);
    [SerializeField] private Vector2 crouchOffset = new Vector2(0f, -0.6f);
    private Vector2 originalSize;
    private Vector2 originalOffset;
    private bool isCrouching = false;

    void Start()
    {
        Application.targetFrameRate = 144;

        checkpointPos = transform.position;

        if (playerCollider != null)
        {
            originalSize = playerCollider.size;
            originalOffset = playerCollider.offset;
        }
    }

    void FixedUpdate()
    {
        if (isDead || isDashing) return;

        bool grounded = isGrounded();

        if (grounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
            ExecuteJump();

        float speed = isCrouching ? crawlSpeed : moveSpeed;
        rb.linearVelocity = new Vector2(horizontalMovement * speed, rb.linearVelocity.y);

        Gravity();
        Flip();
        UpdateAnimations(grounded);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        horizontalMovement = 0;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }

        animator.SetBool("isDying", true);
        animator.SetBool("isJumping", false);
        animator.SetBool("isCrouching", false);
        animator.SetFloat("xVelocity", 0f);

        StartCoroutine(DeathDelay());
    }

    private IEnumerator DeathDelay()
    {
        yield return new WaitForSecondsRealtime(0.8f);

        transform.position = checkpointPos;

        isDead = false;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (playerCollider != null)
        {
            playerCollider.enabled = true;
        }

        animator.SetBool("isDying", false);
        animator.Play("Movement");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spike"))
            Die();

        if (collision.CompareTag("Checkpoint"))
        {
            checkpointPos = collision.transform.position;
            Debug.Log("Checkpoint mentve: " + checkpointPos);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
            Die();
    }


    private void ExecuteJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isDead) return;
        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
            if (isCrouching) StopCrouch();
        }

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
        if (isDashing || isDead) return;

        animator.SetBool("isJumping", !grounded);
        animator.SetBool("isCrouching", isCrouching);
        animator.SetFloat("xVelocity", Mathf.Abs(horizontalMovement));

        if (!grounded)
        {
            float yVel = rb.linearVelocity.y;
            animator.SetFloat("yVelocity", (yVel < 3f && yVel > -3f) ? 0f : yVel);
        }
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
        if (!isDead && context.performed && canDash)
            StartCoroutine(DashCoroutine());
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
        if (isDead) horizontalMovement = 0;
        else horizontalMovement = context.ReadValue<Vector2>().x;
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
    }

    private void StartCrouch()
    {
        if (isCrouching || isDead) return;
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

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed)
            StartCrouch();
        else if (context.canceled)
            StopCrouch();
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPos == null) return;
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
    }
}