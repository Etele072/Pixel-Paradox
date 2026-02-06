using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

<<<<<<< Updated upstream
    Vector2 movement;
=======
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 7f;
    public float decceleration = 7f;
    float horizontalMovement;

    [Header("Dashing")]
    public bool canDash = true;
    public bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;

    [Header("Jumping")]
    public float jumpPower = 10f;
    [Range(0f, 0.5f)] public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;
    [Range(0f, 0.5f)] public float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;
>>>>>>> Stashed changes

    void Update()
    {
        // 1. Get Input (A/D or Left/Right)
        movement.x = Input.GetAxisRaw("Horizontal");

<<<<<<< Updated upstream
        // 2. Set Animator Parameter
        animator.SetFloat("Speed", Mathf.Abs(movement.x));
=======
         //Jump

        if (isGrounded())
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
            ApplyJump();
        }
>>>>>>> Stashed changes

        // 3. Flip Sprite direction
        if (movement.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (movement.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

<<<<<<< Updated upstream
    void FixedUpdate()
    {
        // 4. Apply Physics Movement
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
=======
    private void FixedUpdate()
    {
        if (isDashing) return;

        Run();
    }

    private void Run()
    {
        //Run speed calculation

        float targetSpeed = horizontalMovement * moveSpeed;
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void ApplyJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        jumpBufferCounter = 0f;
        coyoteTimeCounter = 0f; // Megakadályozza a dupla ugrást coyote time alatt
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

        if (isDashing) return;

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
            animator.SetFloat("xVelocity", Mathf.Abs(horizontalMovement));
            animator.SetBool("isJumping", !isGrounded());

            animator.SetTrigger("DashTrigger");

            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        animator.SetBool("isDashing", true);

        float originalGravity = rb.gravityScale;
        
        rb.gravityScale = 0f;
        
        // A jelenlegi velocity helyett a karakter irányát (localScale.x) használjuk.
        // Így álló helyzetbõl is kilõ a karakter.
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

    public void Jump(InputAction.CallbackContext context)
    {
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

    private bool isGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheckPos == null) return;
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
>>>>>>> Stashed changes
    }
}