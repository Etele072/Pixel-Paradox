using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;

        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.CompareTag("Player"))
        {
            PlayerMovement playerScript = collision.GetComponent<PlayerMovement>();

            if(playerScript != null)
            {
                Debug.Log("hit");
                playerScript.Die();
            }

            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
