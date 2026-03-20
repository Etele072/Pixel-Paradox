using System;
using System.Security.Cryptography;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;
    public GameObject bullet;
    public Transform firePoint;


    public float fireRate = 2f;
    public float attackRange = 10f;


    private float nextFireTime = 0f;
    private Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if(player == null)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (anim != null)
        {
            anim.SetTrigger("Shoot");
        }
        Instantiate(bullet,firePoint.position, firePoint.rotation);
    }

    private void OnDrawGizmoSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
