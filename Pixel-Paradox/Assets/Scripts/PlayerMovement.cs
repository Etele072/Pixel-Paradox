using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Vízszintes Mozgás")]
    public float moveSpeed = 10f;
    public float acceleration = 12f;
    public float decceleration = 12f;
    public float velPower = 0.9f;

    [Header("Ugrás Beállítások")]
    public float jumpForce = 14f;
    [Range(0, 1)] public float jumpCutMultiplier = 0.5f;
    public float jumpBufferTime = 0.15f;
    public float coyoteTime = 0.15f;

    [Header("Gravitáció (A lebegés ellen)")]
    [Tooltip("Minél nagyobb, annál gyorsabban zuhan le a karakter.")]
    public float fallGravityMultiplier = 4.5f; // Megemelve 3-ról 4.5-re
    public float jumpPeakGravityMultiplier = 0.8f;

    [Header("Talaj Ellenõrzés")]
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    public LayerMask groundLayer;

    [Header("Referenciák")]
    public Animator animator;

    // Belsõ változók
    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;
    private float lastGroundedTime;
    private float lastJumpPressedTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ha nem húztad be az Animátort, megpróbálja megkeresni magától
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 1. INPUTOK
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump")) lastJumpPressedTime = jumpBufferTime;
        else lastJumpPressedTime -= Time.deltaTime;

        // 2. TALAJ ELLENÕRZÉSE
        isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer);
        if (isGrounded) lastGroundedTime = coyoteTime;
        else lastGroundedTime -= Time.deltaTime;

        // 3. ANIMÁCIÓK VEZÉRLÉSE
        if (animator != null)
        {
            // Futás animáció (ha a sebesség nem 0)
            animator.SetFloat("Speed", Mathf.Abs(moveInput));

            // Ugrás animáció (ha nincs a földön)
            animator.SetBool("isJumping", !isGrounded);
        }

        // 4. UGRÁS INDÍTÁSA
        if (lastJumpPressedTime > 0 && lastGroundedTime > 0)
        {
            Jump();
        }

        // Változó ugrásmagasság
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
        }

        // 5. KARAKTER MEGFORDÍTÁSA
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyGravity();
    }

    private void ApplyMovement()
    {
        float targetSpeed = moveInput * moveSpeed;
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        rb.AddForce(movement * Vector2.right);
    }

    private void Jump()
    {
        lastJumpPressedTime = 0;
        lastGroundedTime = 0;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    private void ApplyGravity()
    {
        if (rb.linearVelocity.y < 0) // Esés közben
        {
            rb.gravityScale = fallGravityMultiplier;
        }
        else if (rb.linearVelocity.y > 0 && Mathf.Abs(rb.linearVelocity.y) < 2f) // Az ugrás csúcsán
        {
            rb.gravityScale = jumpPeakGravityMultiplier;
        }
        else // Felfelé ugrás közben
        {
            rb.gravityScale = 1.2f; // Kicsit nehezebb alap gravitáció
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
    }
}