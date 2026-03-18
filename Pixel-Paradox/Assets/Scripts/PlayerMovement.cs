using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerMovement : MonoBehaviour
{
    public SoundManager soundManager;

    #region Variables: Components & Settings
    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    [SerializeField] private CapsuleCollider2D playerCollider;
    private EnemyManager enemyManager;

    [Header("Health & Checkpoint")]
    private bool isDead = false;
    private Vector2 checkpointPos;

    [Header("Movement")]
    public float moveSpeed = 5f;
    private float horizontalMovement;

    [Header("Slope Handling")]
    [SerializeField] private float slopeCheckDistance = 0.5f;
    private Vector2 slopeNormalPerp;
    private bool isOnSlope;

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

    [Header("Crouch & Crawl")]
    public float crouchSpeed = 2.5f;
    public float crawlSpeed = 1.5f;
    [SerializeField] private Vector2 crouchSize = new Vector2(1f, 0.8f);
    [SerializeField] private Vector2 crouchOffset = new Vector2(0f, -0.6f);
    private Vector2 originalSize;
    private Vector2 originalOffset;
    private bool isCrouching = false;
    private bool wantsToStandUp = false;

    [Header("Physics Checks")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    [SerializeField] private Transform ceilingCheckPos;
    [SerializeField] private float ceilingCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;
    #endregion

    #region Unity Callbacks
    void Start()
    {
        Application.targetFrameRate = 144;
        checkpointPos = transform.position;
        enemyManager = UnityEngine.Object.FindFirstObjectByType<EnemyManager>();

        if (playerCollider != null)
        {
            originalSize = playerCollider.size;
            originalOffset = playerCollider.offset;
        }
    }

    void FixedUpdate()
    {
        if (isDead || isDashing) return;

        CheckIfCanStandUp();
        CheckSlope(); // ÚJ: Lejtő ellenőrzése

        bool grounded = isGrounded();

        // Coyote & Buffer management
        if (grounded)
            coyoteTimeCounter = coyoteTime;
        else
            coyoteTimeCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
            ExecuteJump();

        ApplyMovement(grounded);
        Gravity();
        Flip();
        UpdateAnimations(grounded);
    }
    #endregion

    #region Movement Core Logic
    private void CheckSlope()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, slopeCheckDistance, groundLayer);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            isOnSlope = hit.normal != Vector2.up;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
        }
        else
        {
            isOnSlope = false;
        }
    }

    private void ApplyMovement(bool grounded)
    {
        float speed = isCrouching ? crawlSpeed : moveSpeed;

        if (grounded && isOnSlope && !isDashing)
        {
            rb.linearVelocity = new Vector2(speed * -horizontalMovement * slopeNormalPerp.x, speed * -horizontalMovement * slopeNormalPerp.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(horizontalMovement * speed, rb.linearVelocity.y);
        }

    }

    private void ExecuteJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f;
    }

    private void Gravity()
    {
        if (isOnSlope && horizontalMovement == 0 && isGrounded())
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(0, 0);
        }
        else if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }
    #endregion

    #region Input Handlers
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = isDead ? 0 : context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isDead) return;

        bool ceilingAbove = Physics2D.OverlapCircle(ceilingCheckPos.position, ceilingCheckRadius, groundLayer);
        if (ceilingAbove) return;

        if (context.performed)
        {
            jumpBufferCounter = jumpBufferTime;
        }

        if (context.canceled && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        bool ceilingAbove = Physics2D.OverlapCircle(ceilingCheckPos.position, ceilingCheckRadius, groundLayer);
        if (!isDead && context.performed && canDash && !ceilingAbove)
            StartCoroutine(DashCoroutine());
    }

    public void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            wantsToStandUp = false;
            StartCrouch();
        }
        else if (context.canceled)
        {
            bool ceilingAbove = Physics2D.OverlapCircle(ceilingCheckPos.position, ceilingCheckRadius, groundLayer);
            if (!ceilingAbove)
            {
                StopCrouch();
                wantsToStandUp = false;
            }
            else
            {
                wantsToStandUp = true;
            }
        }
    }
    #endregion

    #region Helper Methods (Dash, Flip, Crouch, Health)
    private IEnumerator DashCoroutine()
    {
        SoundManager.Instance.PlaySound2D("Dash");

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

    private void Flip()
    {
        if (horizontalMovement > 0) transform.localScale = Vector3.one;
        else if (horizontalMovement < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void StartCrouch()
    {
        if (isCrouching || isDead) return;
        isCrouching = true;
        animator.SetBool("isCrouching", true);
        float originalBottom = originalOffset.y - originalSize.y / 2f;
        playerCollider.size = crouchSize;
        playerCollider.offset = new Vector2(originalOffset.x, originalBottom + crouchSize.y / 2f);
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

    private void CheckIfCanStandUp()
    {
        if (wantsToStandUp && isCrouching)
        {
            bool ceilingAbove = Physics2D.OverlapCircle(ceilingCheckPos.position, ceilingCheckRadius, groundLayer);
            if (!ceilingAbove) { StopCrouch(); wantsToStandUp = false; }
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        horizontalMovement = 0;
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        if (playerCollider != null) playerCollider.enabled = false;
        animator.SetBool("isDying", true);
        StartCoroutine(DeathDelay());
    }

    private IEnumerator DeathDelay()
    {
        yield return new WaitForSecondsRealtime(0.8f);
        transform.position = checkpointPos;
        if (enemyManager != null) enemyManager.ResetEnemies();
        isDead = false;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (playerCollider != null) playerCollider.enabled = true;
        animator.SetBool("isDying", false);
        animator.Play("Movement");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Spike")) Die();
        if (collision.CompareTag("Checkpoint")) checkpointPos = collision.transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike")) Die();
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

    private bool isGrounded() => Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPos == null) return;
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        if (ceilingCheckPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ceilingCheckPos.position, ceilingCheckRadius);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * slopeCheckDistance);
    }
    #endregion
}