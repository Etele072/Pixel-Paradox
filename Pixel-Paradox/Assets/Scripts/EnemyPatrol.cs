using UnityEngine;
using System.Collections;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Mozgás & Járõrözés")]
    public float moveSpeed = 2f;
    public float chaseSpeed = 4f;
    public Transform leftPoint;
    public Transform rightPoint;
    private Transform targetPoint;

    [Header("Érzékelés & Támadás")]
    public float detectionRange = 6f; 
    public float attackRange = 1.5f;
    public float explosionRadius = 2.5f;
    public LayerMask playerLayer;

    [Header("Beállítások")]
    public Animator animator;
    public Rigidbody2D rb;
    private Transform player;
    private bool isAttacking = false;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        targetPoint = rightPoint;
    }

    void Update()
    {
        if (isAttacking || isDead || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            StartCoroutine(ExplodeRoutine());
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        float distance = Vector2.Distance(transform.position, targetPoint.position);
        if (distance < 0.2f)
        {
            targetPoint = (targetPoint == rightPoint) ? leftPoint : rightPoint;
        }
        Move(targetPoint.position, moveSpeed);
    }

    void ChasePlayer()
    {
        Move(player.position, chaseSpeed);
    }

    void Move(Vector2 goal, float speed)
    {
        float directionX = goal.x - transform.position.x;
        float moveDir = Mathf.Sign(directionX);

        rb.linearVelocity = new Vector2(moveDir * speed, rb.linearVelocity.y);

        if (moveDir > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveDir < 0) transform.localScale = new Vector3(-1, 1, 1);

        if (animator != null) animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
    }

    IEnumerator ExplodeRoutine()
    {
        isAttacking = true;
        rb.linearVelocity = Vector2.zero; 

        if (animator != null) animator.SetTrigger("AttackTrigger");

        yield return new WaitForSeconds(0.7f);

        if (!isDead)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, explosionRadius, playerLayer);
            if (hit != null)
            {
                hit.GetComponent<PlayerMovement>().SendMessage("Die");
            }
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isDead)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.y < -0.5f)
                {
                    Stomped(collision.gameObject);
                    return;
                }
            }
        }
    }

    void Stomped(GameObject playerObj)
    {
        isDead = true;
        StopAllCoroutines();

        Rigidbody2D playerRb = playerObj.GetComponent<Rigidbody2D>();
        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 12f);

        Debug.Log("Enemy kinyírva ráugrással!");
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        if (leftPoint != null && rightPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(leftPoint.position, rightPoint.position);
            Gizmos.DrawSphere(leftPoint.position, 0.2f);
            Gizmos.DrawSphere(rightPoint.position, 0.2f);
        }
    }
}