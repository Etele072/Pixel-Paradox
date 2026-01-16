using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    Vector2 movement;

    void Update()
    {
        // 1. Get Input (A/D or Left/Right)
        movement.x = Input.GetAxisRaw("Horizontal");

        // 2. Set Animator Parameter
        animator.SetFloat("Speed", Mathf.Abs(movement.x));

        // 3. Flip Sprite direction
        if (movement.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (movement.x < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void FixedUpdate()
    {
        // 4. Apply Physics Movement
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}