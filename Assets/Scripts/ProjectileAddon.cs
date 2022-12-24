using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class ProjectileAddon : MonoBehaviour
{
    public int damage;
    private Rigidbody rb;
    private bool targetHit;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (targetHit)
            return;
        targetHit = true;

        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            Destroy(gameObject);
            enemy.takeDamage(damage);
        }

        rb.isKinematic = true;
        transform.SetParent(collision.transform);
    }
}
